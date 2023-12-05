using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TDivisionLine
    {
        public string  Id { get; set; }
        public string LineId { get; set; }
        public string StoreId { get; set; }
        public string CompanyCode { get; set; } 
        public string ItemCode { get; set; }
        public string SlocId { get; set; }
        public string BarCode { get; set; }
        public string UomCode { get; set; }
        public decimal? Quantity { get; set; }
        
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
       
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
       
    }
}
