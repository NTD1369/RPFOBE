using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TSalesPayment
    {
        public string PaymentCode { get; set; }
        public string CompanyCode { get; set; }
        public string ShortName { get; set; }
        public string TransId { get; set; }
        public string LineId { get; set; }
        public string StoreId { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? ReceivedAmt { get; set; }
        public decimal? PaidAmt { get; set; }
        public decimal? ChangeAmt { get; set; }
        public string PaymentMode { get; set; }
        public string CardType { get; set; }
        public string CardHolderName { get; set; }
        public string CardNo { get; set; }
        public string VoucherBarCode { get; set; }
        public string VoucherSerial { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public decimal? ChargableAmount { get; set; }
        public decimal? PaymentDiscount { get; set; }
        public decimal? CollectedAmount { get; set; }
        public string RefNumber { get; set; }
        public string DataSource { get; set; }
        public string Currency { get; set; }
        public decimal? FCAmount { get; set; }
        public decimal? Rate { get; set; }
        public string ShiftId { get; set; }
        public DateTime? CardExpiryDate { get; set; }
        public string AdjudicationCode { get; set; }
        public string TerminalId { get; set; }
        public DateTime? AuthorizationDateTime { get; set; }
        public decimal? RoundingOff { get; set; }
        public decimal? FCRoundingOff { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public string ForfeitCode { get; set; }
        public decimal? Forfeit { get; set; }
        public string PromoId { get; set; }
        public string PaymentType { get; set; }
    }
}
