using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace PopcornExport.Models.Movie
{
    public class MovieJsonMeta
    {
        [DataMember(Name = "server_time")]
        public int ServerTime { get; set; }

        [DataMember(Name = "server_timezone")]
        public string ServerTimezone { get; set; }

        [DataMember(Name = "api_version")]
        public int ApiVersion { get; set; }

        [DataMember(Name = "execution_time")]
        public string ExecutionTime { get; set; }
    }
}