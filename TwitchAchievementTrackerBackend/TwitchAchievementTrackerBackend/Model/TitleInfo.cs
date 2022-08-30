using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class TitleInfo
    {
        public ActiveConfig Platform { get; set; }
        public string TitleId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public string LogoUri { get; set; }
    }
}
