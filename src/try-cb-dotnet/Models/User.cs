using System.Collections.Generic;
using Couchbase.Linq.Filters;
using Newtonsoft.Json;

namespace try_cb_dotnet.Models
{
    [DocumentTypeFilter("user")]
    public class User
    {
        [JsonProperty("user")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("flights")]
        public List<Flight> Flights { get; set; }

        [JsonProperty("type")]
        public string Type => "user";
    }
}