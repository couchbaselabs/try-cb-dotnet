using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Query;
using Microsoft.Extensions.Options;
using try_cb_dotnet.Models;
using try_cb_dotnet.Helpers;
using Couchbase;

namespace try_cb_dotnet.Services
{
    public interface IFlightService
    {
        Task<(IEnumerable<Route>, string[])> GetFlights(string from, string to, DateTime leaveDate);
    }

    public class FlightService : IFlightService
    {
        private static readonly Random Random = new Random();

        private readonly ICouchbaseService _couchbaseService;
        private readonly AppSettings _appSettings;

        public FlightService(ICouchbaseService couchbaseService, IOptions<AppSettings> appSettings)
        {
            _couchbaseService = couchbaseService;
            _appSettings = appSettings.Value;
        }

        public async Task<(IEnumerable<Route>, string[])> GetFlights(string from, string to, DateTime leaveDate)
        {
            var dayOfWeek = (int)leaveDate.DayOfWeek + 1; // Get weekday number

            String q1 =
                "SELECT faa AS fromAirport, geo.lat, geo.lon " +
                "FROM `travel-sample`.inventory.airport " +
                "WHERE airportname = $1 " +
                "UNION " +
                "SELECT faa AS toAirport, geo.lat, geo.lon " +
                "FROM `travel-sample`.inventory.airport " +
                "WHERE airportname = $2;";

            var airportQueryResult = await _couchbaseService.Cluster.QueryAsync<dynamic>(
                q1,
                options => options.Parameter(from).Parameter(to)
            );

            var ctx1 = $"N1QL query - scoped to inventory.airport: {q1}; -- {from}, {to}";

            if (airportQueryResult.MetaData.Status != QueryStatus.Success)
            {
                return (null, new string[] { "First query failed:", ctx1 });
            }

            dynamic fromAirport = null, toAirport = null;

            await foreach (var row in airportQueryResult)
            {
                if (row.fromAirport != null)
                {
                    fromAirport = row;
                }
                else if (row.toAirport != null)
                {
                    toAirport = row;
                }
            }

            if (fromAirport == null || toAirport == null)
            {
                return (
                    null,
                    new string[] { "One of the specified airports is invalid.", ctx1 });
            }

            var fromCoordinate = new GeoCoordinate((double)fromAirport.lat, (double)fromAirport.lon);
            var toCoordinate = new GeoCoordinate((double)toAirport.lat, (double)toAirport.lon);
            var distance = fromCoordinate.GetDistanceTo(toCoordinate);
            var flightTime = Math.Round(distance / _appSettings.AverageFlightSpeed, 2);

            var q2 = "SELECT a.name, s.flight, s.utc, r.sourceairport, r.destinationairport, r.equipment " +
                "FROM `travel-sample`.inventory.route AS r " +
                "UNNEST r.schedule AS s " +
                "JOIN `travel-sample`.inventory.airline AS a ON KEYS r.airlineid " +
                "WHERE r.sourceairport = $1 " +
                "AND r.destinationairport = $2 " +
                "AND s.day = $3 " +
                "ORDER BY a.name ASC;";

            var flightQueryResult = await _couchbaseService.Cluster.QueryAsync<Route>(
                q2,
                options => options
                    .Parameter(fromAirport.fromAirport)
                    .Parameter(toAirport.toAirport)
                    .Parameter(dayOfWeek)
            );

            var ctx2 = $"N1QL query - scoped to inventory: {q2}; -- {fromAirport.fromAirport}, {toAirport.toAirport}, {dayOfWeek.ToString()}";


            if (flightQueryResult.MetaData.Status != QueryStatus.Success)
            {
                return (
                    null,
                    new string[] { "Second query failed:", ctx2 });
            }

            var flights = await flightQueryResult.Rows.ToListAsync();

            foreach (var flight in flights)
            {
                flight.FlightTime = flightTime;
                flight.Price = Random.Next(2000);
            }

            return (flights, new string[] { ctx1, ctx2 });
        }
    }
}
