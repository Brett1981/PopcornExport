using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PopcornExport.Models.Movie
{
    public class MovieFullJsonNode
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }

        [JsonProperty("data")]
        public WrapperMovie Data { get; set; }

        [JsonProperty("@meta")]
        public MovieJsonMeta Meta { get; set; }

        public class WrapperMovie
        {
            [JsonProperty("movie")]
            public MovieJson Movie { get; set; }
        }
    }
}