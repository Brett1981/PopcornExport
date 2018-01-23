using System.Runtime.Serialization;

namespace PopcornExport.Models.Image
{
    [DataContract]
    public class ImageShowJson
    {
        [DataMember(Name = "poster")]
        public string Poster { get; set; }

        [DataMember(Name = "banner")]
        public string Banner { get; set; }

        [DataMember(Name = "fanart")]
        public string Fanart { get; set; }
    }
}
