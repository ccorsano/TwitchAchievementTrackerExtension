using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class SteamConfiguration : IPlatformConfiguration
    {
        public string WebApiKey { get; set; }
        public string SteamId { get; set; }
        public string AppId { get; set; }
        public string Locale { get; set; }
    }
}
