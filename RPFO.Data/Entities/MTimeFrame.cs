using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MTimeFrame
    {
        public string CompanyCode { get; set; }
        public string TimeFrameId { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public string StartTimeStr { get; set; }
        public string EndTimeStr { get; set; }

        public int? Duration { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
    }
}
