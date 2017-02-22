using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PopcornExport.Models.Watched
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class WatchedModel
    {
        [DataMember]
        [BsonElement("watched")]
        public bool Watched { get; set; }
    }
}
