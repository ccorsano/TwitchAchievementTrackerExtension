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
            fbConfiguration.Version = configuration.Version;

            
            var xConfig = configuration.XBoxLiveConfig as Model.XApiConfiguration;
            var steamConfig = configuration.SteamConfig as Model.SteamConfiguration;

            switch (configuration.ActiveConfig)
            {
                case Model.ActiveConfig.XBoxLive:
                    var flatBufferXBoxConfig = new XApiConfiguration();
                    flatBufferXBoxConfig.XApiKey = xConfig!.XApiKey;
                    flatBufferXBoxConfig.StreamerXuid = string.IsNullOrEmpty(xConfig.StreamerXuid) ? 0 : UInt64.Parse(xConfig.StreamerXuid);
                    flatBufferXBoxConfig.TitleId = string.IsNullOrEmpty(xConfig.TitleId) ? 0 : UInt32.Parse(xConfig.TitleId);
                    flatBufferXBoxConfig.Locale = xConfig.Locale ?? "en-US";
                    fbConfiguration.Config = new PlatformConfiguration(flatBufferXBoxConfig);

                    break;
                case Model.ActiveConfig.Steam:
                    var webKeyString = builder.CreateString(steamConfig!.WebApiKey);
                    var steamId = UInt64.Parse(steamConfig.SteamId!);
                    var appId = steamConfig.AppId;
                    var steamLocale = builder.CreateString(steamConfig.Locale ?? "english");
                    var flatBufferSteamConfig = new SteamConfiguration();
                    flatBufferSteamConfig.WebApiKey = steamConfig.WebApiKey;
                    flatBufferSteamConfig.SteamId = string.IsNullOrEmpty(steamConfig.SteamId) ? 0 : UInt64.Parse(steamConfig.SteamId);
                    flatBufferSteamConfig.AppId = string.IsNullOrEmpty(steamConfig.AppId) ? 0 : UInt32.Parse(steamConfig.AppId);
                    fbConfiguration.Config = new PlatformConfiguration(flatBufferSteamConfig);

                    break;
                default:
                    throw new NotSupportedException("Unknown configuration platform");
            }

            int maxBytesNeeded = TwitchAchievementTracker.Flatbuffer.v1.Configuration.Serializer.GetMaxSize(fbConfiguration);
            byte[] outBuffer = new byte[maxBytesNeeded];
            int bytesWritten = TwitchAchievementTracker.Flatbuffer.v1.Configuration.Serializer.Write(outBuffer, fbConfiguration);

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
            var fbConfiguration = TwitchAchievementTracker.Flatbuffer.v1.Configuration.Serializer.Parse(decrypted);


            SteamConfiguration? steamFbConfig = null;
            XApiConfiguration? xApiFbConfig = null;

            Model.ActiveConfig activeConfig;
            if (fbConfiguration.Config?.Kind == PlatformConfiguration.ItemKind.SteamConfiguration)
            {
                activeConfig = Model.ActiveConfig.Steam;
                steamFbConfig = fbConfiguration.Config?.SteamConfiguration;
            }
            if (fbConfiguration.Config?.Kind == PlatformConfiguration.ItemKind.XApiConfiguration)
            {
                activeConfig = Model.ActiveConfig.XBoxLive;
                xApiFbConfig= fbConfiguration.Config?.XApiConfiguration;
            }
            else
            {
                throw new NotSupportedException("Unknown configuration platform");
            }

            return new Model.ExtensionConfiguration
            {
                Version = fbConfiguration.Version,
                ActiveConfig = activeConfig,
                SteamConfig = steamFbConfig == null ? null : new Model.SteamConfiguration
                {
                    WebApiKey = steamFbConfig.WebApiKey,
                    SteamId = steamFbConfig.SteamId == 0 ? null : steamFbConfig.SteamId.ToString(),
                    AppId = steamFbConfig.AppId == 0 ? null : steamFbConfig.AppId.ToString(),
                    Locale = steamFbConfig.Locale,
                },
                XBoxLiveConfig = xApiFbConfig == null ? null : new Model.XApiConfiguration
                {
                    XApiKey = xApiFbConfig.XApiKey,
                    StreamerXuid = xApiFbConfig.StreamerXuid == 0 ? null : xApiFbConfig.StreamerXuid.ToString(),
                    TitleId = xApiFbConfig.TitleId == 0 ? null : xApiFbConfig.TitleId.ToString(),
                    Locale = xApiFbConfig.Locale,
                }
            };
        }
    }
}
