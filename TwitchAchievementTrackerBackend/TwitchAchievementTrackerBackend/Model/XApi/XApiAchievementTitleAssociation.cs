using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiAchievementTitleAssociation
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
}
