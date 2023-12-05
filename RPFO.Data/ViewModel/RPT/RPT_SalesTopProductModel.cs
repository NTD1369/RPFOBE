using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel.RPT
{
    public class RPT_SalesTopProductModel
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string SlocId { get; set; } 
        public string ItemCode { get; set; } 
        public string UomCode { get; set; }
        public string TotalTransId { get; set; }
        public decimal? TotalQuantity { get; set; }
        public decimal? TotalAmount { get; set; }
       

    }
}
