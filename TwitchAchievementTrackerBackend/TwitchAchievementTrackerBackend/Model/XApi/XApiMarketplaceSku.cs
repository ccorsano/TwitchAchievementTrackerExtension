using System;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceSku
    {
        public DateTimeOffset LastModifiedDate { get; set; }

        public XApiMarketplaceSkuLocalizedProperty[] LocalizedProperties { get; set; }

        public XApiMarketplaceSkuMarketProperty[] MarketProperties { get; set; }

        public string ProductId { get; set; }

        public XApiMarketplaceSkuProperties Properties { get; set; }

        public string SkuASchema { get; set; }

        public string SkuBSchema { get; set; }

        public string SkuId { get; set; }

        public string SkuType { get; set; }

        //public object RecurrencePolicy { get; set; }

        //public object SubscriptionPolicyId { get; set; }
    }
}