using System.Text.Json;
using System.Text.Json.Serialization;

namespace try_cb_dotnet.Models
{
    public class Route
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("flight")]
        public string Flight { get; set; } = string.Empty;

        [JsonPropertyName("utc")]
        public string Time { get; set; } = string.Empty;

        [JsonPropertyName("sourceairport")]
        public string SourceAirport { get; set; } = string.Empty;

        [JsonPropertyName("destinationairport")]
        public string DestinationAirport { get; set; } = string.Empty;

        [JsonPropertyName("equipment")]
        public string Equipment { get; set; } = string.Empty;

        [JsonPropertyName("flighttime")]
        public double FlightTime { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("type")]
        public string Type => "route";
    }
}
