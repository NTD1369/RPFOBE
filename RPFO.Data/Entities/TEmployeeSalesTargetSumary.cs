using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    //public partial class SSalesPlanType
    //{ 
    //    public string CompanyCode { get; set; }
    //    public string Code { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string Status { get; set; }
    //}
    
    
    public partial class TEmployeeSalesTargetSummary
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Salary { get; set; } 
        public decimal? CommisionValue { get; set; }
        public string SalesTarget { get; set; }
        public decimal? LineTotal { get; set; }
        public decimal? LineTotal1 { get; set; }
        public decimal? LineTotal2 { get; set; } 
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; } 
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
   
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}


 