using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PopcornExport.Models.Cast;
using PopcornExport.Models.Torrent.Movie;

namespace PopcornExport.Models.Movie
{
    public class MovieJson
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "imdb_code")]
        public string ImdbCode { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "title_long")]
        public string TitleLong { get; set; }

        [DataMember(Name = "slug")]
        public string Slug { get; set; }

        [DataMember(Name = "year")]
        public int Year { get; set; }

        [DataMember(Name = "rating")]
        public double Rating { get; set; }

        [DataMember(Name = "runtime")]
        public int Runtime { get; set; }

        [DataMember(Name = "genres")]
        public List<string> Genres { get; set; }

        [DataMember(Name = "language")]
        public string Language { get; set; }

        [DataMember(Name = "mpa_rating")]
        public string MpaRating { get; set; }

        [DataMember(Name = "download_count")]
        public int DownloadCount { get; set; }

        [DataMember(Name = "like_count")]
        public int LikeCount { get; set; }

        [DataMember(Name = "description_intro")]
        public string DescriptionIntro { get; set; }

        [DataMember(Name = "description_full")]
        public string DescriptionFull { get; set; }

        [DataMember(Name = "yt_trailer_code")]
        public string YtTrailerCode { get; set; }

        [DataMember(Name = "cast")]
        public List<CastJson> Cast { get; set; }

        [DataMember(Name = "torrents")]
        public List<TorrentMovieJson> Torrents { get; set; }

        [DataMember(Name = "date_uploaded")]
        public string DateUploaded { get; set; }

        [DataMember(Name = "date_uploaded_unix")]
        public int DateUploadedUnix { get; set; }

        [DataMember(Name = "poster_image")]
        public string PosterImage { get; set; }

        [DataMember(Name = "background_image")]
        public string BackgroundImage { get; set; }

        [DataMember(Name = "similar")]
        public List<string> Similar { get; set; }
    }
}