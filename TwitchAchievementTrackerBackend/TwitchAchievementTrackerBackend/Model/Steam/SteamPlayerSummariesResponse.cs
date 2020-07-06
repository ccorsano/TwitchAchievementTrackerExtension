using System.Text.Json.Serialization;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamPlayerSummariesResponse
    {
        [JsonPropertyName("players")]
        public SteamPlayerSummary[] Players { get; set; }
    }
}