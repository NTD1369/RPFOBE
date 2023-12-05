using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public class SLoyaltyPointConvert
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public double Point { get; set; }
        public double Amount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
