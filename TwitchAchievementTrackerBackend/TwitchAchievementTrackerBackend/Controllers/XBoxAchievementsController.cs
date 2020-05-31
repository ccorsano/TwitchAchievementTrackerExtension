using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Services;

namespace TwitchAchievementTrackerBackend.Controllers
{
    [Route("api/achievements")]
    [ApiController]
    public class XBoxAchievementsController : ControllerBase
    {
        public readonly XApiService _xApiService;

        public XBoxAchievementsController(XApiService xApiService)
        {
           _xApiService = xApiService;
        }

        [HttpGet("list")]
        public Task<XApiAchievement[]> GetAchievements()
        {
            return _xApiService.GetAchievementsAsync("2535467661815558", "1659804324");
        }
    }
}