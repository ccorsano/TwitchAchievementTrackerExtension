namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceDisplaySkuAvailability
    {
        public XApiMarketplaceSku Sku { get; set; }

        public XApiMarketplaceAvailability[] Availabilities { get; set; }
    }
}