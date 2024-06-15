using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using TwitchAchievementTrackerBackend.Configuration;
using TwitchAchievementTrackerBackend.Services;
using Xunit;

namespace TwitchAchievementTrackerBackendTests
{
    public class TwitchEBSServiceTest
    {
        private string GenerateTestSecret()
        {
            // Create a byte array to hold the random value.
            byte[] secret = new byte[32];
            
            using (var rngCsp = RandomNumberGenerator.Create())
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(secret);
            }
            return Convert.ToBase64String(secret);
        }

        [Fact]
        public void CanGenerateJWTTokens()
        {
            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient());
            var options = new OptionsWrapper<TwitchOptions>(new TwitchOptions
            {
                ClientId = "blah",
                ExtensionSecret = GenerateTestSecret(),
            });
            var logger = new LoggerFactory().CreateLogger<TwitchEBSService>();
            var service = new TwitchEBSService(factoryMock.Object, options, logger);

            var jwtToken = service.GetJWTToken("test_channel");


            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(options.Value.ExtensionSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(jwtToken);
            Assert.Equal(SecurityAlgorithms.HmacSha256, token.SignatureAlgorithm);
            Assert.Equal("test_channel", token.Payload["channel_id"]);
            var handler = new JwtSecurityTokenHandler();

            handler.ValidateToken(jwtToken, new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateTokenReplay = false,
            }, out var validToken);
        }
    }
}
