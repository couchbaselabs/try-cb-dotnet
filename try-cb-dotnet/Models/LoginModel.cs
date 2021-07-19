using System.Text.Json;
using System.Text.Json.Serialization;

namespace try_cb_dotnet.Models
{
    public class LoginModel
    {
        [JsonPropertyName("user")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("expiry")]
        public uint Expiry { get; set; }
    }
}
