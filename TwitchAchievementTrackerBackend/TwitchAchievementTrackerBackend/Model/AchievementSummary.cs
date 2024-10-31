using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class AchievementSummary
    {
        public string? GameName { get; set; }
        public int Total { get; set; }
        public int Completed { get; set; }
        public int InProgress { get; set; }
        public int NotStarted { get; set; }
        public int CurrentPoints { get; set; }
        public int TotalPoints { get; set; }
    }
}
