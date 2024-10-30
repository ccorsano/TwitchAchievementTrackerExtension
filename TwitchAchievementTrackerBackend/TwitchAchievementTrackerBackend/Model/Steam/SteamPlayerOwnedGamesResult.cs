using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamPlayerOwnedGamesResult
    {
        [JsonPropertyName("response")]
        public SteamPlayerOwnedGamesResponse? Response { get; set; }
    }
}
