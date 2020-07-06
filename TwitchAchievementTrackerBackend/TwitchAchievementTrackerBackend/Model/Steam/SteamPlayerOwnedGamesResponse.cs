using System.Text.Json.Serialization;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamPlayerOwnedGamesResponse
    {
        [JsonPropertyName("game_count")]
        public long GameCount { get; set; }

        [JsonPropertyName("games")]
        public SteamPlayerOwnedGameInfo[] Games { get; set; }
    }
}