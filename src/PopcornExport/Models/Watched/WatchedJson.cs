using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornExport.Models.Watched
{
    public class WatchedJson
    {
        [JsonProperty("watched")]
        public bool Watched { get; set; }
    }
}
