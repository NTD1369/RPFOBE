using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel.RPT
{
    public class RPT_InventoryAuditModel
    {
        public string CompanyCode { get; set; }
        public string ItemCode { get; set; }
        public string UOMCode { get; set; }
        public string SlocId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal? BeginQty { get; set; }
        public decimal? InQty { get; set; }
        public decimal? OutQty { get; set; }
        public decimal? EndQty { get; set; }
        public string ProductId { get; set; }
        public string VariantId { get; set; }	
	    //public string CreatedBy { get; set; }	
	    //public string CreatedOn { get; set; }	
	    //public string ModifiedBy { get; set; }	
	    //public string ModifiedOn { get; set; }
        public string Status { get; set; }
        public string CapacityValue { get; set; }
        public string ItemGroupId { get; set; }

        public string SalesTaxCode { get; set; }
        public string PurchaseTaxCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemCategory_1 { get; set; }
        public string ItemCategory_2 { get; set; }
        public string ItemCategory_3 { get; set; }
        public string ForeignName { get; set; }
        public string InventoryUOM { get; set; }
        public string ImageURL { get; set; }
        public string ImageLink { get; set; }
        public string MCId { get; set; }
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
        public bool? IsBOM { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }

        public Nullable<DateTime> ValidTo { get; set; }
        public string WhsName { get; set; }

    }
}
