using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamVanityUrlResolution
    {
        [JsonPropertyName("steamid")]
        public string SteamId { get; set; }

        [JsonPropertyName("success")]
        public uint Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
