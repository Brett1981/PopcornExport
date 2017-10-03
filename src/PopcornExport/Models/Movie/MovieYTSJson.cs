using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PopcornExport.Models.Torrent.Movie;

namespace PopcornExport.Models.Movie
{
    public class MovieYTSJson
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "imdb_code")]
        public string ImdbCode { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "title_long")]
        public string TitleLong { get; set; }

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

        [DataMember(Name = "small_cover_image")]
        public string SmallCoverImage { get; set; }

        [DataMember(Name = "medium_cover_image")]
        public string MediumCoverImage { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "torrents")]
        public List<TorrentMovieJson> Torrents { get; set; }

        [DataMember(Name = "date_uploaded")]
        public string DateUploaded { get; set; }

        [DataMember(Name = "date_uploaded_unix")]
        public int DateUploadedUnix { get; set; }

        [DataMember(Name = "server_time")]
        public int ServerTime { get; set; }

        [DataMember(Name = "server_timezone")]
        public string ServerTimezone { get; set; }

        [DataMember(Name = "api_version")]
        public int ApiVersion { get; set; }

        [DataMember(Name = "execution_time")]
        public string ExecutionTime { get; set; }

    }
}