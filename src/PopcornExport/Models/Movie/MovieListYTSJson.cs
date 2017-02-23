using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PopcornExport.Models.Movie
{
    public class MovieListYTSJson
    {
        [JsonProperty("movie_count")]
        public int MovieCount { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("page_number")]
        public int PageNumber { get; set; }

        [JsonProperty("movies")]
        public List<MovieYTSJson> Movies { get; set; }
    }
}