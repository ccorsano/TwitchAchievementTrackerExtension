using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamResolveVanityUrlResult
    {
        [JsonPropertyName("response")]
        public SteamVanityUrlResolution? Response { get; set; }
    }
}
