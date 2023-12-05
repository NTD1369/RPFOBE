using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public class SLoyaltyHeader
    {
        public string LoyaltyId { get; set; }
        public string CompanyCode { get; set; }
        public int? LoyaltyType { get; set; }
        public string LoyaltyName { get; set; }
        public string CustomerType { get; set; }
        public DateTime? ValidDateFrom { get; set; }
        public DateTime? ValidDateTo { get; set; }
        public int? ValidTimeFrom { get; set; }
        public int? ValidTimeTo { get; set; }
        public string IsMon { get; set; }
        public string IsTue { get; set; }
        public string IsWed { get; set; }
        public string IsThu { get; set; }
        public string IsFri { get; set; }
        public string IsSat { get; set; }
        public string IsSun { get; set; }
        public double? TotalBuyFrom { get; set; }
        public double? TotalBuyTo { get; set; }
        public string TotalEarnType { get; set; }
        public double? TotalEarnValue { get; set; }
        public double? MaxTotalEarnValue { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
    }
}
