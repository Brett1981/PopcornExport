using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PopcornExport.Models.Movie
{
    public class MovieJsonMeta
    {
        [JsonProperty("server_time")]
        public int ServerTime { get; set; }

        [JsonProperty("server_timezone")]
        public string ServerTimezone { get; set; }

        [JsonProperty("api_version")]
        public int ApiVersion { get; set; }

        [JsonProperty("execution_time")]
        public string ExecutionTime { get; set; }
    }
}