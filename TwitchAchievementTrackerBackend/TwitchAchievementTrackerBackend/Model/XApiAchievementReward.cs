﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model
{
    public class XApiAchievementReward
    {
        public string Type { get; set; }
        public int? Value { get; set; }
        public string ValueType { get; set; }
    }
}
