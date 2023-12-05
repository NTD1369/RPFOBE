using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MBankTerminal
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string CounterId { get; set; }
        public string PaymentMethod { get; set; }
        public string ShortName { get; set; }
        public string TerminalIdDefault { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
    }
}
