using Newtonsoft.Json;

namespace try_cb_dotnet.Models
{
    public class Route
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("flight")]
        public string Flight { get; set; }

        [JsonProperty("utc")]
        public string Time { get; set; }

        [JsonProperty("sourceairport")]
        public string SourceAirport { get; set; }

        [JsonProperty("destinationairport")]
        public string DestinationAirport { get; set; }

        [JsonProperty("equipment")]
        public string Equipment { get; set; }

        [JsonProperty("flighttime")]
        public double FlightTime { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("type")]
        public string Type => "route";
    }
}
