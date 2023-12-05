using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TInventoryTransferLine
    {
        public string keyId { get { return ItemCode + UomCode + BarCode; } }
        public string InvtTransid { get; set; }
        public string CompanyCode { get; set; }
        public string LineId { get; set; }
        public string ItemCode { get; set; }
        public string FrSlocId { get; set; }
        public string ToSlocId { get; set; }
        public string DocType { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }
        public string UomCode { get; set; }
        public decimal? Quantity { get; set; }
        public DateTime? ShipDate { get; set; }
        public decimal? OpenQty { get; set; }
        public decimal? Price { get; set; }
        public decimal? LineTotal { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string BaseTransId { get; set; }
        public string BaseLine { get; set; }
    }
}
