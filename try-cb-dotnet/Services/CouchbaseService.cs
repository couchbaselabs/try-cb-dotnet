using System;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.KeyValue;

namespace try_cb_dotnet.Services
{
    public interface ICouchbaseService
    {
        ICluster Cluster { get; }
        IBucket DefaultBucket { get; }
        ICouchbaseCollection DefaultCollection { get; }
    }

    public class CouchbaseService : ICouchbaseService
    {
        public ICluster Cluster { get; private set; }
        public IBucket DefaultBucket { get; private set; }
        public ICouchbaseCollection DefaultCollection { get; private set; }

        public CouchbaseService()
        {
            try {
                var task = Task.Run(async () => {
                    var cluster = await Couchbase.Cluster.ConnectAsync(
                        "couchbase://localhost",
                        "Administrator",
                        "password");

                    Cluster = cluster;
                    DefaultBucket = await Cluster.BucketAsync("travel-sample");
                    DefaultCollection = await DefaultBucket.DefaultCollectionAsync();
                });
                task.Wait();
            }
            catch (AggregateException ae) {
                ae.Handle((x) => throw x);
            }
        }
    }
}
