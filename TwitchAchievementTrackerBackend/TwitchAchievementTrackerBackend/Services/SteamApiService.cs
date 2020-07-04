using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                throw new ArgumentNullException("Could not find Steam WebApi key in configuration");
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
            var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

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

        public async Task GetAppInfo(SteamConfiguration steamConfig)
        {
            var storeDetails = await GetStoreDetails(uint.Parse(steamConfig.AppId));
        }

        public async Task<SteamStoreDetails> GetStoreDetails(uint appId)
        {
            var cacheKey = $"steam:store:{appId}";

            if (!_cache.TryGetValue(cacheKey, out SteamStoreDetails result))
            {
                var response = await _storeClient.GetAsync($"api/appdetails/?appids={appId}");
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
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

        public async Task<SteamPlayerSummary[]> GetPlayersSummaries(string[] steamIds, string webApiKey = null)
        {
            webApiKey = webApiKey ?? _options.WebApiKey;

            Func<string, string> cacheKey = (string vanityId) => $"steam:profilesummary:{vanityId}";

            var cachedValues = steamIds.ToDictionary(id => id, id => _cache.TryGetValue<SteamPlayerSummary>(cacheKey(id), out var cached) ? cached : null);
            var remainingIds = steamIds.Where(id => cachedValues[id] == null);

            var ids = string.Join(",", remainingIds);
            var request = new HttpRequestMessage(HttpMethod.Get, $"ISteamUser/GetPlayerSummaries/v2/?steamids={ids}");
            request.Headers.Add("x-webapi-key", webApiKey);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var wrapper = await JsonSerializer.DeserializeAsync<SteamPlayerSummariesResult>(responseStream);

                foreach (var playerSummary in wrapper.Response.Players)
                {
                    cachedValues[playerSummary.SteamId] = playerSummary;
                    _cache.Set(cacheKey(playerSummary.SteamId), playerSummary);
                }
            }

            return steamIds.Select(id => cachedValues[id]).ToArray();
        }

        public async Task<SteamVanityUrlResolution> ResolveVanityUrl(string vanityId, string webApiKey = null)
        {
            webApiKey = webApiKey ?? _options.WebApiKey;

            var cacheKey = $"steam:profileurl:{vanityId}";

            if (!_cache.TryGetValue(cacheKey, out SteamVanityUrlResolution result))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"ISteamUser/ResolveVanityURL/v1/?vanityurl={vanityId}");
                request.Headers.Add("x-webapi-key", webApiKey);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var wrapper = await JsonSerializer.DeserializeAsync<SteamResolveVanityUrlResult>(responseStream);
                    _cache.Set(cacheKey, wrapper.Response);
                    result = wrapper.Response;
                }
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
                var request = new HttpRequestMessage(HttpMethod.Get, $"ISteamUserStats/GetPlayerAchievements/v1/?steamid={steamConfig.SteamId}&appid={steamConfig.AppId}");
                request.Headers.Add("x-webapi-key", steamConfig.WebApiKey);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
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
                var response = await _httpClient.GetAsync($"ISteamUserStats/GetSchemaForGame/v2/?appid={steamConfig.AppId}&l={steamConfig.Locale}");
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
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

        public async Task<SteamPlayerOwnedGameInfo[]> GetOwnedGames(string steamId, string webApiKey = null)
        {
            var cacheKey = $"steam:ownedgames:{steamId}";

            if (!_cache.TryGetValue(cacheKey, out SteamPlayerOwnedGameInfo[] result))
            {
                var response = await _httpClient.GetAsync($"IPlayerService/GetOwnedGames/v1/?steamid={steamId}&include_appinfo=true&include_played_free_games=true");
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var wrapper = await JsonSerializer.DeserializeAsync<SteamPlayerOwnedGamesResult>(responseStream);
                    result = wrapper.Response.Games;
                }

                _cache.Set(cacheKey, result, _options.ResultCacheTime);
            }
            return result;
        }
    }
}
