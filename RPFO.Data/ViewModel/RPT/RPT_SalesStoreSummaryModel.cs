using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel.RPT
{
    public class RPT_SalesStoreSummaryModel
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string CounterId { get; set; }
        public string StoreName { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalPayable { get; set; }
        public decimal? TotalDiscountAmt { get; set; }
        public decimal? TotalReceipt { get; set; }
        public decimal? AmountChange { get; set; }
        public decimal? PaymentDiscount { get; set; }
        public decimal? TotalTax { get; set; }
        public string SalesMode { get; set; }
        public long? CountNo { get; set; }
        public decimal? RoundingOff { get; set; }
        public decimal? AVGTotal { get; set; }
        public string CustomField7 { get; set; }
        public string CustomF1 { get; set; } 
        public string CustomF2 { get; set; } 
        public string CustomF3 { get; set; } 
        public string CustomF4 { get; set; } 
        public string CustomF5 { get; set; } 

    }
}
