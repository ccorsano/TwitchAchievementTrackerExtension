using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiTitleHistoryResult
    {
        [JsonPropertyName("xuid")]
        public string? Xuid { get; set; }

        [JsonPropertyName("titles")]
        public XApiTitleInfo[] Titles { get; set; } = new XApiTitleInfo[0];
    }
}
