using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using try_cb_dotnet.Models;

namespace try_cb_dotnet.Services
{
    public interface IUserService
    {
        Task<bool> UserExists(string username);
        Task<User> CreateUser(string username, string password, uint expiry);
        Task<User> GetUser(string username);
        Task<User> GetAndAuthenticateUser(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly ICouchbaseService _couchbaseService;

        public UserService(ICouchbaseService couchbaseService)
        {
            _couchbaseService = couchbaseService;
        }

        public async Task<bool> UserExists(string username)
        {
            var result = await _couchbaseService.DefaultCollection.Exists($"user::{username}");
            return result.Exists;
        }

        public async Task<User> CreateUser(string username, string password, uint expiry)
        {
            var user = new User
            {
                Username = username,
                Password = CalculateMd5Hash(password)
            };

            try
            {
                await _couchbaseService.DefaultCollection.Insert($"user::{username}", user);
            }
            catch
            {
                return null;
            }

            return user;
        }

        public async Task<User> GetUser(string username)
        {
            try
            {
                var result =  await _couchbaseService.DefaultCollection.Get($"user::{username}");
                return result.ContentAs<User>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<User> GetAndAuthenticateUser(string username, string password)
        {
            var user = await GetUser(username);
            if (user == null)
            {
                return null;
            }

            if (user.Password != CalculateMd5Hash(password))
            {
                return null;
            }

            return user;
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
