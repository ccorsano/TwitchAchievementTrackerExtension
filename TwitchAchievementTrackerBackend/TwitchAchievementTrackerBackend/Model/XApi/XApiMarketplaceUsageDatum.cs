using System.Text.Json.Serialization;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceUsageDatum
    {
        public string AggregateTimeSpan { get; set; }

        public double AverageRating { get; set; }

        public long PlayCount { get; set; }

        public long RatingCount { get; set; }

        public string RentalCount { get; set; }

        public string TrialCount { get; set; }

        public string PurchaseCount { get; set; }
    }
}