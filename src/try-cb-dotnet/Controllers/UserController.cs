using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Couchbase;
using Couchbase.Core;
using JWT;
using try_cb_dotnet.Models;

namespace try_cb_dotnet.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IBucket _bucket = ClusterHelper.GetBucket(ConfigurationManager.AppSettings.Get("couchbaseUserBucket"));
        private readonly string _secretKey = ConfigurationManager.AppSettings["JWTTokenSecret"];

        [Route("login")]
        [HttpPost]
        public HttpResponseMessage Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new Error("Expected 'username' and 'password' in message body"));
            }

            var result = _bucket.GetDocument<User>($"user::{model.Username}");
            if (!result.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new Error("Invalid username or password"));
            }

            var user = result.Content;
            if (user.Password != CalcuateMd5Hash(model.Password))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new Error("Invalid username or password"));
            }

            var data = new
            {
                token = BuildToken(user.Username)
            };
            return Request.CreateResponse(HttpStatusCode.OK, new Result(data));
        }

        [Route("signup")]
        [HttpPost]
        public HttpResponseMessage SignUp([FromBody] LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new Error("Invalid username or password"));
            }

            var userKey = $"user::{model.Username}";
            if (_bucket.Exists(userKey))
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, new Error("Username already exists"));
            }

            var userDoc = new Document<User>
            {
                Id = userKey,
                Content = new User
                {
                    Username = model.Username,
                    Password = CalcuateMd5Hash(model.Password)
                },
                Expiry = model.Expiry
            };

            var result = _bucket.Insert(userDoc);
            if (!result.Success)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Error(result.Message));
            }

            var data = new
            {
                token = BuildToken(model.Username)
            };
            var context = $"Created user with ID '{userKey}' in bucket '{_bucket.Name}' that expires in {userDoc.Expiry}ms";
            return Request.CreateResponse(HttpStatusCode.Created, new Result(data, context));
        }

        [Route("{username}/flights")]
        [HttpGet]
        public HttpResponseMessage GetFlightsForUser(string username)
        {
            if (Request.Headers.Authorization == null || Request.Headers.Authorization.Scheme != "Bearer")
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (!VerifyToken(Request.Headers.Authorization.Parameter, username))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            var userDoc = _bucket.Get<User>($"user::{username}");
            if (!userDoc.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            var data = new
            {
                flights = userDoc.Value.Flights
            };
            return Request.CreateResponse(HttpStatusCode.OK, new Result(data));
        }

        [Route("{username}/flights")]
        [HttpPost]
        public HttpResponseMessage RegisterFlightForUser(string username, [FromBody] List<Flight> flights)
        {
            if (Request.Headers.Authorization == null || Request.Headers.Authorization.Scheme != "Bearer")
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (!VerifyToken(Request.Headers.Authorization.Parameter, username))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            if (flights == null || !flights.Any())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            foreach (var flight in flights)
            {
                flight.BookedOn = "try-cb-dotnet";
            }

            var userKey = $"user::{username}";
            var user = _bucket.Get<User>(userKey);
            if (!user.Success)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            user.Value.Flights.AddRange(flights);

            var result = _bucket.Replace(userKey, user.Value);
            if (!result.Success)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result.Message);
            }

            var data = new
            {
                added = flights
            };
            return Request.CreateResponse(HttpStatusCode.Accepted, new Result(data));
        }

        private static string CalcuateMd5Hash(string password)
        {
            byte[] bytes;
            using (var md5 = MD5.Create())
            {
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var builder = new StringBuilder();
            foreach (var @byte in bytes)
            {
                builder.Append(@byte.ToString("x2"));
            }

            return builder.ToString();
        }

        private string BuildToken(string username)
        {
            var claims = new Dictionary<string, string>
            {
                {"user", username}
            };

            return JsonWebToken.Encode(claims, _secretKey, JwtHashAlgorithm.HS512);
        }

        private bool VerifyToken(string token, string username)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(username))
            {
                return false;
            }

            Dictionary<string, string> claims;
            try
            {
                claims = JsonWebToken.DecodeToObject<Dictionary<string, string>>(token, _secretKey);
            }
            catch
            {
                return false;
            }

            string value;
            if (claims == null || !claims.TryGetValue("user", out value))
            {
                return false;
            }

            return username == value;
        }
    }
}
