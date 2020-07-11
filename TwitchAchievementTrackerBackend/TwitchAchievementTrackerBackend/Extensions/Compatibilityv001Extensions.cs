using Google.FlatBuffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Services;
using TwitchAchievementTracker_v1;
using FlatSharp;

namespace TwitchAchievementTrackerBackend.Extensions
{
    public static class Compatibilityv001Extensions
    {
        public static byte[] EncodeConfigurationToken_v1(this ConfigurationTokenService service, ExtensionConfiguration configuration)
        {
            byte[] payload = service.SerializeConfigurationToken_v1(configuration);

            // Encrypt the token
            return service.Encrypt(payload);
        }

        public static byte[] SerializeConfigurationToken_v1(this ConfigurationTokenService service, ExtensionConfiguration configuration)
        {
            // Generate the Flatbuffer token payload
            var builder = new FlatBufferBuilder(512);

            var configuration = new TwitchAchievementTracker.Flatbuffer.Configuration();

            Offset<TwitchAchievementTracker_v1.Configuration> configurationOffset;
            var version = builder.CreateString(configuration.Version);

            var xConfig = configuration.XBoxLiveConfig as Model.XApiConfiguration;
            var steamConfig = configuration.SteamConfig as Model.SteamConfiguration;

            switch (configuration.ActiveConfig)
            {
                case ActiveConfig.XBoxLive:

                    var flatBufferXBoxConfig = new TwitchAchievementTracker.Flatbuffer.XApiConfiguration();
                    flatBufferXBoxConfig.xApiKey = xConfig.XApiKey;
                    flatBufferXBoxConfig.streamerXuid = string.IsNullOrEmpty(xConfig.StreamerXuid) ? 0 : UInt64.Parse(xConfig.StreamerXuid);
                    flatBufferXBoxConfig.titleId = string.IsNullOrEmpty(xConfig.TitleId) ? 0 : UInt32.Parse(xConfig.TitleId);
                    flatBufferXBoxConfig.locale = xConfig.Locale ?? "en-US";

                    var xConfigOffset = TwitchAchievementTracker_v1.XApiConfiguration.CreateXApiConfiguration(builder, apiKeyString, xuid64, titleId32, liveLocale);
                    configurationOffset = TwitchAchievementTracker_v1.Configuration.CreateConfiguration(builder, version, TwitchAchievementTracker_v1.PlatformConfiguration.XApiConfiguration, xConfigOffset.Value);

                    break;
                case ActiveConfig.Steam:
                    var webKeyString = builder.CreateString(steamConfig.WebApiKey);
                    var steamId = UInt64.Parse(steamConfig.SteamId);
                    var appId = steamConfig.AppId;
                    var steamLocale = builder.CreateString(steamConfig.Locale ?? "english");

                    var steamConfigOffset = TwitchAchievementTracker_v1.SteamConfiguration.CreateSteamConfiguration(builder, webKeyString, uint.Parse(appId), steamId, steamLocale);
                    configurationOffset = TwitchAchievementTracker_v1.Configuration.CreateConfiguration(builder, version, TwitchAchievementTracker_v1.PlatformConfiguration.SteamConfiguration, steamConfigOffset.Value);
                    break;
                default:
                    throw new NotSupportedException("Unknown configuration platform");
            }

            builder.Finish(configurationOffset.Value);
            return builder.SizedByteArray();
        }

        // Legacy token decryption from published version v0.1.1
        public static ExtensionConfiguration DecodeConfigurationToken_v1(this ConfigurationTokenService service, byte[] payload)
        {
            // Decrypt the token
            byte[] decrypted = service.Decrypt(payload);
            return service.DeserializeConfigurationToken_v1(decrypted);
        }

        public static ExtensionConfiguration DeserializeConfigurationToken_v1(this ConfigurationTokenService service, byte[] decrypted)
        {

            IPlatformConfiguration platformConfiguration;
            ActiveConfig activeConfig;

            // Initialize Flatbuffer
            var fbConfig = TwitchAchievementTracker_v1.Configuration.GetRootAsConfiguration(new ByteBuffer(decrypted));
            switch (fbConfig.ConfigType)
            {
                case TwitchAchievementTracker_v1.PlatformConfiguration.XApiConfiguration:

                    var xConfig = fbConfig.Config<TwitchAchievementTracker_v1.XApiConfiguration>().GetValueOrDefault();

                    activeConfig = ActiveConfig.XBoxLive;
                    platformConfiguration = new Model.XApiConfiguration
                    {
                        XApiKey = xConfig.XApiKey,
                        StreamerXuid = xConfig.StreamerXuid == 0 ? null : xConfig.StreamerXuid.ToString(),
                        TitleId = xConfig.TitleId == 0 ? null : xConfig.TitleId.ToString(),
                        Locale = xConfig.Locale,
                    };
                    break;
                case TwitchAchievementTracker_v1.PlatformConfiguration.SteamConfiguration:

                    var steamConfig = fbConfig.Config<TwitchAchievementTracker_v1.SteamConfiguration>().GetValueOrDefault();

                    activeConfig = ActiveConfig.Steam;
                    platformConfiguration = new Model.SteamConfiguration
                    {
                        WebApiKey = steamConfig.WebApiKey,
                        SteamId = steamConfig.SteamId == 0 ? null : steamConfig.SteamId.ToString(),
                        AppId = steamConfig.AppId == 0 ? null : steamConfig.AppId.ToString(),
                        Locale = steamConfig.Locale,
                    };
                    break;
                default:
                    throw new NotSupportedException("Unknown type");
            }

            return new ExtensionConfiguration
            {
                Version = fbConfig.Version,
                ActiveConfig = activeConfig,
                XBoxLiveConfig = platformConfiguration as Model.XApiConfiguration,
                SteamConfig = platformConfiguration as Model.SteamConfiguration
            };
        }
    }
}
