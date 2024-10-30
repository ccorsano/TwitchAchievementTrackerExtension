using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiTitleInfoImage
    {
        [JsonPropertyName("url")]
        public Uri? Url { get; set; }

        [JsonPropertyName("type")]
        public XApiImageType Type { get; set; }
    }
}
