using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class InventoryCountingViewModel : TInventoryCountingHeader
    {
        public InventoryCountingViewModel()
        {
            Lines = new List<InventoryCountingLineViewModel>();
        }
        public List<InventoryCountingLineViewModel> Lines { get; set; }
        public string TerminalId { get; set; }

    }

    public class InventoryCountingLineViewModel : TInventoryCountingLine
    {
        public InventoryCountingLineViewModel()
        {
            Lines = new List<TInventoryCountingLineSerial>();
        }
        public List<TInventoryCountingLineSerial> Lines { get; set; }
    }
}
