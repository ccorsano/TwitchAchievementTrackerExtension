using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.Configure<XApiOptions>(Configuration.GetSection("xapi"));
            services.Configure<TwitchOptions>(Configuration.GetSection("twitch"));
            services.AddSingleton<XApiService>();
            services.AddMemoryCache();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretBase64 = Configuration.GetValue<string>("twitch:ExtensionSecret");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(secretBase64)),
                        ValidateAudience = false, // No audience on extension tokens
                        ValidateIssuer = false, // No issuer either
                    };
                });
                //.AddOpenIdConnect(options =>
                //{
                //    options.Authority = "https://id.twitch.tv";
                //    options.ClientId = "04ijuz20mxlw1wk5b6alh6l4mb48zu";
                //    options.ClientSecret = "IHp/3dvc00rXf0DFL1IQhFYT2AN3jNFKpZvwDlOZKAw=";
                //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }

            //app.UseHttpsRedirection();

            app.UseCors(config =>
            {
                config.AllowAnyOrigin();
                config.WithHeaders("Authorization");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
