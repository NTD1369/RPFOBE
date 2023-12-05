using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TEndDate
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? CreateOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }
        public string TerminalId { get; set; }
        public decimal? TotalSales { get; set; }
        public decimal? TotalCount { get; set; }
        public decimal? TotalCollected { get; set; }
        public decimal? TotalBalance { get; set; }


        public List<TEndDateDetail> Lines { get; set; } = new List<TEndDateDetail>();
        public List<TEndDatePayment> Payments { get; set; } = new List<TEndDatePayment>();

        public decimal? TransTotalQty { get; set; }
        public decimal? TransTotalAmt { get; set; }
        public decimal? CompletedTotalQty { get; set; }
        public decimal? CompletedTotalAmt { get; set; }
        public decimal? CanceledTotalQty { get; set; }
        public decimal? CanceledTotalAmt { get; set; }
        public decimal? TaxTotal { get; set; }
        public decimal? DiscountTotal { get; set; }
        public decimal? PaymentTotal { get; set; }
        public decimal? LineItemCount { get; set; }
        public decimal? TaxCount { get; set; }
        public decimal? DiscountCount { get; set; }
        public decimal? PaymentCount { get; set; } 
        public decimal? AmtNotInBank { get; set; } 
       
        public List<EndDateItemSumary> ItemSumary { get; set; } = new List<EndDateItemSumary>();
        public List<EndDateItemSumary> ItemInventorySumary { get; set; } = new List<EndDateItemSumary>();


    }
    public class EndDateItemSumary
    {
        public string Type { get; set; }
        public string CreatedBy { get; set; }
        public string ShiftId { get; set; }
        public string Remarks { get; set; }
        public string ItemCode { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemGroupName { get; set; } 
        public string EOD_Code { get; set; }
        public string Description { get; set; }
        public string UOMCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? LineTotal { get; set; }

    }
    public partial class TEndDateDetail
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public Guid Id { get; set; }
        public string EndDateId { get; set; }
        public int LineId { get; set; }
        public string ItemCode { get; set; }
        public string UoMCode { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? LineTotal { get; set; }
        //public DateTime? CreateOn { get; set; }
        //public DateTime? ModifiedOn { get; set; }
        //public string CreateBy { get; set; }
        //public string ModifiedBy { get; set; }
    }
    public partial class TEndDatePayment
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string CreatedBy { get; set; }
        public string FullName { get; set; }
        public string EOD_Code { get; set; }
        public string EndDateId { get; set; }
        public Guid Id { get; set; }
        public int LineId { get; set; }
        public string CounterId { get; set; }
        public string ShiftId { get; set; }
        public string Status { get; set; }
        //public DateTime? CreateOn { get; set; }
        //public DateTime? ModifiedOn { get; set; }
        public string PaymentCode { get; set; }
        public string ShortName { get; set; }
        public string Currency { get; set; }
        public decimal? Amount { get; set; }
        public decimal? FCAmount { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? ChargableAmount { get; set; }
        public decimal? PaymentDiscount { get; set; }
        public decimal? CollectedAmount { get; set; }
        public decimal? FCCollectedAmount { get; set; }
        public decimal? Balance { get; set; }
        public decimal? OpenAmt { get; set; }
        public decimal? ChangeAmt { get; set; }
        public decimal? BankInAmt { get; set; } 
        public decimal? BankInBalance { get; set; }
    }


    public partial class EODSummaryModel
    {
        public string EODType { get; set; }
        public string Currency { get; set; }
        public string PaymentCode { get; set; }
        public string PaymentDesc { get; set; }
        public decimal? Amount { get; set; }
        public string IsTitle { get; set; }
        public string Ref { get; set; }
        public string CreatedBy { get; set; }
        public string Date { get; set; }
        public string FCAmount { get; set; }
        public string LCAmount { get; set; }
        public string Rate { get; set; }
        public string IsBankIn { get; set; }
      
    }
}


 