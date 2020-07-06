using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiGamerCard
    {
        [JsonPropertyName("gamertag")]
        public string Gamertag { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("gamerscore")]
        public long Gamerscore { get; set; }

        [JsonPropertyName("tier")]
        public string Tier { get; set; }

        [JsonPropertyName("motto")]
        public string Motto { get; set; }

        [JsonPropertyName("avatarBodyImagePath")]
        public Uri AvatarBodyImagePath { get; set; }

        [JsonPropertyName("gamerpicSmallImagePath")]
        public string GamerpicSmallImagePath { get; set; }

        [JsonPropertyName("gamerpicLargeImagePath")]
        public string GamerpicLargeImagePath { get; set; }

        [JsonPropertyName("gamerpicSmallSslImagePath")]
        public string GamerpicSmallSslImagePath { get; set; }

        [JsonPropertyName("gamerpicLargeSslImagePath")]
        public string GamerpicLargeSslImagePath { get; set; }

        [JsonPropertyName("avatarManifest")]
        public byte[] AvatarManifest { get; set; }
    }
}
