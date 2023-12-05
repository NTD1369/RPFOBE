using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.EntitiesMWI
{
    public partial class MPriceList
    {
        public string PriceListId { get; set; }
        public string StoreId { get; set; }
        public string ItemCode { get; set; }
        public string Uomcode { get; set; }
        public string BarCode { get; set; }
        public decimal? PriceBeforeTax { get; set; }
        public decimal? PriceAfterTax { get; set; }
        public string Status { get; set; }
    }
}
