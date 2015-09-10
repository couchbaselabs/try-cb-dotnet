using System;
using System.Collections.Generic;
using System.Web.Http;
using try_cb_dotnet.Models;
using try_cb_dotnet.Storage.Couchbase;

namespace try_cb_dotnet.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        [ActionName("Login")]
        public object Login()
        {
            // HACK : Only allows guest/guest 
            // Missing JWT .net implementation.

            /*
            Implement JwtSecurityTokenHandler();
            */

            return new { success = FakeUserModel.GetFakeUser.JWTToken };
        }

        [HttpPost]
        [ActionName("Login")]
        public object CreateLogin([FromBody] UserModel user)
        {
            // HACK : Only allows guest/guest 
            // Missing JWT .net implementation.

            /*
            Implement JwtSecurityTokenHandler();
            */

            return new { success = FakeUserModel.GetFakeUser.JWTToken };
        }

        [HttpGet]
        [ActionName("flights")]
        public object Flights(string token)
        {
            //return new List<dynamic>
            //{
            //    new {_type="Flight",_id="f0099c24-3ad4-482e-8352-704f9cbf1a43",name="American Airlines",flight="AA550",date="09/10/2015",sourceairport="SFO",destinationairport="LAX",bookedon=1441881827623},
            //    new {_type="Flight",_id="f0099c24-3ad4-482e-8352-704f9cbf1a43",name="American Airlines",flight="AA550",date="09/10/2015",sourceairport="SFO",destinationairport="LAX",bookedon=1441881827623},
            //    new {_type="Flight",_id="f0099c24-3ad4-482e-8352-704f9cbf1a43",name="American Airlines",flight="AA550",date="09/10/2015",sourceairport="SFO",destinationairport="LAX",bookedon=1441881827623},
            //};

            return CouchbaseStorageHelper
                .Instance
                .Get("bookings::" + token)
                .Value;

            // response: 
            /*
            [{"_type":"Flight","_id":"d500a3d1-2cca-43a5-8a66-f11828a35969","name":"American Airlines","flight":"AA344","date":"09/10/2015","sourceairport":"SFO","destinationairport":"LAX","bookedon":"1441881827622"},{"_type":"Flight","_id":"bf676b0d-e63b-4ff6-aade-7ac1c182b3de","name":"American Airlines","flight":"AA787","date":"09/11/2015","sourceairport":"LAX","destinationairport":"SFO","bookedon":"1441881827623"},{"_type":"Flight","_id":"f0099c24-3ad4-482e-8352-704f9cbf1a43","name":"American Airlines","flight":"AA550","date":"09/10/2015","sourceairport":"SFO","destinationairport":"LAX","bookedon":"1441881827623"}]
            */
        }

        [HttpPost]
        [ActionName("flights")]
        public object BookFlights([FromBody] dynamic request)
        {
            // request.flights[0]._data.equipment.Value
            List<FlightModel> flights = new List<FlightModel>();

            foreach(var flight in request.flights)
            {
                flights.Add(new FlightModel
                {
                    name = flight._data.name,
                    bookedon = DateTime.Now.ToString(),
                    date = flight._data.date,
                    destinationairport = flight._data.destinationairport,
                    sourceairport = flight._data.sourceairport
                });
            }

            CouchbaseStorageHelper
                .Instance
                .Upsert("bookings::"+request.token, flights);


            return new { added = flights.Count };

            //Request:
            /*
            {"token":"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiZ3Vlc3QiLCJpYXQiOjE0NDE4Njk5NTR9.5jPBtqralE3W3LPtS - j3MClTjwP9ggXSCDt3 - zZOoKU",
            "flights":[{"_id":"AA907","_name":"American Airlines-AA907","_price":53,"_quantity":1,"_data":{"destinationairport":"SFO","equipment":738,"flight":"AA907","id":5746,"name":"American Airlines","sourceairport":"LAX","utc":"00:29:00","flighttime":1,"price":53,"utcland":"1:29:00","date":"09/10/2015"}}]}
            */

            // response: {"added":3}
        }
    }
}