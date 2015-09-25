using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using try_cb_dotnet.Models;
using Couchbase;
using JWT;

namespace try_cb_dotnet.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        [ActionName("Login")]
        public object Login(string password, string user)
        {
            try
            {
                var result = ClusterHelper
                    .GetBucket("default")
                    .Get<dynamic>("profile::" + user);

                if (result.Success && result.Status == Couchbase.IO.ResponseStatus.Success && result.Exception == null && result.Value != null)
                {
                    var jsonDecodedTokenString =
                        JsonWebToken
                        .Decode(result.Value, CouchbaseConfigHelper.Instance.JWTTokenSecret, false);

                    var jwtToken = JsonConvert.DeserializeAnonymousType(jsonDecodedTokenString, new { user = "", iat = "" });

                    if (jwtToken.iat == password)
                    {
                        return new { success = result.Value };
                    }
                }
            }
            catch (Exception)
            {
                // Silence the Exception
            }

            return new { success = false };
        }

        [HttpPost]
        [ActionName("Login")]
        public object CreateLogin([FromBody] UserModel user)
        {
            try
            {
                if (ClusterHelper.GetBucket("default").Exists("profile::" + user.User))
                {
                    throw new Exception("User already Exists!");
                }

                string jsonToken =
                    JsonWebToken
                    .Encode(
                        new { user = user.User, iat = user.Password },
                        CouchbaseConfigHelper.Instance.JWTTokenSecret,
                        JwtHashAlgorithm.HS512);

                var result = ClusterHelper
                    .GetBucket("default")
                    .Upsert<dynamic>("profile::" + user.User, jsonToken);

                if (!result.Success || result.Exception != null)
                {
                    throw new Exception("could not save user to Couchbase");
                }

                return new { success = jsonToken };
            }
            catch (Exception)
            {
                // Silence the Exception
            }

            return new { success = false };
        }

        [HttpGet]
        [ActionName("flights")]
        public object Flights(string token)
        {
            return ClusterHelper
                    .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                    .Get<dynamic>("bookings::" + token)
                    .Value;
        }

        [HttpPost]
        [ActionName("flights")]
        public object BookFlights([FromBody] dynamic request)
        {
            List<FlightModel> flights = new List<FlightModel>();

            foreach (var flight in request.flights)
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

            ClusterHelper
               .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
               .Upsert("bookings::" + request.token, flights);

            return new { added = flights.Count };
        }
    }
}