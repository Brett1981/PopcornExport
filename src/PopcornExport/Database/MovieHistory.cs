namespace PopcornExport.Database
{
    using System;
    using System.Collections.Generic;

    public partial class MovieHistory
    {
        public int Id { get; set; }

        public string ImdbId { get; set; }
        public bool Seen { get; set; }
        public bool Favorite { get; set; }
    }
}
