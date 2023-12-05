using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class GoodReceiptViewModel : TGoodsReceiptHeader
    {
        
        public GoodReceiptViewModel()
        {
            Lines = new List<GoodReceiptLineViewModel>();
        }
        public List<GoodReceiptLineViewModel> Lines { get; set; }
        public string TerminalId { get; set; }

    }

    public class GoodReceiptLineViewModel : TGoodsReceiptLine
    {
        public GoodReceiptLineViewModel()
        {
            Lines = new List<TGoodsReceiptLineSerial>();
        }
        public List<TGoodsReceiptLineSerial> Lines { get; set; }
    }
}
