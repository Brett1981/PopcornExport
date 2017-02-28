using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PopcornExport.Models.Episode;
using PopcornExport.Models.Image;
using PopcornExport.Models.Rating;

namespace PopcornExport.Models.Anime
{
    public class AnimeJson
    {
        [JsonProperty("mal_id")]
        public string MalId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("synopsis")]
        public string Synopsis { get; set; }

        [JsonProperty("runtime")]
        public string Runtime { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("__v")]
        public int V { get; set; }

        [JsonProperty("last_updated")]
        public long LastUpdated { get; set; }

        [JsonProperty("num_seasons")]
        public int NumSeasons { get; set; }

        [JsonProperty("episodes")]
        public List<EpisodeAnimeJson> Episodes { get; set; }

        [JsonProperty("genres")]
        public IEnumerable<string> Genres { get; set; }

        [JsonProperty("images")]
        public ImageShowJson Images { get; set; }

        [JsonProperty("rating")]
        public RatingJson Rating { get; set; }
    }
}
