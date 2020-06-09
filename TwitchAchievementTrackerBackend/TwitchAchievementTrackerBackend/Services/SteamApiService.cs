using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Model.Steam;

namespace TwitchAchievementTrackerBackend.Services
{
    public class SteamApiService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _storeClient;
        private readonly SteamApiOptions _options;
        private readonly IMemoryCache _cache;

        public SteamApiService(IHttpClientFactory httpClientFactory, IOptions<SteamApiOptions> options, IMemoryCache memoryCache)
        {
            _httpClient = httpClientFactory.CreateClient();
            if (string.IsNullOrEmpty(options.Value.WebApiKey))
            {
                throw new ArgumentNullException("Could not find XApi key in configuration");
            }
            _options = options.Value;
            _httpClient.DefaultRequestHeaders.Add("x-webapi-key", _options.WebApiKey);
            _httpClient.BaseAddress = new Uri("https://api.steampowered.com/");
            _storeClient = httpClientFactory.CreateClient();
            _storeClient.BaseAddress = new Uri("https://store.steampowered.com/");

            _cache = memoryCache;
            var unawaited = LoadAppList();
        }

        public string GetCacheKey(string call, SteamConfiguration config)
        {
            return $"{call}:{config.AppId}:{config.SteamId}:{config.Locale}";
        }

        public async Task<SteamApp[]> LoadAppList()
        {
            var cacheKey = "steam:applist";
            var message = new HttpRequestMessage(HttpMethod.Get, $"ISteamApps/GetAppList/v2/");
            _httpClient.DefaultRequestHeaders.Add("x-webapi-key", _options.WebApiKey);
            var response = await _httpClient.SendAsync(message);
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var result = await JsonSerializer.DeserializeAsync<SteamAppListResult>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                // Cache for 1 minute (configurable)
                _cache.Set(cacheKey, result.AppList.Apps, _options.ResultCacheTime);
                return result.AppList.Apps;
            }
        }

        public Task GetAppInfo(SteamConfiguration steamConfig)
        {
            throw new NotImplementedException();
        }

        public async Task<SteamStoreDetails> GetStoreDetails(uint appId)
        {
            var cacheKey = $"steam:store:{appId}";

            if (! _cache.TryGetValue(cacheKey, out SteamStoreDetails result))
            {
                using (var responseStream = await _storeClient.GetStreamAsync($"api/appdetails/?appids={appId}"))
                {
                    var wrapper = await JsonSerializer.DeserializeAsync<SteamStoreResult>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    result = wrapper[appId.ToString()].Data;
                }

                _cache.Set(cacheKey, result);
            }
            return result;
        }

        public async Task<IEnumerable<TitleInfo>> SearchTitle(string query)
        {
            if (!_cache.TryGetValue("steam:applist", out SteamApp[] fullAppList))
            {
                fullAppList = await LoadAppList();
            }

            return fullAppList
                .Where(a => a.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Select(a => new TitleInfo
                {
                    TitleId = a.AppId.ToString(),
                    ProductTitle = a.Name,
                    ProductDescription = "",
                    LogoUri = "",
                });
        }

        public async Task<SteamPlayerAchievement[]> GetAchievementsAsync(SteamConfiguration steamConfig)
        {
            var cacheKey = $"steam:achievements:{steamConfig.AppId}";

            if (!_cache.TryGetValue(cacheKey, out SteamPlayerAchievement[] result))
            {
                using (var responseStream = await _httpClient.GetStreamAsync($"ISteamUserStats/GetPlayerAchievements/v1/?steamid={steamConfig.SteamId}&appid={steamConfig.AppId}"))
                {
                    var wrapper = await JsonSerializer.DeserializeAsync<SteamUserStatsAchievementsResult>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    result = wrapper.Playerstats.Achievements;
                }

                _cache.Set(cacheKey, result, _options.ResultCacheTime);
            }

            return result;
        }

        public async Task<SteamUserStatsGameSchema> GetGameSchema(SteamConfiguration steamConfig)
        {
            var cacheKey = $"steam:gameschema:{steamConfig.AppId}:{steamConfig.Locale}";

            if (!_cache.TryGetValue(cacheKey, out SteamUserStatsGameSchema result))
            {
                using (var responseStream = await _httpClient.GetStreamAsync($"ISteamUserStats/GetSchemaForGame/v2/?appid={steamConfig.AppId}&l={steamConfig.Locale}"))
                {
                    var wrapper = await JsonSerializer.DeserializeAsync<SteamUserStatsGameSchemaResult>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    result = wrapper.Game;
                }

                _cache.Set(cacheKey, result, _options.AppListCacheTime);
            }
            return result;
        }
    }
}
