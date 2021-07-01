using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using try_cb_dotnet.Helpers;

namespace try_cb_dotnet.Services
{
    public interface IAuthTokenService
    {
        string CreateToken(string tenant, string username);
        bool VerifyToken(string encodedToken, string tenant, string username);
    }

    public class AuthTokenService : IAuthTokenService
    {
        private readonly AppSettings _appSettings;

        public AuthTokenService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string CreateToken(string tenant, string username)
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim("tenant", tenant),
                    new Claim("user", username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool VerifyToken(string encodedToken, string tenant, string username)
        {
            if (encodedToken.StartsWith("Bearer "))
            {
                encodedToken = encodedToken.Substring(7);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(encodedToken);

            return token.Claims.Any(x => x.Type == "user" && x.Value == username)
                && token.Claims.Any(x => x.Type == "tenant" && x.Value == tenant);
        }
    }
}
