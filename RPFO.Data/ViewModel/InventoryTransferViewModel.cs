using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class InventoryTransferViewModel : TInventoryTransferHeader
    {
        public InventoryTransferViewModel()
        {
            Lines = new List<InventoryTransferLineViewModel>();
        }
        public List<InventoryTransferLineViewModel> Lines { get; set; }
         
    }

    public class InventoryTransferLineViewModel : TInventoryTransferLine
    {
        public InventoryTransferLineViewModel()
        {
            Lines = new List<TInventoryTransferLineSerial>();
        }
        public List<TInventoryTransferLineSerial> Lines { get; set; }
    }
}
