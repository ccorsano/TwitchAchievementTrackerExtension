using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Configuration
{
    public class SteamApiOptions
    {
        public TimeSpan ResultCacheTime { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan AppListCacheTime { get; set; } = TimeSpan.FromDays(1);
        public string WebApiKey { get; set; }
        public string TestSteamId { get; set; } = "76561199073493245";
    }
}
