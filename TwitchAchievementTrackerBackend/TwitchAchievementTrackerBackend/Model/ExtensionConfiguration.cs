using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class ExtensionConfiguration
    {
        public string Version { get; set; }
        public XApiConfiguration XBoxLiveConfig { get; set; }
        public SteamConfiguration SteamConfig { get; set; }
    }
}
