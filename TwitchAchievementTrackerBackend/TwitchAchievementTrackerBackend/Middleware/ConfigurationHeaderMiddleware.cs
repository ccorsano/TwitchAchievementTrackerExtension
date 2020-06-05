using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Helpers;
using TwitchAchievementTrackerBackend.Services;

namespace TwitchAchievementTrackerBackend.Middleware
{
    /// <summary>
    /// Extract the encrypted configuration Token coming for the broadcaster configuration store.
    /// Once extracted the decrypted and deserialized configuration is stored in the HttpContext.
    /// </summary>
    public class ConfigurationHeaderMiddleware
    {
        public class ConfigurationHeaderException : Exception
        {
            public ConfigurationHeaderException(string errorMessage)
                :base(errorMessage)
            {

            }
        }

        private readonly RequestDelegate _next;

        public ConfigurationHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("X-Config-Token"))
            {
                var token = context.Request.Headers["X-Config-Token"].First();
                var configService = context.RequestServices.GetRequiredService<ConfigurationTokenService>();

                try
                {
                    var configuration = configService.DecryptConfigurationToken(Convert.FromBase64String(token));
                    context.SetExtensionConfiguration(configuration);
                }
                catch (Exception)
                {
                    throw new ConfigurationHeaderException("Error reading configuration token");
                }
            }

            return _next.Invoke(context);
        }
    }
}
