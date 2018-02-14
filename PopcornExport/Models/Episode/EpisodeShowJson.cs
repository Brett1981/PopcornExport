using System.Runtime.Serialization;
using PopcornExport.Models.Torrent.Show;

namespace PopcornExport.Models.Episode
{
    [DataContract]
    public class EpisodeShowJson
    {
        [DataMember(Name = "torrents")]
        public TorrentShowNodeJson Torrents { get; set; }

        [DataMember(Name = "first_aired")]
        public long FirstAired { get; set; }

        [DataMember(Name = "date_based")]
        public bool DateBased { get; set; }

        [DataMember(Name = "overview")]
        public string Overview { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "episode")]
        public object EpisodeNumber { get; set; }

        [DataMember(Name = "season")]
        public object Season { get; set; }

        [DataMember(Name = "tvdb_id")]
        public object TvdbId { get; set; }
    }
}
