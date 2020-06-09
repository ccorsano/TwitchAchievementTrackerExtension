using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceTitleInfo
    {
        public string[] BigIds { get; set; }
        public bool HasMorePages { get; set; }
        public XApiMarketplaceProduct[] Products { get; set; }
        public int TotalResultCount { get; set; }
    }
}
