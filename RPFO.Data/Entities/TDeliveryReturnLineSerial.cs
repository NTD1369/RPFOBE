using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public partial class TReturnLineSerial
    {
        public string PurchaseId { get; set; }
        public string LineId { get; set; }
        public string CompanyCode { get; set; }
        public string ItemCode { get; set; }
        public string SerialNum { get; set; }
        public string SLocId { get; set; }
        public decimal Quantity { get; set; }
        public string UOMCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public decimal OpenQty { get; set; }
        public string BaseLine { get; set; }
        public string BaseTransId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
    }
}
