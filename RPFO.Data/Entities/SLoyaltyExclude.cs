using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public partial class SLoyaltyExclude
    {
        public string KeyId
        {
            get
            {

                string uoM = String.IsNullOrEmpty(LineUom) ? "" : LineUom;
                return LineCode + uoM;
            }
        }
        public string LoyaltyId { get; set; }
        public string CompanyCode { get; set; }
        public string LineType { get; set; }
        public string LineCode { get; set; }
        public string LineUom { get; set; }
        public string LineName { get; set; }
        public int? LineNum { get; set; }
    }
}
