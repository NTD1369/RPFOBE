using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MItemRejectPayment
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
      
        public string ItemCode { get; set; }
        public string PaymentType { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; } 
        public string Status { get; set; }
    }
}
