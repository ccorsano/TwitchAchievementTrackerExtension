using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Helpers;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiAchievementReward
    {
        [JsonPropertyName("name")]
        public object Name { get; set; }

        [JsonPropertyName("description")]
        public object Description { get; set; }

        [JsonPropertyName("value")]
        [JsonConverter(typeof(ParseStringConverter<int>))]
        public int Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("mediaAsset")]
        public XApiMediaAsset MediaAsset { get; set; }

        [JsonPropertyName("valueType")]
        public string ValueType { get; set; }
    }
}
