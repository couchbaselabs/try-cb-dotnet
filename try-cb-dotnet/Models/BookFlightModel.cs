using System.Collections.Generic;
using Newtonsoft.Json;

namespace try_cb_dotnet.Models
{
    public class BookFlightModel
    {
        [JsonProperty("flights")]
        public List<Flight> Flights { get; set; }
    }
}
