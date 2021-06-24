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
            var task = Task.Factory.StartNew(async () =>
            {
                 var options = new ClusterOptions
                {
                    UserName = "Administrator",
                    Password = "password"
                };
                var cluster = await Couchbase.Cluster.ConnectAsync(
                    "couchbase://10.112.193.101"
                );
                Cluster = cluster;
                DefaultBucket = await Cluster.BucketAsync("travel-sample");
                DefaultCollection = await DefaultBucket.DefaultCollectionAsync();

                return Task.CompletedTask;
            });
            task.ConfigureAwait(false);
            task.Wait();
        }
    }
}
