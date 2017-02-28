using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;
using MongoDB.Bson;
using PopcornExport.Models.Torrent.Show;

namespace PopcornExport.Models.Episode
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class EpisodeShowBson
    {
        [DataMember]
        [BsonElement("torrents")]
        public TorrentShowNodeBson Torrents { get; set; }

        [DataMember]
        [BsonRepresentation(BsonType.Double)]
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
