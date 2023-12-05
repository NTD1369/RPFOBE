using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TInvoiceHeader
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
        public string IsCanceled { get; set; }
        public string Remarks { get; set; }
        public string SalesPerson { get; set; }

        public string SalesPersonName { get; set; }
        public string SalesMode { get; set; }
        public string RefTransId { get; set; }
        public string ManualDiscount { get; set; }
      
        public string SalesType { get; set; }
        public string DataSource { get; set; }

        public string POSType { get; set; }
        public string InvoiceType { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
        public string CusName { get; set; }
        public string CusAddress { get; set; }
        public string Reason { get; set; }
        public string Chanel { get; set; }
        public string TerminalId { get; set; }
        public string ApprovalId { get; set; } 
        public string CustomF1 { get; set; } 
        public string CustomF2 { get; set; } 
        public string CustomF3 { get; set; } 
        public string CustomF4 { get; set; } 
        public string CustomF5 { get; set; } 
         
    }
}
