using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using try_cb_dotnet.Models;
using try_cb_dotnet.Services;

namespace try_cb_dotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthTokenService _authTokenService;

        public UserController(IUserService userService, IAuthTokenService authTokenService)
        {
            _userService = userService;
            _authTokenService = authTokenService;
        }

        [HttpPost, Route("signup")]
        public async Task<ActionResult> SignUp([FromBody] LoginModel model)
        {
            // TODO: use exists instead
            var user = await _userService.Authenticate(model.Username, model.Password);
            if (user != null)
            {
                return Conflict($"Username '{model.Username}' already exists");
            }

            try
            {
                await _userService.CreateUser(model.Username, model.Password, model.Expiry);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            var data = new {token = _authTokenService.CreateToken(model.Username)};
            return Accepted(new Result(data));
        }

        [HttpPost, Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userService.Authenticate(model.Username, model.Password);
            if (user == null)
            {
                return BadRequest("Invalid username / password");
            }

            var data = new {token = _authTokenService.CreateToken(user.Username)};
            return Ok(new Result(data));
        }

        [HttpGet, Route("{username}/flights")]
        public async Task<ActionResult> GetFlightsForUser(string username)
        {
            throw new NotImplementedException();
        }

        [HttpPost, Route("{username}/flights")]
        public Task<ActionResult> RegisterFlightForUser(string username, BookFlightModel model)
        {
            throw new NotImplementedException();
        }
    }
}
