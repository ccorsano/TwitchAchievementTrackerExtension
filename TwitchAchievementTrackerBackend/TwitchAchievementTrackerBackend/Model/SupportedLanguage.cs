using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class SupportedLanguage
    {
        public string LangCode { get; set; }
        public string DisplayName { get; set; }
        public string DefaultRegion { get; set; }
    }
}
