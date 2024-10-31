using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Configuration
{
    public class XApiOptions
    {
        public TimeSpan ResultCacheTime { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan StaticDataCacheTime { get; set; } = TimeSpan.FromHours(1);
        public string? XApiAccount { get; set; }
        public string? XApiKey { get; set; }
    }
}
