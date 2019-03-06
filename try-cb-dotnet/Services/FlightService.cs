using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Services.Query;
using Microsoft.Extensions.Options;
using try_cb_dotnet.Models;
using try_cb_dotnet.Helpers;

namespace try_cb_dotnet.Services
{
    public interface IFlightService
    {
        Task<IEnumerable<Route>> GetFlights(string from, string to, DateTime leaveDate);
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

        public async Task<IEnumerable<Route>> GetFlights(string from, string to, DateTime leaveDate)
        {
            var dayOfWeek = (int)leaveDate.DayOfWeek + 1; // Get weekday number

            var airportQueryResult = await _couchbaseService.Cluster.Query<dynamic>(
                "SELECT faa AS fromAirport, geo.lat, geo.lon " +
                "FROM `travel-sample` " +
                "WHERE faa = $1 " +
                "UNION " +
                "SELECT faa AS toAirport, geo.lat, geo.lon " +
                "FROM `travel-sample` " +
                "WHERE faa = $2;",
                parameters => parameters.Add(from)
                    .Add(to),
                options => options.UseStreaming(false)
            );

            if (airportQueryResult.Status != QueryStatus.Success || airportQueryResult.Rows.Any())
            {
                return null;
            }

            var fromAirport = airportQueryResult.Rows.First(x => x.fromAirport != null);
            var toAirport = airportQueryResult.Rows.First(x => x.toAirport != null);

            var fromCoordinate = new GeoCoordinate((double)fromAirport.lat, (double)fromAirport.lon);
            var toCoordinate = new GeoCoordinate((double)toAirport.lat, (double)toAirport.lon);
            var distance = fromCoordinate.GetDistanceTo(toCoordinate);
            var flightTime = Math.Round(distance / _appSettings.AverageFlightSpeed, 2);

            var flightQueryResult = await _couchbaseService.Cluster.Query<Route>(
                "SELECT a.name, s.flight, s.utc, r.sourceairport, r.destinationairport, r.equipment " +
                "FROM `travel-sample` AS r " +
                "UNNEST r.schedule AS s " +
                "JOIN `travel-sample` AS a ON KEYS r.airlineid " +
                "WHERE r.sourceairport = $1 " +
                "AND r.destinationairport = $2 " +
                "AND s.day = $3 " +
                "ORDER BY a.name ASC;",
                parameters => parameters.Add((string) fromAirport.fromAirport)
                    .Add((string) toAirport.toAirport)
                    .Add(dayOfWeek),
                options => options.UseStreaming(false)
            );

            if (flightQueryResult.Status != QueryStatus.Success)
            {
                return null;
            }

            var flights = flightQueryResult.Rows;
            foreach (var flight in flights)
            {
                flight.FlightTime = flightTime;
                flight.Price = Random.Next(2000);
            }

            return flights;
        }
    }
}
