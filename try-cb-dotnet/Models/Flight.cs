using Newtonsoft.Json;

namespace try_cb_dotnet.Models
{
    public class Flight
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("flight")]
        public string FlightName { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("sourceairport")]
        public string SourceAirport { get; set; }

        [JsonProperty("destinationairport")]
        public string DestinationAirport { get; set; }

        [JsonProperty("bookedon")]
        public string BookedOn { get; set; }

        [JsonProperty("type")]
        public string Type => "flight";
    }
}
