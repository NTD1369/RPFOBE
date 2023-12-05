using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MStorePayment
    {
        public string StoreId { get; set; }
        public string PaymentCode { get; set; }
        public string PaymentDesc { get; set; }
        public string ShortName { get; set; }
        public bool? AllowMix { get; set; }
        public bool? IsShow { get; set; }
        public int? OrderNum { get; set; }
        public string Status { get; set; }
    }
}
