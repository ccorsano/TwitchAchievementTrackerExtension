using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiTitleInfo
    {
        [JsonPropertyName("titleId")]
        public string TitleId { get; set; }

        [JsonPropertyName("pfn")]
        public string Pfn { get; set; }

        [JsonPropertyName("bingId")]
        public Guid? BingId { get; set; }

        [JsonPropertyName("serviceConfigId")]
        public Guid? ServiceConfigId { get; set; }

        [JsonPropertyName("windowsPhoneProductId")]
        public Guid? WindowsPhoneProductId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("devices")]
        public string[] Devices { get; set; }

        [JsonPropertyName("displayImage")]
        public Uri DisplayImage { get; set; }

        [JsonPropertyName("mediaItemType")]
        public string MediaItemType { get; set; }

        [JsonPropertyName("modernTitleId")]
        public string ModernTitleId { get; set; }

        [JsonPropertyName("isBundle")]
        public bool IsBundle { get; set; }

        [JsonPropertyName("achievement")]
        public Achievement Achievement { get; set; }

        //[JsonPropertyName("stats")]
        //public object Stats { get; set; }

        //[JsonPropertyName("gamePass")]
        //public object GamePass { get; set; }

        [JsonPropertyName("images")]
        public XApiTitleInfoImage[] Images { get; set; }

        [JsonPropertyName("titleHistory")]
        public XApiTitleHistorySummary TitleHistory { get; set; }

        //[JsonPropertyName("detail")]
        //public object Detail { get; set; }

        //[JsonPropertyName("friendsWhoPlayed")]
        //public object FriendsWhoPlayed { get; set; }

        //[JsonPropertyName("alternateTitleIds")]
        //public object AlternateTitleIds { get; set; }

        [JsonPropertyName("contentBoards")]
        public object ContentBoards { get; set; }

        [JsonPropertyName("xboxLiveTier")]
        public string XboxLiveTier { get; set; }
    }
}
