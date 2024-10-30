using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XUIDInfo
    {
        public UInt64 Xuid { get; set; }
        public string? GamerTag { get; set; }
    }
}
