using Newtonsoft.Json;
using PopcornExport.Models.Torrent.Show;
using PopcornExport.Models.Watched;

namespace PopcornExport.Models.Episode
{
    public class EpisodeShowJson
    {
        [JsonProperty("torrents")]
        public TorrentShowNodeJson Torrents { get; set; }

        [JsonProperty("watched")]
        public WatchedJson Watched { get; set; }

        [JsonProperty("first_aired")]
        public long FirstAired { get; set; }

        [JsonProperty("date_based")]
        public bool DateBased { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("episode")]
        public int EpisodeNumber { get; set; }

        [JsonProperty("season")]
        public int Season { get; set; }

        [JsonProperty("tvdb_id")]
        public int? TvdbId { get; set; }
    }
}
