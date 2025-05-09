﻿using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Configuration;

namespace TwitchAchievementTrackerBackend.Services
{
    public class TwitchEBSService
    {
        static readonly DateTimeOffset EPOCH = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private readonly TwitchOptions _options;
        private HttpClient _twitchExtensionClient;
        private SigningCredentials _jwtSigningCredentials;

        public TwitchEBSService(IHttpClientFactory httpClientFactory, IOptions<TwitchOptions> options, ILogger<TwitchEBSService> logger)
        {
            _options = options.Value;
            _twitchExtensionClient = httpClientFactory.CreateClient("twitchExt");
            _twitchExtensionClient.BaseAddress = new Uri("https://api.twitch.tv/extensions/");
            _twitchExtensionClient.DefaultRequestHeaders.Add("Client-Id", _options.ClientId);

            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_options.ExtensionSecret!));
            _jwtSigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        public string GetUserJWTToken(string userId, string channelId, string role)
        {
            var iat = DateTimeOffset.UtcNow - EPOCH;

            var token = new JwtSecurityToken(null, null, null, null, DateTime.UtcNow.AddDays(1), _jwtSigningCredentials);
            token.Payload["channel_id"] = channelId;
            token.Payload["role"] = role;
            token.Payload["opaque_user_id"] = userId;
            token.Payload["iat"] = (int) iat.TotalSeconds;
            token.Payload["pubsub_perms"] = new
            {
                listen = new string[] { "broadcast", "global" }
            };

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetJWTToken(string channelId)
        {
            var exp = DateTimeOffset.UtcNow - EPOCH;

            var token = new JwtSecurityToken(null, null, null, null, DateTime.UtcNow.AddDays(1), _jwtSigningCredentials);
            token.Payload["channel_id"] = channelId;
            token.Payload["role"] = "external";
            token.Payload["pubsub_perms"] = new Dictionary<string, string[]>
            {
                { "listen", new string[] { "broadcast" } },
                { "send", new string[] { "broadcast", "global" } }
            };

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task BroadcastJson(string channelId, string jsonPayload)
        {
            var token = GetJWTToken(channelId);
            var requestBody = new
            {
                content_type = "application/json",
                message = jsonPayload,
                targets = new string[] { "broadcast" },
            };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var message = new HttpRequestMessage(HttpMethod.Post, $"message/{channelId}");
            message.Content = content;
            message.SetBearerToken(token);
            var response = await _twitchExtensionClient.SendAsync(message);
            response.EnsureSuccessStatusCode();
        }

        public async Task BroadcastJson(string channelId, object payload)
        {
            var contentStr = JsonSerializer.Serialize(payload);
            await BroadcastJson(channelId, contentStr);
        }
    }
}
