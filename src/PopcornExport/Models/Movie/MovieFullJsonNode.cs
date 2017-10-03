using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace PopcornExport.Models.Movie
{
    public class MovieFullJsonNode
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "status_message")]
        public string StatusMessage { get; set; }

        [DataMember(Name = "data")]
        public WrapperMovie Data { get; set; }

        [DataMember(Name = "@meta")]
        public MovieJsonMeta Meta { get; set; }

        public class WrapperMovie
        {
            [DataMember(Name = "movie")]
            public MovieJson Movie { get; set; }
        }
    }
}