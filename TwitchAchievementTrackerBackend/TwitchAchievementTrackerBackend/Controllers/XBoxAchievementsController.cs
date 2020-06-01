using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TwitchAchievementTrackerBackend.Model;
using TwitchAchievementTrackerBackend.Services;

namespace TwitchAchievementTrackerBackend.Controllers
{
    [Route("api/achievements")]
    [ApiController]
    [Authorize]
    public class XBoxAchievementsController : ControllerBase
    {
        public readonly XApiService _xApiService;

        public XBoxAchievementsController(XApiService xApiService)
        {
           _xApiService = xApiService;
        }

        [HttpGet("summary")]
        public async Task<AchievementSummary> GetSummary()
        {
            var achievements = await _xApiService.GetAchievementsAsync("2535467661815558", "1659804324");
            var stateSummary = achievements.GroupBy(a => a.ProgressState).ToDictionary(a => a.Key, a => a.Count());

            return new AchievementSummary
            {
                Total = achievements.Length,
                Completed = stateSummary.TryGetValue(ProgressState.Achieved, out int completed) ? completed : 0,
                InProgress = stateSummary.TryGetValue(ProgressState.InProgress, out int inProgress) ? inProgress : 0,
                NotStarted = stateSummary.TryGetValue(ProgressState.NotStarted, out int notStarted) ? notStarted : 0,
                CurrentPoints = achievements
                    .Where(a => a.ProgressState == ProgressState.Achieved)
                    .SelectMany(a => a.Rewards.Where(r => r.Type == "Gamerscore"))
                    .Sum(r => r.Value ?? 0),
                TotalPoints = achievements.SelectMany(a => a.Rewards.Where(r => r.Type == "Gamerscore")).Sum(r => r.Value ?? 0)
            };
        }

        [HttpGet("list")]
        public Task<XApiAchievement[]> GetAchievements()
        {
            return _xApiService.GetAchievementsAsync("2535467661815558", "1659804324");
        }
    }
}