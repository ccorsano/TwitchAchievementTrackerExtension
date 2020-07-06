using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Extensions;
using TwitchAchievementTrackerBackend.Helpers;
using TwitchAchievementTrackerBackend.Model;
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
            public ConfigurationHeaderException(string errorMessage, Exception innerException)
                :base(errorMessage, innerException)
            {

            }
        }

        private readonly RequestDelegate _next;

        public ConfigurationHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Middleware Invoke method.
        /// Read custom HTTP headers X-Config-Version, X-Config-Token, decode the configuration token,
        /// and inject the resulting configuration object into the HTTP context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task InvokeAsync(HttpContext context)
        {
            var version = "0.0.1";
            if (context.Request.Headers.ContainsKey("X-Config-Version"))
            {
                version = context.Request.Headers["X-Config-Version"].First();
            }
            if (context.Request.Headers.ContainsKey("X-Config-Token"))
            {   
                var token = context.Request.Headers["X-Config-Token"].First();

                if (! string.IsNullOrEmpty(token))
                {
                    var configService = context.RequestServices.GetRequiredService<ConfigurationTokenService>();

                    ExtensionConfiguration configuration = null;
                    try
                    {
                        switch (version)
                        {
                            // Legacy versions
                            case "0.0.1":
                                configuration = configService.DecodeConfigurationToken_v1(Convert.FromBase64String(token));
                                break;

                            // Current version
                            case "0.0.2":
                            case "0.0.3":
                                configuration = configService.DecodeConfigurationToken(Convert.FromBase64String(token));
                                break;

                            default:
                                throw new NotSupportedException($"Configuration version {version} is not supported on this EBS");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ConfigurationHeaderException("Error reading configuration token", ex);
                    }
                    context.SetExtensionConfiguration(configuration);
                }
            }

            return _next.Invoke(context);
        }
    }
}
