using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TShiftLine
    {
        public string ShiftId { get; set; }
        public string CompanyCode { get; set; }
        public string PaymentCode { get; set; }
        public decimal? Value { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Status { get; set; }
    }
}
