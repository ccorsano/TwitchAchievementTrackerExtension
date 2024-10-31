using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Helpers;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProgressState
    {
        NotStarted,
        InProgress,
        Achieved,
    }

    public class XApiAchievement
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("serviceConfigId")]
        public Guid ServiceConfigId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("titleAssociations")]
        public XApiAchievementTitleAssociation[] TitleAssociations { get; set; } = new XApiAchievementTitleAssociation[0];

        [JsonPropertyName("progressState")]
        public ProgressState ProgressState { get; set; }

        [JsonPropertyName("progression")]
        public XApiAchievementProgression? Progression { get; set; }
        
        [JsonPropertyName("mediaAssets")]
        public XApiMediaAsset[] MediaAssets { get; set; } = new XApiMediaAsset[0];

        [JsonPropertyName("platforms")]
        public string[] Platforms { get; set; } = new string[0];

        [JsonPropertyName("isSecret")]
        public bool IsSecret { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("lockedDescription")]
        public string? LockedDescription { get; set; }

        [JsonPropertyName("productId")]
        public Guid ProductId { get; set; }

        [JsonPropertyName("achievementType")]
        public string? AchievementType { get; set; }

        [JsonPropertyName("participationType")]
        public string? ParticipationType { get; set; }

        [JsonPropertyName("timeWindow")]
        public object? TimeWindow { get; set; }

        [JsonPropertyName("rewards")]
        public XApiAchievementReward[] Rewards { get; set; } = new XApiAchievementReward[0];

        [JsonPropertyName("estimatedTime")]
        public TimeSpan? EstimatedTime { get; set; }

        [JsonPropertyName("deeplink")]
        public string? Deeplink { get; set; }

        [JsonPropertyName("isRevoked")]
        public bool IsRevoked { get; set; }

        [JsonPropertyName("rarity")]
        public XApiRarity? Rarity { get; set; }
    }
}
