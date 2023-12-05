using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SCurrencyRoundingOff
    {
        public Guid? Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? RoundingOff { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
       
        public string Status { get; set; }
        
    }
}
