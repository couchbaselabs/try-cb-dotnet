using System.Web.Http;
using Couchbase.N1QL;
using try_cb_dotnet.Storage.Couchbase;

namespace try_cb_dotnet.Controllers
{
    public class AirportController : ApiController
    {
        [HttpGet]
        [ActionName("findAll")]
        public object FindAll(string search, string token)
        {
            if (search.Length == 3)
            {
                // LAX
                var query = 
                    new QueryRequest("SELECT airportname FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` WHERE faa=$1")
                    .AddPositionalParameter(search.ToUpper());

                return CouchbaseStorageHelper.Instance.ExecuteQuery(query).Rows;
            }
            else if (search.Length == 4)
            {
                // KLAX
                var query =
                    new QueryRequest("SELECT airportname FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` WHERE icao = '$1'")
                    .AddPositionalParameter(search.ToUpper());

                return CouchbaseStorageHelper.Instance.ExecuteQuery(query).Rows;
            }
            else
            {
                // Los Angeles
                var query =
                    new QueryRequest("SELECT airportname FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` WHERE airportname LIKE $1")
                    .AddPositionalParameter("%" + search + "%");

                return CouchbaseStorageHelper.Instance.ExecuteQuery(query).Rows;
            }
        }
    }
}