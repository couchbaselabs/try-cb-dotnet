using System.Text.Json;
using System.Text.Json.Serialization;

namespace try_cb_dotnet.Models
{
    public class Flight
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("flight")]
        public string FlightName { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public double Price { get; set; } 

        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("sourceairport")]
        public string SourceAirport { get; set; } = string.Empty;

        [JsonPropertyName("destinationairport")]
        public string DestinationAirport { get; set; } = string.Empty;

        [JsonPropertyName("bookedon")]
        public string BookedOn { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type => "flight";
    }
}
