using System.Text.Json.Serialization;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiAchievementsResult
    {
        [JsonPropertyName("achievements")]
        public XApiAchievement[] Achievements { get; set; } = new XApiAchievement[0];

        [JsonPropertyName("pagingInfo")]
        public XApiPagingInfo? PagingInfo { get; set; }
    }
}
