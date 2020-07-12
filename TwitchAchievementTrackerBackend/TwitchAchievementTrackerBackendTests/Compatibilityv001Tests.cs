using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Extensions;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Services;
using Xunit;

namespace TwitchAchievementTrackerBackendTests
{
    public class Compatibilityv001Tests
    {
        [Fact]
        public void CheckLegacyTokenSerialization()
        {
            // Test deserialization from legacy encrypted token using known secret
            var encrypted = "X1Pmbd3v86Ck731UlGCnFfQ0zjhr0gNRe+fJARWIqtgk80IJfCm6HjmcGGifBndUr3AspJJldYnRHvOmsdj1UUWtjUw4ZK+AqVtIBz+YxbUipbKlcCypRCnsa0n1CLwBqFQd0Ci/9utTVFituJh1wl1VL52F8VWEqQEoNYTz+TKSj/ShJWKNodSkvK1US96F8RMpVHsJvz2M4OuisrRwTagSTW59U75Mz0KJ/hFkyu8=";
            var secret = "deadbeef1337deadbeef";

            var options = new OptionsWrapper<ConfigurationTokenOptions>(new ConfigurationTokenOptions
            {
                EncryptionSecret = secret
            });
            var loggerFactory = new LoggerFactory();
            
            var tokenService = new ConfigurationTokenService(options, loggerFactory.CreateLogger<ConfigurationTokenService>());

            var decryptedPayload = tokenService.Decrypt(Convert.FromBase64String(encrypted));
            var configurationIn = tokenService.DeserializeConfigurationToken_v1(decryptedPayload);

            // Test serialization from current version ExtensionConfiguration type
            var configuration = new ExtensionConfiguration
            {
                Version = "0.0.1",
                ActiveConfig = ActiveConfig.XBoxLive,
                SteamConfig = null,
                XBoxLiveConfig = new XApiConfiguration
                {
                    StreamerXuid = "2112233445566778",
                    TitleId = "123456",
                    XApiKey = "deadbeef1337551c773676461c1c1337deadbeef",
                    Locale = "fr-FR",
                },
            };
            var configurationOut = tokenService.SerializeConfigurationToken_v1(configuration);

            Assert.Equal(configuration.Version, configurationIn.Version);
            Assert.Equal(configuration.SteamConfig, configurationIn.SteamConfig);
            Assert.Equal(configuration.XBoxLiveConfig.XApiKey, configurationIn.XBoxLiveConfig.XApiKey);
            Assert.Equal(configuration.XBoxLiveConfig.StreamerXuid, configurationIn.XBoxLiveConfig.StreamerXuid);
            Assert.Equal(configuration.XBoxLiveConfig.TitleId, configurationIn.XBoxLiveConfig.TitleId);
            Assert.Equal(configuration.XBoxLiveConfig.Locale, configurationIn.XBoxLiveConfig.Locale);
        }
    }
}
