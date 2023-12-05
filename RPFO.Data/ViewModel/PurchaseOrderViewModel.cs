
using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public partial class PurchaseOrderViewModel : TPurchaseOrderHeader
    { 
        public List<TPurchaseOrderLine> Lines { get; set; } = new List<TPurchaseOrderLine>();
        public List<TPurchaseOrderLineSerial> SerialLines { get; set; } = new List<TPurchaseOrderLineSerial>();
       

    }
     

}
