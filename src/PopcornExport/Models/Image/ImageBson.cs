using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PopcornExport.Models.Image
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class ImageBson
    {
        [DataMember]
        [BsonElement("poster")]
        public string Poster { get; set; }

        [DataMember]
        [BsonElement("fanart")]
        public string Fanart { get; set; }

        [DataMember]
        [BsonElement("banner")]
        public string Banner { get; set; }
    }
}
