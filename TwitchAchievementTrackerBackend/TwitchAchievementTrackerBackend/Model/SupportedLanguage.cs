using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class SupportedLanguage
    {
        public required string LangCode { get; init; }
        public required string DisplayName { get; init; }
        public string? DefaultRegion { get; init; }
    }
}
