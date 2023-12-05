using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SPromoHeader
    {
        public string PromoId { get; set; }
        public string CompanyCode { get; set; }
        public int? PromoType { get; set; }
        public string PromoTypeName { get; set; }
        public string PromoName { get; set; }
        public string CustomerType { get; set; }
        public DateTime? ValidDateFrom { get; set; }
        public DateTime? ValidDateTo { get; set; }
        public int? ValidTimeFrom { get; set; }
        public int? ValidTimeTo { get; set; }
        public string IsMon { get; set; }
        public string IsTue { get; set; }
        public string IsWed { get; set; }
        public string IsThu { get; set; }
        public string IsFri { get; set; }
        public string IsSat { get; set; }
        public string IsSun { get; set; }
        public string IsUsed { get; set; }
        public double? TotalBuyFrom { get; set; }
        public double? TotalBuyTo { get; set; }
        public string TotalGetType { get; set; }
        public double? TotalGetValue { get; set; }
        public double? MaxTotalGetValue { get; set; }
        public string IsCombine { get; set; }
        public Nullable<bool> IsVoucher { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string SAPPromoId { get; set; }
        public string SAPBonusBuyId { get; set; }
        public string IsApply { get; set; }
        public string MaxApplyType { get; set; }
        public double? MaxApplyValue { get; set; }
        public double? MaxQtyByReceipt { get; set; }
        public double? MaxQtyByStore { get; set; }
        public decimal? RemainQty { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
    }
}
