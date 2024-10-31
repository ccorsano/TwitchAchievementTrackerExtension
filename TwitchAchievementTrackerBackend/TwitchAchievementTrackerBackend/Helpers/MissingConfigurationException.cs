using System;

namespace TwitchAchievementTrackerBackend.Helpers
{
    public class MissingConfigurationException : Exception
    {
        public MissingConfigurationException(string? message, Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
