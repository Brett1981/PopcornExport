using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace PopcornExport.Models.Cast
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class CastBson
    {
        [DataMember]
        [BsonElement("name")]
        public string Name { get; set; }

        [DataMember]
        [BsonElement("character_name")]
        public string CharacterName { get; set; }

        [DataMember]
        [BsonElement("url_small_image")]
        public string SmallImage { get; set; }

        [DataMember]
        [BsonElement("imdb_code")]
        public string ImdbCode { get; set; }
    }
}
