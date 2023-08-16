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

    public static class StringExtension
    {
        public static string DefaultIfEmpty(this string str, string defaultValue)
            => string.IsNullOrWhiteSpace(str) ? defaultValue : str;
    }

    public class CouchbaseService : ICouchbaseService
    {
        public ICluster Cluster { get; private set; }
        public IBucket TravelSampleBucket { get; private set; }
        public ICouchbaseCollection HotelCollection { get; private set; }

        public CouchbaseService()
        {
            // TODO: get these variables via DI, possibly overriding config in appsettings.json
            var CB_HOST = Environment.GetEnvironmentVariable("CB_HOST").DefaultIfEmpty("localhost");
            var CB_USER = Environment.GetEnvironmentVariable("CB_USER").DefaultIfEmpty("Administrator");
            var CB_PSWD = Environment.GetEnvironmentVariable("CB_PSWD").DefaultIfEmpty("P@ssw0rd12");

            Console.WriteLine(
                $"Connecting to couchbase://{CB_HOST} with {CB_USER} / {CB_PSWD}");

            try {
                var task = Task.Run(async () => {
                    var cluster = await Couchbase.Cluster.ConnectAsync(
                        $"couchbase://{CB_HOST}",
                        CB_USER,
                        CB_PSWD);

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
