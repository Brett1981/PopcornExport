using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace PopcornExport.Models.Movie
{
    [DataContract]
    public class MovieShortJsonNode
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "status_message")]
        public string StatusMessage { get; set; }

        [DataMember(Name = "data")]
        public MovieListYTSJson Data { get; set; }

        [DataMember(Name = "@meta")]
        public MovieJsonMeta Meta { get; set; }
    }
}
