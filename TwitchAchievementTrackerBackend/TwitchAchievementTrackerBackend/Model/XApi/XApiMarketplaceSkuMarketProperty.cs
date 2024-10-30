using System;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceSkuMarketProperty
    {
        public DateTimeOffset? FirstAvailableDate { get; set; }

        public string[] SupportedLanguages { get; set; } = new string[0];

        //public object PackageIds { get; set; }

        //public object PiFilter { get; set; }

        public string[] Markets { get; set; } = new string[0];
    }
}