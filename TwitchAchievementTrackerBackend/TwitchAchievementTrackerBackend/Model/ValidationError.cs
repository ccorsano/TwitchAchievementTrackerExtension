using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class ValidationError
    {
        public string? Path { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorDescription { get; set; }
    }
}
