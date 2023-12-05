using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public partial class GReturnDeliveryViewModel: TReturnHeader
    {
        public List<TReturnLine> Lines { get; set; } = new List<TReturnLine>();
        public List<TReturnLineSerial> SerialLines { get; set; } = new List<TReturnLineSerial>();
    }
}
