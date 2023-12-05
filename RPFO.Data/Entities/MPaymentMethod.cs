using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public  class MPaymentMethodMapping
    {
        public string CompanyCode { get; set; }
        public string PaymentCode { get; set; }
        public string FatherId { get; set; }
        //public string PaymentDesc { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public string ModifiedBy { get; set; }
        //public DateTime? ModifiedOn { get; set; } 
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public string CustomF6 { get; set; }
        public string Status { get; set; } 
    }
    public partial class MPaymentMethod
    {
        public string PaymentCode { get; set; }
        public string CompanyCode { get; set; }
        public string ForfeitCode { get; set; }
        public string PaymentDesc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string AccountCode { get; set; }
        public string PaymentType { get; set; } 
        public bool? IsRequireRefnum { get; set; }
        public bool? AllowChange { get; set; }
        public bool? RejectReturn { get; set; }
        public bool? RejectVoid { get; set; }
        public bool? RejectExchange { get; set; }
        public string StoreId { get; set; }
        public string ApiURL { get; set; }
        public string ShortName { get; set; }
        public bool? EODApply { get; set; }
        public string EODCode { get; set; }
        public string Currency { get; set; }
        public bool? AllowRefund { get; set; }
        public bool? RequireTerminal { get; set; }
        public string VoucherCategory { get; set; }
        public string FatherId { get; set; }
        public string BankPaymentType { get; set; }
        public string CustomF1 { get; set; } 
        public string CustomF2 { get; set; } 
        public string CustomF3 { get; set; } 
        public string CustomF4 { get; set; } 
        public string CustomF5 { get; set; } 
        public bool? isFatherShow { get; set; }

        public List<MPaymentMethodMapping> Mappings { get; set; } = new List<MPaymentMethodMapping>();
    }
}
