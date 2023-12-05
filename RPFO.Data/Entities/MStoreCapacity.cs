using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MStoreCapacity
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string StoreAreaId { get; set; }
        public string TimeFrameId { get; set; }
        public int? MaxCapacity { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string KeyId { get { return StoreId + StoreAreaId + TimeFrameId; } }


    }
}
