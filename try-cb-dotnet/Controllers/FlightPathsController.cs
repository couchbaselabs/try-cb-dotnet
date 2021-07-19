using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using try_cb_dotnet.Models;
using try_cb_dotnet.Services;

namespace try_cb_dotnet.Controllers
{
    [ApiController]
    [Route("api/flightPaths")]
    public class FlightPathsController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightPathsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet("{from}/{to}")]
        public async Task<ActionResult> GetFlights(string from, string to, string leave)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return BadRequest("Missing / invalid arguments.");
            }

            if (!DateTime.TryParse(
                    leave,
                    new CultureInfo("en-US"), // we accept US `mm/dd/yyyy` format
                    DateTimeStyles.None,
                    out var leaveDate))
            {
                return BadRequest("Unable to parse leave date");
            }

            var (routes, context) = await _flightService.GetFlights(from, to, leaveDate);
            return Ok(new Result(routes, context));
        }
    }
}
