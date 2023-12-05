using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MItem
    {
        public string ItemCode { get; set; }
        public string CompanyCode { get; set; }
        public string ProductId { get; set; }
        public string VariantId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public int? CapacityValue { get; set; }
        public string ItemGroupId { get; set; }
        public string SalesTaxCode { get; set; }
        public string PurchaseTaxCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemCategory_1 { get; set; }
        public string ItemCategory_2 { get; set; }
        public string ItemCategory_3 { get; set; }
        public string ForeignName { get; set; }
        public string InventoryUom { get; set; }
        public string ImageUrl { get; set; }
        public string ImageLink { get; set; }
        public string Mcid { get; set; }
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
        public bool? IsVoucher { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string RejectPayType { get; set; }
        public bool? Returnable { get; set; }
        public string VoucherCollection { get; set; }
        public bool? IsPriceTime { get; set; }
    }


    public class StoreListingModel
    {
        public string CompanyCode { get; set; }
        public string CreatedBy { get; set; }
        public List<MStore> StoreList { get; set; } = new List<MStore>();
        public List<MItem> ItemList { get; set; } = new List<MItem>();
    }


    public class ItemFilterModel
    {
        public string CompanyCode { get; set; }
        public string ViewBy { get; set; } 
        public List<string> ItemList { get; set; } = new List<string>();
    }
}
