using System.Threading.Tasks;
using Couchbase;

namespace try_cb_dotnet.Services
{
    public interface ICouchbaseService
    {
        ICluster Cluster { get; }
        IBucket Bucket { get; }
        ICollection Collection { get; }
    }

    public class CouchbaseService : ICouchbaseService
    {
        public ICluster Cluster { get; private set; }
        public IBucket Bucket { get; private set; }
        public ICollection Collection { get; private set; }

        public CouchbaseService()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                var cluster = new Cluster();
                await cluster.Initialize(
                    new Configuration()
                        .WithServers("couchbase://10.112.193.101")
                        .WithBucket("default")
                        .WithCredentials("Administrator", "password")
                ).ConfigureAwait(false);

                Cluster = cluster;
                Bucket = await Cluster.Bucket("default").ConfigureAwait(false);
                Collection = await Bucket.DefaultCollection.ConfigureAwait(false);

                return Task.CompletedTask;
            });
            task.ConfigureAwait(false);
            task.Wait();
        }
    }
}
