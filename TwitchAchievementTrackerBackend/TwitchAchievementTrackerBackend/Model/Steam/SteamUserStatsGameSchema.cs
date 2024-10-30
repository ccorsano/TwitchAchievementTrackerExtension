using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamUserStatsGameSchema
    {
        public string? GameName { get; set; }

        public string? GameVersion { get; set; }

        public SteamGameSchemaAvailableStats? AvailableGameStats { get; set; }
    }
}
