using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Model.XApi;

namespace TwitchAchievementTrackerBackend.Services
{
    public class XApiService
    {
        private readonly HttpClient _httpClient;
        private readonly XApiOptions _options;
        private readonly IMemoryCache _cache;

        public XApiService(IHttpClientFactory httpClientFactory, IOptions<XApiOptions> options, IMemoryCache memoryCache)
        {
            _httpClient = httpClientFactory.CreateClient();
            if (string.IsNullOrEmpty(options.Value.XApiKey))
            {
                throw new ArgumentNullException("Could not find XApi key in configuration");
            }
            _options = options.Value;
            _httpClient.DefaultRequestHeaders.Add("X-AUTH", _options.XApiKey);
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            _httpClient.BaseAddress = new Uri("https://xapi.us/v2/");
            _cache = memoryCache;
        }

        public string GetCacheKey(string call, XApiConfiguration config)
        {
            return $"{call}:{config.TitleId}:{config.StreamerXuid}:{config.Locale}";
        }

        public async Task<XApiMarketplaceSearchResult> SearchTitle(string query)
        {
            var cacheKey = $"titlesearch:{query}";
            if (!_cache.TryGetValue<XApiMarketplaceSearchResult>(cacheKey, out var result))
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"marketplace/search/{query}");
                var response = await _httpClient.SendAsync(message);
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    result = await JsonSerializer.DeserializeAsync<XApiMarketplaceSearchResult>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    // Cache for 1 minute (configurable)
                    _cache.Set(cacheKey, result, _options.ResultCacheTime);
                }
            }

            return result;
        }

        public async Task<string> ResolveXuid(string gamerTag)
        {
            var cacheKey = $"xuid:{gamerTag}";

            if (!_cache.TryGetValue<string>(cacheKey, out var result))
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"xuid/{gamerTag}");
                var response = await _httpClient.SendAsync(message);
                result = await response.Content.ReadAsStringAsync();
                _cache.Set(cacheKey, result);
            }

            return result;
        }

        public async Task<XApiAchievement[]> GetAchievementsAsync(XApiConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            string xuid = config.StreamerXuid;
            string titleId = config.TitleId;

            var cacheKey = GetCacheKey($"achievements", config);
            if (! _cache.TryGetValue<XApiAchievement[]>(cacheKey, out var result))
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"{xuid}/achievements/{titleId}");
                message.Headers.Add("X-AUTH", config.XApiKey);
                message.Headers.Add("Accept-Language", $"{config.Locale};q=1.0");
                var response = await _httpClient.SendAsync(message);
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    result = await JsonSerializer.DeserializeAsync<XApiAchievement[]>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    // Cache for 1 minute (configurable)
                    _cache.Set(cacheKey, result, _options.ResultCacheTime);
                }
            }

            return result;
        }

        public async Task<XApiMarketplaceTitleInfo> GetMarketplaceAsync(XApiConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            string titleId = config.TitleId;

            var cacheKey = GetCacheKey($"marketplace", config);
            if (!_cache.TryGetValue<XApiMarketplaceTitleInfo>(cacheKey, out var result))
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"marketplace/show/{titleId}");
                message.Headers.Add("X-AUTH", config.XApiKey);
                message.Headers.Add("Accept-Language", $"{config.Locale};q=1.0");
                var response = await _httpClient.SendAsync(message);
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    result = await JsonSerializer.DeserializeAsync<XApiMarketplaceTitleInfo>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    // Cache indefinitly
                    _cache.Set(cacheKey, result);
                }
            }

            return result;
        }
    }
}
