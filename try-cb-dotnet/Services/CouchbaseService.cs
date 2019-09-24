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
                var cluster = new Cluster(
                    new Configuration()
                        .WithServers("couchbase://10.143.191.101")
                        .WithBucket("travel-sample")
                        .WithCredentials("Danzibob", "C0uchbase123")
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
