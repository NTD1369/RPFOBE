using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
   public partial class PurchaseRequestViewModel:TPurchaseRequestHeader
    {

        public List<TPurchaseRequestLine> Lines { get; set; } = new List<TPurchaseRequestLine>();
        public List<TPurchaseRequestLineSerial> SerialLines { get; set; } = new List<TPurchaseRequestLineSerial>();
        public string TerminalId { get; set; }
    }

    public partial class AverageNumberSaleListModel
    {
        public List<AverageNumberSaleModel> AverageNumberSaleModel { get; set; }
        public List<AverageNumberSaleModel> QtyPOModel { get; set; }
    }
    public partial class AverageNumberSaleModel
    {
        //public string ItemCode { get; set; }
        //public string BarCode { get; set; }
        //public string UomCode { get; set; }
        //public string SlocId { get; set; }
        //public string FromDate { get; set; }
        //public string ToDate { get; set; }
        public decimal? QuantityTotal { get; set; }
        public decimal? QuantityCancel { get; set; }
        public decimal? QuantityReturn { get; set; }
    }
    public partial class AverageNumberSaleModel
    {
        public string ItemCode { get; set; }
        public string BarCode { get; set; }
        public string UomCode { get; set; }
        public string SlocId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Quantity { get; set; }
    }
}
