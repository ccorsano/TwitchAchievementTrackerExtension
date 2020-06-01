using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class XApiMarketplaceLocalizedProperties
    {
        public string DeveloperName { get; set; }

        public string PublisherName { get; set; }

        public string PublisherWebsiteUri { get; set; }

        public Uri SupportUri { get; set; }

        //public EligibilityProperties EligibilityProperties { get; set; }

        //public object[] Franchises { get; set; }

        public XApiMarketplaceImage[] Images { get; set; }

        //public Video[] Videos { get; set; }

        public string ProductDescription { get; set; }

        public string ProductTitle { get; set; }

        public string ShortTitle { get; set; }

        public string SortTitle { get; set; }

        public object FriendlyTitle { get; set; }

        public string ShortDescription { get; set; }

        //public SearchTitle[] SearchTitles { get; set; }

        public string VoiceTitle { get; set; }

        public object RenderGroupDetails { get; set; }

        public object[] ProductDisplayRanks { get; set; }

        public object InteractiveModelConfig { get; set; }

        public bool Interactive3DEnabled { get; set; }

        public string Language { get; set; }

        public string[] Markets { get; set; }
    }
}
