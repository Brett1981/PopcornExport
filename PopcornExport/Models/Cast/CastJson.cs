using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PopcornExport.Models.Cast
{
    [DataContract]
    public class CastJson
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "character_name")]
        public string CharacterName { get; set; }

        [DataMember(Name = "url_small_image")]
        public string SmallImage { get; set; }

        [DataMember(Name = "imdb_code")]
        public string ImdbCode { get; set; }
    }
}
