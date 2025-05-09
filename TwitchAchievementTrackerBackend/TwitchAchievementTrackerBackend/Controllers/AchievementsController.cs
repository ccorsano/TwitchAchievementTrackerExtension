﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Services;
using TwitchAchievementTrackerBackend.Helpers;
using TwitchAchievementTrackerBackend.Model.XApi;
using System;
using Microsoft.AspNetCore.Connections.Features;

namespace TwitchAchievementTrackerBackend.Controllers
{
    [Route("api/achievements")]
    [ApiController]
    [Authorize]
    public class AchievementsController : ControllerBase
    {
        public readonly XApiService _xApiService;
        public readonly SteamApiService _steamApiService;
        public readonly ConfigurationTokenService _configurationService;

        public AchievementsController(XApiService xApiService, SteamApiService steamApiService, ConfigurationTokenService configurationService)
        {
            _xApiService = xApiService;
            _steamApiService = steamApiService;
            _configurationService = configurationService;
        }

        [HttpGet("title")]
        [HttpGet("/api/title")]
        public async Task<TitleInfo> GetTitleInfo()
        {
            var config = this.GetExtensionConfiguration();

            if (config.ActiveConfig == ActiveConfig.XBoxLive)
            {
                var xConfig = config.XBoxLiveConfig!;

                var titleInfo = (await _xApiService.GetMarketplaceAsync(xConfig.TitleId!, xConfig.XApiKey!))!;

                return new TitleInfo
                {
                    Platform = ActiveConfig.XBoxLive,
                    TitleId = xConfig.TitleId,
                    ProductTitle = titleInfo.Products.FirstOrDefault()?.LocalizedProperties?.FirstOrDefault()?.ProductTitle ?? "Unknown",
                    ProductDescription = titleInfo.Products.FirstOrDefault()?.LocalizedProperties?.FirstOrDefault()?.ProductDescription ?? "-",
                    LogoUri = titleInfo.Products.FirstOrDefault()?.LocalizedProperties?.FirstOrDefault()?.Images?.FirstOrDefault(i => i.ImagePurpose == "Logo" || i.ImagePurpose == "BoxArt" || i.ImagePurpose == "FeaturePromotionalSquareArt")?.Uri,
                };
            }
            else if (config.ActiveConfig == ActiveConfig.Steam)
            {
                var steamConfig = config.SteamConfig!;

                var libraryUrlTask = _steamApiService.GetLibraryTileImage(long.Parse(steamConfig.AppId!));
                var titleInfo = await _steamApiService.GetStoreDetails(uint.Parse(steamConfig.AppId!));

                return new TitleInfo
                {
                    Platform = ActiveConfig.Steam,
                    TitleId = titleInfo.SteamAppid.ToString(),
                    ProductTitle = titleInfo.Name,
                    ProductDescription = titleInfo.ShortDescription,
                    LogoUri =  (await libraryUrlTask) ?? titleInfo.HeaderImage?.ToString(),
                };
            }

            throw new NotSupportedException("No active config");
        }

        [HttpGet("/api/title/steam/search/{query}")]
        public async Task<IEnumerable<TitleInfo>> SearchSteamTitleInfo(string query)
        {
            return await _steamApiService.SearchTitle(query);
        }

        [HttpGet("/api/title/xapi/search/{query}")]
        public async Task<IEnumerable<TitleInfo>> SearchXApiTitleInfo(string query, string? xApiKey = null)
        {
            XApiConfiguration xConfig = new XApiConfiguration();
            if (this.HasExtensionConfiguration())
            {
                var config = this.GetExtensionConfiguration();
                xConfig = config.XBoxLiveConfig ?? new XApiConfiguration();
            }
            if (! string.IsNullOrEmpty(xApiKey))
            {
                xConfig.XApiKey = xApiKey;
            }

            var searchResult = await _xApiService.SearchTitle(query, xConfig);

            return searchResult!.Products.Select(product =>
                new TitleInfo
                {
                    TitleId = product.AlternateIds?.FirstOrDefault(id => id.IdType == "XboxTitleId")?.Value ?? "",
                    ProductTitle = product.LocalizedProperties?.FirstOrDefault()?.ProductTitle ?? "Unknown",
                    ProductDescription = product.LocalizedProperties?.FirstOrDefault()?.ProductDescription ?? "-",
                    LogoUri = product.LocalizedProperties?.FirstOrDefault()?.Images?.FirstOrDefault(i => i.ImagePurpose == "Logo" || i.ImagePurpose == "BoxArt" || i.ImagePurpose == "FeaturePromotionalSquareArt")?.Uri,
                }
            );
        }

        [HttpGet("/api/title/search/{query}")]
        public async Task<IEnumerable<TitleInfo>> SearchTitleInfo(string query)
        {
            ExtensionConfiguration config = this.GetExtensionConfiguration();

            if (config.ActiveConfig == ActiveConfig.Steam)
            {
                return await _steamApiService.SearchTitle(query);
            }
            else
            {
                var searchResult = await _xApiService.SearchTitle(query, config.XBoxLiveConfig!);

                return searchResult!.Products.Select(product =>
                    new TitleInfo
                    {
                        Platform = ActiveConfig.XBoxLive,
                        TitleId = product.AlternateIds?.FirstOrDefault(id => id.IdType == "XboxTitleId")?.Value ?? "",
                        ProductTitle = product.LocalizedProperties?.FirstOrDefault()?.ProductTitle ?? "Unknown",
                        ProductDescription = product.LocalizedProperties?.FirstOrDefault()?.ProductDescription ?? "-",
                        LogoUri = product.LocalizedProperties?.FirstOrDefault()?.Images?.FirstOrDefault(i => i.ImagePurpose == "Logo" || i.ImagePurpose == "BoxArt" || i.ImagePurpose == "FeaturePromotionalSquareArt")?.Uri,
                    }
                );
            }
        }


        [HttpGet("summary")]
        public async Task<AchievementSummary> GetSummary()
        {
            var config = this.GetExtensionConfiguration();

            if (config.ActiveConfig == ActiveConfig.XBoxLive)
            {
                var achievements = await _xApiService.GetAchievementsAsync(config.XBoxLiveConfig!);
                var stateSummary = achievements.GroupBy(a => a.ProgressState).ToDictionary(a => a.Key, a => a.Count());

                return new AchievementSummary
                {
                    Total = achievements.Length,
                    Completed = stateSummary.TryGetValue(ProgressState.Achieved, out int completed) ? completed : 0,
                    InProgress = stateSummary.TryGetValue(ProgressState.InProgress, out int inProgress) ? inProgress : 0,
                    NotStarted = stateSummary.TryGetValue(ProgressState.NotStarted, out int notStarted) ? notStarted : 0,
                    CurrentPoints = achievements
                        .Where(a => a.ProgressState == ProgressState.Achieved)
                        .SelectMany(a => a.Rewards.Where(r => r.Type == "Gamerscore"))
                        .Sum(r => r.Value),
                    TotalPoints = achievements.SelectMany(a => a.Rewards.Where(r => r.Type == "Gamerscore")).Sum(r => r.Value)
                };
            }
            else if (config.ActiveConfig == ActiveConfig.Steam)
            {
                var achievements = await _steamApiService.GetAchievementsAsync(config.SteamConfig!);
                var notCompleted = achievements.Count(a => a.Achieved == 0);
                var completed = achievements.Length - notCompleted;

                return new AchievementSummary
                {
                    Total = achievements.Length,
                    Completed = achievements.Count(a => a.Achieved != 0),
                    InProgress = notCompleted,
                    NotStarted = notCompleted,
                    CurrentPoints = completed,
                    TotalPoints = achievements.Length,
                };
            }

            throw new NotSupportedException("No active config");
        }

        [HttpGet("list")]
        public async Task<IEnumerable<Achievement>> GetAchievements()
        {
            var config = this.GetExtensionConfiguration();

            if (config.ActiveConfig == ActiveConfig.XBoxLive)
            {
                return (await _xApiService.GetAchievementsAsync(config.XBoxLiveConfig!).ContinueWith(t => t.Result.OrderByDescending(a => $"{a.ProgressState}:{a.Id}"))).Select(x => new Achievement
                {
                    Id = x.Id!.ToString(),
                    Name = x.Name,
                    Completed = x.ProgressState == ProgressState.Achieved,
                    Description = x.ProgressState == ProgressState.Achieved ? x.Description : x.LockedDescription,
                    UnlockTime = x.Progression?.TimeUnlocked ?? DateTimeOffset.MaxValue,
                });
            }
            else if (config.ActiveConfig == ActiveConfig.Steam)
            {
                var gameSchemaTask = _steamApiService.GetGameSchema(config.SteamConfig!.AppId!, config.SteamConfig!.Locale!);
                var userAchievements = await _steamApiService.GetAchievementsAsync(config.SteamConfig);
                return (await gameSchemaTask).AvailableGameStats!.Achievements.Select(aDev =>
                {
                    var userAchievement = userAchievements.FirstOrDefault(ua => ua.Apiname == aDev.Name);
                    return new Achievement
                    {
                        Id = aDev.Name,
                        Name = aDev.DisplayName,
                        Completed = userAchievement is not null && userAchievement.Achieved != 0,
                        Description = aDev.Hidden == 0 ? aDev.Description : "*************",
                        UnlockTime = userAchievement is null || userAchievement.Unlocktime == 0 ? DateTimeOffset.MinValue : DateTime.UnixEpoch.AddSeconds(userAchievement.Unlocktime),
                    };
                }).OrderBy(a => a.Completed);
            }

            throw new NotSupportedException("No active config");
        }
    }
}
