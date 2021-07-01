using System.Text.Json;
using System.Text.Json.Serialization;

namespace try_cb_dotnet.Models
{
    public class Route
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("flight")]
        public string Flight { get; set; }

        [JsonPropertyName("utc")]
        public string Time { get; set; }

        [JsonPropertyName("sourceairport")]
        public string SourceAirport { get; set; }

        [JsonPropertyName("destinationairport")]
        public string DestinationAirport { get; set; }

        [JsonPropertyName("equipment")]
        public string Equipment { get; set; }

        [JsonPropertyName("flighttime")]
        public double FlightTime { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("type")]
        public string Type => "route";
    }
}
