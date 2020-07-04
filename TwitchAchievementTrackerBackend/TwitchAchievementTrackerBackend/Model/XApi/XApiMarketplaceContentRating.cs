namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceContentRating
    {
        public string RatingSystem { get; set; }

        public string RatingId { get; set; }

        public string[] RatingDescriptors { get; set; }

        public object[] RatingDisclaimers { get; set; }

        public object[] InteractiveElements { get; set; }
    }
}