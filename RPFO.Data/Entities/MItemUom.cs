using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MItemUom
    {
        public string CompanyCode { get; set; }
      
        public string ItemCode { get; set; }
        public string UomCode { get; set; }
        public decimal? Factor { get; set; }
        public decimal? DefaultFixedQty { get; set; }
        public string BarCode { get; set; }
        public string QrCode { get; set; }
        public string PLU_Flag   { get; set; }
        public string PLU { get; set; }
        public decimal? WeightValue { get; set; }
        public int WeightCount { get; set; }
        public string WeightUnit { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
      
        public string Status { get; set; }
    }
}
