using System.Text.Json;
using System.Text.Json.Serialization;

namespace try_cb_dotnet.Models
{
    public class Flight
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("flight")]
        public string FlightName { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("sourceairport")]
        public string SourceAirport { get; set; }

        [JsonPropertyName("destinationairport")]
        public string DestinationAirport { get; set; }

        [JsonPropertyName("bookedon")]
        public string BookedOn { get; set; }

        [JsonPropertyName("type")]
        public string Type => "flight";
    }
}
