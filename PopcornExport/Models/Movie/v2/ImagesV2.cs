using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PopcornExport.Models.Movie.v2
{
    public class ImagesV2
    {
        [DataMember(Name = "poster")]
        public string Poster { get; set; }

        [DataMember(Name = "fanart")]
        public string Fanart { get; set; }

        [DataMember(Name = "banner")]
        public string Banner { get; set; }
    }
}
