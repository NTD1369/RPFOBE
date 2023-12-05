using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SLoyaltyBuy
    {
        public string KeyId { get { return LineCode + LineUom; } }
        public string LoyaltyId { get; set; }
        public string CompanyCode { get; set; }
        public int LineNum { get; set; }
        public string LineType { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }
        public string LineUom { get; set; }
        public string ValueType { get; set; }
        public string Condition1 { get; set; }
        public double? Value1 { get; set; }
        public string Condition2 { get; set; }
        public double? Value2 { get; set; }
    }
}
