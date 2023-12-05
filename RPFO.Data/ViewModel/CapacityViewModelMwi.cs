using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class CapacityViewModelMwi
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string StoreAreaId { get; set; }
        public string TimeFrameId { get; set; }
        public DateTime TransDate { get; set; }
        public int StartTimeNum { get; set; }
        public int EndTimeNum { get; set; }
        public int Duration { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentCapacity { get; set; }
        public int RemainCapacity { get; set; }
    }
}
