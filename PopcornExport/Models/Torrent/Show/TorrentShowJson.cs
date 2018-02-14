using System.Runtime.Serialization;

namespace PopcornExport.Models.Torrent.Show
{
    [DataContract]
    public class TorrentShowJson
    {
        [DataMember(Name = "peers")]
        public int Peers { get; set; }

        [DataMember(Name = "seeds")]
        public int Seeds { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
