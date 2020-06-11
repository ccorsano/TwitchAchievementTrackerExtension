using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Helpers;
using TwitchAchievementTrackerBackend.Middleware;
using TwitchAchievementTrackerBackend.Services;

namespace TwitchAchievementTrackerBackend
{
    class TestMessageHandler : HttpMessageHandler
    {
        private readonly HttpClient _httpClient;
        
        public TestMessageHandler()
        {
            _httpClient = new HttpClient();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _httpClient.SendAsync(request, cancellationToken);
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container. 
        // Ceci est un commentaire
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRazorPages();
            services.AddHttpClient();
            services.Configure<XApiOptions>(Configuration.GetSection("xapi"));
            services.Configure<SteamApiOptions>(Configuration.GetSection("steam"));
            services.Configure<TwitchOptions>(Configuration.GetSection("twitch"));
            services.Configure<ConfigurationTokenOptions>(Configuration.GetSection("config"));
            services.AddSingleton<XApiService>();
            services.AddSingleton<SteamApiService>();
            services.AddSingleton<ConfigurationTokenService>();
            services.AddMemoryCache();

            services.AddApplicationInsightsTelemetry();

            // For development mode
            if (Configuration.GetValue<bool>("config:IgnoreAuthentication", false))
            {
                // Disable authentication and authorization.
                services.AddSingleton<IPolicyEvaluator, DisableAuthenticationPolicyEvaluator>();
            }
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretsBase64 = new List<string>();
                    Configuration.GetSection("twitch:ExtensionSecrets").Bind(secretsBase64);
                    var signingKeys = secretsBase64.Select(base64Secret => new SymmetricSecurityKey(Convert.FromBase64String(base64Secret)));
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKeys = signingKeys,
                        ValidateAudience = false, // No audience on extension tokens
                        ValidateIssuer = false, // No issuer either
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }

            // Note: I am deploying this behind an HTTPS reverse proxy, so the HTTPs redirection is handled there.
            //app.UseHttpsRedirection();

            app.UseCors(config =>
            {
                if (env.IsDevelopment())
                {
                    config.AllowAnyOrigin();
                }
                else
                {
                    config.WithOrigins("https://*.ext-twitch.tv")
                        .SetIsOriginAllowedToAllowWildcardSubdomains();
                }
                config.WithHeaders("Authorization", "X-Config-Token", "Content-Type");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            // Register Twitch configuration middleware
            app.UseMiddleware<ConfigurationHeaderMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
