using System;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceSkuProperties
    {
        public object EarlyAdopterEnrollmentUrl { get; set; }

        //public FulfillmentData FulfillmentData { get; set; }

        public object FulfillmentType { get; set; }

        public object FulfillmentPluginId { get; set; }

        public bool HasThirdPartyIaPs { get; set; }

        public DateTimeOffset LastUpdateDate { get; set; }

        //public HardwareProperties HardwareProperties { get; set; }

        //public object[] HardwareRequirements { get; set; }

        //public object[] HardwareWarningList { get; set; }

        public string InstallationTerms { get; set; }

        //public Package[] Packages { get; set; }

        public string VersionString { get; set; }

        public string[] SkuDisplayGroupIds { get; set; }

        public bool XboxXpa { get; set; }

        //public object[] BundledSkus { get; set; }

        public bool IsRepurchasable { get; set; }

        public long SkuDisplayRank { get; set; }

        //public object DisplayPhysicalStoreInventory { get; set; }

        //public object[] VisibleToB2BServiceIds { get; set; }

        //public object[] AdditionalIdentifiers { get; set; }

        public bool IsTrial { get; set; }

        public bool IsPreOrder { get; set; }

        public bool IsBundle { get; set; }
    }
}