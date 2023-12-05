using System;

namespace RPFO.Data.Entities
{
    public partial class TPlacePrint
    {
        public int PrintId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string PlaceId { get; set; }
        public string PrintName { get; set; }
        public string GroupItem { get; set; }
        public string Status { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public string IGName { get; set; }
        public string IGId { get; set; }
        public string ItemName { get; set; }
    }
}
