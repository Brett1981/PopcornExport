using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PopcornExport.Models.Movie
{
    public class MovieShortJsonNode
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }

        [JsonProperty("data")]
        public MovieListYTSJson Data { get; set; }

        [JsonProperty("@meta")]
        public MovieJsonMeta Meta { get; set; }
    }
}
