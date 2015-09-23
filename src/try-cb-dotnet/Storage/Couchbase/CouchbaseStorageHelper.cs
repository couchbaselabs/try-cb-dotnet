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
            config.BucketConfigs.Clear();
            config.BucketConfigs.Add(
                CouchbaseConfigHelper.Instance.Bucket, 
                new BucketConfiguration
                {
                    BucketName = CouchbaseConfigHelper.Instance.Bucket,
                    Username = CouchbaseConfigHelper.Instance.User,
                    Password = CouchbaseConfigHelper.Instance.Password
                });

            config.BucketConfigs.Add(
                "default",
                new BucketConfiguration
                {
                    BucketName = "default",
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
            return Upsert(id, model, CouchbaseConfigHelper.Instance.Bucket);
        }

        public IOperationResult<dynamic> Upsert(string id, object model, string bucket)
        {
            return ClusterHelper
                .GetBucket(bucket)
                .Upsert<dynamic>(id, model);
        }

        public IOperationResult<dynamic> Get(string id)
        {
            return Get(id, CouchbaseConfigHelper.Instance.Bucket);
        }

        public IOperationResult<dynamic> Get(string id, string bucket)
        {
            return ClusterHelper
                .GetBucket(bucket)
                .Get<dynamic>(id);
        }

        public bool Exists(string id)
        {
            return Exists(id, CouchbaseConfigHelper.Instance.Bucket);
        }

        public bool Exists(string id, string bucket)
        {
            return ClusterHelper
                .GetBucket(bucket)
                .Exists(id);
        }
    }
}