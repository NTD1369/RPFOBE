using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TSalesStaff
    { 
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string TransId { get; set; }
        public string ItemLine { get; set; }
        public int? LineId { get; set; } 
        public string Staff { get; set; }
        public string Position { get; set; }
        public string Remark { get; set; }
        public decimal? Percent { get; set; }
        public decimal? Amount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
       
    }
}
