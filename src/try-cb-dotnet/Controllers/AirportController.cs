using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Couchbase;
using Couchbase.Linq;
using try_cb_dotnet.Models;

namespace try_cb_dotnet.Controllers
{
    [RoutePrefix("api/airports")]
    public class AirportController : ApiController
    {
        private readonly IBucketContext _context = new BucketContext(ClusterHelper.GetBucket(ConfigurationManager.AppSettings.Get("couchbaseTravelBucket")));

        [Route("")]
        [HttpGet]
        public HttpResponseMessage Search(string search = null)
        {
            if (string.IsNullOrEmpty(search))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new Models.Error("Missing or empty query string parameter 'search'"));
            }

            string query;
            IEnumerable<string> airports;
            if (IsFaaCode(search))
            {
                query = $"SELECT airportname FROM `travel-sample` WHERE type = 'airport' AND faa = '{search.ToUpper()}'";
                airports = _context.Query<Airport>()
                    .Where(x => x.Faa == search.ToUpper())
                    .Select(x => x.Airportname);
            }
            else if (IsIcaoCode(search))
            {
                query = $"SELECT airportname FROM `travel-sample` WHERE type = 'airport' AND icao = '{search.ToUpper()}'";
                airports = _context.Query<Airport>()
                    .Where(x => x.Icao == search.ToUpper())
                    .Select(x => x.Airportname);
            }
            else
            {
                query = $"SELECT airportname FROM `travel-sample` WHERE type = 'airport' AND airportname LIKE '{search}'";
                airports = _context.Query<Airport>()
                    .Where(x => x.Airportname.Contains(search))
                    .Select(x => x.Airportname);
            }

            var data = new
            {
                airports = airports.Select(airportname => new { airportname })
            };
            return Request.CreateResponse(new Result(data, query));
        }

        private static bool IsFaaCode(string search)
        {
            return search.Length == 3;
        }

        private static bool IsIcaoCode(string search)
        {
            return search.Length == 4 && (search == search.ToLower() || search == search.ToUpper());
        }
    }
}