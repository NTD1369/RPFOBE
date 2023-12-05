using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MEmployeeStore
    {
        public Guid Id { get; set; }
        public string EmployeeId { get; set; }
        public string StoreId { get; set; }
        public string Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
