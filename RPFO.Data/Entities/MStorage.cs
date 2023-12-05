using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MStorage
    {
        public string SlocId { get; set; }
        public string CompanyCode { get; set; }
        public string SlocName { get; set; }
        public string WhsCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool? IsNegative { get; set; }
        public string Status { get; set; }
    }
}
