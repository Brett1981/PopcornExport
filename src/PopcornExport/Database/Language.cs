namespace PopcornExport.Database
{
    using System;
    using System.Collections.Generic;

    public partial class Language
    {
        public int Id { get; set; }

        public virtual string Culture { get; set; }
    }
}
