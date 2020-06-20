using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
    }
}