using System.Text.Json.Serialization;
using System;

namespace TwitchAchievementTrackerBackend.Model.XApi
{
    public partial class XApiKeyProfile
    {
        [JsonPropertyName("userKey")]
        public object UserKey { get; set; }

        [JsonPropertyName("administeredConsoles")]
        public object[] AdministeredConsoles { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public DateTimeOffset DateOfBirth { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("gamerTag")]
        public string GamerTag { get; set; }

        [JsonPropertyName("homeAddressInfo")]
        public XApiHomeAddressInfo HomeAddressInfo { get; set; }

        [JsonPropertyName("homeConsole")]
        public object HomeConsole { get; set; }

        [JsonPropertyName("imageUrl")]
        public Uri ImageUrl { get; set; }

        [JsonPropertyName("isAdult")]
        public bool IsAdult { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("legalCountry")]
        public string LegalCountry { get; set; }

        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        [JsonPropertyName("msftOptin")]
        public bool MsftOptin { get; set; }

        [JsonPropertyName("ownerHash")]
        public object OwnerHash { get; set; }

        [JsonPropertyName("ownerXuid")]
        public long OwnerXuid { get; set; }

        [JsonPropertyName("midasConsole")]
        public object MidasConsole { get; set; }

        [JsonPropertyName("partnerOptin")]
        public bool PartnerOptin { get; set; }

        [JsonPropertyName("userHash")]
        public string UserHash { get; set; }

        [JsonPropertyName("userXuid")]
        public long UserXuid { get; set; }

        [JsonPropertyName("touAcceptanceDate")]
        public DateTimeOffset TouAcceptanceDate { get; set; }

        [JsonPropertyName("gamerTagChangeReason")]
        public object GamerTagChangeReason { get; set; }
    }

    public partial class XApiHomeAddressInfo
    {
        [JsonPropertyName("street1")]
        public object Street1 { get; set; }

        [JsonPropertyName("street2")]
        public object Street2 { get; set; }

        [JsonPropertyName("city")]
        public object City { get; set; }

        [JsonPropertyName("state")]
        public object State { get; set; }

        [JsonPropertyName("postalCode")]
        public object PostalCode { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }
}
