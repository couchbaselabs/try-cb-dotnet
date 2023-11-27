using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using try_cb_dotnet.Helpers;
using Couchbase.Search;
using Couchbase.Search.Queries.Simple;
using Couchbase.Search.Queries.Compound;
using Couchbase.KeyValue;

namespace try_cb_dotnet.Services
{
    public interface IHotelService
    {
        Task<(IEnumerable<dynamic>, string[])> FindHotel(string location, string description);
    }

    public class HotelService : IHotelService
    {
        private readonly ICouchbaseService _couchbaseService;
        private readonly AppSettings _appSettings;

        public HotelService(ICouchbaseService couchbaseService, IOptions<AppSettings> appSettings)
        {
            _couchbaseService = couchbaseService;
            _appSettings = appSettings.Value;
        }

        public async Task<(IEnumerable<dynamic>, string[])> FindHotel(string description, string location)
        {
            var query = new ConjunctionQuery();

            if (!string.IsNullOrEmpty(description) && description != "*")
            {
                query.And(new DisjunctionQuery(
                    new PhraseQuery(description).Field("name"),
                    new PhraseQuery(description).Field("description")
                ));
            }

            if (!string.IsNullOrEmpty(location) && location != "*")
            {
                query.And(new DisjunctionQuery(
                    new MatchQuery(location).Field("address"),
                    new MatchQuery(location).Field("city"),
                    new MatchQuery(location).Field("state"),
                    new MatchQuery(location).Field("country")
                ));
            }
            // // uncomment next line to show representation of the query in JSON
            // Console.WriteLine(query.Export());

            var opts = new SearchOptions().Limit(100);

            var result = await _couchbaseService.Cluster.SearchQueryAsync(
                "hotels-index",
                query,
                opts
            );

            var hotels = new List<dynamic>();

            var cols = new string[] {
                "name",         //0
                "description",  //1
                "address",      //2
                "city",         //3
                "state",        //4
                "country"       //5
            };

            var lookupInSspec =
                from col in cols
                select LookupInSpec.Get(col);

            foreach (var row in result)
            {
                var fragment = await _couchbaseService.HotelCollection.LookupInAsync(
                    row.Id,
                    lookupInSspec
                );

                var address = string.Join(", ", new []
                {
                    fragment.ContentAs<string>(2),
                    fragment.ContentAs<string>(3),
                    fragment.ContentAs<string>(4),
                    fragment.ContentAs<string>(5)
                }.Where(x => !string.IsNullOrEmpty(x)));

                hotels.Add(new
                {
                    name = fragment.ContentAs<string>(0),
                    description = fragment.ContentAs<string>(1),
                    address
                });
            }

            var context = new string[] {
                $"FTS search - scoped to: inventory.hotel within fields {String.Join(", ", cols)}\n{query.Export()}"
            };

            return (hotels, context);
        }
    }
}
