using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceMarketProperties
    {
        public DateTimeOffset OriginalReleaseDate { get; set; }

        public long MinimumUserAge { get; set; }

        public XApiMarketplaceContentRating[] ContentRatings { get; set; }

        public XApiMarketplaceRelatedProduct[] RelatedProducts { get; set; }

        public XApiMarketplaceUsageDatum[] UsageData { get; set; }

        public object BundleConfig { get; set; }

        public string[] Markets { get; set; }
    }
}
