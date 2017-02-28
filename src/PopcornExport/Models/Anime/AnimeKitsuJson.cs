using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PopcornExport.Models.Anime
{
    public class AnimeKitsuWrapperJson
    {
        [JsonProperty("data")]
        public AnimeKitsuJson Anime { get; set; }
    }

    public class AnimeKitsuJson
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("attributes")]
        public AttributesJson Attributes { get; set; }
    }

    public class AttributesJson
    {
        [JsonProperty("posterImage")]
        public AnimeKitsuImageJson PosterImage { get; set; }

        [JsonProperty("coverImage")]
        public AnimeKitsuImageJson CoverImage { get; set; }
    }

    public class AnimeKitsuImageJson
    {
        [JsonProperty("tiny")]
        public string Tiny { get; set; }

        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("medium")]
        public string Medium { get; set; }

        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("original")]
        public string Original { get; set; }
    }
}
