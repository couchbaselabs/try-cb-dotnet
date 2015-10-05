using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using try_cb_dotnet.Models;
using Couchbase.N1QL;
using Couchbase.Linq.Extensions;
using Newtonsoft.Json.Linq;
using Couchbase;
using Couchbase.Linq.SampleBuckets.Documents.TravelDocuments;

namespace try_cb_dotnet.Controllers
{
    public class FlightPathController : ApiController
    {
        [HttpGet]
        [ActionName("findAll")]
        public object FindAll(string from, DateTime leave, string to, string token)
        {
            //var airlinenames = ...

            List<dynamic> airlinenames = 
                new List<object>{
                    new { fromAirport = "SFO" },
                    new { toAirport = "LAX" },
                };

            var airlinesResult = airlinenames.ToList();

            string queryFrom = null;
            string queryTo = null;
            var queryLeave = (int)leave.DayOfWeek;

            foreach (var row in airlinesResult)
            {
                try
                {
                    if (row.fromAirport != null) queryFrom = row.fromAirport;
                }
                catch (Exception)
                {
                    // silence the exception as this is known to throw one time,
                    // for the row that does not contain toAirport. 
                    // There is no easy way to test for the missing attribute on the 
                    // dynamic type.
                }

                try
                {
                    if (row.toAirport != null) queryTo = row.toAirport;
                }
                catch (Exception)
                {
                    // silence the exception as this is known to throw one time,
                    // for the row that does not contain fromAirport. 
                    // There is no easy way to test for the missing attribute on the 
                    // dynamic type.
                }
            }

            // raw query
            var query2 =
                   new QueryRequest(
                       "SELECT r.id, a.name, s.flight, s.utc, r.sourceairport, r.destinationairport, r.equipment FROM " +
                       "`" + CouchbaseConfigHelper.Instance.Bucket + "` r " +
                       "UNNEST r.schedule s JOIN " +
                       "`" + CouchbaseConfigHelper.Instance.Bucket + "` " +
                       "a ON KEYS r.airlineid WHERE r.sourceairport=$from " +
                       "AND r.destinationairport=$to " +
                       "AND s.day=$leave " +
                       "ORDER BY a.name")
                   .AddNamedParameter("from", queryFrom)
                   .AddNamedParameter("to", queryTo)
                   .AddNamedParameter("leave", queryLeave);

            return ClusterHelper
                    .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                    .Query<dynamic>(query2)
                    .Rows;
        }
    }
}