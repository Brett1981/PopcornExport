using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using PopcornExport.Models.Torrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PopcornExport
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class TorrentWrapper
    {
        [DataMember()]
        [BsonElement("0")]
        public TorrentModel Torrent_0 { get; set; }

        [DataMember()]
        [BsonElement("480p")]
        public TorrentModel Torrent_480p { get; set; }

        [DataMember()]
        [BsonElement("720p")]
        public TorrentModel Torrent_720p { get; set; }

        [DataMember()]
        [BsonElement("1080p")]
        public TorrentModel Torrent_1080p { get; set; }
    }
}