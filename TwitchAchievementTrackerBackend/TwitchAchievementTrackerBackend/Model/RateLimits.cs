using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class RateLimits
    {
        public int HourlyLimit { get; set; }
        public int Remaining { get; set; }
        public DateTimeOffset ResetTime { get; set; }
    }
}
