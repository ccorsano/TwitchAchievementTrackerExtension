using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiRarity
    {
        [JsonPropertyName("currentCategory")]
        public string CurrentCategory { get; set; }

        [JsonPropertyName("currentPercentage")]
        public double CurrentPercentage { get; set; }
    }
}
