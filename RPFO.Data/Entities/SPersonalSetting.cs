using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SPersonalSetting
    {
        public string SettingId { get; set; }
        public string CompanyCode { get; set; }
        public string FunctionId { get; set; }
        public string SettingName { get; set; }
        public string SettingType { get; set; }
        public string SettingValue { get; set; }
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
    }
}
