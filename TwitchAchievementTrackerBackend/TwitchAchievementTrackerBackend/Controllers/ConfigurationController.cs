using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Helpers;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Services;

namespace TwitchAchievementTrackerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConfigurationController : ControllerBase
    {
        private ConfigurationService _service;
        private ConfigurationTokenService _tokenService;

        public ConfigurationController(ConfigurationTokenService tokenService, ConfigurationService service)
        {
            _service = service;
            _tokenService = tokenService;
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
                ConfigToken = Convert.ToBase64String(_tokenService.EncodeConfigurationToken(configuration))
            };
        }

        [HttpPost("validate")]
        public Task<ValidationError[]> ValidateConfiguration(ExtensionConfiguration configuration)
        {
            return _service.ValidateConfiguration(configuration);
        }

        [HttpGet("steam/resolveVanity/{vanityUrl}")]
        public Task<PlayerInfoCard> ResolveSteamProfileUrl(string vanityUrl, string webApiKey = null)
        {
            return _service.ResolveSteamProfileUrl(vanityUrl, webApiKey);
        }

        [HttpGet("steam/playerInfo/{steamid}")]
        public Task<PlayerInfoCard> GetSteamPlayerInfo(string steamid, string webApiKey = null)
        {
            return _service.GetSteamPlayerInfo(steamid, webApiKey);
        }

        [HttpGet("steam/{steamId}/ownedGames")]
        public async Task<TitleInfo[]> GetSteamOwnedGames(string steamId, string webApiKey = null)
        {
            var ownedGames = await _service.GetSteamOwnedGames(steamId, webApiKey);
            return ownedGames
                .OrderByDescending(game => game.PlaytimeForever)
                .Select(game => new TitleInfo
            {
                TitleId = game.AppId.ToString(),
                LogoUri = game.ImgLogoUrl,
                ProductTitle = game.Name,
                ProductDescription = "",
            }).ToArray();
        }

        [HttpGet("steam/languages/{gameid}")]
        public Task<SupportedLanguage[]> GetSteamGameSupportedLanguages(string gameId, string webApiKey = null)
        {
            return _service.GetSteamSupportedLanguages(gameId, webApiKey);
        }

        [HttpGet("xapi/gamertag/{gamertag}")]
        public Task<PlayerInfoCard> ResolveXBoxLiveGamerTag(string gamertag, string xApiKey = null)
        {
            return _service.ResolveXBoxLiveGamertag(gamertag, xApiKey);
        }

        [HttpGet("xapi/playerInfo/{xuid}")]
        public Task<PlayerInfoCard> GetXBoxLivePlayerInfo(string xuid, string xApiKey = null)
        {
            return _service.GetXBoxLivePlayerInfo(xuid, xApiKey);
        }

        [HttpGet("xapi/titleInfo/{titleId}")]
        public Task<TitleInfo> GetXBoxLiveTitleInfo(string titleId, string xApiKey = null)
        {
            return _service.GetXBoxLiveTitleInfo(titleId, xApiKey);
        }

        [HttpGet("xapi/recentTitles/{xuid}")]
        public Task<TitleInfo[]> GetRecentTitles(string xuid, string xApiKey = null)
        {
            return _service.GetXBoxLiveRecentTitles(xuid, xApiKey);
        }

        [HttpGet("xapi/languages/{titleId}")]
        public Task<SupportedLanguage[]> GetXBoxLiveGameSupportedLanguages(string titleId, string xApiKey = null)
        {
            return _service.GetXBoxLiveSupportedLanguages(titleId, xApiKey);
        }
    }
}