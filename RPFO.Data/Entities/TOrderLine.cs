using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TOrderLine
    {
        public string TransId { get; set; }
        public string LineId { get; set; }
        public string CompanyCode { get; set; }
        public string ItemCode { get; set; }
        public string SlocId { get; set; }
        public string BarCode { get; set; }
        public string Uomcode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? LineTotal { get; set; }
        public string DiscountType { get; set; }
        public decimal? DiscountAmt { get; set; }
        public decimal? DiscountRate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string PromoId { get; set; }
        public string PromoType { get; set; }
        public decimal? PromoPercent { get; set; }
        public string PromoBaseItem { get; set; }
        public string SalesMode { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmt { get; set; }
        public string TaxCode { get; set; }
        public decimal? MinDepositAmt { get; set; }
        public decimal? MinDepositPercent { get; set; }
        public string DeliveryType { get; set; }
        public string Posservice { get; set; }
        public string Description { get; set; }
    }
}
