using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public partial class SLoyaltyCustomer
    {
        public string LoyaltyId { get; set; }
        public string CompanyCode { get; set; }
        public int? LineNum { get; set; }
        public string CustomerValue { get; set; }
        public string CustomerType { get; set; }
    }
}
