using System;
using System.Collections.Generic;
using System.Configuration;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;
using try_cb_dotnet.Models;
using Error = try_cb_dotnet.Models.Error;

namespace try_cb_dotnet.Controllers
{
    [RoutePrefix("api/flightPaths")]
    public class FlightsController : ApiController
    {
        private readonly Random _random = new Random();
        private readonly IBucket _bucket = ClusterHelper.GetBucket(ConfigurationManager.AppSettings.Get("CouchbaseTravelBucket"));
        private static readonly double AverageFlightSpeed = double.Parse(ConfigurationManager.AppSettings.Get("AverageFlightSpeed"));

        [Route("{from}/{to}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetFlights(string from, string to, string leave)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return Content(HttpStatusCode.InternalServerError, new Error("Missing or invalid from and/or to airports"));
            }

            DateTime leaveDate;
            if (!DateTime.TryParse(leave, out leaveDate))
            {
                return Content(HttpStatusCode.InternalServerError, new Error("Missing or invalid leave date"));
            }

            var queries = new List<string>();
            var dayOfWeek = (int) leaveDate.DayOfWeek + 1; // Get weekday number

            var airportQuery = new QueryRequest()
                .Statement("SELECT faa AS fromAirport, geo.lat, geo.lon " +
                           "FROM `travel-sample` " +
                           "WHERE airportname = $1 " +
                           "UNION " +
                           "SELECT faa AS toAirport, geo.lat, geo.lon " +
                           "FROM `travel-sample` " +
                           "WHERE airportname = $2;")
                .AddPositionalParameter(from, to);
            queries.Add(airportQuery.GetOriginalStatement());

            var airportQueryResult = await _bucket.QueryAsync<dynamic>(airportQuery);
            if (!airportQueryResult.Success)
            {
                return Content(HttpStatusCode.InternalServerError, new Error(airportQueryResult.Message));
            }

            if (airportQueryResult.Rows.Count != 2)
            {
                return Content(HttpStatusCode.InternalServerError, new Error($"Could not find both source '{from}' and destination '{to}' airports"));
            }

            dynamic fromAirport = airportQueryResult.Rows.First(x => x.fromAirport != null);
            dynamic toAirport = airportQueryResult.Rows.First(x => x.toAirport != null);

            var fromCoordinate = new GeoCoordinate((double) fromAirport.lat, (double) fromAirport.lon);
            var toCoordinate = new GeoCoordinate((double) toAirport.lat, (double) toAirport.lon);
            var distance = fromCoordinate.GetDistanceTo(toCoordinate);
            var flightTime = Math.Round(distance/AverageFlightSpeed, 2);

            var flightQuery = new QueryRequest()
                .Statement("SELECT a.name, s.flight, s.utc, r.sourceairport, r.destinationairport, r.equipment " +
                           "FROM `travel-sample` AS r " +
                           "UNNEST r.schedule AS s " +
                           "JOIN `travel-sample` AS a ON KEYS r.airlineid " +
                           "WHERE r.sourceairport = $1 " +
                           "AND r.destinationairport = $2 " +
                           "AND s.day = $3 " +
                           "ORDER BY a.name ASC;")
                .AddPositionalParameter((string) fromAirport.fromAirport, (string) toAirport.toAirport, dayOfWeek);
            queries.Add(flightQuery.GetOriginalStatement());

            var flightQueryResult = await _bucket.QueryAsync<Route>(flightQuery);
            if (!flightQueryResult.Success)
            {
                return Content(HttpStatusCode.InternalServerError, new Error(flightQueryResult.Message));
            }

            var flights = flightQueryResult.Rows;
            foreach (var flight in flights)
            {
                flight.FlightTime = flightTime;
                flight.Price = _random.Next(2000);
            }

            return Content(HttpStatusCode.OK, new Result(flights, queries.ToArray()));
        }
    }
}
