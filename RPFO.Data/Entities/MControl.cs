using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MControl
    {
        public string ControlId { get; set; }
        public string CompanyCode { get; set; }
        public string ControlName { get; set; }
        public string FunctionId { get; set; }
        public string Action { get; set; }
        public string ControlType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? OrderNum { get; set; }
        public bool? Require { get; set; }
        public string OptionName { get; set; }
        public string OptionKey { get; set; }
        public string OptionValue { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Status { get; set; }
        public string GroupNum { get; set; }
        public string TotalItem { get; set; }
        public string GroupItem { get; set; }
        public bool? ReadOnly { get; set; }

        public string QueryStr { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; } 

    }
}
