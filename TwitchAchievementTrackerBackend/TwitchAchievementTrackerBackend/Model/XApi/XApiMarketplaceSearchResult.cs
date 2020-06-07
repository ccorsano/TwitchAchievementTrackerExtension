using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceSearchResult
    {
        public string[] ProductIds { get; set; }

        public object[] Aggregations { get; set; }

        public bool HasMorePages { get; set; }

        public XApiMarketplaceProduct[] Products { get; set; }

        public long TotalResultCount { get; set; }
    }
}
