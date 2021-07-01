using System;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.KeyValue;

namespace try_cb_dotnet.Services
{
    public interface ICouchbaseService
    {
        ICluster Cluster { get; }
        IBucket TravelSampleBucket { get; }
        ICouchbaseCollection HotelCollection { get; }
        public Task<ICouchbaseCollection> TenantCollection(string tenant, string collection);
    }

    public class CouchbaseService : ICouchbaseService
    {
        public ICluster Cluster { get; private set; }
        public IBucket TravelSampleBucket { get; private set; }
        public ICouchbaseCollection HotelCollection { get; private set; }

        public CouchbaseService()
        {
            try {
                var task = Task.Run(async () => {
                    var cluster = await Couchbase.Cluster.ConnectAsync(
                        "couchbase://localhost",
                        "Administrator",
                        "password");

                    Cluster = cluster;
                    TravelSampleBucket = await Cluster.BucketAsync("travel-sample");
                    var inventoryScope = await TravelSampleBucket.ScopeAsync("inventory");
                    HotelCollection = await inventoryScope.CollectionAsync("hotel");
                });
                task.Wait();
            }
            catch (AggregateException ae) {
                ae.Handle((x) => throw x);
            }
        }

        public async Task<ICouchbaseCollection> TenantCollection(string tenant, string collection)
        {
            var tenantScope = await TravelSampleBucket.ScopeAsync(tenant);
            var tenantCollection = await tenantScope.CollectionAsync(collection);
            return tenantCollection;
        }
    }
}
