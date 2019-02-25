using System.Collections.Generic;
using Newtonsoft.Json;

namespace try_cb_dotnet.Models
{
    public class Result
    {
        public Result(dynamic data, params string[] contexts)
        {
            Data = data;
            Context = contexts ?? new string[] {};
        }

        [JsonProperty("data")]
        public dynamic Data { get; }

        [JsonProperty("context")]
        public IEnumerable<string> Context { get; }
    }
}
