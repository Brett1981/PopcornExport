using MongoDB.Bson.Serialization.Attributes;
using PopcornExport.Models.Watched;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using PopcornExport.Models.Torrent.Show;

namespace PopcornExport.Models.Episode
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class EpisodeAnimeBson
    {
        [DataMember]
        [BsonElement("torrents")]
        public TorrentShowNodeBson Torrents { get; set; }

        [DataMember]
        [BsonElement("watched")]
        public WatchedBson watched { get; set; }

        [DataMember]
        [BsonElement("overview")]
        public string Overview { get; set; }

        [DataMember]
        [BsonElement("title")]
        public string Title { get; set; }

        [DataMember]
        [BsonElement("episode")]
        public int EpisodeNumber { get; set; }

        [DataMember]
        [BsonElement("season")]
        public int Season { get; set; }

        [DataMember]
        [BsonElement("tvdb_id")]
        public string TvdbId { get; set; }
    }
}
