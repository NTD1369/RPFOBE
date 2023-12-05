using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TItemStorage
    {
        public string SlocId { get; set; }
        public string CompanyCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public string UomCode { get; set; }
        public string StoreId { get; set; }
        public decimal? Quantity { get; set; }

        public List<TItemStorage> Lines = new List<TItemStorage>();
    }



}
