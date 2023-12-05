 
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public class PromoHeaderViewModel  
    {
        public string PromoId { get; set; } 
        public string PromoName { get; set; }
        public string PromoType { get; set; } 
        public string SAPPromoId { get; set; } 
        public string SAPBonusBuyId { get; set; } 
        public DateTime? ValidDateFrom { get; set; } 
        public DateTime? ValidDateTo { get; set; } 
        public string Status { get; set; }
        public string IsApply { get; set; }
        public string IsUsed { get; set; }

    }
}
