using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TVoucherTransaction
    {
        public string CompanyCode { get; set; }
        public string ItemCode { get; set; }
        public string VoucherNo { get; set; }
        public decimal? VoucherValue { get; set; }
        public string VoucherType { get; set; }
        public string IssueTransId { get; set; }
        public DateTime? IssueDate { get; set; }
        public string RedeemTransId { get; set; }
        public DateTime? RedeemDate { get; set; }
      
    }
}


 