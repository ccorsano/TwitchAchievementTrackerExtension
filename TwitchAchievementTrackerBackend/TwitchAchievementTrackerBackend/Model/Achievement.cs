using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class Achievement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; }
        public DateTimeOffset UnlockTime { get; set; }
        public string Description { get; set; }

        //public XApiRarity Rarity { get; set; } // TODO: when Steam implementation is done ?
    }
}
