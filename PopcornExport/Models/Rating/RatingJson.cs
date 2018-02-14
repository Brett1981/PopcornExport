using System.Runtime.Serialization;

namespace PopcornExport.Models.Rating
{
    [DataContract]
    public class RatingJson
    {
        [DataMember(Name = "percentage")]
        public double Percentage { get; set; }

        [DataMember(Name = "votes")]
        public int Votes { get; set; }

        [DataMember(Name = "loved")]
        public int Loved { get; set; }

        [DataMember(Name = "hated")]
        public int Hated { get; set; }

        [DataMember(Name = "watching")]
        public int Watching { get; set; }
    }
}
