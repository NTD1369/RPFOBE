using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MShipping
    {
        public string CompanyCode { get; set; }
        public string ShippingCode { get; set; }
        public string ShippingName { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public string Status { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; } 
        public string CustomF6 { get; set; } 
        public string CustomF7 { get; set; } 
        public string CustomF8 { get; set; } 
        public string CustomF9 { get; set; } 
        public string CustomF10 { get; set; } 
        public string LicensePlate { get; set; } 
        public string Driver { get; set; } 
        public decimal? Amount1 { get; set; } 
        public decimal? Amount2 { get; set; } 
        public decimal? Amount3 { get; set; } 
        public decimal? Amount4 { get; set; } 
        public decimal? Amount5 { get; set; } 

    }
}
