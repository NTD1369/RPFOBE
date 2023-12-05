using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public partial class MSalesChannel
    {
        public string CompanyCode { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
    }
}
