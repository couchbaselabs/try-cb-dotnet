using System.Linq;
using Newtonsoft.Json;

namespace try_cb_dotnet.Models
{
    public class Hotel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("type")]
        public string Type => "hotel";

        public string GetFullAddress()
        {
            return string.Join(", ", new[]
            {
                Address,
                City,
                State,
                Country
            }.Where(x => !string.IsNullOrEmpty(x)));
        }
    }
}