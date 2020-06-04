using Google.FlatBuffers;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Model;

namespace TwitchAchievementTrackerBackend.Services
{
    public class ConfigurationTokenService
    {
        private readonly ConfigurationTokenOptions _options;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public ConfigurationTokenService(IOptions<ConfigurationTokenOptions> options)
        {
            _options = options.Value;
            var keys = GetHashKeys(_options.EncryptionSecret);
            _key = keys[0];
            _iv = keys[1];

        }

        private Aes CreateAes()
        {
            var aes = new AesManaged();
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            return aes;
        }

        public byte[] EncryptConfigurationToken(ExtensionConfiguration configuration)
        {
            var builder = new FlatBufferBuilder(512);
            var apiKeyString = builder.CreateString(configuration.XApiKey);
            var xuid64 = UInt64.Parse(configuration.StreamerXuid);
            var titleId32 = UInt32.Parse(configuration.TitleId);
            var localeString = builder.CreateString(configuration.Locale ?? "en-US");
            var configOffset = TwitchAchievementTracker.Configuration.CreateConfiguration(builder, apiKeyString, xuid64, titleId32, localeString);
            builder.Finish(configOffset.Value);
            var payload = builder.SizedByteArray();

            //using (Aes myAes = CreateAes())
            //{
            //    return Encrypt(payload);
            //}
            return payload;
        }

        public ExtensionConfiguration DecryptConfigurationToken(byte[] payload)
        {
            byte[] decrypted = payload;
            //using (Aes myAes = CreateAes())
            //{
            //    decrypted = Decrypt(payload);
            //}

            var fbConfig = TwitchAchievementTracker.Configuration.GetRootAsConfiguration(new ByteBuffer(decrypted));
            var testKey = fbConfig.GetXApiKeyBytes();

            return new ExtensionConfiguration
            {
                XApiKey = fbConfig.XApiKey,
                StreamerXuid = fbConfig.StreamerXuid.ToString(),
                TitleId = fbConfig.TitleId.ToString(),
                Locale = fbConfig.Locale,
            };
        }

        byte[] Encrypt(byte[] payload)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (Aes aes = CreateAes())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(payload, 0, payload.Length);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }
        byte[] Decrypt(byte[] payload)
        {
            byte[] decrypted = new byte[payload.Length*2];
            // Create AesManaged    
            using (Aes aes = CreateAes())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(payload))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        cs.Read(decrypted, 0, payload.Length);
                    }
                }
            }
            return decrypted;
        }

        private byte[][] GetHashKeys(string key)
        {
            byte[][] result = new byte[2][];
            Encoding enc = Encoding.UTF8;

            SHA256 sha2 = new SHA256CryptoServiceProvider();

            byte[] rawKey = enc.GetBytes(key);
            byte[] rawIV = enc.GetBytes(key);

            byte[] hashKey = sha2.ComputeHash(rawKey);
            byte[] hashIV = sha2.ComputeHash(rawIV);

            Array.Resize(ref hashIV, 16);

            result[0] = hashKey;
            result[1] = hashIV;

            return result;
        }
    }
}
