using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{

    public partial class TSalesDelivery
    {
        public Guid Id { get; set; }
        public string TransId { get; set; }
        public string CompanyCode { get; set; }
        public string DeliveryType { get; set; }
        public string DeliveryMethod { get; set; }
        public decimal? DeliveryFee { get; set; }

        public string DeliveryId { get; set; }
        public string DeliveryPartner { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }


    }
}
