using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamAchievement
    {
        public string? Name { get; set; }

        public long Defaultvalue { get; set; }

        public string? DisplayName { get; set; }

        public long Hidden { get; set; }

        public string? Description { get; set; }

        public Uri? Icon { get; set; }

        public Uri? Icongray { get; set; }
    }
}
