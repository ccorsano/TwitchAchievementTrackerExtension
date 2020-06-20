using Google.FlatBuffers;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TwitchAchievementTracker;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Model;

namespace TwitchAchievementTrackerBackend.Services
{
    public class ConfigurationTokenService
    {
        private readonly ConfigurationTokenOptions _options;

        public ConfigurationTokenService(IOptions<ConfigurationTokenOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Generates a 128bit random salt using the system provided crypto RNG
        /// </summary>
        /// <returns>16 bytes salt</returns>
        private byte[] GenerateSalt()
        {
            // Create a byte array to hold the random value.
            byte[] salt = new byte[16];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(salt);
            }
            return salt;
        }

        /// <summary>
        /// Initialize an AES cipher using a random salt and the configured EncryptionSecret
        /// </summary>
        /// <param name="salt"></param>
        /// <returns></returns>
        private Aes CreateAes(byte[] salt)
        {
            var rfc = new Rfc2898DeriveBytes(_options.EncryptionSecret, salt);
            byte[] Key = rfc.GetBytes(16);
            byte[] IV = rfc.GetBytes(16);

            var aes = Aes.Create();
            aes.Padding = PaddingMode.ISO10126;
            aes.Key = Key;
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;
            return aes;
        }

        public byte[] EncryptConfigurationToken(ExtensionConfiguration configuration)
        {
            // Generate the Flatbuffer token payload
            var builder = new FlatBufferBuilder(512);

            Offset<TwitchAchievementTracker.Configuration> configurationOffset;
            var version = builder.CreateString(configuration.Version);

            var xConfig = configuration.XBoxLiveConfig as Model.XApiConfiguration;
            var steamConfig = configuration.SteamConfig as Model.SteamConfiguration;

            ActiveConfiguration activeConfig;
            Offset<TwitchAchievementTracker.XApiConfiguration> xConfigOffset = new Offset<TwitchAchievementTracker.XApiConfiguration>();
            Offset<TwitchAchievementTracker.SteamConfiguration> steamConfigOffset = new Offset<TwitchAchievementTracker.SteamConfiguration>();

            if (xConfig != null)
            {
                var apiKeyString = builder.CreateString(xConfig.XApiKey);
                var xuid64 = string.IsNullOrEmpty(xConfig.StreamerXuid) ? 0 : UInt64.Parse(xConfig.StreamerXuid);
                var titleId32 = string.IsNullOrEmpty(xConfig.TitleId) ? 0 : UInt32.Parse(xConfig.TitleId);
                var liveLocale = builder.CreateString(xConfig.Locale ?? "en-US");
                xConfigOffset = TwitchAchievementTracker.XApiConfiguration.CreateXApiConfiguration(builder, apiKeyString, xuid64, titleId32, liveLocale);
            }

            if (steamConfig != null)
            {
                var webKeyString = builder.CreateString(steamConfig.WebApiKey);
                var steamId = UInt64.Parse(steamConfig.SteamId);
                var appId = steamConfig.AppId;
                var steamLocale = builder.CreateString(steamConfig.Locale ?? "english");

                steamConfigOffset = TwitchAchievementTracker.SteamConfiguration.CreateSteamConfiguration(builder, webKeyString, uint.Parse(appId), steamId, steamLocale);
            }

            switch (configuration.ActiveConfig)
            {
                case ActiveConfig.XBoxLive:
                    activeConfig = ActiveConfiguration.XApiConfiguration;
                    break;
                case ActiveConfig.Steam:
                    activeConfig = ActiveConfiguration.SteamConfiguration;
                    break;
                default:
                    throw new NotSupportedException("Unknown configuration platform");
            }

            configurationOffset = TwitchAchievementTracker.Configuration.CreateConfiguration(builder, version, activeConfig, xConfigOffset, steamConfigOffset);

            builder.Finish(configurationOffset.Value);
            var payload = builder.SizedByteArray();

            // Encrypt the token
            return Encrypt(payload);
        }

        // Legacy token decryption from published version v0.1.1
        public ExtensionConfiguration DecryptConfigurationToken_v1(byte[] payload)
        {
            // Decrypt the token
            byte[] decrypted = Decrypt(payload);

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

        public ExtensionConfiguration DecryptConfigurationToken(byte[] payload)
        {
            // Decrypt the token
            byte[] decrypted = Decrypt(payload);

            ActiveConfig activeConfig;
            Model.XApiConfiguration xApiConfiguration = null;
            Model.SteamConfiguration steamConfiguration = null;

            // Initialize Flatbuffer
            var fbConfig = TwitchAchievementTracker.Configuration.GetRootAsConfiguration(new ByteBuffer(decrypted));

            if (fbConfig.XBoxLiveConfig.HasValue)
            {
                var xConfig = fbConfig.XBoxLiveConfig.Value;
                xApiConfiguration = new Model.XApiConfiguration
                {
                    XApiKey = xConfig.XApiKey,
                    StreamerXuid = xConfig.StreamerXuid == 0 ? null : xConfig.StreamerXuid.ToString(),
                    TitleId = xConfig.TitleId == 0 ? null : xConfig.TitleId.ToString(),
                    Locale = xConfig.Locale,
                };
            }

            if (fbConfig.SteamConfig.HasValue)
            {
                var steamConfig = fbConfig.SteamConfig.Value;

                steamConfiguration = new Model.SteamConfiguration
                {
                    WebApiKey = steamConfig.WebApiKey,
                    SteamId = steamConfig.SteamId == 0 ? null : steamConfig.SteamId.ToString(),
                    AppId = steamConfig.AppId == 0 ? null : steamConfig.AppId.ToString(),
                    Locale = steamConfig.Locale,
                };
            }

            switch(fbConfig.Active)
            {
                case ActiveConfiguration.XApiConfiguration:
                    activeConfig = ActiveConfig.XBoxLive;
                    break;
                case ActiveConfiguration.SteamConfiguration:
                    activeConfig = ActiveConfig.Steam;
                    break;
                default:
                    throw new NotSupportedException("Unknown type");
            }

            return new ExtensionConfiguration
            {
                Version = fbConfig.Version,
                ActiveConfig = activeConfig,
                XBoxLiveConfig = xApiConfiguration,
                SteamConfig = steamConfiguration,
            };
        }

        byte[] Encrypt(byte[] payload)
        {
            // Generate salt to derive secret Key and Initialization Vectory, will store the salt as a prefix of the payload
            var salt = GenerateSalt();
            // Create Aes, will derive Key and IV
            using (Aes aes = CreateAes(salt))
            {
                // Create encryptor with generated Key and IV
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                // Encrypt message
                byte[] encryptedMessage = encryptor.TransformFinalBlock(payload, 0, payload.Length);
                // Create output with extra space to store salt as prefix
                byte[] output = new byte[aes.IV.Length + encryptedMessage.Length];
                // Store salt as prefix to the encrypted payload
                Array.Copy(salt, output, salt.Length);
                // Copy message payload
                Array.Copy(encryptedMessage, 0, output, salt.Length, encryptedMessage.Length);

                return output;
            }
        }

        byte[] Decrypt(byte[] payload)
        {
            // Extract salt prefix to derive secret Key and IV
            byte[] salt = payload.Take(16).ToArray();
            using (Aes aes = CreateAes(salt))
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                // Decrypt the message payload
                return decryptor.TransformFinalBlock(payload, salt.Length, payload.Length - salt.Length);
            }
        }
    }
}
