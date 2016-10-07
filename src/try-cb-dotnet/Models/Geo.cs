using Couchbase.Linq.Filters;
using Newtonsoft.Json;

namespace try_cb_dotnet.Models
{
    [DocumentTypeFilter("geo")]
    public class Geo
    {
        [JsonProperty("alt")]
        public double Alt { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }
    }
}