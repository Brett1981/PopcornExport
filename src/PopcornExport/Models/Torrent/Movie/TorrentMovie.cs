using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PopcornExport.Models.Torrent.Movie
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class TorrentMovie
    {
        [DataMember]
        [BsonElement("provider")]
        public string Provider { get; set; }

        [DataMember]
        [BsonElement("peer")]
        public int Peers { get; set; }

        [DataMember]
        [BsonElement("seed")]
        public int Seeds { get; set; }

        [DataMember]
        [BsonElement("url")]
        public string Url { get; set; }

        [DataMember]
        [BsonElement("filesize")]
        public string Filesize { get; set; }

        [DataMember]
        [BsonElement("size")]
        public double Size { get; set; }
    }
}
