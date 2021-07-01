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
                token = _authTokenService.CreateToken(tenant, username)};
            return Accepted(new Result(data));
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(string tenant, [FromBody] LoginModel model)
        {
            var user = await _userService.GetAndAuthenticateUser(tenant, model.Username, model.Password);
            if (user == null)
            {
                return BadRequest("Invalid username / password");
            }

            var data = new {token = _authTokenService.CreateToken(tenant, user.Username)};
            return Ok(new Result(data));
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

            return Ok(new Result(user.Flights));
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

            return Accepted(new Result(model.Flights));
        }
    }
}
