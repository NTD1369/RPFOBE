using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TSalesRedeemVoucher
    {
        public string Id { get; set; }
        public string CompanyCode { get; set; }
        public string TransId { get; set; }
        public int LineNum { get; set; }
        public string VoucherCode { get; set; }
        public string Name { get; set; }
        public string DiscountCode { get; set; }
        public string DiscountValue { get; set; }
        public string DiscountType { get; set; }
        public DateTime? ValidTill { get; set; } 

    }
}
