using System;
using System.Collections.Generic;
using System.Text;
using PopcornExport.Database;

namespace PopcornExport.Comparers
{
    public class EpisodeComparer : IEqualityComparer<EpisodeShow>
    {
        /// <summary>
        /// Compare two shows
        /// </summary>
        /// <param name="x">First show</param>
        /// <param name="y">Second show</param>
        /// <returns>True if both shows are the same, false otherwise</returns>
        public bool Equals(EpisodeShow x, EpisodeShow y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.TvdbId == y.TvdbId && x.Season == y.Season && x.EpisodeNumber == y.EpisodeNumber;
        }

        /// <summary>
        /// Define a unique hash code for a show
        /// </summary>
        /// <param name="show">A show</param>
        /// <returns>Unique hashcode</returns>
        public int GetHashCode(EpisodeShow show)
        {
            //Check whether the object is null
            if (ReferenceEquals(show, null)) return 0;

            //Get hash code for the Id field
            var hashId = show.TvdbId.GetHashCode();

            var hashSeason = show.Season.GetHashCode();

            var hashEpisodeNumber = show.EpisodeNumber.GetHashCode();

            return hashId ^ hashSeason ^ hashEpisodeNumber;
        }
    }
}
