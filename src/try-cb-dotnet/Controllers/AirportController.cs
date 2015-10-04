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
            /// [{"airportname":"San Francisco Intl"}]
            return new List<dynamic>()
            {
                new {airportname = "San Francisco Intl"}
            };
        }
    }
}