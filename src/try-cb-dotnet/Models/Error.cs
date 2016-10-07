using Newtonsoft.Json;

namespace try_cb_dotnet.Models
{
    public class Error
    {
        public Error(string failure)
        {
            Failure = failure;
        }

        [JsonProperty("failure")]
        public string Failure { get; }
    }
}