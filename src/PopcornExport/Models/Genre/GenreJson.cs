using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PopcornExport.Models.Genre
{
    public class GenreJson
    {
        [JsonProperty]
        public IEnumerable<string> Name { get; set; }
    }
}
