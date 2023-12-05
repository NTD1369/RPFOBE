using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MItemSerialStock
    {
        public string CompanyCode { get; set; }
        public string ItemCode { get; set; }
        public string SlocId { get; set; }
        public string SerialNum { get; set; }
        public decimal? StockQty { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
       
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; } 
        public DateTime? ExpDate { get; set; }
    }
}
