using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TDeliveryLineSerial
    {
        public string TransId { get; set; }
        public string LineId { get; set; }
        public string CompanyCode { get; set; }
        public string ItemCode { get; set; }
        public string StoreId { get; set; }
        public string SerialNum { get; set; }
        public string SlocId { get; set; }
        public decimal? Quantity { get; set; }
        public string UomCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ExpDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public decimal? OpenQty { get; set; }
        public int? BaseLine { get; set; }
        public string BaseTransId { get; set; }
        public int? LineNum { get; set; }
        public string Description { get; set; }
        public string Prefix { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
    }
}
