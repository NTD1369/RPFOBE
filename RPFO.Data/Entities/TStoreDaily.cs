using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TStoreDaily
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string DailyId { get; set; }
        public string DeviceId { get; set; }
        public DateTime? CreateDate { get; set; }
        public decimal? TotalSales { get; set; }
        public decimal? TotalCount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
    }
}
