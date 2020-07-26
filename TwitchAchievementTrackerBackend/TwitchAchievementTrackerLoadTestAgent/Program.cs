using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NBomber;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Services;

namespace TwitchAchievementTrackerLoadTestAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configureDelegate =>
                {
                    configureDelegate.AddUserSecrets(Assembly.GetExecutingAssembly());
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddLogging();
                    services.AddHostedService<TestAgent>();
                    services.AddHttpClient();
                    services.AddSingleton<TwitchEBSService>();
                    services.Configure<TwitchOptions>(hostContext.Configuration.GetSection("twitch"));
                });
    }
}
