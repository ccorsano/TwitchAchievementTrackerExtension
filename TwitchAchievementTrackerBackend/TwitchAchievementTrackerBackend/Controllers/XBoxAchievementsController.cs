using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Services;
using TwitchAchievementTrackerBackend.Helpers;
using System.ComponentModel;

namespace TwitchAchievementTrackerBackend.Controllers
{
    [Route("api/achievements")]
    [ApiController]
    [Authorize]
    public class XBoxAchievementsController : ControllerBase
    {
        public readonly XApiService _xApiService;
        public readonly ConfigurationTokenService _configurationService;

        public XBoxAchievementsController(XApiService xApiService, ConfigurationTokenService configurationService)
        {
            _xApiService = xApiService;
            _configurationService = configurationService;
        }

        [HttpGet("title")]
        [HttpGet("/api/title")]
        public async Task<TitleInfo> GetTitleInfo()
        {
            var config = this.GetExtensionConfiguration();

            var titleInfo = await _xApiService.GetMarketplaceAsync(config);

            return new TitleInfo
            {
                TitleId = config.TitleId,
                ProductTitle = titleInfo.Products.FirstOrDefault()?.LocalizedProperties?.FirstOrDefault()?.ProductTitle ?? "Unknown",
                ProductDescription = titleInfo.Products.FirstOrDefault()?.LocalizedProperties?.FirstOrDefault()?.ProductDescription ?? "-",
                LogoUri = titleInfo.Products.FirstOrDefault()?.LocalizedProperties?.FirstOrDefault()?.Images?.FirstOrDefault(i => i.ImagePurpose == "Logo" || i.ImagePurpose == "BoxArt" || i.ImagePurpose == "FeaturePromotionalSquareArt")?.Uri,
            };
        }

        [HttpGet("/api/title/search/{query}")]
        public async Task<IEnumerable<TitleInfo>> SearchTitleInfo(string query)
        {
            var searchResult = await _xApiService.SearchTitle(query);

            return searchResult.Products.Select(product =>
                new TitleInfo
                {
                    TitleId = product.AlternateIds?.FirstOrDefault(id => id.IdType == "XboxTitleId")?.Value ?? "",
                    ProductTitle = product.LocalizedProperties?.FirstOrDefault()?.ProductTitle ?? "Unknown",
                    ProductDescription = product.LocalizedProperties?.FirstOrDefault()?.ProductDescription ?? "-",
                    LogoUri = product.LocalizedProperties?.FirstOrDefault()?.Images?.FirstOrDefault(i => i.ImagePurpose == "Logo" || i.ImagePurpose == "BoxArt" || i.ImagePurpose == "FeaturePromotionalSquareArt")?.Uri,
                }
            );
        }

        [HttpGet("/api/xuid/{gamertag}")]
        public async Task<string> ResolveXuid(string gamerTag)
        {
            return await _xApiService.ResolveXuid(gamerTag);
        }


        [HttpGet("summary")]
        public async Task<AchievementSummary> GetSummary()
        {
            var config = this.GetExtensionConfiguration();

            var achievements = await _xApiService.GetAchievementsAsync(config);
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
                    .Sum(r => r.Value ?? 0),
                TotalPoints = achievements.SelectMany(a => a.Rewards.Where(r => r.Type == "Gamerscore")).Sum(r => r.Value ?? 0)
            };
        }

        [HttpGet("list")]
        public async Task<IEnumerable<XApiAchievement>> GetAchievements()
        {
            var config = this.GetExtensionConfiguration();

            return await _xApiService.GetAchievementsAsync(config).ContinueWith(t => t.Result.OrderByDescending(a => $"{a.ProgressState}:{a.Id}"));
        }
    }
}