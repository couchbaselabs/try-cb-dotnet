namespace try_cb_dotnet.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public uint Expiry { get; set; }
    }
}