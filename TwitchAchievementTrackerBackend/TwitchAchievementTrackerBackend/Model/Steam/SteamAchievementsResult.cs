using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamAchievementsResult
    {
        public SteamAchievementsGame? Game { get; set; }
    }

    public class SteamAchievementsGame
    {
        public string? GameName { get; set; }

        public long GameVersion { get; set; }

        public SteamAchievementsAvailableGameStats? AvailableGameStats { get; set; }
    }

    public partial class SteamAchievementsAvailableGameStats
    {
        public SteamAchievement[] Achievements { get; set; } = new SteamAchievement[0];
    }
}
