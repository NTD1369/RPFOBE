using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MUom
    {
        public string UomCode { get; set; }
        public string CompanyCode { get; set; }
        public string UomName { get; set; }
        public string CreatedBy { get; set; }
        
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }

        public Nullable<bool> AllowDecimal { get; set; }
        public string DecimalFormat { get; set; }
        public string ThousandFormat { get; set; }
        public string DecimalPlacesFormat { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
    }
}
