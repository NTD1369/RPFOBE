using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RPFO.Data.ViewModel
{
    public class AddOrUpdateViewModel
    {
        public AddOrUpdateViewModel()
        {
            Items = new List<BasketAddItemViewModel>();
            addItemOrder = new BasketAddItemViewModel();
        }
        public List<BasketAddItemViewModel> Items { get; set; }
        public BasketAddItemViewModel addItemOrder { get; set; }
        public decimal? Quantity { get; set; }
        public string PriceWScaleWithCfg { get; set; }
    }
}

public class BasketAddItemViewModel
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
    public decimal? PromotionRate { get; set; }
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
    public List<BasketAddItemViewModel> LineItems { get; set; } = new List<BasketAddItemViewModel>();
    public List<BasketAddItemViewModel> DiscountHistory { get; set; } = new List<BasketAddItemViewModel>();

    //add coloumn 
    public string BookletNo { get; set; }
    public bool? IsNegative { get; set; }
    public string Custom1 { get; set; }
    public bool? IsWeightScaleItem { get; set; }
    public bool? IsFixedQty { get; set; }
    public decimal? DefaultFixedQty { get; set; }
    public bool? AllowSalesNegative { get; set; }
    public string Phone { get; set; }
    public string Name { get; set; }
    public string PriceWScaleWithCfg { get; set; }
    public string WeightScaleBarcode { get; set; }
    public int? LineNum { get; set; }
}

public enum SalesType
{
    //It using field customField1 for emun.
    [Description("Exchange")]
    Exchange = 0,
    [Description("ex")]
    Return = 1,
    [Description("return")]
    Ex = 3,
    [Description("Retail")]
    Retail = 4,
    [Description("member")]
    Member = 5,
    [Description("class")]
    ClassCss = 6,
    [Description("card")]
    Card = 7,
    [Description("Fixed Quantity")]
    FixedQuantity = 8,
    [Description("tp")]
    TP = 9,
    [Description("bp")]
    BP = 10,
    [Description("pn")]
    PN = 11,
    [Description("Discount Amount")]
    DiscountAmount = 12
}