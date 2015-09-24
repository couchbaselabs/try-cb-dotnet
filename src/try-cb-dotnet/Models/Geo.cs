using Couchbase.Linq.Filters;
using Newtonsoft.Json;

namespace Couchbase.Linq.SampleBuckets.Documents.TravelDocuments
{
    [EntityTypeFilter("geo")]
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

/*

{
    "alt": 40,
    "lat": 59.3675,
    "lon": -2.43444
}

*/