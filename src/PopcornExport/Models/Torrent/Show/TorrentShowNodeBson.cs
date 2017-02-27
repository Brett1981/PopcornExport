using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PopcornExport.Models.Torrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PopcornExport.Models.Torrent.Show
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class TorrentShowNodeBson
    {
        [DataMember]
        [BsonElement("0")]
        public TorrentShowBson Torrent_0 { get; set; }

        [DataMember]
        [BsonElement("480p")]
        public TorrentShowBson Torrent_480p { get; set; }

        [DataMember]
        [BsonElement("720p")]
        public TorrentShowBson Torrent_720p { get; set; }

        [DataMember]
        [BsonElement("1080p")]
        public TorrentShowBson Torrent_1080p { get; set; }
    }

}