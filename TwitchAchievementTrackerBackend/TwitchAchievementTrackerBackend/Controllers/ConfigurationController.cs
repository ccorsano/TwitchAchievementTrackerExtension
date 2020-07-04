using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Helpers;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Model.Steam;
using TwitchAchievementTrackerBackend.Services;

namespace TwitchAchievementTrackerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConfigurationController : ControllerBase
    {
        private ConfigurationTokenService _service;

        public ConfigurationController(ConfigurationTokenService service)
        {
            _service = service;
        }

        [HttpGet("")]
        public ExtensionConfiguration GetConfiguration()
        {
            return this.GetExtensionConfiguration();
        }

        [HttpPost("")]
        public ConfigurationToken SetConfiguration(ExtensionConfiguration configuration)
        {
            return new ConfigurationToken
            {
                ConfigToken = Convert.ToBase64String(_service.EncodeConfigurationToken(configuration))
            };
        }

        [HttpGet("languages")]
        public Task<SupportedLanguage[]> SupportedLanguages(ActiveConfig activeConfig, string gameId)
        {
            ExtensionConfiguration configuration = new ExtensionConfiguration
            {
                ActiveConfig = activeConfig,
            };
            if (HttpContext.HasExtensionConfiguration())
            {
                configuration = HttpContext.GetExtensionConfiguration();
            }
            configuration.ActiveConfig = activeConfig;
            switch (activeConfig)
            {
                case ActiveConfig.XBoxLive:
                    configuration.XBoxLiveConfig.TitleId = gameId;
                    break;
                case ActiveConfig.Steam:
                    configuration.SteamConfig.AppId = gameId;
                    break;
                default:
                    break;
            }
            return _service.GetSupportedLanguages(configuration);
        }

        [HttpPost("validate")]
        public Task<ValidationError[]> ValidateConfiguration(ExtensionConfiguration configuration)
        {
            return _service.ValidateConfiguration(configuration);
        }

        [HttpGet("steam/resolveVanity")]
        public Task<PlayerInfoCard> ResolveSteamProfileUrl(string vanityUrl, string webApiKey = null)
        {
            return _service.ResolveSteamProfileUrl(vanityUrl, webApiKey);
        }

        [HttpGet("steam/playerInfo")]
        public Task<PlayerInfoCard> GetSteamPlayerInfo(string steamid, string webApiKey = null)
        {
            return _service.GetPlayerInfo(steamid, webApiKey);
        }

        [HttpGet("steam/{steamId}/ownedGames")]
        public async Task<TitleInfo[]> GetSteamOwnedGames(string steamId, string webApiKey = null)
        {
            var ownedGames = await _service.GetSteamOwnedGames(steamId, webApiKey);
            return ownedGames.Select(game => new TitleInfo
            {
                TitleId = game.AppId.ToString(),
                LogoUri = game.ImgLogoUrl,
                ProductTitle = game.Name,
                ProductDescription = "",
            }).ToArray();
        }
    }
}