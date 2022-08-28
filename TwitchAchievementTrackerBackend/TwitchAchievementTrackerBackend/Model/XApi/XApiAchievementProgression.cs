using System.Text.Json.Serialization;
using System;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiAchievementProgression
    {
        [JsonPropertyName("requirements")]
        public XApiRequirement[] Requirements { get; set; }

        [JsonPropertyName("timeUnlocked")]
        public DateTimeOffset TimeUnlocked { get; set; }
    }
}
