using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using Couchbase;
using Couchbase.Core;
using Couchbase.Search;
using Couchbase.Search.Queries.Compound;
using Couchbase.Search.Queries.Simple;
using Newtonsoft.Json;
using try_cb_dotnet.Models;

namespace try_cb_dotnet.Controllers
{
    [RoutePrefix("api/hotels")]
    public class HotelController : ApiController
    {
        private readonly IBucket _bucket = ClusterHelper.GetBucket(ConfigurationManager.AppSettings.Get("couchbaseTravelBucket"));

        [Route("{description?}/{location?}")]
        [HttpGet]
        public HttpResponseMessage FindHotel(string description = null, string location = null)
        {
            var query = new ConjunctionQuery(
                new TermQuery("hotel").Field("type")
            );

            if (!string.IsNullOrEmpty(description) && description != "*")
            {
                query.And(new DisjunctionQuery(
                    new PhraseQuery(description).Field("name"),
                    new PhraseQuery(description).Field("description")
                ));
            }

            if (!string.IsNullOrEmpty(location) && location != "*")
            {
                query.And(new DisjunctionQuery(
                    new PhraseQuery(location).Field("address"),
                    new PhraseQuery(location).Field("city"),
                    new PhraseQuery(location).Field("state"),
                    new PhraseQuery(location).Field("country")
                ));
            }

            var search = new SearchQuery();
            search.Index = "hotel";
            search.Query = query;
            search.Limit(10);

            var queryJson = ""; query.Export().ToString(Formatting.None);
            var hotels = new List<dynamic>();

            var result = _bucket.Query(search);
            foreach (var row in result)
            {
                var fragment = _bucket.LookupIn<Hotel>(row.Id)
                    .Get("name")
                    .Get("description")
                    .Get("address")
                    .Get("city")
                    .Get("state")
                    .Get("country")
                    .Execute();

                hotels.Add(new
                {
                    name = fragment.Value.Name,
                    description = fragment.Value.Description,
                    address = fragment.Value.GetFullAddress()
                });
            }

            return Request.CreateResponse(new Result(hotels, queryJson));
        }
    }
}
