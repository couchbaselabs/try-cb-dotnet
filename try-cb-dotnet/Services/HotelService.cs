﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Query;
using Microsoft.Extensions.Options;
using try_cb_dotnet.Models;
using try_cb_dotnet.Helpers;
using Couchbase;
using Couchbase.Search;
using Couchbase.Search.Queries.Simple;
using Couchbase.Search.Queries.Compound;
using Couchbase.KeyValue;

namespace try_cb_dotnet.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<dynamic>> FindHotel(string location, string description);
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

        public async Task<IEnumerable<dynamic>> FindHotel(string description, string location)
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
                    new PhraseQuery(location).Field("address"),
                    new PhraseQuery(location).Field("city"),
                    new PhraseQuery(location).Field("state"),
                    new PhraseQuery(location).Field("country")
                ));
            }
            // var Q = new SearchQuery { Query = query };

            // var queryString = (Q).ToJson();

            var opts = new SearchOptions();
            //opts.Limit(100);

            // TODO: this still returns nothing every time
            // Wireshark claims wrong URL is being hit (just ://IP:8094, no endpoint)
            var result = await _couchbaseService.Cluster.SearchQueryAsync(
                "hotels-index",
                query,
                opts
            );

            var hotels = new List<dynamic>();

            foreach (var row in result)
            {
                var fragment = await _couchbaseService.DefaultCollection.LookupInAsync(
                    row.Id,
                    new List<LookupInSpec>{
                        LookupInSpec.Get("name"),         //0
                        LookupInSpec.Get("description"),  //1
                        LookupInSpec.Get("address"),      //2
                        LookupInSpec.Get("city"),         //3
                        LookupInSpec.Get("state"),        //4
                        LookupInSpec.Get("country")       //5
                    });

                var address = string.Join(", ", new[]
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

            return hotels;
        }
    }
}
