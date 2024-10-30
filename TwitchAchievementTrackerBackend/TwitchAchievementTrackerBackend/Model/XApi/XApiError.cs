using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiError
    {
        public bool Success { get; set; }
        public int Error_Code { get; set; }
        [JsonPropertyName("message")]
        public string? Error_Message { get; set; }
    }
}
