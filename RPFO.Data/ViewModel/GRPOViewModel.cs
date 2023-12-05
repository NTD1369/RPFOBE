
using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public partial class GRPOViewModel : TGoodsReceiptPoheader
    { 
        public List<TGoodsReceiptPoline> Lines { get; set; } = new List<TGoodsReceiptPoline>();
        public List<TGoodsReceiptPolineSerial> SerialLines { get; set; } = new List<TGoodsReceiptPolineSerial>();
       
        public string TerminalId { get; set; }
    }
     

}
