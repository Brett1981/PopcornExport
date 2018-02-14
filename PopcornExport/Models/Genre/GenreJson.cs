using System.Runtime.Serialization;

namespace PopcornExport.Models.Genre
{
    [DataContract]
    public class GenreJson
    {
        [DataMember]
        public string Name { get; set; }
    }
}
