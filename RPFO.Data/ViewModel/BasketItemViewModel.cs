 
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public class BasketItemViewModel  
    {
        public string Id { get; set; } 
        public string ProductName { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public string PictureUrl { get; set; }
        public string PromotionType { get; set; }
        public string DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public string Uom { get; set; }
        public string Barcode { get; set; }
        public string Brand { get; set; }

        public string Type { get; set; }
        public decimal? LineTotal { get; set; }
        public string Note { get; set; }
        public string SlocId { get; set; }
        public bool? isSerial { get; set; }
        public bool? isBOM { get; set; }
        public bool? isVoucher { get; set; }
        public bool? isBOMLine { get; set; }
        public bool? isCapacity { get; set; }
        public decimal? CapacityValue { get; set; }
        public string SerialNum { get; set; }
        public string StoreAreaId { get; set; }
        public string TimeFrameId { get; set; }
        public string AppointmentDate { get; set; } 
        public decimal? PromotionDiscountPercent { get; set; }
        public decimal? PromotionLineTotal { get; set; }
        public string PromotionItemGroup { get; set; }
        public decimal? PromotionPriceAfDisAndVat { get; set; }
        public decimal? PromotionRate{ get; set; }
        public string PromotionCollection { get; set; }
        public decimal? PromotionDisAmt { get; set; }
        public decimal? PromotionDisPrcnt { get; set; }
        public string PromotionIsPromo { get; set; }
        public decimal? PromotionPriceAfDis { get; set; }
        public string PromotionPromoCode { get; set; }
        public string PromotionPromoName { get; set; }
        public string PromotionSchemaCode { get; set; }
        public string PromotionSchemaName { get; set; }
        public decimal? PromotionTotalAfDis { get; set; }
        public decimal? PromotionUnitPrice { get; set; }
        public string PromotionUoMCode { get; set; }
        public decimal? PromotionUoMEntry { get; set; }
        public string PromotionVatGroup { get; set; }
        public decimal? PromotionVatPerPriceAfDis { get; set; }

        public string PrepaidCardNo { get; set; }
        public decimal? MemberValue { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public string CustomField6 { get; set; }
        public string CustomField7 { get; set; }
        public string CustomField8 { get; set; }
        public string CustomField9 { get; set; }
        public string CustomField10 { get; set; }
        public DateTime? MemberDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string RejectPayType { get; set; }

        public Decimal? OriQty { get; set; }
        public Decimal? OpenQty { get; set; }

        public bool? CanRemove { get; set; }
        public string SalesTaxCode { get; set; }
        public decimal? SalesTaxRate { get; set; }
     
        public string PurchaseTaxCode { get; set; }
        public decimal? PurchaseTaxRate { get; set; }
        public string BaseTransId { get; set; }
        public string BaseLine { get; set; }
        public List<BasketItemViewModel> LineItems { get; set; } = new List<BasketItemViewModel>();
        public List<BasketItemViewModel> DiscountHistory { get; set; } = new List<BasketItemViewModel>();

    }
}
