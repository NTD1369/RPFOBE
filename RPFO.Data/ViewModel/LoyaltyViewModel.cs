using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class LoyaltyViewModel : SLoyaltyHeader
    {
        public List<SLoyaltyCustomer> LoyaltyCustomers { get; set; }

        public List<SLoyaltyStore> LoyaltyStores { get; set; }
        public List<SLoyaltyBuy> LoyaltyBuy { get; set; }
        public List<SLoyaltyEarn> LoyaltyEarns { get; set; }
        public List<SLoyaltyExclude> LoyaltyExcludes { get; set; }

        public string LuckyNo { get; set; }

        public LoyaltyViewModel()
        {
            this.LoyaltyCustomers = new List<SLoyaltyCustomer>();
            this.LoyaltyStores = new List<SLoyaltyStore>();
            this.LoyaltyBuy = new List<SLoyaltyBuy>();
            this.LoyaltyEarns = new List<SLoyaltyEarn>();
            this.LoyaltyExcludes = new List<SLoyaltyExclude>();
        }
    }
}
