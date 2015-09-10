using System.Web.Http;
using try_cb_dotnet.Storage.Couchbase;

namespace try_cb_dotnet.Controllers
{
    public class AirportController : ApiController
    {
        [HttpGet]
        [ActionName("findAll")]
        public object FindAll(string search, string token)
        {
            // [{"airportname":"San Francisco Intl"}]
            //return new List<dynamic>()
            //{
            //    new {airportname = "San Francisco Intl"}
            //};

            string queryStr = search;
            string queryPrep;
            if (queryStr.Length == 3)
            {
                queryPrep = "SELECT airportname FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` WHERE faa ='" + queryStr.ToUpper() + "'";
            }
            else if (queryStr.Length == 4 && (queryStr == queryStr.ToUpper() || queryStr == queryStr.ToLower()))
            {
                queryPrep = "SELECT airportname FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` WHERE icao ='" + queryStr.ToUpper() + "'";
            }
            else
            {
                queryPrep = "SELECT airportname FROM `" + CouchbaseConfigHelper.Instance.Bucket + "` WHERE airportname LIKE '" + queryStr + "%'";
            }

            var result = CouchbaseStorageHelper.Instance.ExecuteQuery(queryPrep);
            return result.Rows;
        }
    }
}