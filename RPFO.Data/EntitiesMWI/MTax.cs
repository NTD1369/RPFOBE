using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.EntitiesMWI
{
    public partial class MTax
    {
        public string TaxCode { get; set; }
        public string TaxName { get; set; }
        public decimal? TaxPercent { get; set; }
        public string TaxType { get; set; }
        public string Status { get; set; }
    }
}
