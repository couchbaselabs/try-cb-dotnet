using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using try_cb_dotnet.Models;

namespace try_cb_dotnet.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        [ActionName("Login")]
        public object Login(string password, string user)
        {
            /// Task:
            /// This is a Web API call, a method that is called from the static html (index.html).
            /// The js in the static html expectes this "Login" web api call to return a
            /// "success" status code containing a JWT token. 
            /// The JWT token is used to reference and store data about the user's trips/bookings and login credentials.
            /// Response should be in a json format like this:
            /// Round trip: 
            /// [{"success":"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiZ3Vlc3QiLCJpYXQiOjE0NDE4Njk5NTR9.5jPBtqralE3W3LPtS - j3MClTjwP9ggXSCDt3 - zZOoKU"}]

            /// Implement the method to return a "success" allowing the user to login.
            /// Later we will implement a JWT token issuer and store user data in Couchbase for later look-up.
            /// The token is created for the user:
            /// username: guest
            /// passowrd: guest

            return string.Empty;
        }

        [HttpPost]
        [ActionName("Login")]
        public object CreateLogin([FromBody] UserModel user)
        {
            /// Task:
            /// This is a Web API call, a method that is called from the static html (index.html).
            /// The js in the static html expectes this "Login" web api call to return a
            /// "success" status code containing a JWT token. 
            /// The JWT token is used to reference and store data about the user's trips/booking and login credentials.
            /// Response should be in a json format like this:
            /// Round trip: 
            /// [{"success":"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiZ3Vlc3QiLCJpYXQiOjE0NDE4Njk5NTR9.5jPBtqralE3W3LPtS - j3MClTjwP9ggXSCDt3 - zZOoKU"}]

            /// Implement the method to return a "success" faking the creation of a new user and allowing the user to login.
            /// Later we will implement a JWT token issuer and store user data in Couchbase for later look-up.
            /// The token is created for the user:
            /// username: guest
            /// passowrd: guest

            return string.Empty;
        }

        [HttpGet]
        [ActionName("flights")]
        public object Flights(string token)
        {
            /// Task:
            /// This is a Web API call, a method that is called from the static html (index.html).
            /// The js in the static html expectes this "flights" web api call to return a
            /// all bookings done by this user. 
            /// The JWT token is used to look-up the user and find all bookings.
            /// In this fake implementation we are not going to use the Token, but instead return a static list of bookings.
            /// Response should be in a json format like this:
            /// Bookings:
            ///  [{"_type":"Flight","_id":"d500a3d1-2cca-43a5-8a66-f11828a35969","name":"American Airlines","flight":"AA344","date":"09/10/2015","sourceairport":"SFO","destinationairport":"LAX","bookedon":"1441881827622"},{"_type":"Flight","_id":"bf676b0d-e63b-4ff6-aade-7ac1c182b3de","name":"American Airlines","flight":"AA787","date":"09/11/2015","sourceairport":"LAX","destinationairport":"SFO","bookedon":"1441881827623"},{"_type":"Flight","_id":"f0099c24-3ad4-482e-8352-704f9cbf1a43","name":"American Airlines","flight":"AA550","date":"09/10/2015","sourceairport":"SFO","destinationairport":"LAX","bookedon":"1441881827623"}]
            ///
            /// Implement the method to return the fake "bookings" for the guest user.
            /// Later we will look-up bookings with the JWT token, but for now a static list is what we need.
            /// Hint: return the same booking multiple times in a list, re-using the sample json above.
            
            return string.Empty;
        }

        [HttpPost]
        [ActionName("flights")]
        public object BookFlights([FromBody] dynamic request)
        {
            /// Task:
            /// This is a Web API call, a method that is called from the static html (index.html).
            /// The js in the static html expectes this "flights" web api call to save the selected flight in a booking's document.
            /// 
            /// The JWT token is used as a key to the users bookings.
            /// In this fake implementation we are not going to use the Token, nor store any data about the bookings.
            /// Instead we return a static value to indicate that the bokking was successfull.
            /// Response should be in a json format like this:
            /// Bookings:
            /// {"added":3}
            /// 
            /// Implement the method to return the fake "booking success" for the guest user.
            /// Later we will store bookings using the JWT token, but for now a static response is what we need.

            return string.Empty;
        }
    }
}