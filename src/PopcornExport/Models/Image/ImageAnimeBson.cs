using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace PopcornExport.Models.Image
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class ImageAnimeBson
    {
        [DataMember]
        [BsonElement("poster")]
        public string OriginalPoster { get; set; }

        [DataMember]
        [BsonElement("cover")]
        public string OriginalCover { get; set; }

        [DataMember]
        [BsonElement("poster_kitsu")]
        public AnimeKitsuImage Poster { get; set; }

        [DataMember]
        [BsonElement("cover_kitsu")]
        public AnimeKitsuImage Cover { get; set; }
    }

    [BsonIgnoreExtraElements]
    [DataContract]
    public class AnimeKitsuImage
    {
        [DataMember]
        [BsonElement("tiny")]
        public string Tiny { get; set; }

        [DataMember]
        [BsonElement("small")]
        public string Small { get; set; }

        [DataMember]
        [BsonElement("medium")]
        public string Medium { get; set; }

        [DataMember]
        [BsonElement("large")]
        public string Large { get; set; }

        [DataMember]
        [BsonElement("original")]
        public string Original { get; set; }
    }
}
