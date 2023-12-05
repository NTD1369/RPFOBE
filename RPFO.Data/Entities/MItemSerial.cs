using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MItemSerial
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string ItemCode { get; set; }
        public string SlocId { get; set; }
        public string SerialNum { get; set; }
        public decimal? Quantity { get; set; }
        public DateTime? ExpDate { get; set; }
        public DateTime? StoredDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string Prefix { get; set; }
        public string TransId { get; set; }
        public string RedeemedTransId { get; set; }
        public MItemSerialStock serialStock { get; set; }
    }
}
