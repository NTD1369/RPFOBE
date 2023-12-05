using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class InventoryViewModel : TInventoryHeader
    {
        public InventoryViewModel()
        {
            Lines = new List<InventoryLineViewModel>();
        }
        public List<InventoryLineViewModel> Lines { get; set; }
        public string TerminalId { get; set; }


    }

    public class InventoryLineViewModel : TInventoryLine
    {
        public string StoreId { get; set; }
        public InventoryLineViewModel()
        {
            Lines = new List<TInventoryLineSerial>();
        }
        public List<TInventoryLineSerial> Lines { get; set; }
    }
}
