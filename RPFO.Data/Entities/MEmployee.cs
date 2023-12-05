using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MEmployee
    {
        public string EmployeeId { get; set; }
        public string CompanyCode { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public decimal? TargetAmount { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public List<MEmployeeStore> Stores { get; set; } = new List<MEmployeeStore>();
    }
}
