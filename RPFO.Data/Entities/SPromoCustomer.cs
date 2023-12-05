using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SPromoCustomer
    {
        public string PromoId { get; set; }
        public string CompanyCode { get; set; }
        public int? LineNum { get; set; }
        public string CustomerValue { get; set; }
        public string CustomerType { get; set; }
    }
}
