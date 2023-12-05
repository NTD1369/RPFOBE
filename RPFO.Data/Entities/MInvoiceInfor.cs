using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MInvoiceInfor
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public string CompanyCode { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string TaxCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        
    }
}
