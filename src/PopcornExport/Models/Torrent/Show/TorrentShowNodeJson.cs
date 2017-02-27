using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornExport.Models.Torrent.Show
{
    public class TorrentShowNodeJson
    {
        [JsonProperty("0")]
        public TorrentShowBson Torrent_0 { get; set; }

        [JsonProperty("480p")]
        public TorrentShowBson Torrent_480p { get; set; }

        [JsonProperty("720p")]
        public TorrentShowBson Torrent_720p { get; set; }

        [JsonProperty("1080p")]
        public TorrentShowBson Torrent_1080p { get; set; }
    }
}
