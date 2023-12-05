using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TDeliveryLine
    {
        public string TransId { get; set; }
        public string LineId { get; set; }
        public string StoreId { get; set; }
        public string CompanyCode { get; set; }
        public string ItemCode { get; set; }
        public string SlocId { get; set; }
        public string BarCode { get; set; }
        public string UomCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? LineTotal { get; set; }
        public decimal? LineTotalBefDis { get; set; }
        public decimal? LineTotalDisIncludeHeader { get; set; }
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
        public string StoreAreaId { get; set; }
        public string TimeFrameId { get; set; }
        public int? Duration { get; set; }
        public string BomId { get; set; }
        public DateTime? AppointmentDate { get; set; } 
        public decimal? PromoPrice { get; set; }
        public decimal? PromoLineTotal { get; set; }
        public decimal? OpenQty { get; set; }
        public int? BaseLine { get; set; }
        public string BaseTransId { get; set; }
        public decimal? PromoDisAmt { get; set; }
        public string IsPromo { get; set; }
        public bool? IsSerial { get; set; }
        public bool? IsVoucher { get; set; }
        public bool? AllowSalesNegative { get; set; }
       
        public string Description { get; set; }
        public string PrepaidCardNo { get; set; }
        public DateTime? MemberDate { get; set; }
        public int? MemberValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ItemType { get; set; }
        public string SerialNum { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string ItemTypeS4 { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
        public string ProductId { get; set; }
        public string PriceListId { get; set; }
        public string WeightScaleBarcode { get; set; }
        public string BookletNo { get; set; }
        public decimal? OrgQty { get; set; }
        public decimal? DeliveryQty { get; set; }
        public decimal? ReceiptQty { get; set; }
        public decimal? TotalOrder { get; set; }
        public decimal? DivisionQty { get; set; }
        public bool? Result { get; set; }
        public List<TDeliveryLineSerial> SerialLines { get; set; } = new List<TDeliveryLineSerial>();
    }
}
