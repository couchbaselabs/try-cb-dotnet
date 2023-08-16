using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace try_cb_dotnet.Models
{
    public class Result
    {
        public Result(dynamic data, params string[] contexts)
        {
            Data = data;
            Context = contexts ?? Array.Empty<string> ();
        }

        [JsonPropertyName("data")]
        public dynamic Data { get; }

        [JsonPropertyName("context")]
        public IEnumerable<string> Context { get; }
    }
}
