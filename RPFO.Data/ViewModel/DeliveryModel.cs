using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class DeliveryModel: T_DeliveryHeader
    {
        public List<T_DeliveryLine> Lines { get; set; } = new List<T_DeliveryLine>();
        public List<T_DeliveryLineSerial> SerialLines { get; set; } = new List<T_DeliveryLineSerial>();


    }
}
