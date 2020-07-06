using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamPlayerSummariesResult
    {
        [JsonPropertyName("response")]
        public SteamPlayerSummariesResponse Response { get; set; }
    }
}
