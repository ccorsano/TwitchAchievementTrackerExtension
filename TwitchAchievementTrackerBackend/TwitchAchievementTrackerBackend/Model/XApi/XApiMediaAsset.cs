using System.Text.Json.Serialization;
using System;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMediaAsset
    {
        [JsonPropertyName("name")]
        public Guid Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("url")]
        public Uri? Url { get; set; }
    }
}
