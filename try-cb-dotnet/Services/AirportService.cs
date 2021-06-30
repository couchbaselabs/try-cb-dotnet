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
    public interface IAirportsService
    {
        Task<(IEnumerable<Airport>, string[])> GetAirports(string search);
    }

    public class AirportsService : IAirportsService
    {
        private readonly ICouchbaseService _couchbaseService;
        private readonly AppSettings _appSettings;

        public AirportsService(ICouchbaseService couchbaseService, IOptions<AppSettings> appSettings)
        {
            _couchbaseService = couchbaseService;
            _appSettings = appSettings.Value;
        }

        public async Task<(IEnumerable<Airport>, string[])> GetAirports(string search)
        {
            var q = "SELECT airportname FROM `travel-sample`.inventory.airport WHERE ";

            if (search.Length == 3)        // Is an faa code
            {
                q += "faa=$1";
                search = search.ToUpper();
            } else if (search.Length == 4) // Is an icao code
            {
                q += "icao=$1";
                search = search.ToUpper();
            } else                         // Is not a code
            {
                q += "airportname LIKE $1";
                search = char.ToUpper(search[0]) + search.Substring(1) + '%';
            }

            var airportsResult = await _couchbaseService.Cluster.QueryAsync<Airport>(
                q,
                options => options.Parameter(search)
            );

            if (airportsResult.MetaData.Status != QueryStatus.Success)
            {
                Console.WriteLine(airportsResult.Errors.OfType<string>());
                return (null, new string[] { "Query Failed." });
            }

            var airports = await airportsResult.Rows.ToListAsync();

            var context = new string[] {
                $"N1QL query - scoped to inventory: {q}; -- {search}"
            };

            return (airports, context);
        }
    }
}
