using Microsoft.CodeAnalysis.Operations;
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
            _httpClient.BaseAddress = new Uri("https://xapi.us/v2/");
            _cache = memoryCache;
        }

        public async Task<XApiAchievement[]> GetAchievementsAsync(string xuid, string titleId)
        {
            var cacheKey = $"{xuid}:{titleId}";
            if (! _cache.TryGetValue<XApiAchievement[]>(cacheKey, out var result))
            {
                using (var responseStream = await _httpClient.GetStreamAsync($"{xuid}/achievements/{titleId}"))
                {
                    result = await JsonSerializer.DeserializeAsync<XApiAchievement[]>(responseStream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    _cache.Set(cacheKey, result);
                }
            }

            return result;
        }
    }
}
