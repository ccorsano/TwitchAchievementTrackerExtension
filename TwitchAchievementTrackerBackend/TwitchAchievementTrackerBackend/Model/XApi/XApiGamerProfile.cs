﻿using System.Text.Json.Serialization;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public partial class XApiGamerProfile
    {
        [JsonPropertyName("xuid")]
        public string? Xuid { get; set; }

        [JsonPropertyName("isFavorite")]
        public bool IsFavorite { get; set; }

        [JsonPropertyName("isFollowingCaller")]
        public bool IsFollowingCaller { get; set; }

        [JsonPropertyName("isFollowedByCaller")]
        public bool IsFollowedByCaller { get; set; }

        [JsonPropertyName("isIdentityShared")]
        public bool IsIdentityShared { get; set; }

        [JsonPropertyName("addedDateTimeUtc")]
        public object? AddedDateTimeUtc { get; set; }

        [JsonPropertyName("displayName")]
        public object? DisplayName { get; set; }

        [JsonPropertyName("realName")]
        public string? RealName { get; set; }

        [JsonPropertyName("displayPicRaw")]
        public Uri? DisplayPicRaw { get; set; }

        [JsonPropertyName("showUserAsAvatar")]
        public string? ShowUserAsAvatar { get; set; }

        [JsonPropertyName("gamertag")]
        public string? Gamertag { get; set; }

        [JsonPropertyName("gamerScore")]
        public string? GamerScore { get; set; }

        [JsonPropertyName("modernGamertag")]
        public string? ModernGamertag { get; set; }

        [JsonPropertyName("modernGamertagSuffix")]
        public string? ModernGamertagSuffix { get; set; }

        [JsonPropertyName("uniqueModernGamertag")]
        public string? UniqueModernGamertag { get; set; }

        [JsonPropertyName("xboxOneRep")]
        public string? XboxOneRep { get; set; }

        [JsonPropertyName("presenceState")]
        public string? PresenceState { get; set; }

        [JsonPropertyName("presenceText")]
        public string? PresenceText { get; set; }

        [JsonPropertyName("presenceDevices")]
        public object? PresenceDevices { get; set; }

        [JsonPropertyName("isBroadcasting")]
        public bool IsBroadcasting { get; set; }

        [JsonPropertyName("isCloaked")]
        public bool IsCloaked { get; set; }

        [JsonPropertyName("isQuarantined")]
        public bool IsQuarantined { get; set; }

        [JsonPropertyName("isXbox360Gamerpic")]
        public bool IsXbox360Gamerpic { get; set; }

        [JsonPropertyName("lastSeenDateTimeUtc")]
        public object? LastSeenDateTimeUtc { get; set; }

        [JsonPropertyName("suggestion")]
        public object? Suggestion { get; set; }

        [JsonPropertyName("recommendation")]
        public object? Recommendation { get; set; }

        [JsonPropertyName("search")]
        public object? Search { get; set; }

        [JsonPropertyName("titleHistory")]
        public object? TitleHistory { get; set; }

        [JsonPropertyName("multiplayerSummary")]
        public XApiGamerProfileMultiplayerSummary? MultiplayerSummary { get; set; }

        [JsonPropertyName("recentPlayer")]
        public object? RecentPlayer { get; set; }

        [JsonPropertyName("follower")]
        public object? Follower { get; set; }

        [JsonPropertyName("preferredColor")]
        public XApiGamerProfilePreferredColor? PreferredColor { get; set; }

        [JsonPropertyName("presenceDetails")]
        public object[] PresenceDetails { get; set; } = new object[0];

        [JsonPropertyName("titlePresence")]
        public object? TitlePresence { get; set; }

        [JsonPropertyName("titleSummaries")]
        public object? TitleSummaries { get; set; }

        [JsonPropertyName("presenceTitleIds")]
        public object? PresenceTitleIds { get; set; }

        [JsonPropertyName("detail")]
        public XApiGamerProfileDetail? Detail { get; set; }

        [JsonPropertyName("communityManagerTitles")]
        public object? CommunityManagerTitles { get; set; }

        [JsonPropertyName("socialManager")]
        public object? SocialManager { get; set; }

        [JsonPropertyName("broadcast")]
        public object? Broadcast { get; set; }

        [JsonPropertyName("tournamentSummary")]
        public object? TournamentSummary { get; set; }

        [JsonPropertyName("avatar")]
        public object? Avatar { get; set; }

        [JsonPropertyName("linkedAccounts")]
        public object[] LinkedAccounts { get; set; } = new object[0];

        [JsonPropertyName("colorTheme")]
        public string? ColorTheme { get; set; }

        [JsonPropertyName("preferredFlag")]
        public string? PreferredFlag { get; set; }

        [JsonPropertyName("preferredPlatforms")]
        public object[] PreferredPlatforms { get; set; } = new object[0];
    }

    public partial class XApiGamerProfileDetail
    {
        [JsonPropertyName("accountTier")]
        public string? AccountTier { get; set; }

        [JsonPropertyName("bio")]
        public object? Bio { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("location")]
        public object? Location { get; set; }

        [JsonPropertyName("tenure")]
        public object? Tenure { get; set; }

        [JsonPropertyName("watermarks")]
        public object[] Watermarks { get; set; } = new object[0];

        [JsonPropertyName("blocked")]
        public bool Blocked { get; set; }

        [JsonPropertyName("mute")]
        public bool Mute { get; set; }

        [JsonPropertyName("followerCount")]
        public long FollowerCount { get; set; }

        [JsonPropertyName("followingCount")]
        public long FollowingCount { get; set; }

        [JsonPropertyName("hasGamePass")]
        public bool HasGamePass { get; set; }
    }

    public partial class XApiGamerProfileMultiplayerSummary
    {
        [JsonPropertyName("InMultiplayerSession")]
        public long InMultiplayerSession { get; set; }

        [JsonPropertyName("InParty")]
        public long InParty { get; set; }
    }

    public partial class XApiGamerProfilePreferredColor
    {
        [JsonPropertyName("primaryColor")]
        public string? PrimaryColor { get; set; }

        [JsonPropertyName("secondaryColor")]
        public string? SecondaryColor { get; set; }

        [JsonPropertyName("tertiaryColor")]
        public string? TertiaryColor { get; set; }
    }
}
