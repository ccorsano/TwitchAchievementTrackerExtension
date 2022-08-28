using System.Text.Json.Serialization;
using System;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiProfileForGamertag
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("hostId")]
        public string HostId { get; set; }

        [JsonPropertyName("settings")]
        public XApiProfileForGamertagSetting[] Settings { get; set; }

        [JsonPropertyName("isSponsoredUser")]
        public bool IsSponsoredUser { get; set; }
    }

    public partial class XApiProfileForGamertagSetting
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
