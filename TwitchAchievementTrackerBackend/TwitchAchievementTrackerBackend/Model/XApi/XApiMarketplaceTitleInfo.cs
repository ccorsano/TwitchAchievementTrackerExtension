using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceTitleInfo
    {
        public string[] BigIds { get; set; } = new string[0];
        public bool HasMorePages { get; set; }
        public XApiMarketplaceProduct[] Products { get; set; } = new XApiMarketplaceProduct[0];
        public int TotalResultCount { get; set; }
    }
}
