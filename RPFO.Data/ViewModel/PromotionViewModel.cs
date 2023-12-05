using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public partial class PromotionViewModel : SPromoHeader
    {
        public List<SPromoCustomer> PromoCustomers { get; set; }
        public List<SPromoStore> PromoStores { get; set; }
        public List<SPromoBuy> PromoBuys { get; set; }
        public List<SPromoGet> PromoGets { get; set; }

        public PromotionViewModel()
        {
            this.PromoCustomers = new List<SPromoCustomer>();
            this.PromoStores = new List<SPromoStore>();
            this.PromoBuys = new List<SPromoBuy>();
            this.PromoGets = new List<SPromoGet>();
        }
    }
}
