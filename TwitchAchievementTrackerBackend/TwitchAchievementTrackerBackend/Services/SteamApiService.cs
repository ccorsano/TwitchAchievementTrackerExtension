using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
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
        private readonly ILogger _logger;

        public static SupportedLanguage[] STEAM_SUPPORTED_LANGUAGES = new SupportedLanguage[]
        {
            new SupportedLanguage { DisplayName = "Arabic", LangCode = "arabic" },
            new SupportedLanguage { DisplayName = "Bulgarian", LangCode = "bulgarian" },
            new SupportedLanguage { DisplayName = "Chinese (Simplified)", LangCode = "schinese" },
            new SupportedLanguage { DisplayName = "Chinese (Traditional)", LangCode = "tchinese" },
            new SupportedLanguage { DisplayName = "Czech", LangCode = "czech" },
            new SupportedLanguage { DisplayName = "Danish", LangCode = "danish" },
            new SupportedLanguage { DisplayName = "Dutch", LangCode = "dutch" },
            new SupportedLanguage { DisplayName = "English", LangCode = "english" },
            new SupportedLanguage { DisplayName = "Finnish", LangCode = "finnish" },
            new SupportedLanguage { DisplayName = "French", LangCode = "french" },
            new SupportedLanguage { DisplayName = "German", LangCode = "german" },
            new SupportedLanguage { DisplayName = "Greek", LangCode = "greek" },
            new SupportedLanguage { DisplayName = "Hungarian", LangCode = "hungarian" },
            new SupportedLanguage { DisplayName = "Italian", LangCode = "italian" },
            new SupportedLanguage { DisplayName = "Japanese", LangCode = "japanese" },
            new SupportedLanguage { DisplayName = "Korean", LangCode = "koreana" },
            new SupportedLanguage { DisplayName = "Norwegian", LangCode = "norwegian" },
            new SupportedLanguage { DisplayName = "Polish", LangCode = "polish" },
            new SupportedLanguage { DisplayName = "Portuguese", LangCode = "portuguese" },
            new SupportedLanguage { DisplayName = "Portuguese-Brazil", LangCode = "brazilian" },
            new SupportedLanguage { DisplayName = "Portuguese - Brazil", LangCode = "brazilian" },
            new SupportedLanguage { DisplayName = "Romanian", LangCode = "romanian" },
            new SupportedLanguage { DisplayName = "Russian", LangCode = "russian" },
            new SupportedLanguage { DisplayName = "Spanish - Spain", LangCode = "spanish" },
            new SupportedLanguage { DisplayName = "Spanish-Spain", LangCode = "spanish" },
            new SupportedLanguage { DisplayName = "Spanish - Latin America", LangCode = "latam" },
            new SupportedLanguage { DisplayName = "Spanish-Latin America", LangCode = "latam" },
            new SupportedLanguage { DisplayName = "Swedish", LangCode = "swedish" },
            new SupportedLanguage { DisplayName = "Thai", LangCode = "thai" },
            new SupportedLanguage { DisplayName = "Turkish", LangCode = "turkish" },
            new SupportedLanguage { DisplayName = "Ukrainian", LangCode = "ukrainian" },
            new SupportedLanguage { DisplayName = "Vietnamese", LangCode = "vietnamese" }
        };

        public class GameDetailsProtectedException : Exception
        {
            public GameDetailsProtectedException()
            {
            }

            public GameDetailsProtectedException(string message) : base(message)
            {
            }

            public GameDetailsProtectedException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        public SteamApiService(IHttpClientFactory httpClientFactory, IOptions<SteamApiOptions> options, IMemoryCache memoryCache, ILogger<SteamApiService> logger)
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
            _logger = logger;

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
                _cache.Set(cacheKey, result!.AppList!.Apps, _options.ResultCacheTime);
                return result.AppList.Apps;
            }
        }

        public async Task<bool> PurgeCache(SteamConfiguration steamConfig)
        {
            var cacheKey = $"steam:achievements:{steamConfig.AppId}:{steamConfig.SteamId}";

            var wasCached = _cache.TryGetValue<SteamPlayerAchievement[]>(cacheKey, out var cachedAchievements);
            if (wasCached)
            {
                _cache.Remove(cacheKey);
            }

            var reloaded = await GetAchievementsAsync(steamConfig);

            return reloaded.Count(a => a?.Achieved == 1) != cachedAchievements?.Count(a => a?.Achieved == 1);
        }

        public async Task GetAppInfo(SteamConfiguration steamConfig)
        {
            var storeDetails = await GetStoreDetails(uint.Parse(steamConfig.AppId!));
        }

        public async Task<SteamStoreDetails> GetStoreDetails(uint appId)
        {
            var cacheKey = $"steam:store:{appId}";

            if (!_cache.TryGetValue(cacheKey, out SteamStoreDetails? result))
            {
                var response = await _storeClient.GetAsync($"api/appdetails/?l=english&appids={appId}");
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var wrapper = await JsonSerializer.DeserializeAsync<SteamStoreResult>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    result = wrapper![appId.ToString()].Data;
                }

                _cache.Set(cacheKey, result);
            }
            return result!;
        }

        public async Task<SteamPlayerSummary[]> GetPlayersSummaries(string[] steamIds, string? webApiKey = null)
        {
            webApiKey = webApiKey ?? _options.WebApiKey;

            Func<string, string> cacheKey = (string steamId) => $"steam:profilesummary:{steamId}";

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

                foreach (var playerSummary in wrapper!.Response!.Players)
                {
                    cachedValues[playerSummary.SteamId!] = playerSummary;
                    _cache.Set(cacheKey(playerSummary.SteamId!), playerSummary);
                }
            }

            return steamIds.Select(id => cachedValues[id]!).ToArray();
        }

        public async Task<SteamVanityUrlResolution> ResolveVanityUrl(string vanityId, string? webApiKey = null)
        {
            webApiKey = webApiKey ?? _options.WebApiKey;

            var cacheKey = $"steam:profileurl:{vanityId}";

            if (!_cache.TryGetValue(cacheKey, out SteamVanityUrlResolution? result))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"ISteamUser/ResolveVanityURL/v1/?vanityurl={vanityId}");
                request.Headers.Add("x-webapi-key", webApiKey);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var wrapper = await JsonSerializer.DeserializeAsync<SteamResolveVanityUrlResult>(responseStream);
                    _cache.Set(cacheKey, wrapper!.Response);
                    result = wrapper.Response;
                }
            }

            return result!;
        }

        public async Task<IEnumerable<TitleInfo>> SearchTitle(string query)
        {
            if (!_cache.TryGetValue("steam:applist", out SteamApp[]? fullAppList))
            {
                fullAppList = await LoadAppList();
            }

            return fullAppList?
                .Where(a => a.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
                .Select(a => new TitleInfo
                {
                    Platform = ActiveConfig.Steam,
                    TitleId = a.AppId.ToString(),
                    ProductTitle = a.Name,
                    ProductDescription = "",
                    LogoUri = "",
                }) ?? Array.Empty<TitleInfo>();
        }

        public async Task<SteamPlayerAchievement[]> GetAchievementsAsync(SteamConfiguration steamConfig)
        {
            var cacheKey = $"steam:achievements:{steamConfig.AppId}:{steamConfig.SteamId}";

            if (!_cache.TryGetValue(cacheKey, out SteamPlayerAchievement[]? result))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"ISteamUserStats/GetPlayerAchievements/v1/?steamid={steamConfig.SteamId}&appid={steamConfig.AppId}");
                request.Headers.Add("x-webapi-key", steamConfig.WebApiKey);
                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new GameDetailsProtectedException("Could not get Game Details");
                }

                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var wrapper = await JsonSerializer.DeserializeAsync<SteamUserStatsAchievementsResult>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    result = wrapper!.Playerstats!.Achievements;
                }

                _cache.Set(cacheKey, result, _options.ResultCacheTime);
            }

            return result!;
        }

        public async Task<SteamUserStatsGameSchema> GetGameSchema(string appId, string locale = "english")
        {
            var cacheKey = $"steam:gameschema:{appId}:{locale}";

            if (!_cache.TryGetValue(cacheKey, out SteamUserStatsGameSchema? result))
            {
                var response = await _httpClient.GetAsync($"ISteamUserStats/GetSchemaForGame/v2/?appid={appId}&l={locale}");
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var wrapper = await JsonSerializer.DeserializeAsync<SteamUserStatsGameSchemaResult>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    result = wrapper!.Game;
                }

                _cache.Set(cacheKey, result, _options.AppListCacheTime);
            }
            return result!;
        }

        public async Task<bool> TestApiKey(string webApiKey)
        {
            if (string.IsNullOrEmpty(webApiKey))
            {
                return false;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"ISteamUser/GetPlayerSummaries/v2/?steamids={_options.TestSteamId}");
            request.Headers.Add("x-webapi-key", webApiKey);
            var response = await _httpClient.SendAsync(request);

            if (! response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed Steam WebAPI test call: {response.StatusCode}; {response.ReasonPhrase}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<SteamPlayerOwnedGameInfo[]> GetOwnedGames(string steamId, string? webApiKey = null)
        {
            var cacheKey = $"steam:ownedgames:{steamId}";

            if (!_cache.TryGetValue(cacheKey, out SteamPlayerOwnedGameInfo[]? result))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"IPlayerService/GetOwnedGames/v1/?steamid={steamId}&include_appinfo=true&include_played_free_games=true");
                request.Headers.Add("x-webapi-key", webApiKey);
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var wrapper = await JsonSerializer.DeserializeAsync<SteamPlayerOwnedGamesResult>(responseStream);
                    result = wrapper?.Response?.Games;
                }
                
                if (result == null)
                {
                    return new SteamPlayerOwnedGameInfo[0];
                }

                Func<long, string?, string> buildImageUri = (appId, imgId) => $"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/{appId}/{imgId}.jpg";

                var resolveTasks = result.Select(async game =>
                {
                    game.LibraryTileUrl = await GetLibraryTileImage(game.AppId);
                    game.ImgIconUrl = buildImageUri(game.AppId, game.ImgIconUrl);
                    game.ImgLogoUrl = buildImageUri(game.AppId, game.ImgLogoUrl);
                });

                await Task.WhenAll(resolveTasks);

                _cache.Set(cacheKey, result, _options.ResultCacheTime);
            }
            return result!;
        }

        public async Task<string> GetLibraryTileImage(long appId)
        {

            var cacheKey = $"steam:librarytile:{appId}";

            if (!_cache.TryGetValue(cacheKey, out string? tileUrl))
            {
                Func<long, string, string> buildLibraryImageUri = (appId, imgName) => $"https://steamcdn-a.akamaihd.net/steam/apps/{appId}/{imgName}";

                var libraryTileImageUrl = $"https://steamcdn-a.akamaihd.net/steam/apps/{appId}/library_600x900.jpg";
                var headRequest = new HttpRequestMessage(HttpMethod.Head, libraryTileImageUrl);
                var libraryTileExistsResponse = await _httpClient.SendAsync(headRequest);
                if (libraryTileExistsResponse.IsSuccessStatusCode)
                {
                    tileUrl = libraryTileImageUrl;
                    _cache.Set(cacheKey, tileUrl, _options.AppListCacheTime);
                }
            }
            return tileUrl!;
        }
    }
}
