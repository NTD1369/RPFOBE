using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public class MLoyaltyType
    {
        public int LoyaltyType { get; set; }
        public string TypeName { get; set; }
        public int? PriorityNo { get; set; }
        public string Status { get; set; }
    }
}
