using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using try_cb_dotnet.Models;
using try_cb_dotnet.Services;

namespace try_cb_dotnet.Controllers
{
    [ApiController]
    [Route("api/tenants/{tenant}/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthTokenService _authTokenService;

        public UserController(IUserService userService, IAuthTokenService authTokenService)
        {
            _userService = userService;
            _authTokenService = authTokenService;
        }

        [HttpPost("signup")]
        public async Task<ActionResult> SignUp(string tenant, LoginModel model)
        {
            string username = model.Username;

            if (await _userService.UserExists(tenant, username))
            {
                return Conflict($"Username '{username}' already exists");
            }

            await _userService.CreateUser(tenant, username, model.Password, model.Expiry);

            var data = new {
                token = _authTokenService.CreateToken(tenant, username)
            };
            var context = new string[] {
                $"KV insert - scoped to {tenant}.users: document {username}"
            };
            return Created("", new Result(data, context));
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(string tenant, [FromBody] LoginModel model)
        {
            var user = await _userService.GetAndAuthenticateUser(tenant, model.Username, model.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username / password");
            }

            var data = new {
                token = _authTokenService.CreateToken(tenant, user.Username),

            };
            var context = new string[] {
                $"KV get - scoped to {tenant}.users: for password field in document {user}"
            };
            return Ok(new Result(data, context));
        }

        [HttpGet("{username}/flights")]
        public async Task<ActionResult> GetFlightsForUser(string tenant, string username)
        {
            if (!_authTokenService.VerifyToken(Request.Headers["Authorization"], tenant, username))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUser(tenant, username);
            if (user == null)
            {
                return BadRequest("Invalid username");
            }
            
            if (user.Flights == null)
            {
                user.Flights = new List<Flight>();
            }

            var context = new string[] {
                $"KV get - scoped to {tenant}.users: for {user.Flights.Count} bookings in document {username}"
            };

            return Ok(new Result(user.Flights, context));
        }

        [HttpPut("{username}/flights")]
        public async Task<ActionResult> BookFlightsForUser(string tenant, string username, BookFlightModel model)
        {
            if (!_authTokenService.VerifyToken(Request.Headers["Authorization"], tenant, username))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUser(tenant, username);
            if (user == null)
            {
                return BadRequest("Invalid username");
            }

            foreach (var flight in model.Flights)
            {
                flight.BookedOn = "try-cb-dotnet";
            }

            if (user.Flights == null)
            {
                user.Flights = new List<Flight>();
            }

            user.Flights.AddRange(model.Flights);

            await _userService.UpdateUser(tenant, user);

            return Ok(
                new Result(
                    new { added = model.Flights },
                    new string[] {
                        $"KV update - scoped to {tenant}.users: for bookings subdocument field in document {username}"
                    }
                )
            );
        }
    }
}
