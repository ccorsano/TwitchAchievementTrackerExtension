using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
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
        public uint Id { get; set; }
        public string Name { get; set; }
        public ProgressState ProgressState { get; set; }
        public XApiAchievementProgression Progression { get; set; }
        public string Description { get; set; }
        public string LockedDescription { get; set; }
        public XApiRarity Rarity { get; set; }
        public XApiAchievementReward[] Rewards { get; set; }
    }
}
