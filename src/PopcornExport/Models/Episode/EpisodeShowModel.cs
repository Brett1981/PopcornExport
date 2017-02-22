using MongoDB.Bson.Serialization.Attributes;
using PopcornExport.Models.Watched;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PopcornExport.Models.Episode
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class EpisodeShowModel
    {
        [DataMember]
        [BsonElement("torrents")]
        public TorrentWrapper Torrents { get; set; }

        [DataMember]
        [BsonElement("watched")]
        public WatchedModel watched { get; set; }

        [DataMember]
        [BsonElement("first_aired")]
        public long FirstAired { get; set; }

        [DataMember]
        [BsonElement("date_based")]
        public bool DateBased { get; set; }

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
        public int? TvdbId { get; set; }
    }
}
