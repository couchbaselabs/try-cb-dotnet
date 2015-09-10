using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.N1QL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace try_cb_dotnet.Storage.Couchbase
{
    public class CouchbaseStorageHelper
    {
        public CouchbaseStorageHelper()
        {
            Initialize();
        }

        private static CouchbaseStorageHelper instance = null;

        public static CouchbaseStorageHelper Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new CouchbaseStorageHelper();
                }

                return instance;
            }
        }

        public void Initialize()
        {
            var config = new ClientConfiguration();
            config.Servers = new List<Uri>(new Uri[] { new Uri(CouchbaseConfigHelper.Instance.Server) });
            config.BucketConfigs.Add(
                CouchbaseConfigHelper.Instance.Bucket, 
                new BucketConfiguration
                {
                    BucketName = CouchbaseConfigHelper.Instance.Bucket,
                    Username = CouchbaseConfigHelper.Instance.User,
                    Password = CouchbaseConfigHelper.Instance.Password
                });

            ClusterHelper.Initialize(config);
        }

        public IQueryResult<dynamic> ExecuteQuery(string query)
        {
            return ClusterHelper
                .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                .Query<dynamic>(new QueryRequest(query));
        }

        public string ExecuteQuery(string query, Formatting format)
        {
            return JsonConvert.SerializeObject(ExecuteQuery(query), format);
        }

        public IOperationResult<dynamic> Upsert(string id, object model)
        {
            return ClusterHelper
                .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                .Upsert<dynamic>(id, model);
        }

        public IOperationResult<dynamic> Get(string id)
        {
            return ClusterHelper
                .GetBucket(CouchbaseConfigHelper.Instance.Bucket)
                .Get<dynamic>(id);
        }
    }
}