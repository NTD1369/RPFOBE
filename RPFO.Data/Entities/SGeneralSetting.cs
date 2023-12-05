using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SGeneralSetting
    {
        public string SettingId { get; set; }
        public string CompanyCode { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public string SettingDescription { get; set; }
        public string ValueType { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string StoreId { get; set; }
        public string DefaultValue { get; set; }
         
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public decimal? TokenExpired { get; set; }
    }


    public class GeneralSettingStore : MStore
    {
         
        public List<SGeneralSetting> GeneralSettings { get; set; } = new List<SGeneralSetting>();
    }
    
}
