using Microsoft.AspNetCore.Http;
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

        public static ExtensionConfiguration GetExtensionConfiguration(this ControllerBase controller)
        {
            return controller.HttpContext.Items[CONFIGURATION_KEY] as ExtensionConfiguration;
        }

        public static ExtensionConfiguration GetExtensionConfiguration(this HttpContext context)
        {
            return context.Items[CONFIGURATION_KEY] as ExtensionConfiguration;
        }

        public static void SetExtensionConfiguration(this HttpContext context, ExtensionConfiguration configuration)
        {
            context.Items[CONFIGURATION_KEY] = configuration;
        }
    }
}
