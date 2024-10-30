using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamPlayerAchievement
    {
        public string? Apiname { get; set; }

        public long Achieved { get; set; }

        public long Unlocktime { get; set; }
    }
}
