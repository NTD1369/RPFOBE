using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SLoyaltyRank
    {
        public string CompanyCode { get; set; }
        public string RankId { get; set; }
        public string RankName { get; set; }
        public decimal? Factor { get; set; }
        public decimal? TargetAmount { get; set; }
        public int? Period { get; set; }
        public string Status { get; set; }
        
    }
}
