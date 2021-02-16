using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Configuration;

namespace TwitchAchievementTrackerBackend.Services
{
    public class TwitchHelixApiService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _twitchHelixClient;
        private readonly TwitchOptions _options;

        public TwitchHelixApiService(IHttpClientFactory httpClientFactory, IOptions<TwitchOptions> options, ILogger<TwitchHelixApiService> logger)
        {
            _logger = logger;
            _options = options.Value;
            _twitchHelixClient = httpClientFactory.CreateClient("twitchExt");
            _twitchHelixClient.BaseAddress = new Uri("https://api.twitch.tv/helix/");
            _twitchHelixClient.DefaultRequestHeaders.Add("Client-Id", _options.ClientId);
        }
    }
}
