using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PopcornExport.Models.Movie.v2
{
    public class MovieV2
    {
        [DataMember(Name = "imdb_id")]
        public string ImdbId { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "year")]
        public string Year { get; set; }

        [DataMember(Name = "synopsis")]
        public string Synopsis { get; set; }

        [DataMember(Name = "runtime")]
        public string Runtime { get; set; }

        [DataMember(Name = "released")]
        public long Released { get; set; }

        [DataMember(Name = "trailer")]
        public string Trailer { get; set; }

        [DataMember(Name = "certification")]
        public string Certification { get; set; }

        [DataMember(Name = "torrents")]
        public TorrentV2 Torrents { get; set; }

        [DataMember(Name = "genres")]
        public List<string> Genres { get; set; }

        [DataMember(Name = "images")]
        public ImagesV2 Images { get; set; }

        [DataMember(Name = "rating")]
        public RatingV2 Rating { get; set; }
    }
}
