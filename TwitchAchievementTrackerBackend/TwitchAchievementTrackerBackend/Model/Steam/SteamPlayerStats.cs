using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamPlayerStats
    {
        public string SteamId { get; set; }

        public string GameName { get; set; }

        public SteamPlayerAchievement[] Achievements { get; set; }

        public bool Success { get; set; }
    }
}
