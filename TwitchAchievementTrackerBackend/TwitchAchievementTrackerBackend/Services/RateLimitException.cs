using System;
using System.Runtime.Serialization;

namespace TwitchAchievementTrackerBackend.Services
{
    internal class RateLimitException : Exception
    {
        public RateLimitException()
        {
        }

        public RateLimitException(string message) : base(message)
        {
        }

        public RateLimitException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}