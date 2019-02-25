using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using try_cb_dotnet.Models;

namespace try_cb_dotnet.Services
{
    public interface IUserService
    {
        Task<bool> Exists(string username);
        Task<User> CreateUser(string username, string password, uint expiry);
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<Flight>> GetFlightsForUser(string username);
    }

    public class UserService : IUserService
    {
        private readonly ICouchbaseService _couchbaseService;

        public UserService(ICouchbaseService couchbaseService)
        {
            _couchbaseService = couchbaseService;
        }

        public Task<bool> Exists(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<User> CreateUser(string username, string password, uint expiry)
        {
            var user = new User
            {
                Username = username,
                Password = password
            };

            try
            {
                await _couchbaseService.Collection.Insert($"user::{username}", user);
            }
            catch (Exception)
            {
                return null;
            }

            return user;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            User user;
            try
            {
                var result =  await _couchbaseService.Collection.Get($"user::{username}");
                user = result.ContentAs<User>();
            }
            catch (Exception)
            {
                return null;
            }

            //TODO: verify password

            return user;
        }

        public async Task<IEnumerable<Flight>> GetFlightsForUser(string username)
        {
            throw new NotSupportedException();
        }
    }
}
