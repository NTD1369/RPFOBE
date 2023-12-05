using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
   public class TPurchaseRequestLine
    {
        public string KeyId { get { return ItemCode + UomCode; } }
        public string PurchaseId { get; set; }
        public string CompanyCode { get; set; }
        public string LineId { get; set; }
        public string ItemCode { get; set; }
        public string SlocId { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }
        public string UomCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? OpenQty { get; set; }
        public decimal? Price { get; set; }
        public string BaseSAPId { get; set; }
        public int? BaseSAPLine { get; set; }
        public string LineStatus { get; set; }
        public decimal? DiscPercent { get; set; }
        public decimal? Vatpercent { get; set; }
        public decimal? LineTotal { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public List<TPurchaseRequestLineSerial> SerialLines { get; set; } = new List<TPurchaseRequestLineSerial>();
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal NumOfDate { get; set; }
        public decimal SalesDay { get; set; }
        public decimal SalesWeek { get; set; }
        public decimal SalesMonth { get; set; }
        public decimal Turns { get; set; }
        public decimal LastQtyOrder { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public string CustomField6 { get; set; }
        public string CustomField7 { get; set; }
        public string CustomField8 { get; set; }
        public string CustomField9 { get; set; }
        public string CustomField10 { get; set; }

    }
}
