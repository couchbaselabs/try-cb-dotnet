using Newtonsoft.Json;

namespace try_cb_dotnet.Models
{
    public class LoginModel
    {
        [JsonProperty("user")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("expiry")]
        public uint Expiry { get; set; }
    }
}