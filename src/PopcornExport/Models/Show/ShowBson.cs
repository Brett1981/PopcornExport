using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PopcornExport.Models.Episode;
using PopcornExport.Models.Image;
using PopcornExport.Models.Rating;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PopcornExport.Models.Show
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class ShowBson
    {
        [DataMember]
        [BsonElement("imdb_id")]
        public string ImdbId { get; set; }

        [DataMember]
        [BsonElement("tvdb_id")]
        public string TvdbId { get; set; }

        [DataMember]
        [BsonElement("title")]
        public string Title { get; set; }

        [DataMember]
        [BsonElement("year")]
        public string Year { get; set; }

        [DataMember]
        [BsonElement("slug")]
        public string Slug { get; set; }

        [DataMember]
        [BsonElement("synopsis")]
        public string Synopsis { get; set; }

        [DataMember]
        [BsonElement("runtime")]
        public string Runtime { get; set; }

        [DataMember]
        [BsonElement("country")]
        public string Country { get; set; }

        [DataMember]
        [BsonElement("network")]
        public string Network { get; set; }

        [DataMember]
        [BsonElement("air_day")]
        public string AirDay { get; set; }

        [DataMember]
        [BsonElement("air_time")]
        public string AirTime { get; set; }

        [DataMember]
        [BsonElement("status")]
        public string Status { get; set; }

        [DataMember]
        [BsonElement("num_seasons")]
        public int NumSeasons { get; set; }

        [DataMember]
        [BsonElement("last_updated")]
        public long LastUpdated { get; set; }

        [DataMember]
        [BsonElement("__v")]
        public int V { get; set; }

        [DataMember]
        [BsonElement("episodes")]
        public List<EpisodeShowBson> Episodes { get; set; }

        [DataMember]
        [BsonElement("genres")]
        public BsonArray Genres { get; set; }

        [DataMember]
        [BsonElement("images")]
        public ImageShowBson Images { get; set; }

        [DataMember]
        [BsonElement("rating")]
        public RatingBson Rating { get; set; }
    }
}
