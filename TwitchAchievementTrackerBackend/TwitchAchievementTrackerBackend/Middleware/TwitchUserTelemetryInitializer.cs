using IdentityModel.Client;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Middleware
{
    public class TwitchUserTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HashAlgorithm _sessionHashAlgorithm;

        public TwitchUserTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionHashAlgorithm = MD5.Create();
        }

        public void Initialize(ITelemetry telemetry)
        {
            var ctx = _httpContextAccessor.HttpContext;

            // If telemetry initializer is called as part of request execution and not from some async thread
            if (ctx != null)
            {
                if (ctx.User?.Identity != null)
                {
                    var userId = ctx.User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
                    var opaqueUserId =  ctx.User.Claims.FirstOrDefault(c => c.Type == "opaque_user_id")?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        telemetry.Context.User.Id = userId;
                        telemetry.Context.User.AuthenticatedUserId = userId;
                    }
                    else if (!string.IsNullOrEmpty(opaqueUserId))
                    {
                        telemetry.Context.User.Id = opaqueUserId;
                    }

                    var channelId = ctx.User.Claims.FirstOrDefault(c => c.Type == "channel_id")?.Value;
                    if (!string.IsNullOrEmpty(channelId))
                    {
                        telemetry.Context.GlobalProperties["channel_id"] = channelId;
                    }

                    if (ctx.Request.Headers.ContainsKey("Authorization"))
                    {
                        var authzHeader = ctx.Request.Headers["Authorization"];
                        var hashBytes = _sessionHashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(authzHeader));
                        telemetry.Context.Session.Id = Convert.ToBase64String(hashBytes);
                    }
                }
            }
        }
    }
}
