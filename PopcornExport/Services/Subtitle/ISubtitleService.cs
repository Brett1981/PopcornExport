﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Popcorn.OSDB;

namespace PopcornExport.Services.Subtitle
{
    public interface ISubtitleService
    {
        /// <summary>
        /// Get subtitles languages
        /// </summary>
        /// <returns>Languages</returns>
        Task<IEnumerable<Popcorn.OSDB.Language>> GetSubLanguages();

        /// <summary>
        /// Search subtitles by imdb code and languages
        /// </summary>
        /// <param name="languages">Languages</param>
        /// <param name="imdbId">Imdb code</param>
        /// <param name="season">Season number</param>
        /// <param name="episode">Episode number</param>
        /// <returns></returns>
        Task<IList<Popcorn.OSDB.Subtitle>> SearchSubtitlesFromImdb(string languages, string imdbId, int? season, int? episode);

        /// <summary>
        /// Download a subtitle to a path
        /// </summary>
        /// <param name="subtitleId">Opensubtitle Id</param>
        /// <param name="lang">Subtitle language</param>
        /// <returns>Downloaded subtitle path</returns>
        Task<string> DownloadSubtitleToPath(string subtitleId, string lang);
    }
}
