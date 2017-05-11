using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;

namespace try_cb_dotnet
{
    public static class CouchbaseConfig
    {
        private static readonly List<string> TravelSampleIndexNames = new List<string>
        {
            "def_sourceairport",
            "def_airportname",
            "def_type",
            "def_faa",
            "def_icao",
            "def_city"
        };

        public static void Register()
        {
            var couchbaseServer = ConfigurationManager.AppSettings.Get("CouchbaseServer");
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(couchbaseServer) }
            });

            var bucketName = ConfigurationManager.AppSettings.Get("CouchbaseTravelBucket");
            var username = ConfigurationManager.AppSettings.Get("CouchbaseUser");
            var password = ConfigurationManager.AppSettings.Get("CouchbasePassword");

			// provide authentication to cluster
	        ClusterHelper.Get().Authenticate(new PasswordAuthenticator(username, password));

	        EnsureIndexes(bucketName);
        }

        private static void EnsureIndexes(string bucketName)
        {
            var bucket = ClusterHelper.GetBucket(bucketName);
            var bucketManager = bucket.CreateManager();

            var indexes = bucketManager.ListN1qlIndexes();
            if (!indexes.Any(index => index.IsPrimary))
            {
                bucketManager.CreateN1qlPrimaryIndex(true);
            }

            var missingIndexes = TravelSampleIndexNames.Except(indexes.Where(x => !x.IsPrimary).Select(x => x.Name)).ToList();
            if (!missingIndexes.Any())
            {
                return;
            }

            foreach (var missingIndex in missingIndexes)
            {
                var propertyName = missingIndex.Replace("def_", string.Empty);
                bucketManager.CreateN1qlIndex(missingIndex, true, propertyName);
            }

            bucketManager.BuildN1qlDeferredIndexes();
            bucketManager.WatchN1qlIndexes(missingIndexes, TimeSpan.FromSeconds(30));
        }

        public static void CleanUp()
        {
            ClusterHelper.Close();
        }
    }
}