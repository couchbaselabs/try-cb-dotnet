using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using try_cb_dotnet.Models;

namespace try_cb_dotnet.Services
{
    public interface IUserService
    {
        Task<bool> UserExists(string tenant, string username);
        Task<User> CreateUser(string tenant, string username, string password, uint expiry);
        Task<User> GetUser(string tenant, string username);
        Task<User> GetAndAuthenticateUser(string tenant, string username, string password);
        Task UpdateUser(string tenant, User user);
    }

    public class UserService : IUserService
    {
        private readonly ICouchbaseService _couchbaseService;

        public UserService(ICouchbaseService couchbaseService)
        {
            _couchbaseService = couchbaseService;
        }

        public async Task<bool> UserExists(string tenant, string username)
        {
            var userCollection = await _couchbaseService.TenantCollection(tenant, "users");
            var result = await userCollection.ExistsAsync(
                $"user::{username}",
                new Couchbase.KeyValue.ExistsOptions());
            return result.Exists;
        }

        public async Task<User> CreateUser(string tenant, string username, string password, uint expiry)
        {
            var user = new User
            {
                Username = username,
                Password = CalculateMd5Hash(password)
            };

            try
            {
                var userCollection = await _couchbaseService.TenantCollection(tenant, "users");
                await userCollection.InsertAsync($"user::{username}", user, new Couchbase.KeyValue.InsertOptions());
            }
            catch
            {
                return null;
            }

            return user;
        }

        public async Task<User> GetUser(string tenant, string username)
        {
            try
            {
                var userCollection = await _couchbaseService.TenantCollection(tenant, "users");
                var result =  await userCollection.GetAsync($"user::{username}", new Couchbase.KeyValue.GetOptions());
                return result.ContentAs<User>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<User> GetAndAuthenticateUser(string tenant, string username, string password)
        {
            var user = await GetUser(tenant, username);
            if (user == null)
            {
                Console.WriteLine("User not found!");
                return null;
            }

            if (user.Password != password)
            {
                Console.WriteLine("User password wrong");
                return null;
            }

            return user;
        }

        public async Task UpdateUser(string tenant, User user)
        {
            var userCollection = await _couchbaseService.TenantCollection(tenant, "users");
            await userCollection.ReplaceAsync(
                $"user::{user.Username}",
                user,
                new Couchbase.KeyValue.ReplaceOptions());
        }

        private static string CalculateMd5Hash(string password)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                return string.Concat(bytes.Select(x => x.ToString("x2")));
            }
        }
    }
}
