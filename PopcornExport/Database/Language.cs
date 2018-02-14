using System;
using System.Collections.Generic;
using System.Text;

namespace PopcornExport.Database
{
    public partial class Language
    {
        public int Id { get; set; }
        public string SubLanguageId { get; set; }
        public string LanguageName { get; set; }
        public string Iso639 { get; set; }
    }
}
