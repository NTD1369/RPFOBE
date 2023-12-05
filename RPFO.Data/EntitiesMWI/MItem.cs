using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.EntitiesMWI
{
       public partial class MItem
    {
        public string ItemCode { get; set; }
        //public string UomCode { get; set; }
        //public string Barcode { get; set; }
        public string ProductId { get; set; }
        public string VariantId { get; set; }
        public string ItemName { get; set; }
        public string ForeignName { get; set; }
        public string InventoryUom { get; set; }
        public int? ItemGroupId { get; set; }
        public string Status { get; set; }
        public decimal? DefaultPrice { get; set; }
        public bool IsSerial { get; set; }
        public bool IsBom { get; set; }
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
        public string SalesTaxCode { get; set; }
        public double SaleTaxPercent { get; set; }
        public int CapacityValue { get; set; }
        public string PurchaseTaxCode { get; set; }
        public string ItemCategory_1 { get; set; }
        public string ItemCategory_2 { get; set; }
        public string ItemCategory_3 { get; set; }
        public string ImageURL { get; set; }
        //public string ImageLink { get; set; }
        public string MCId { get; set; }
        public bool IsVoucher { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public MItem()
        {
            BOMLines = new List<MBomline>();
        }

        public List<MBomline> BOMLines { get; set; }
    }
}
