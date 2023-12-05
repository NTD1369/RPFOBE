using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MReason
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string Value { get; set; }
        public string Language { get; set; }
        public string Remark { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
      
    }
}
