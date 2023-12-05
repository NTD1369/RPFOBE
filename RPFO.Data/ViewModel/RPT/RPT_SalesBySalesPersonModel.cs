using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel.RPT
{
    public class RPT_SalesBySalesPersonModel
    {
         
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string SalesPerson { get; set; }
        public string CustomField7 { get; set; }
        public string EmployeeName { get; set; }
        public decimal? Year { get; set; }
        public decimal? TotalTransId { get; set; }
        public decimal? TotalQuantity { get; set; }
        public decimal? TotalAmount { get; set; }
          
    }
}
