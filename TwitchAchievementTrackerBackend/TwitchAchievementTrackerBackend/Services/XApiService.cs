using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Model.XApi;

namespace TwitchAchievementTrackerBackend.Services
{

    [Serializable]
    public class XApiException : Exception
    {
        public string ErrorCode { get; private set; }

        public XApiException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
        public XApiException(string errorCode, string message, Exception inner) : base(message, inner)
        {
            ErrorCode = errorCode;
        }
    }

    public class XApiService
    {
        private readonly HttpClient _httpClient;
        private readonly XApiOptions _options;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        public XApiService(IHttpClientFactory httpClientFactory, IOptions<XApiOptions> options, IMemoryCache memoryCache, ILogger<XApiService> logger)
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
            _logger = logger;
        }

        public string GetCacheKey(string call, XApiConfiguration config)
        {
            return $"{call}:{config.TitleId}:{config.StreamerXuid}:{config.Locale}";
        }

        private void SaveRateLimit(string xApiKey, HttpResponseMessage response)
        {
            var rateLimit = int.Parse(response.Headers.GetValues("X-RateLimit-Limit").FirstOrDefault() ?? "60");
            var rateLimitRemaining = int.Parse(response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault() ?? "60");
            var rateLimitResetTime = int.Parse(response.Headers.GetValues("X-RateLimit-Reset").FirstOrDefault() ?? "3600");
            var rateLimitObj = new RateLimits
            {
                HourlyLimit = rateLimit,
                Remaining = rateLimitRemaining,
                ResetTime = DateTimeOffset.UtcNow.AddSeconds(rateLimitResetTime),
            };

            _cache.Set($"ratelimit:{xApiKey}", rateLimitObj, rateLimitObj.ResetTime);
        }

        public RateLimits GetRateLimit(string xApiKey)
        {
            if (_cache.TryGetValue<RateLimits>($"ratelimit:{xApiKey}", out var rateLimit))
            {
                return rateLimit;
            }
            return new RateLimits
            {
                HourlyLimit = 60,
                Remaining = 60,
                ResetTime = DateTimeOffset.UtcNow
            };
        }

        private void CheckRateLimit(string xApiKey)
        {
            if (_cache.TryGetValue<RateLimits>($"ratelimit:{xApiKey}", out var rateLimit))
            {
                if (rateLimit.Remaining < rateLimit.HourlyLimit * 0.1)
                {
                    _logger.LogWarning($"Nearing xApi rate limit with {rateLimit.Remaining}/{rateLimit.HourlyLimit}, reset in {rateLimit.ResetTime - DateTimeOffset.UtcNow}");
                }
            }
        }

        public async Task<XUIDInfo> GetApiKeyXuid(string xApiKey)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"accountXuid");
            message.Headers.Add("X-AUTH", xApiKey);
            var response = await _httpClient.SendAsync(message);
            var responseBody = await response.Content.ReadAsStringAsync();

            SaveRateLimit(xApiKey, response);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = JsonSerializer.Deserialize<XApiError>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                var errorCode = "XApiUnauthorizedError";
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && errorMessage.Error_Message.Contains("fresh login"))
                {
                    errorCode = "ExpiredXBLToken";
                }
                throw new XApiException(errorCode, errorMessage.Error_Message);
            }

            return JsonSerializer.Deserialize<XUIDInfo>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<XApiMarketplaceSearchResult> SearchTitle(string query, XApiConfiguration config = null)
        {
            var cacheKey = $"titlesearch:{query}";
            if (!_cache.TryGetValue<XApiMarketplaceSearchResult>(cacheKey, out var result))
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"marketplace/search/{query}");

                if (config != null)
                {
                    message.Headers.Add("X-AUTH", config.XApiKey);
                    message.Headers.Add("Accept-Language", $"{config.Locale};q=1.0");
                }

                var response = await _httpClient.SendAsync(message);

                SaveRateLimit(config.XApiKey, response);

                response.EnsureSuccessStatusCode();

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    result = await JsonSerializer.DeserializeAsync<XApiMarketplaceSearchResult>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    // Cache for 1 hour (configurable)
                    _cache.Set(cacheKey, result, _options.StaticDataCacheTime);
                }
            }

            return result;
        }

        public async Task<string> ResolveXuid(string gamerTag, string xApiKey)
        {
            var cacheKey = $"xuid:{gamerTag}";

            if (!_cache.TryGetValue<string>(cacheKey, out var result))
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"xuid/{gamerTag}");
                message.Headers.Add("X-AUTH", xApiKey);

                var response = await _httpClient.SendAsync(message);

                SaveRateLimit(xApiKey, response);

                response.EnsureSuccessStatusCode();

                result = await response.Content.ReadAsStringAsync();
                _cache.Set(cacheKey, result);
            }

            return result;
        }

        public async Task<bool> PurgeCache(XApiConfiguration config)
        {
            var cacheKey = GetCacheKey($"achievements", config);
            var wasCached = _cache.TryGetValue<XApiAchievement[]>(cacheKey, out var cachedAchievements);
            if (wasCached)
            {
                _cache.Remove(cacheKey);
            }

            var reloaded = await GetAchievementsAsync(config);

            return reloaded.Count(a => a?.ProgressState == ProgressState.Achieved) != cachedAchievements.Count(a => a?.ProgressState == ProgressState.Achieved);
        }

        public async Task<XApiGamerCard> GetGamerCard(string xuid, string xApiKey)
        {
            var cacheKey = $"gamercard:{xuid}";
            if (!_cache.TryGetValue<XApiGamerCard>(cacheKey, out var result))
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"{xuid}/gamercard");
                message.Headers.Add("X-AUTH", xApiKey);
                var response = await _httpClient.SendAsync(message);

                SaveRateLimit(xApiKey, response);

                response.EnsureSuccessStatusCode();
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    result = await JsonSerializer.DeserializeAsync<XApiGamerCard>(responseStream);
                    // Cache for 1 hour (configurable)
                    _cache.Set(cacheKey, result, _options.StaticDataCacheTime);
                }
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

                SaveRateLimit(config.XApiKey, response);

                response.EnsureSuccessStatusCode();
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

        public async Task<XApiMarketplaceTitleInfo> GetMarketplaceAsync(string titleId, string xApiKey, string locale = null)
        {
            var cacheKey = $"marketplace:{titleId}:{locale}";
            if (!_cache.TryGetValue<XApiMarketplaceTitleInfo>(cacheKey, out var result))
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"marketplace/show/{titleId}");
                message.Headers.Add("X-AUTH", xApiKey);
                if (! string.IsNullOrEmpty(locale))
                {
                    message.Headers.Add("Accept-Language", $"{locale};q=1.0");
                }
                var response = await _httpClient.SendAsync(message);

                SaveRateLimit(xApiKey, response);

                response.EnsureSuccessStatusCode();
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

        public async Task<XApiTitleHistoryResult> GetRecentTitlesAsync(string xuid, string xApiKey, string locale = null)
        {
            var cacheKey = $"recenttitles:{xuid}:{locale}";
            if (!_cache.TryGetValue<XApiTitleHistoryResult>(cacheKey, out var result))
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"{xuid}/title-history");
                message.Headers.Add("X-AUTH", xApiKey);
                if (!string.IsNullOrEmpty(locale))
                {
                    message.Headers.Add("Accept-Language", $"{locale};q=1.0");
                }
                var response = await _httpClient.SendAsync(message);

                SaveRateLimit(xApiKey, response);

                response.EnsureSuccessStatusCode();
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    result = await JsonSerializer.DeserializeAsync<XApiTitleHistoryResult>(responseStream);

                    foreach(var game in result.Titles)
                    {
                        foreach(var image in game.Images)
                        {
                            if (image.Url.Scheme != "https")
                            {
                                var httpsUrl = new UriBuilder(image.Url);
                                httpsUrl.Scheme = "https";
                                httpsUrl.Port = 443;
                                image.Url = httpsUrl.Uri;
                            }
                        }
                    }
                    // Cache for 1 min, overridable
                    _cache.Set(cacheKey, result, _options.ResultCacheTime);
                }
            }

            return result;
        }
    }
}
