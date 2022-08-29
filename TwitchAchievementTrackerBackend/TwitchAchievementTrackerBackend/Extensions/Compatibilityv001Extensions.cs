using Google.FlatBuffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Services;
using TwitchAchievementTracker.Flatbuffer.v1;
using FlatSharp;

namespace TwitchAchievementTrackerBackend.Extensions
{
    public static class Compatibilityv001Extensions
    {
        public static byte[] EncodeConfigurationToken_v1(this ConfigurationTokenService service, Model.ExtensionConfiguration configuration)
        {
            byte[] payload = service.SerializeConfigurationToken_v1(configuration);

            // Encrypt the token
            return service.Encrypt(payload);
        }

        public static byte[] SerializeConfigurationToken_v1(this ConfigurationTokenService service, Model.ExtensionConfiguration configuration)
        {
            // Generate the Flatbuffer token payload
            var builder = new FlatBufferBuilder(512);

            var fbConfiguration = new TwitchAchievementTracker.Flatbuffer.v1.Configuration();
            fbConfiguration.version = configuration.Version;

            
            var xConfig = configuration.XBoxLiveConfig as Model.XApiConfiguration;
            var steamConfig = configuration.SteamConfig as Model.SteamConfiguration;

            switch (configuration.ActiveConfig)
            {
                case Model.ActiveConfig.XBoxLive:

                    var flatBufferXBoxConfig = new XApiConfiguration();
                    flatBufferXBoxConfig.xApiKey = xConfig.XApiKey;
                    flatBufferXBoxConfig.streamerXuid = string.IsNullOrEmpty(xConfig.StreamerXuid) ? 0 : UInt64.Parse(xConfig.StreamerXuid);
                    flatBufferXBoxConfig.titleId = string.IsNullOrEmpty(xConfig.TitleId) ? 0 : UInt32.Parse(xConfig.TitleId);
                    flatBufferXBoxConfig.locale = xConfig.Locale ?? "en-US";
                    fbConfiguration.config = new PlatformConfiguration(flatBufferXBoxConfig);

                    break;
                case Model.ActiveConfig.Steam:
                    var webKeyString = builder.CreateString(steamConfig.WebApiKey);
                    var steamId = UInt64.Parse(steamConfig.SteamId);
                    var appId = steamConfig.AppId;
                    var steamLocale = builder.CreateString(steamConfig.Locale ?? "english");
                    var flatBufferSteamConfig = new SteamConfiguration();
                    flatBufferSteamConfig.webApiKey = steamConfig.WebApiKey;
                    flatBufferSteamConfig.steamId = string.IsNullOrEmpty(steamConfig.SteamId) ? 0 : UInt64.Parse(steamConfig.SteamId);
                    flatBufferSteamConfig.appId = string.IsNullOrEmpty(steamConfig.AppId) ? 0 : UInt32.Parse(steamConfig.AppId);
                    fbConfiguration.config = new PlatformConfiguration(flatBufferSteamConfig);

                    break;
                default:
                    throw new NotSupportedException("Unknown configuration platform");
            }

            int maxBytesNeeded = FlatBufferSerializer.Default.GetMaxSize(fbConfiguration);
            byte[] outBuffer = new byte[maxBytesNeeded];
            int bytesWritten = FlatBufferSerializer.Default.Serialize(fbConfiguration, outBuffer);

            return new ArraySegment<byte>(outBuffer, 0, bytesWritten).ToArray();
        }

        // Legacy token decryption from published version v0.1.1
        public static Model.ExtensionConfiguration DecodeConfigurationToken_v1(this ConfigurationTokenService service, byte[] payload)
        {
            // Decrypt the token
            byte[] decrypted = service.Decrypt(payload);
            return service.DeserializeConfigurationToken_v1(decrypted);
        }

        public static Model.ExtensionConfiguration DeserializeConfigurationToken_v1(this ConfigurationTokenService service, byte[] decrypted)
        {
            var fbConfiguration = FlatBufferSerializer.Default.Parse<TwitchAchievementTracker.Flatbuffer.v1.Configuration>(decrypted);


            SteamConfiguration steamFbConfig = null;
            XApiConfiguration xApiFbConfig = null;

            Model.ActiveConfig activeConfig;
            if (fbConfiguration.config?.Kind == PlatformConfiguration.ItemKind.SteamConfiguration)
            {
                activeConfig = Model.ActiveConfig.Steam;
            }
            if (fbConfiguration.config?.Kind == PlatformConfiguration.ItemKind.XApiConfiguration)
            {
                activeConfig = Model.ActiveConfig.XBoxLive;
            }
            else
            {
                throw new NotSupportedException("Unknown configuration platform");
            }

            return new Model.ExtensionConfiguration
            {
                Version = fbConfiguration.version,
                ActiveConfig = activeConfig,
                SteamConfig = steamFbConfig == null ? null : new Model.SteamConfiguration
                {
                    WebApiKey = steamFbConfig.webApiKey,
                    SteamId = steamFbConfig.steamId == 0 ? null : steamFbConfig.steamId.ToString(),
                    AppId = steamFbConfig.appId == 0 ? null : steamFbConfig.appId.ToString(),
                    Locale = steamFbConfig.locale,
                },
                XBoxLiveConfig = xApiFbConfig == null ? null : new Model.XApiConfiguration
                {
                    XApiKey = xApiFbConfig.xApiKey,
                    StreamerXuid = xApiFbConfig.streamerXuid == 0 ? null : xApiFbConfig.streamerXuid.ToString(),
                    TitleId = xApiFbConfig.titleId == 0 ? null : xApiFbConfig.titleId.ToString(),
                    Locale = xApiFbConfig.locale,
                }
            };
        }
    }
}
