using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public class XApiMarketplaceImage
    {
        public string? FileId { get; set; }

        public object? EisListingIdentifier { get; set; }

        public string? BackgroundColor { get; set; }

        public string? Caption { get; set; }

        public long FileSizeInBytes { get; set; }

        public string? ForegroundColor { get; set; }

        public long Height { get; set; }

        public string? ImagePositionInfo { get; set; }

        public string? ImagePurpose { get; set; }

        public string? UnscaledImageSha256Hash { get; set; }

        public string? Uri { get; set; }

        public long Width { get; set; }
    }
}
