using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class EmployeeViewModel: MEmployee
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; } 
        public Nullable<DateTime> FromDate { get; set; } 
        public Nullable<DateTime> ToDate { get; set; } 
    }
}
