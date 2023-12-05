using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel.RPT
{
    public class RPT_SalesTransactionPaymentModel
    {
         
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string TransId { get; set; }
        public string StoreName { get; set; }
        public string PaymentCode { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? ChargableAmount { get; set; }
        public decimal? CollectedAmount { get; set; }
        public string Currency { get; set; }
        public decimal? FCAmount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Forfeit { get; set; }
        public string RefNumber { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; } 
        public string CustomF6 { get; set; } 
        public string CustomF7 { get; set; } 
        public string CustomF8 { get; set; } 
        public string CustomF9 { get; set; } 
        public string CustomF10 { get; set; }  
        public string ShiftId { get; set; }
        public string CounterId { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }
}
