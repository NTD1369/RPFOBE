
using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public class ItemModel
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string FrSlocId { get; set; }
        public string ItemCode { get; set; }
        public string BarCode { get; set; }
        public string UomCode { get; set; }
        public string ItemName { get; set; }
        public decimal? DefaultPrice { get; set; }
        public double Quantity { get; set; }
        public decimal? Stock { get; set; }
        public Boolean? IsOnhand { get; set; }
        public Boolean? IsNotfound { get; set; }
        public Boolean? Iscount { get; set; }
    }

    public class ItemCheckModel
    {
        public string ItemCode { get; set; }
        public string SlocId { get; set; }
        public string UomCode { get; set; }
        public double Quantity { get; set; }
        public string Barcode { get; set; }
        public string PLU { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
         
    }
    public partial class ItemViewModel 
    {
        public ItemViewModel()
        {
            Lines = new List<MBomline>();
        }
        public string KeyId { get { return ItemCode + UomCode + BarCode; } }
        public string ItemCode { get; set; }
        public string CompanyCode { get; set; }
        public string ProductId { get; set; }
        public string VariantId { get; set; } 
        public int? CapacityValue { get; set; }
        public string ItemGroupId { get; set; }
        public string ItemGroupName { get; set; }
        public string SalesTaxCode { get; set; }
        public string SalesTaxName { get; set; }
        public decimal? SalesTaxRate { get; set; }
        public decimal? SalesTaxAmount { get; set; }
        public string PurchaseTaxCode { get; set; } 
        public string PurchaseTaxName { get; set; }
        public decimal? PurchaseTaxRate { get; set; }
        public decimal? PurchaseTaxAmount { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemCategory_1 { get; set; }
        public string ItemCategory_2 { get; set; }
        public string ItemCategory_3 { get; set; }
        public string ForeignName { get; set; }
        public string UomCode { get; set; }
        public string UomName { get; set; }
        public string ImageUrl { get; set; }
        public string ImageLink { get; set; }
        public string Mcid { get; set; }
        public string McName { get; set; }
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
        public decimal? DefaultPrice { get; set; }
        public bool? IsSerial { get; set; }
        public bool? IsBom { get; set; }
        public bool? isVoucher { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string BarCode { get; set; }
        public string QRCode { get; set; }
        public decimal? PriceBeforeTax { get; set; } 
        public decimal? PriceAfterTax { get; set; }
        public string SlocId { get; set; }
        public string PriceListId { get; set; }
        public string PriceListName { get; set; }
        public string IsHighLight { get; set; }
        public string RejectPayType { get; set; }
        public string Status { get; set; }
        public bool? Returnable { get; set; }
        public decimal? BOMQty { get; set; }
        public decimal? Quantity { get; set; }
        public bool? IsPriceTime { get; set; }
        public bool? IsWeightScaleItem { get; set; }
        public bool? AllowSalesNegative { get; set; } 
        public string VoucherCollection { get; set; }
        public string MappingCode { get; set; }
        public List<MBomline> Lines { get; set; }

        public string InventoryUomCode { get; set; }
        public string InventoryUomName { get; set; }

        public string InventoryUOM { get; set; }
        public bool? IsFixedQty { get; set; }
        public decimal? DefaultFixedQty { get; set; }
        public string AssignCommission { get; set; }
    }
}
