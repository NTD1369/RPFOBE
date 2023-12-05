using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
   public class TPurchaseRequestLineSerial
    {
        public string KeyId { get { return ItemCode + UomCode; } }
        public string PurchaseId { get; set; }
        public string LineId { get; set; }
        public string CompanyCode { get; set; }
        public string ItemCode { get; set; }
        public string SerialNum { get; set; }
        public string SlocId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? OpenQty { get; set; }
        public string UomCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Description { get; set; }

    }
}
