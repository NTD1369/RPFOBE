using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class StorePaymentViewModel
    {
        public string PaymentCode { get; set; }
        public string CompanyCode { get; set; }
        public string ForfeitCode { get; set; }
        public string PaymentDesc { get; set; }
        public string ShortName { get; set; }
        public string ApiUrl { get; set; }
        public Nullable<bool> isShow { get; set; }
        public int? OrderNum { get; set; }
        public string Status { get; set; }
        public bool? IsRequireRefnum { get; set; }
        public bool? AllowMix { get; set; }
        public bool? RejectReturn { get; set; }
        public bool? RejectExchange { get; set; }
        public string PaymentType { get; set; }
        public string AccountCode { get; set; }  
        public bool? AllowChange { get; set; } 
        public bool? RejectVoid { get; set; }
        public bool? AllowRefund { get; set; }
        public string StoreId { get; set; }
        public string Currency { get; set; }
        public string TerminalIdDefault { get; set; }
        public bool? RequireTerminal { get; set; }
        public string VoucherCategory { get; set; }
        public string FatherId { get; set; }
        public bool? isFatherShow { get; set; }
        public string BankPaymentType { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }



        public string BankCustomF1 { get; set; }
        public string BankCustomF2 { get; set; }
        public string BankCustomF3 { get; set; }
        public string BankCustomF4 { get; set; }
        public string BankCustomF5 { get; set; }

    }
}
