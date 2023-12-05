
using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public class EndShiftPrintViewModel : TShiftHeader
    {  
        public decimal? TransTotalQty { get; set; }
        public decimal? TransTotalAmt { get; set; }
        public decimal? CompletedTotalQty { get; set; }
        public decimal? CompletedTotalAmt { get; set; }
        public decimal? CanceledTotalQty { get; set; }
        public decimal? CanceledTotalAmt { get; set; }
        public string CounterId { get; set; }
        public EndShiftPrintViewModel()
        {
            Payments = new List<EndShiftPayment>();
            ItemSumary = new List<EndShiftItemSumary>();
            ItemInventorySumary = new List<EndShiftItemSumary>();
        }
        public List<EndShiftPayment> Payments { get; set; }
        public List<EndShiftPayment> CashierPayments { get; set; }
        public List<EndShiftItemSumary> ItemSumary { get; set; }
        public List<EndShiftItemSumary> ItemInventorySumary { get; set; }
    }
    public class EndShiftItemSumary
    {
        public string Type { get; set; }
        public string CreatedBy { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemGroupName { get; set; }
        public string TransId { get; set; }
        public string ShiftId { get; set; }
        public string Remarks { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string UOMCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? LineTotal { get; set; }
        
    }
    public class EndShiftPayment
    {
        public string PaymentCode { get; set; }
        public string CreatedBy { get; set; }
        public string ShortName { get; set; }
        public string CounterId { get; set; }  
        public string Currency { get; set; }  
        public bool? EODApply { get; set; }  
        public decimal? FCAmount { get; set; }
        public decimal? Rate { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? CollectedAmount { get; set; }
        public decimal? ChargableAmount { get; set; }
        public decimal? ChangeAmt { get; set; }
        public decimal? CountedBalance { get; set; }
        public decimal? BankInAmt { get; set; }
        public decimal? BankInBalance { get; set; }
        public string Cashier { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
    }
}
