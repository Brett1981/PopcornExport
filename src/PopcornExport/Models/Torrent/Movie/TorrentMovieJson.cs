using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornExport.Models.Torrent.Movie
{
    public class TorrentMovieJson
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "hash")]
        public string Hash { get; set; }

        [DataMember(Name = "quality")]
        public string Quality { get; set; }

        [DataMember(Name = "seeds")]
        public int Seeds { get; set; }

        [DataMember(Name = "peers")]
        public int Peers { get; set; }

        [DataMember(Name = "size")]
        public string Size { get; set; }

        [DataMember(Name = "size_bytes")]
        public long SizeBytes { get; set; }

        [DataMember(Name = "date_uploaded")]
        public string DateUploaded { get; set; }

        [DataMember(Name = "date_uploaded_unix")]
        public int DateUploadedUnix { get; set; }
    }
}
