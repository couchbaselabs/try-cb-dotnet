using System.Collections.Generic;
using System.Web.Http;
using Couchbase;
using Couchbase.N1QL;

namespace try_cb_dotnet.Controllers
{
    public class AirportController : ApiController
    {
        [HttpGet]
        [ActionName("findAll")]
        public object FindAll(string search, string token)
        {
            IQueryRequest query = null;

            if (search.Length == 3)
            {
                string sql = "SELECT airportname FROM `travel-sample` WHERE faa = $1";

                query = new QueryRequest(sql)
                   .AddPositionalParameter(search.ToUpper());
            }
            else if (search.Length == 4)
            {

                string sql = "SELECT airportname FROM `travel-sample` WHERE icao = $1";

                query = new QueryRequest(sql)
                   .AddPositionalParameter(search.ToUpper());
            }
            else
            {
                string sql = "SELECT airportname FROM `travel-sample` WHERE type='airport' AND airportname IS NOT NULL AND (LOWER(airportname) LIKE $search OR LOWER(city) LIKE $search)";

                query = new QueryRequest(sql)
                   .AddNamedParameter("search", "%" + search.ToLower() + "%");
            }

            var result = ClusterHelper.GetBucket("travel-sample")
                    .Query<dynamic>(query);

            return result.Rows;

            /// [{"airportname":"San Francisco Intl"}]
            //return new List<dynamic>()
            //{
            //    new {airportname = "San Francisco Intl"}
            //};
        }
    }
}