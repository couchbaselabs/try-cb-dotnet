using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace try_cb_dotnet.Models
{
    public class User
    {
        [JsonPropertyName("user")] public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("flights")] public List<Flight> Flights { get; set; } = new();

        [JsonPropertyName("type")]
        public string Type => "user";
    }
}
