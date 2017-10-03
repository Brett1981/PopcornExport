using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace PopcornExport.Models.Movie
{
    public class MovieListYTSJson
    {
        [DataMember(Name = "movie_count")]
        public int MovieCount { get; set; }

        [DataMember(Name = "limit")]
        public int Limit { get; set; }

        [DataMember(Name = "page_number")]
        public int PageNumber { get; set; }

        [DataMember(Name = "movies")]
        public List<MovieYTSJson> Movies { get; set; }
    }
}