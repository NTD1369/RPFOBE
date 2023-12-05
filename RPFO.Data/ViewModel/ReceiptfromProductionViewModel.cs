using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class ReceiptfromProductionViewModel: TReceiptfromProductionHeader
    {
        public List<TReceiptfromProductionLine> Lines { get; set; } = new List<TReceiptfromProductionLine>();
        public List<TReceiptfromProductionLineSerial> SerialLines { get; set; } = new List<TReceiptfromProductionLineSerial>();


    }
}

