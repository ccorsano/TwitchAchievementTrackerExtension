using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class XApiConfiguration : IPlatformConfiguration
    {
        public string XApiKey { get; set; }
        public string StreamerXuid { get; set; }
        public string TitleId { get; set; }
        public string Locale { get; set; }
    }
}
