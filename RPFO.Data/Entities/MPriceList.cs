using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MPriceList
    { 
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemGoupId { get; set; }
        public string ItemGroupName { get; set; }
        public string UomCode { get; set; }
        public string BarCode { get; set; }
        public string PriceListId { get; set; }
        public string PriceListName { get; set; }
        public decimal? PriceBeforeTax { get; set; }
        public decimal? PriceAfterTax { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string ListingStatus { get; set; }
    }

    public partial class MPriorityPriceList
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string CusGrpId { get; set; }
        public string CusGrpDesc { get; set; }
        public string PriceListId { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
       
    }
}
