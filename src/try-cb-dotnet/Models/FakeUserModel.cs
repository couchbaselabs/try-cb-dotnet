using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace try_cb_dotnet.Models
{
    public class FakeUserModel : UserModel
    {
        public string JWTToken { get; set; }

        public static FakeUserModel GetFakeUser
        {
            get
            {
                return new FakeUserModel
                {
                    Password = "guest",
                    User = "guest",
                    JWTToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyIjoiZ3Vlc3QiLCJpYXQiOjE0NDE4Njk5NTR9.5jPBtqralE3W3LPtS - j3MClTjwP9ggXSCDt3 - zZOoKU"
                };
            }
        }
    }
}