using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MStoreCurrency
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; } 
        public string StoreName { get; set; } 
        public string Currency { get; set; } 
        public string CurrencyName { get; set; } 
        public string RoundingMethod { get; set; } 
        public decimal? Rate { get; set; } 
        public string Status { get; set; }
    }
}
