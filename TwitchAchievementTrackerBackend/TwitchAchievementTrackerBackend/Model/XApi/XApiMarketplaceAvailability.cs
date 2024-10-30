using System;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceAvailability
    {
        public string[] Actions { get; set; } = new string[0];

        public string? AvailabilityASchema { get; set; }

        public string? AvailabilityBSchema { get; set; }

        public string? AvailabilityId { get; set; }

        //public Conditions Conditions { get; set; }

        public DateTimeOffset LastModifiedDate { get; set; }

        public string[] Markets { get; set; } = new string[0];

        //public OrderManagementData OrderManagementData { get; set; }

        //public AvailabilityProperties Properties { get; set; }

        public string? SkuId { get; set; }

        public long DisplayRank { get; set; }

        //public AlternateId[] AlternateIds { get; set; }

        public bool RemediationRequired { get; set; }

        //public AvailabilityRemediation[] Remediations { get; set; }

        //public LicensingData LicensingData { get; set; }

        public string? AffirmationId { get; set; }
    }
}