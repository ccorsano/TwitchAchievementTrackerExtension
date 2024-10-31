using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamPlayerOwnedGameInfo
    {
        [JsonPropertyName("appid")]
        public long AppId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("playtime_forever")]
        public long PlaytimeForever { get; set; }

        [JsonPropertyName("img_icon_url")]
        public string? ImgIconUrl { get; set; }

        [JsonPropertyName("img_logo_url")]
        public string? ImgLogoUrl { get; set; }

        [JsonPropertyName("has_community_visible_stats")]
        public bool? HasCommunityVisibleStats { get; set; }

        [JsonPropertyName("playtime_windows_forever")]
        public long PlaytimeWindowsForever { get; set; }

        [JsonPropertyName("playtime_mac_forever")]
        public long PlaytimeMacForever { get; set; }

        [JsonPropertyName("playtime_linux_forever")]
        public long PlaytimeLinuxForever { get; set; }

        [JsonPropertyName("playtime_2weeks")]
        public long? Playtime2Weeks { get; set; }

        [JsonPropertyName("library_tile_url")]
        public string? LibraryTileUrl { get; set; }
    }
}
