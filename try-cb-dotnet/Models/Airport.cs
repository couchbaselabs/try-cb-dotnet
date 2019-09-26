using Newtonsoft.Json;

namespace try_cb_dotnet.Models
{
    public class Airport
    {
        [JsonProperty("airportname")]
        public string Airportname { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("faa")]
        public string Faa { get; set; }

        [JsonProperty("icao")]
        public string Icao { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type => "airport";
    }
}