using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceSearchResult
    {
        public string[] ProductIds { get; set; } = new string[0];

        public object[] Aggregations { get; set; } = new object[0];

        public bool HasMorePages { get; set; }

        public XApiMarketplaceProduct[] Products { get; set; } = new XApiMarketplaceProduct[0];

        public long TotalResultCount { get; set; }
    }
}
