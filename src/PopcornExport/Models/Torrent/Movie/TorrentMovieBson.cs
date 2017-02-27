using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PopcornExport.Models.Torrent.Movie
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class TorrentMovieBson
    {
        [DataMember]
        [BsonElement("url")]
        public string Url { get; set; }

        [DataMember]
        [BsonElement("hash")]
        public string Hash { get; set; }

        [DataMember]
        [BsonElement("quality")]
        public string Quality { get; set; }

        [DataMember]
        [BsonElement("seeds")]
        public int Seeds { get; set; }

        [DataMember]
        [BsonElement("peers")]
        public int Peers { get; set; }

        [DataMember]
        [BsonElement("size")]
        public string Size { get; set; }

        [DataMember]
        [BsonRepresentation(BsonType.Double)]
        [BsonElement("size_bytes")]
        public long SizeBytes { get; set; }

        [DataMember]
        [BsonElement("date_uploaded")]
        public string DateUploaded { get; set; }

        [DataMember]
        [BsonElement("date_uploaded_unix")]
        public int DateUploadedUnix { get; set; }
    }
}
