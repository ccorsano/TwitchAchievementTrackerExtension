using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum XApiImageType {
        BoxArt,
        BrandedKeyArt,
        FeaturePromotionalSquareArt,
        Hero,
        Image,
        ImageGallery,
        Logo,
        Poster,
        Screenshot,
        SellImage,
        SuperHeroArt,
        Thumbnail,
        Tile,
        TitledHeroArt,
        TypeLogo,
        TypeTile,
        WideBackgroundImage
    };
}
