using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TSalesInvoice
    {
        public string TransId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; } 
        public string StoreName { get; set; }
        public string Name { get; set; }
        public string CustomerName { get; set; }
        public string TaxCode { get; set; }
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
