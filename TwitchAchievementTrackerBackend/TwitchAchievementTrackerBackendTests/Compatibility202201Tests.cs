using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Services;
using Xunit;
using Xunit.Abstractions;

namespace TwitchAchievementTrackerBackendTests
{
    public class Compatibility202201Tests
    {
        private readonly ITestOutputHelper _out;

        public Compatibility202201Tests(ITestOutputHelper output)
        {
            _out = output;
        }

        [Fact]
        public void CheckCompatibility()
		{
            string token = "l1I9C/i8GrqKAH4t2VR94fLYDmigHHhRfTyoN7g+uIKc/X7yWs3OYkvJiaFIPmcA0dITPc2Yg5nD47aEb8qXeQfqLl09wUbpYAXIz6HiP8IutHvrBi6LhOa/neBelmcRWa9Gx6lHvXgsNBJCDi5dJi0s3ZAECPIxpE9eDCPu8HX+EhWjSQCHZxsKSv78x2ueRsdhDCXxDyZcNq48pTAgQbXOFLpvllBxilvEOAqFBWWqVZa6WWgvzra5x8OuM7GWHrwnrjWefBksDed8/QBV3DEz8DB6LBYVN/zg/uA+3+b3QEHzm7K1gVMjQ3irJ28QYCgjxO+7hHHwUTn97yeheg==";
            string secret = "deadbeef1337deadbeef";

            var options = new OptionsWrapper<ConfigurationTokenOptions>(new ConfigurationTokenOptions
            {
                EncryptionSecret = secret
            });
            var loggerFactory = new LoggerFactory();
            var tokenService = new ConfigurationTokenService(options, loggerFactory.CreateLogger<ConfigurationTokenService>());
            var decodedToken = tokenService.DecodeConfigurationToken(Convert.FromBase64String(token));

            Assert.Equal(TwitchAchievementTrackerBackend.Model.ActiveConfig.Steam, decodedToken.ActiveConfig);
            Assert.Equal("2020.4", decodedToken.Version);
            Assert.Equal("962130", decodedToken.SteamConfig.AppId);
            Assert.Equal("77777777777777777", decodedToken.SteamConfig.SteamId);
            Assert.Equal("4DEADBEEF47DEADBEEF0123456789012", decodedToken.SteamConfig.WebApiKey);
            Assert.Equal("english", decodedToken.SteamConfig.Locale);

            options.Value.EncryptionSecret = "deadbeef1337deadbeef";
            tokenService = new ConfigurationTokenService(options, loggerFactory.CreateLogger<ConfigurationTokenService>());
            var encodedToken = tokenService.EncodeConfigurationToken(decodedToken);

            Assert.NotEqual(token, Convert.ToBase64String(encodedToken));
        }
    }
}
