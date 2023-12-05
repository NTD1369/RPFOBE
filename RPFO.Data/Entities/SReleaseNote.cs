using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SReleaseNote
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string Version { get; set; }
        public DateTime? VersionReleaseTime { get; set; }
        public string VersionBy { get; set; }
        public string VersionDescription { get; set; }
        public string Description { get; set; }
        public DateTime? ReleaseTime { get; set; }
        public string ReleaseContent { get; set; }
        public string ReleaseContentForeign { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; } 
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
    }
}
