namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceContentRating
    {
        public string? RatingSystem { get; set; }

        public string? RatingId { get; set; }

        public string[] RatingDescriptors { get; set; } = new string[0];

        public object[] RatingDisclaimers { get; set; } = new object[0];

        public object[] InteractiveElements { get; set; } = new object[0];
    }
}