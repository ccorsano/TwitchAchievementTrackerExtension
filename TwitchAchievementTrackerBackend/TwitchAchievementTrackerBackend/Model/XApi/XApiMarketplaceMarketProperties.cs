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

        public XApiMarketplaceContentRating[] ContentRatings { get; set; } = new XApiMarketplaceContentRating[0];

        public XApiMarketplaceRelatedProduct[] RelatedProducts { get; set; } = new XApiMarketplaceRelatedProduct[0];

        public XApiMarketplaceUsageDatum[] UsageData { get; set; } = new XApiMarketplaceUsageDatum[0];

        public object? BundleConfig { get; set; }

        public string[] Markets { get; set; } = new string[0];
    }
}
