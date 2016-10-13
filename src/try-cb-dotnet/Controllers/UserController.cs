using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Couchbase;
using Couchbase.Core;
using Couchbase.IO;
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
        public async Task<IHttpActionResult> Login(LoginModel model)
        {
            if (model == null || !model.IsValid())
            {
                return Content(HttpStatusCode.BadRequest, new Error("Missing or empty 'user' and 'password' properties in message body"));
            }

            var userKey = CreateUserKey(model.Username);
            var result = await _bucket.GetAsync<User>(userKey);
            if (!result.Success)
            {
                if (result.Status == ResponseStatus.KeyNotFound)
                {
                    return Content(HttpStatusCode.Unauthorized, new Error("Invalid username and/or password"));
                }
                return Content(HttpStatusCode.InternalServerError, new Error(result.Message));
            }

            var user = result.Value;
            if (user.Password != CalcuateMd5Hash(model.Password))
            {
                return Content(HttpStatusCode.Unauthorized, new Error("Invalid username and/or password"));
            }

            var data = new
            {
                token = BuildToken(user.Username)
            };
            return Content(HttpStatusCode.OK, new Result(data));
        }

        [Route("signup")]
        [HttpPost]
        public async Task<IHttpActionResult> SignUp(LoginModel model)
        {
            if (model == null || !model.IsValid())
            {
                return Content(HttpStatusCode.BadRequest, new Error("Invalid username and/or password"));
            }

            var userKey = CreateUserKey(model.Username);
            if (await _bucket.ExistsAsync(userKey))
            {
                return Content(HttpStatusCode.Conflict, new Error($"Username '{model.Username}' already exists"));
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

            var result = await _bucket.InsertAsync(userDoc);
            if (!result.Success)
            {
                return Content(HttpStatusCode.InternalServerError, new Error(result.Message));
            }

            var data = new
            {
                token = BuildToken(model.Username)
            };
            var context = $"Created user with ID '{userKey}' in bucket '{_bucket.Name}' that expires in {userDoc.Expiry}ms";
            return Content(HttpStatusCode.Accepted, new Result(data, context));
        }

        [Route("{username}/flights")]
        [HttpGet]
        public async Task<IHttpActionResult> GetFlightsForUser(string username)
        {
            var authHeaderValue = GetAuthHeaderValue(Request.Headers);
            if (string.IsNullOrEmpty(authHeaderValue))
            {
                return Content(HttpStatusCode.Unauthorized, string.Empty);
            }
            if (!VerifyToken(authHeaderValue, username))
            {
                return Content(HttpStatusCode.Forbidden, string.Empty);
            }

            var result = await _bucket.GetAsync<User>($"user::{username}");
            if (!result.Success)
            {
                return Content(HttpStatusCode.Forbidden, string.Empty);
            }

            var data = result.Value.Flights;
            return Content(HttpStatusCode.OK, new Result(data));
        }

        [Route("{username}/flights")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterFlightForUser(string username, BookFlightModel model)
        {
            var authHeaderValue = GetAuthHeaderValue(Request.Headers);
            if (string.IsNullOrEmpty(authHeaderValue))
            {
                return Content(HttpStatusCode.Unauthorized, string.Empty);
            }
            if (!VerifyToken(authHeaderValue, username))
            {
                return Content(HttpStatusCode.Forbidden, string.Empty);
            }

            if (model == null || !model.Flights.Any())
            {
                return Content(HttpStatusCode.BadRequest, string.Empty);
            }

            foreach (var flight in model.Flights)
            {
                flight.BookedOn = "try-cb-dotnet";
            }

            var userKey = CreateUserKey(username);
            var getUserResult = await _bucket.GetAsync<User>(userKey);
            if (!getUserResult.Success)
            {
                if (getUserResult.Status == ResponseStatus.KeyNotFound)
                {
                    return Content(HttpStatusCode.Forbidden, string.Empty);
                }
                return Content(HttpStatusCode.InternalServerError, new Error(getUserResult.Message));
            }

            if (getUserResult.Value.Flights == null)
            {
                getUserResult.Value.Flights = new List<Flight>(model.Flights);
            }
            else
            {
                getUserResult.Value.Flights.AddRange(model.Flights);
            }

            var result = await _bucket.ReplaceAsync(userKey, getUserResult.Value);
            if (!result.Success)
            {
                return Content(HttpStatusCode.InternalServerError, result.Message);
            }

            var data = new
            {
                added = model.Flights
            };
            return Content(HttpStatusCode.Accepted, new Result(data));
        }

        private static string CreateUserKey(string username)
        {
            return $"user::{username}";
        }

        private static string GetAuthHeaderValue(HttpHeaders headers)
        {
            IEnumerable<string> headerValues;
            if (!headers.TryGetValues("authentication", out headerValues))
            {
                return null;
            }

            return headerValues.FirstOrDefault(x => x.StartsWith("Bearer"));
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

            token = token.Substring(7);
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
