using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SFormatConfig
    {
        public int FormatId { get; set; }
        public string CompanyCode { get; set; }
        public string FormatName { get; set; }
        public string SetupType { get; set; }
        public string SetupCode { get; set; }
        public string DateFormat { get; set; }
        public string DecimalFormat { get; set; }
        public string ThousandFormat { get; set; }
        public string DecimalPlacesFormat { get; set; }
        public string QtyDecimalPlacesFormat { get; set; }
        public string PerDecimalPlacesFormat { get; set; }
        public string RateDecimalPlacesFormat { get; set; }
       
        public string Status { get; set; }
    
        public string QuantityRoundingMethod { get; set; }
        public string QuantityFormat { get; set; }
        public string AmountRoundingMethod { get; set; }
        public string AmountFormat { get; set; }
        public string PointRoundingMethod { get; set; }
        public string PointFormat { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public decimal ? InventoryPercent { get; set; }
    }
}
