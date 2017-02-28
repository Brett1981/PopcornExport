using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PopcornExport.Models.Image
{
    public class ImageAnimeJson
    {
        [JsonProperty("poster")]
        public string Poster { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }
    }
}
