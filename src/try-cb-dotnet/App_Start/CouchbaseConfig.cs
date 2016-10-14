using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Couchbase;
using Couchbase.Configuration.Client;

namespace try_cb_dotnet
{
    public static class CouchbaseConfig
    {
        public static void Register()
        {
            var couchbaseServer = ConfigurationManager.AppSettings.Get("couchbaseServer");
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(couchbaseServer) }
            });

            EnsureIndexes();
        }

        private static void EnsureIndexes()
        {
            var indexNames = new List<string>
                {"def_sourceairport", "def_airportname", "def_type", "def_faa", "def_icao", "def_city"};

            var bucket = ClusterHelper.GetBucket("travel-sample");
            var result = bucket.Query<dynamic>("SELECT indexes.* FROM system:indexes WHERE keyspace_id = 'travel-sample';");


            var foundIndexes = new List<string>();
            var hasPrimary = false;
            foreach (var row in result.Rows)
            {
                if (row.is_primary == "true")
                {
                    hasPrimary = true;
                }
                else
                {
                    foundIndexes.Add((string) row.name);
                }
            }

            if (!hasPrimary)
            {
                bucket.Query<dynamic>("CREATE PRIMARY INDEX `def_primary` ON `travel-sample`;");
            }

            var missingIndexes = indexNames.Where(index => !foundIndexes.Contains(index)).ToList();
            if (!missingIndexes.Any())
            {
                return;
            }

            foreach (var missingIndex in missingIndexes)
            {
                var propertyName = missingIndex.Replace("def_", string.Empty);
                bucket.Query<dynamic>($"CREATE INDEX `{missingIndex}` ON `travel-sample`(`{propertyName}`);");
            }

            bucket.Query<dynamic>($"BUILD INDEX ON `travel-sample` ({string.Join(", ", missingIndexes)});");
        }

        public static void CleanUp()
        {
            ClusterHelper.Close();
        }
    }
}