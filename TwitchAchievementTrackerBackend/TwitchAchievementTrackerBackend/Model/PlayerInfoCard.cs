using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class PlayerInfoCard
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
