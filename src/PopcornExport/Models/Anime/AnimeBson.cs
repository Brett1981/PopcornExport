using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PopcornExport.Models.Episode;
using PopcornExport.Models.Image;
using PopcornExport.Models.Rating;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PopcornExport.Models.Anime
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class AnimeBson
    {
        [DataMember]
        [BsonElement("mal_id")]
        public string MalId { get; set; }

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
        [BsonElement("status")]
        public string Status { get; set; }

        [DataMember]
        [BsonElement("type")]
        public string Type { get; set; }

        [DataMember]
        [BsonElement("__v")]
        public int V { get; set; }

        [DataMember]
        [BsonElement("last_updated")]
        public long LastUpdated { get; set; }

        [DataMember]
        [BsonElement("num_seasons")]
        public int NumSeasons { get; set; }

        [DataMember]
        [BsonElement("episodes")]
        public List<EpisodeAnimeBson> Episodes { get; set; }

        [DataMember]
        [BsonElement("genres")]
        public BsonArray Genres { get; set; }

        [DataMember]
        [BsonElement("images")]
        public ImageBson Images { get; set; }

        [DataMember]
        [BsonElement("rating")]
        public RatingBson Rating { get; set; }
    }
}
