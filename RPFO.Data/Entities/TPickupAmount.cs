using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TPickupAmount
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string DailyId { get; set; }
        public string CounterId { get; set; }
        public string ShiftId { get; set; }
        public string PickupBy { get; set; }
        public decimal? Amount { get; set; }
        public string Remarks { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; } 
        public string Status { get; set; }
        public DateTime? ShiftDate { get; set; }
    }
}
