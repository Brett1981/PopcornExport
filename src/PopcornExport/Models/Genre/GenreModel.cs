using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PopcornExport.Models.Genre
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class GenreModel
    {
        [DataMember]
        [BsonElement()]
        public BsonArray Name { get; set; }
    }
}
