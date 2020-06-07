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
            var version = builder.CreateString("0.0.1");

            var xConfig = configuration.XBoxLiveConfig as Model.XApiConfiguration;
            var steamConfig = configuration.SteamConfig as Model.SteamConfiguration;

            if (xConfig != null)
            {
                var apiKeyString = builder.CreateString(xConfig.XApiKey);
                var xuid64 = UInt64.Parse(xConfig.StreamerXuid);
                var titleId32 = UInt32.Parse(xConfig.TitleId);
                var localeString = builder.CreateString(xConfig.Locale ?? "en-US");

                var xConfigOffset = TwitchAchievementTracker.XApiConfiguration.CreateXApiConfiguration(builder, apiKeyString, xuid64, titleId32, localeString);
                configurationOffset = TwitchAchievementTracker.Configuration.CreateConfiguration(builder, version, TwitchAchievementTracker.PlatformConfiguration.XApiConfiguration, xConfigOffset.Value);
            }
            else if (steamConfig != null)
            {
                var webKeyString = builder.CreateString(steamConfig.WebApiKey);
                var steamId = UInt64.Parse(steamConfig.SteamId);
                var appId = UInt32.Parse(steamConfig.AppId);
                var localeString = builder.CreateString(steamConfig.Locale ?? "en-US");

                var steamConfigOffset = TwitchAchievementTracker.SteamConfiguration.CreateSteamConfiguration(builder, webKeyString, appId, steamId, localeString);
                configurationOffset = TwitchAchievementTracker.Configuration.CreateConfiguration(builder, version, TwitchAchievementTracker.PlatformConfiguration.SteamConfiguration, steamConfigOffset.Value);
            }
            else
            {
                throw new NotSupportedException("Unknown configuration platform");
            }

            builder.Finish(configurationOffset.Value);
            var payload = builder.SizedByteArray();

            // Encrypt the token
            return Encrypt(payload);
        }

        public ExtensionConfiguration DecryptConfigurationToken(byte[] payload)
        {
            // Decrypt the token
            byte[] decrypted = Decrypt(payload);

            IPlatformConfiguration platformConfiguration;

            // Initialize Flatbuffer
            var fbConfig = TwitchAchievementTracker.Configuration.GetRootAsConfiguration(new ByteBuffer(decrypted));
            switch(fbConfig.ConfigType)
            {
                case PlatformConfiguration.XApiConfiguration:

                    var xConfig = fbConfig.Config<TwitchAchievementTracker.XApiConfiguration>().GetValueOrDefault();

                    platformConfiguration = new Model.XApiConfiguration
                    {
                        XApiKey = xConfig.XApiKey,
                        StreamerXuid = xConfig.StreamerXuid.ToString(),
                        TitleId = xConfig.TitleId.ToString(),
                        Locale = xConfig.Locale,
                    };
                    break;
                case PlatformConfiguration.SteamConfiguration:

                    var steamConfig = fbConfig.Config<TwitchAchievementTracker.SteamConfiguration>().GetValueOrDefault();

                    platformConfiguration = new Model.SteamConfiguration
                    {
                        WebApiKey = steamConfig.WebApiKey,
                        SteamId = steamConfig.SteamId.ToString(),
                        AppId = steamConfig.AppId.ToString(),
                        Locale = steamConfig.Locale,
                    };
                    break;
                default:
                    throw new NotSupportedException("Unknown type");
            }

            return new ExtensionConfiguration
            {
                Version = fbConfig.Version,
                XBoxLiveConfig = platformConfiguration as Model.XApiConfiguration,
                SteamConfig = platformConfiguration as Model.SteamConfiguration
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
