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
        private readonly IBucket _bucket = ClusterHelper.GetBucket(ConfigurationManager.AppSettings.Get("couchbaseTravelBucket"));

        [Route("{from}/{to}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetFlights(string from, string to, string leave)
        {
            DateTime leaveDate;
            if (!DateTime.TryParse(leave, out leaveDate))
            {
                return Content(HttpStatusCode.InternalServerError, new Error("Missing or invalid leave date"));
            }

            var queries = new List<string>();
            var dayOfWeek = (int) leaveDate.DayOfWeek + 1;

            var airportQuery = new QueryRequest()
                .Statement("SELECT faa AS fromAirport, geo.lat, geo.lon " +
                           "FROM `travel-sample` " +
                           "WHERE airportname = '$1' " +
                           "UNION " +
                           "SELECT faa AS toAirport, geo.lat, geo.lon " +
                           "FROM `travel-sample` " +
                           "WHERE airportname = '$2';")
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

            //TODO: get real calulations
            var flightTime = Math.Round(distance/150);
            var price = Math.Round(distance*1.5, 2);

            var flightQuery = "SELECT a.name, s.flight, s.utc, r.sourceairport, r.destinationairport, r.equipment " +
                              "FROM `travel-sample` AS r " +
                              "UNNEST r.schedule AS s " +
                              "JOIN `travel-sample` AS a ON KEYS r.airlineid " +
                             $"WHERE r.sourceairport = '{fromAirport.fromAirport}' " +
                             $"AND r.destinationairport = '{toAirport.toAirport}' " +
                             $"AND s.day = {dayOfWeek} " +
                              "ORDER BY a.name ASC;";
            queries.Add(flightQuery);

            var flightQueryResult = await _bucket.QueryAsync<dynamic>(flightQuery);
            if (!flightQueryResult.Success)
            {
                return Content(HttpStatusCode.InternalServerError, new Error(flightQueryResult.Message));
            }

            var flights = flightQueryResult.Rows;
            foreach (var flight in flights)
            {
                flight.flighttime = flightTime;
                flight.price = Math.Round(_random.NextDouble()*price/100, 2); // TODO: fix calculation
            }

            return Ok(new Result(flights, queries.ToArray()));
        }
    }
}
