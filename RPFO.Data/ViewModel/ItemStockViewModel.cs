using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public partial class ItemStockViewModel
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string SlocId { get; set; }
        public string ItemCode { get; set; }
        public string UomCode { get; set; }
        public decimal? Quantity { get; set; }
        public string BarCode { get; set; }
        public string SerialNum { get; set; }
    }
}
