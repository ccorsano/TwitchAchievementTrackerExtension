using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class XApiMarketplaceProduct
    {
        public class IdRecord
        {
            public string IdType { get; set; }
            public string Value { get; set; }
        }

        public DateTimeOffset LastModifiedDate { get; set; }
        public string ProductKind { get; set; }
        public string ProductType { get; set; }
        public string ProductFamily { get; set; }
        public IdRecord[] AlternateIds { get; set; }
        public XApiMarketplaceLocalizedProperties[] LocalizedProperties { get; set; }

    }
}
