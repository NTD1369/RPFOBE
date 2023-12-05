using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TGoodsReceiptPolineSerial
    {
        public string KeyId { get { return ItemCode + UomCode; } }
        public string PurchaseId { get; set; }
        public string LineId { get; set; }
        public string CompanyCode { get; set; }
        public string ItemCode { get; set; }
        public string SerialNum { get; set; }
        public string SlocId { get; set; }
        public decimal? Quantity { get; set; }
        public string UomCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int BaseLine { get; set; }
        public string BaseTransId { get; set; }
        public string Description { get; set; }
    }
}
