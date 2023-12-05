using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class LoyaltyHeaderViewModel
    {
        public string LoyaltyId { get; set; }
        public string LoyaltyName { get; set; }
        public string LoyaltyType { get; set; }
        public DateTime? ValidDateFrom { get; set; }
        public DateTime? ValidDateTo { get; set; }
        public string Status { get; set; }
        //public string IsApply { get; set; }
    }
}
