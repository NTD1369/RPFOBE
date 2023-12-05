using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TOrderHeader
    {
        public string TransId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string ContractNo { get; set; }
        public string StoreName { get; set; }
        public string ShiftId { get; set; }
        public string CusId { get; set; }
        public string CusIdentifier { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalPayable { get; set; }
        public decimal? TotalDiscountAmt { get; set; }
        public decimal? TotalReceipt { get; set; }
        public decimal? AmountChange { get; set; }
        public decimal? PaymentDiscount { get; set; }
        public decimal? TotalTax { get; set; }
        public string DiscountType { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountRate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Status { get; set; }
        public bool? IsCanceled { get; set; }
        public string Remarks { get; set; }
        public string SalesPerson { get; set; }
        public string SalesMode { get; set; }
        public string RefTransId { get; set; }
        public string ManualDiscount { get; set; }
    }
}
