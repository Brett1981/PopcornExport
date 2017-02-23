using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace PopcornExport.Models.Torrent.Show
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class TorrentShow
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
    }
}
