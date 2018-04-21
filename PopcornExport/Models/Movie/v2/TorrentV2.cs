using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PopcornExport.Models.Movie.v2
{
    public class TorrentV2
    {
        [DataMember(Name = "en")]
        public TorrentV2Lang En { get; set; }
    }

    public class TorrentV2Lang
    {
        [DataMember(Name = "1080p")]
        public TorrentV2Details Quality1080p { get; set; }

        [DataMember(Name = "720p")]
        public TorrentV2Details Quality720p { get; set; }
    }

    public class TorrentV2Details
    {
        [DataMember(Name = "provider")]
        public string Provider { get; set; }

        [DataMember(Name = "filesize")]
        public string Filesize { get; set; }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        [DataMember(Name = "peer")]
        public int Peer { get; set; }

        [DataMember(Name = "seed")]
        public int Seed { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
