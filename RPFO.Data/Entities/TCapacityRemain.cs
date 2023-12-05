using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TCapacityRemain
    {
        public string StoreId { get; set; }
        public string CompanyCode { get; set; }
        public string TimeFrameId { get; set; }
        public string StoreAreaId { get; set; }
        public DateTime TransDate { get; set; }
        public int? MaxCapacity { get; set; }
        public int? CurrentCapacity { get; set; }
        public int? RemainCapacity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
    }
}
