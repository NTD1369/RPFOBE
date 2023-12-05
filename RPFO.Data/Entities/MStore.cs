using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MStore
    {
        public string StoreId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreName { get; set; }
        public string StoreDescription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string ForeignName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string DefaultCusId { get; set; }
        public string StoreGroupId { get; set; }
        public string ProvinceId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string CountryCode { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public string AreaCode { get; set; }
        public string CurrencyCode { get; set; }
        public string StoreType { get; set; }
        public string ListType { get; set; }
        public string FormatConfigId { get; set; }
        public string WhsCode { get; set; }
        public string RegionCode { get; set; }
        public string MappingType { get; set; }
        public string PrintRemarks { get; set; }
    }
}
