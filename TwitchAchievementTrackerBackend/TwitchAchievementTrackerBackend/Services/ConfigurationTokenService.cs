using FlatSharp;
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
            var configurationRoot = new TwitchAchievementTracker.Flatbuffer.Configuration();

            configurationRoot.active = configuration.ActiveConfig switch
            {
                ActiveConfig.Steam => TwitchAchievementTracker.Flatbuffer.ActiveConfiguration.SteamConfiguration,
                ActiveConfig.XBoxLive => TwitchAchievementTracker.Flatbuffer.ActiveConfiguration.XApiConfiguration,
                _ => throw new NotSupportedException(),
            };
            configurationRoot.version = configuration.Version;
            if (configuration.XBoxLiveConfig != null)
            {
                var xApiFbConfig = new TwitchAchievementTracker.Flatbuffer.XApiConfiguration();
                xApiFbConfig.xApiKey = configuration.XBoxLiveConfig.XApiKey;
                xApiFbConfig.streamerXuid = string.IsNullOrEmpty(configuration.XBoxLiveConfig.StreamerXuid) ? 0 : UInt64.Parse(configuration.XBoxLiveConfig.StreamerXuid);
                xApiFbConfig.titleId = string.IsNullOrEmpty(configuration.XBoxLiveConfig.TitleId) ? 0 : UInt32.Parse(configuration.XBoxLiveConfig.TitleId);
                xApiFbConfig.locale = configuration.XBoxLiveConfig.Locale ?? "en-US";
                configurationRoot.xBoxLiveConfig = xApiFbConfig;
            }
            if (configuration.SteamConfig != null)
            {
                var steamFbConfig = new TwitchAchievementTracker.Flatbuffer.SteamConfiguration();
                steamFbConfig.webApiKey = configuration.SteamConfig.WebApiKey;
                steamFbConfig.steamId = string.IsNullOrEmpty(configuration.SteamConfig.SteamId) ? 0 : UInt64.Parse(configuration.SteamConfig.SteamId);
                steamFbConfig.appId = string.IsNullOrEmpty(configuration.SteamConfig.AppId) ? 0 : uint.Parse(configuration.SteamConfig.AppId);
                steamFbConfig.locale = configuration.SteamConfig.Locale ?? "english";
                configurationRoot.steamConfig = steamFbConfig;
            }

            int maxBytesNeeded = FlatBufferSerializer.Default.GetMaxSize(configurationRoot);
            byte[] outBuffer = new byte[maxBytesNeeded];
            int bytesWritten = FlatBufferSerializer.Default.Serialize(configurationRoot, outBuffer);

            return new ArraySegment<byte>(outBuffer, 0, bytesWritten).ToArray();
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
            var fbConfiguration = FlatBufferSerializer.Default.Parse<TwitchAchievementTracker.Flatbuffer.Configuration>(decrypted);

            return new ExtensionConfiguration
            {
                Version = fbConfiguration.version,
                ActiveConfig = fbConfiguration.active switch
                {
                    TwitchAchievementTracker.Flatbuffer.ActiveConfiguration.SteamConfiguration => ActiveConfig.Steam,
                    TwitchAchievementTracker.Flatbuffer.ActiveConfiguration.XApiConfiguration => ActiveConfig.XBoxLive,
                    _ => throw new NotSupportedException("Unknown type"),
                },
                XBoxLiveConfig = fbConfiguration.xBoxLiveConfig == null ? null : new XApiConfiguration
                {
                    XApiKey = fbConfiguration.xBoxLiveConfig.xApiKey,
                    StreamerXuid = fbConfiguration.xBoxLiveConfig.streamerXuid == 0 ? null : fbConfiguration.xBoxLiveConfig.streamerXuid.ToString(),
                    TitleId = fbConfiguration.xBoxLiveConfig.titleId == 0 ? null : fbConfiguration.xBoxLiveConfig.titleId.ToString(),
                    Locale = fbConfiguration.xBoxLiveConfig.locale,
                },
                SteamConfig = fbConfiguration.steamConfig == null ? null : new SteamConfiguration
                {
                    WebApiKey = fbConfiguration.steamConfig.webApiKey,
                    SteamId = fbConfiguration.steamConfig.steamId == 0 ? null : fbConfiguration.steamConfig.steamId.ToString(),
                    AppId = fbConfiguration.steamConfig.appId == 0 ? null : fbConfiguration.steamConfig.appId.ToString(),
                    Locale = fbConfiguration.steamConfig.locale,
                },
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
