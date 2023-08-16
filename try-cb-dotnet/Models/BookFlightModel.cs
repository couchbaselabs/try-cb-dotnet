using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace try_cb_dotnet.Models
{
    public class BookFlightModel
    {
        [JsonPropertyName("flights")] public List<Flight> Flights { get; set; } = new ();
    }
}
