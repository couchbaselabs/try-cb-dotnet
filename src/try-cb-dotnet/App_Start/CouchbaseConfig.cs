using System;
using System.Collections.Generic;
using System.Configuration;
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
        }

        public static void CleanUp()
        {
            ClusterHelper.Close();
        }
    }
}