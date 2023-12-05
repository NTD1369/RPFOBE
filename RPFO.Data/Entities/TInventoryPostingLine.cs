using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TInventoryPostingLine
    {
        public string KeyId { get{ return ItemCode + UomCode + BarCode; } }
        public string Ipid { get; set; }
        public string CompanyCode { get; set; }
        public string LineId { get; set; }
        public string ItemCode { get; set; }
        public string SlocId { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }
        public string UomCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string BaseRef { get; set; }
        public string BaseType { get; set; }
        public string BaseEntry { get; set; }
        public string LineStatus { get; set; }
        public decimal? LineTotal { get; set; }
        public string Comment { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; } 
    }
}
