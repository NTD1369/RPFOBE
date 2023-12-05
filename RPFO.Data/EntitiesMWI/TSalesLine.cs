using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.EntitiesMWI
{
    public partial class TSalesLine
    {
        public string PostransId { get; set; }
        public int LineId { get; set; }
        public string LineStatus { get; set; }
        public string ItemCode { get; set; }
        public string UomCode { get; set; }
        public string BarCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? OpenQty { get; set; }
        public decimal? Price { get; set; }
        public string WhsCode { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal? DiscSum { get; set; }
        public string TaxCode { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmt { get; set; }
        public string Remark { get; set; }
        public string PromoId { get; set; }
        public string PromoType { get; set; }
        public decimal? LineTotal { get; set; }
    }
}
