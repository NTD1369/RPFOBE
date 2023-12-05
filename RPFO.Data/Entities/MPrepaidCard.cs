using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MPrepaidCard
    {
        public string CompanyCode { get; set; }
        public string PrepaidCardNo { get; set; }
        public decimal? MainBalance { get; set; }
        public decimal? SubBalance { get; set; }
        public DateTime? StartDate { get; set; }
        public int? Duration { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
