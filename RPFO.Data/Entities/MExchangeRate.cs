using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MExchangeRate
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; } 
        public string Currency { get; set; } 
        public DateTime? Date { get; set; } 
        public decimal? Rate { get; set; } 
        public string Status { get; set; }
    }
}
