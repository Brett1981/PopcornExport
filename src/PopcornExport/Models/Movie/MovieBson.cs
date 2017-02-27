using System.Collections.Generic;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using PopcornExport.Models.Cast;
using PopcornExport.Models.Torrent.Movie;

namespace PopcornExport.Models.Movie
{
    [BsonIgnoreExtraElements]
    [DataContract]
    public class MovieBson
    {
        [DataMember]
        [BsonElement("url")]
        public string Url { get; set; }

        [DataMember]
        [BsonElement("imdb_code")]
        public string ImdbCode { get; set; }

        [DataMember]
        [BsonElement("title")]
        public string Title { get; set; }

        [DataMember]
        [BsonElement("title_long")]
        public string TitleLong { get; set; }

        [DataMember]
        [BsonElement("slug")]
        public string Slug { get; set; }

        [DataMember]
        [BsonElement("year")]
        public int Year { get; set; }

        [DataMember]
        [BsonElement("rating")]
        public double Rating { get; set; }

        [DataMember]
        [BsonElement("runtime")]
        public int Runtime { get; set; }

        [DataMember]
        [BsonElement("genres")]
        public List<string> Genres { get; set; }

        [DataMember]
        [BsonElement("language")]
        public string Language { get; set; }

        [DataMember]
        [BsonElement("mpa_rating")]
        public string MpaRating { get; set; }

        [DataMember]
        [BsonElement("download_count")]
        public string DownloadCount { get; set; }

        [DataMember]
        [BsonElement("like_count")]
        public string LikeCount { get; set; }

        [DataMember]
        [BsonElement("rt_critics_score")]
        public string RtCrtiticsScore { get; set; }

        [DataMember]
        [BsonElement("rt_critics_rating")]
        public string RtCriticsRating { get; set; }

        [DataMember]
        [BsonElement("rt_audience_score")]
        public string RtAudienceScore { get; set; }

        [DataMember]
        [BsonElement("rt_audience_rating")]
        public string RtAudienceRating { get; set; }

        [DataMember]
        [BsonElement("description_intro")]
        public string DescriptionIntro { get; set; }

        [DataMember]
        [BsonElement("description_full")]
        public string DescriptionFull { get; set; }

        [DataMember]
        [BsonElement("yt_trailer_code")]
        public string YtTrailerCode { get; set; }

        [DataMember]
        [BsonElement("cast")]
        public List<CastBson> Cast { get; set; }

        [DataMember]
        [BsonElement("torrents")]
        public List<TorrentMovieBson> Torrents { get; set; }

        [DataMember]
        [BsonElement("date_uploaded")]
        public string DateUploaded { get; set; }

        [DataMember]
        [BsonElement("date_uploaded_unix")]
        public int DateUploadedUnix { get; set; }
        
        [DataMember]
        [BsonElement("poster_image")]
        public string PosterImage { get; set; }

        [DataMember]
        [BsonElement("backdrop_image")]
        public string BackdropImage { get; set; }

        [DataMember]
        [BsonElement("background_image")]
        public string BackgroundImage { get; set; }

        [DataMember]
        [BsonElement("small_cover_image")]
        public string SmallCoverImage { get; set; }

        [DataMember]
        [BsonElement("medium_cover_image")]
        public string MediumCoverImage { get; set; }

        [DataMember]
        [BsonElement("large_cover_image")]
        public string LargeCoverImage { get; set; }

        [DataMember]
        [BsonElement("medium_screenshot_image1")]
        public string MediumScreenshotImage1 { get; set; }

        [DataMember]
        [BsonElement("medium_screenshot_image2")]
        public string MediumScreenshotImage2 { get; set; }

        [DataMember]
        [BsonElement("medium_screenshot_image3")]
        public string MediumScreenshotImage3 { get; set; }

        [DataMember]
        [BsonElement("large_screenshot_image1")]
        public string LargeScreenshotImage1 { get; set; }

        [DataMember]
        [BsonElement("large_screenshot_image2")]
        public string LargeScreenshotImage2 { get; set; }

        [DataMember]
        [BsonElement("large_screenshot_image3")]
        public string LargeScreenshotImage3 { get; set; }
    }
}