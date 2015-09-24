using Couchbase.Linq.Filters;
using Newtonsoft.Json;

namespace Couchbase.Linq.SampleBuckets.Documents.TravelDocuments
{
    [EntityTypeFilter("airport")]
    public class Airport
    {
        [JsonProperty("airportname")]
        public string Airportname { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("faa")]
        public string Faa { get; set; }

        [JsonProperty("geo")]
        public Geo Geo { get; set; }

        [JsonProperty("icao")]
        public string Icao { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tz")]
        public string Tz { get; set; }
    }
}

/*

{
    "airportname": "North Ronaldsay Airport",
    "city": "North Ronaldsay",
    "country": "United Kingdom",
    "faa": "NRL",
    "geo": {
        "alt": 40,
        "lat": 59.3675,
        "lon": -2.43444
    },
    "icao": "EGEN",
    "id": 5566,
    "type": "airport",
    "tz": "Europe/London"
    }

*/