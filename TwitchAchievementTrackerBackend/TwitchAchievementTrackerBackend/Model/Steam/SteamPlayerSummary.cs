using System;
using System.Text.Json.Serialization;

namespace TwitchAchievementTrackerBackend.Model.Steam
{
    public class SteamPlayerSummary
    {
        [JsonPropertyName("steamid")]
        public string? SteamId { get; set; }

        [JsonPropertyName("communityvisibilitystate")]
        public long CommunityVisibilityState { get; set; }

        [JsonPropertyName("profilestate")]
        public long ProfileState { get; set; }

        [JsonPropertyName("personaname")]
        public string? PersonaName { get; set; }

        [JsonPropertyName("profileurl")]
        public Uri? ProfileUrl { get; set; }

        [JsonPropertyName("avatar")]
        public Uri? Avatar { get; set; }

        [JsonPropertyName("avatarmedium")]
        public Uri? AvatarMedium { get; set; }

        [JsonPropertyName("avatarfull")]
        public Uri? AvatarFull { get; set; }

        [JsonPropertyName("avatarhash")]
        public string? AvatarHash { get; set; }

        [JsonPropertyName("lastlogoff")]
        public long LastLogOff { get; set; }

        [JsonPropertyName("personastate")]
        public long PersonaState { get; set; }

        [JsonPropertyName("realname")]
        public string? RealName { get; set; }

        [JsonPropertyName("primaryclanid")]
        public string? PrimaryClanId { get; set; }

        [JsonPropertyName("timecreated")]
        public long TimeCreated { get; set; }

        [JsonPropertyName("personastateflags")]
        public long PersonaStateFlags { get; set; }

        [JsonPropertyName("loccountrycode")]
        public string? LocCountryCode { get; set; }

        [JsonPropertyName("locstatecode")]
        public string? LocStateCode { get; set; }

        [JsonPropertyName("loccityid")]
        public long LocCityId { get; set; }
    }
}