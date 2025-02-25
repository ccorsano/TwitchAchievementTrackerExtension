﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchAchievementTrackerBackend.Model;

namespace TwitchAchievementTrackerBackend.Helpers
{
    public static class ControllerHelper
    {
        private const string CONFIGURATION_KEY = "XCONFIGTOKEN";

        public static bool HasExtensionConfiguration(this ControllerBase controller)
        {
            return controller.HttpContext.HasExtensionConfiguration();
        }

        public static bool HasExtensionConfiguration(this HttpContext context)
        {
            return context.Items.ContainsKey(CONFIGURATION_KEY);
        }

        public static ExtensionConfiguration GetExtensionConfiguration(this ControllerBase controller)
        {
            return controller.HttpContext.GetExtensionConfiguration();
        }

        public static ExtensionConfiguration GetExtensionConfiguration(this HttpContext context)
        {
            if (!context.Items.ContainsKey(CONFIGURATION_KEY))
            {
                throw new MissingConfigurationException("Configuration header is missing");
            }
            return (context.Items[CONFIGURATION_KEY] as ExtensionConfiguration)!;
        }

        public static void SetExtensionConfiguration(this HttpContext context, ExtensionConfiguration configuration)
        {
            context.Items[CONFIGURATION_KEY] = configuration;
        }
    }
}
