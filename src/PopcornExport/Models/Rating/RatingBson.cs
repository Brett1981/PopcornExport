using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PopcornExport.Models.Rating
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class RatingBson
    {
        [DataMember]
        [BsonElement("percentage")]
        public int Percentage { get; set; }

        [DataMember]
        [BsonElement("watching")]
        public int Watching { get; set; }

        [DataMember]
        [BsonElement("votes")]
        public int Votes { get; set; }

        [DataMember]
        [BsonElement("loved")]
        public int Loved { get; set; }

        [DataMember]
        [BsonElement("hated")]
        public int Hated { get; set; }
    }
}
