using System.Text.Json.Serialization;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiAchievementsResult
    {
        [JsonPropertyName("achievements")]
        public XApiAchievement[] Achievements { get; set; }

        [JsonPropertyName("pagingInfo")]
        public XApiPagingInfo PagingInfo { get; set; }
    }
}
