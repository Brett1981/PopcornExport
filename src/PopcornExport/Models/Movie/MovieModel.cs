using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PopcornExport.Models.Image;
using PopcornExport.Models.Rating;
using PopcornExport.Models.Torrent;
using System.Runtime.Serialization;
using PopcornExport.Models.Torrent.Movie;

namespace PopcornExport.Models.Movie
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class MovieModel
    {
        [DataMember]
        [BsonElement("imdb_id")]
        public string ImdbId { get; set; }

        [DataMember]
        [BsonElement("title")]
        public string Title { get; set; }

        [DataMember]
        [BsonElement("year")]
        public string Year { get; set; }

        [DataMember]
        [BsonElement("synopsis")]
        public string Synopsis { get; set; }

        [DataMember]
        [BsonElement("runtime")]
        public string Runtime { get; set; }

        [DataMember]
        [BsonElement("released")]
        public double Released { get; set; }

        [DataMember]
        [BsonElement("trailer")]
        public string Trailer { get; set; }

        [DataMember]
        [BsonElement("certification")]
        public string Certification { get; set; }

        [DataMember]
        [BsonElement("torrents")]
        public TorrentMovieLang Torrents { get; set; }

        [DataMember]
        [BsonElement("country")]
        public string Country { get; set; }

        [DataMember]
        [BsonElement("last_updated")]
        public double LastUpdated { get; set; }

        [DataMember]
        [BsonElement("genres")]
        public BsonArray Genres { get; set; }

        [DataMember]
        [BsonElement("images")]
        public ImageModel Images { get; set; }

        [DataMember]
        [BsonElement("rating")]
        public RatingModel Rating { get; set; }
    }
}
