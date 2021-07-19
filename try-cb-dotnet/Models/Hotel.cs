using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace try_cb_dotnet.Models
{
    public class Hotel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("type")]
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