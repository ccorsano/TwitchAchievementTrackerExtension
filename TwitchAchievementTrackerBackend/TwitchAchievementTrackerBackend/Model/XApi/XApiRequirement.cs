using System.Text.Json.Serialization;
using System;
using TwitchAchievementTrackerBackend.Helpers;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiRequirement
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("current")]
        [JsonConverter(typeof(ParseStringConverter<long>))]
        public long Current { get; set; }

        [JsonPropertyName("target")]
        [JsonConverter(typeof(ParseStringConverter<long>))]
        public long Target { get; set; }

        [JsonPropertyName("operationType")]
        public string? OperationType { get; set; }

        [JsonPropertyName("valueType")]
        public string? ValueType { get; set; }

        [JsonPropertyName("ruleParticipationType")]
        public string? RuleParticipationType { get; set; }
    }
}
