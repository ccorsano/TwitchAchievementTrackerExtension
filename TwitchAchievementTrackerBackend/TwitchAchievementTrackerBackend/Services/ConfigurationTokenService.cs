using Google.FlatBuffers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchAchievementTracker;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Model.Steam;
using TwitchAchievementTrackerBackend.Model.XApi;

namespace TwitchAchievementTrackerBackend.Services
{
    public class ConfigurationTokenService
    {
        private readonly ConfigurationTokenOptions _options;
        private readonly ILogger _logger;

        public ConfigurationTokenService(IOptions<ConfigurationTokenOptions> options, ILogger<ConfigurationTokenService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Serialize and encrypt configuration object
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public byte[] EncodeConfigurationToken(ExtensionConfiguration configuration)
        {
            byte[] payload = SerializeConfigurationToken(configuration);

            // Encrypt the token
            return Encrypt(payload);
        }

        /// <summary>
        /// Serialize configuration object to a flatbuffer binary representation
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static byte[] SerializeConfigurationToken(ExtensionConfiguration configuration)
        {
            // Generate the Flatbuffer token payload
            var builder = new FlatBufferBuilder(512);

            // Flatbuffer serialization: we serialize inside-out and set buffer offsets
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
                var steamId = string.IsNullOrEmpty(steamConfig.SteamId) ? 0 : UInt64.Parse(steamConfig.SteamId);
                var appId = string.IsNullOrEmpty(steamConfig.AppId) ? 0 : uint.Parse(steamConfig.AppId);
                var steamLocale = builder.CreateString(steamConfig.Locale ?? "english");

                steamConfigOffset = TwitchAchievementTracker.SteamConfiguration.CreateSteamConfiguration(builder, webKeyString, appId, steamId, steamLocale);
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
            return payload;
        }

        /// <summary>
        /// Decrypt and deserialize configuration token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ExtensionConfiguration DecodeConfigurationToken(byte[] token)
        {
            // Decrypt the token
            byte[] decrypted = Decrypt(token);

            return DeserializeConfigurationToken(decrypted);
        }

        /// <summary>
        /// Deserialize a Flatbuffer configuration payload to an ExtensionConfiguration object
        /// </summary>
        /// <param name="decrypted">Flatbuffer token</param>
        /// <returns>Configuration object</returns>
        private static ExtensionConfiguration DeserializeConfigurationToken(byte[] decrypted)
        {
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

            switch (fbConfig.Active)
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

        /// <summary>
        /// Encrypt a binary payload using AES CBC
        /// </summary>
        /// <param name="payload">Binary payload to encrypt</param>
        /// <returns>Encrypted payload, 16bit salt as a prefix, then AES encrypted payload</returns>
        public byte[] Encrypt(byte[] payload)
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

        /// <summary>
        /// Decrypt an AES CBC binary payload
        /// </summary>
        /// <param name="payload">Encrypted payload, 16bit salt as a prefix, then AES encrypted payload</param>
        /// <returns>Decrypted buffer</returns>
        public byte[] Decrypt(byte[] payload)
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
