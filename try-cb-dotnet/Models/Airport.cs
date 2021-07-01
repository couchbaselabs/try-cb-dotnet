using System.Text.Json;
using System.Text.Json.Serialization;

namespace try_cb_dotnet.Models
{
    public class Airport
    {
        [JsonPropertyName("airportname")]
        public string Airportname { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("faa")]
        public string Faa { get; set; }

        [JsonPropertyName("icao")]
        public string Icao { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type => "airport";
    }
}