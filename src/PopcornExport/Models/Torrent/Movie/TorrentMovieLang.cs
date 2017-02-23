using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace PopcornExport.Models.Torrent.Movie
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class TorrentMovieLang
    {
        [DataMember]
        [BsonElement("en")]
        public TorrentMovieNode Torrent { get; set; }
    }
}
