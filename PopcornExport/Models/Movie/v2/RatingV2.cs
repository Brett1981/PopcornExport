using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PopcornExport.Models.Movie.v2
{
    public class RatingV2
    {
        [DataMember(Name = "percentage")]
        public int Percentage { get; set; }

        [DataMember(Name = "watching")]
        public int Watching { get; set; }

        [DataMember(Name = "votes")]
        public int Votes { get; set; }

        [DataMember(Name = "loved")]
        public int Loved { get; set; }

        [DataMember(Name = "hated")]
        public int Hated { get; set; }
    }
}
