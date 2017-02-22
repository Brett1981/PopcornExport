using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PopcornExport.Models.Torrent
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class TorrentModel
    {
        [DataMember]
        [BsonElement("provider")]
        public string Provider { get; set; }

        [DataMember]
        [BsonElement("peers")]
        public int Peers { get; set; }

        [DataMember]
        [BsonElement("seeds")]
        public int Seeds { get; set; }

        [DataMember]
        [BsonElement("url")]
        public string Url { get; set; }

        [DataMember]
        [BsonElement("filesize")]
        public string Filesize { get; set; }
    }
}
