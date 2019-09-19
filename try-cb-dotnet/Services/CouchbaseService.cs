using System.Threading.Tasks;
using Couchbase;

namespace try_cb_dotnet.Services
{
    public interface ICouchbaseService
    {
        ICluster Cluster { get; }
        IBucket DefaultBucket { get; }
        ICollection DefaultCollection { get; }
    }

    public class CouchbaseService : ICouchbaseService
    {
        public ICluster Cluster { get; private set; }
        public IBucket DefaultBucket { get; private set; }
        public ICollection DefaultCollection { get; private set; }

        public CouchbaseService()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                var cluster = new Cluster(new Configuration()
                    .WithServers("couchbase://10.112.193.101")
                    .WithBucket("default")
                    .WithCredentials("Administrator", "password"));

                Cluster = cluster;
                DefaultBucket = await Cluster.BucketAsync("default").ConfigureAwait(false);
                DefaultCollection = await DefaultBucket.DefaultCollectionAsync().ConfigureAwait(false);

                return Task.CompletedTask;
            });
            task.ConfigureAwait(false);
            task.Wait();
        }
    }
}
