using System.Web.Http;

namespace try_cb_dotnet.Controllers
{
    public class AirportController : ApiController
    {
        [HttpGet]
        [ActionName("findAll")]
        public object FindAll(string search, string token)
        {
            /// Task:
            /// This is a Web API call, a method that is called from the static html (index.html).
            /// The js in the static html expectes this "findAll" web api call to return a
            /// "airportname" in a json format like this:
            /// [{"airportname":"San Francisco Intl"}]
            /// 
            /// Implement the method to return a single airport name.
            /// Later we will use Couchbase to do the look-up but for now a "constant" is returned.

            return string.Empty;
        }
    }
}