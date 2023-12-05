using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MCurrency
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string Rounding { get; set; }
        public string RoundingDif { get; set; }
        public decimal? MaxValue { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        
    }
}
