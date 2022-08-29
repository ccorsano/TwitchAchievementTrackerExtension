using System.Text.Json.Serialization;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiPagingInfo
    {
        [JsonPropertyName("continuationToken")]
        public string ContinuationToken { get; set; }

        [JsonPropertyName("totalRecords")]
        public long TotalRecords { get; set; }
    }
}
