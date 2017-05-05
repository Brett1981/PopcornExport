namespace PopcornExport.Database
{
    using System;
    using System.Collections.Generic;

    public partial class User
    {
        public int Id { get; set; }
        public Guid MachineGuid { get; set; }
        public int DownloadLimit { get; set; }
        public int UploadLimit { get; set; }
        public bool DefaultHdQuality { get; set; }
        public string DefaultSubtitleLanguage { get; set; }

        public virtual Language Language { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MovieHistory> MovieHistory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShowHistory> ShowHistory { get; set; }
    }
}
