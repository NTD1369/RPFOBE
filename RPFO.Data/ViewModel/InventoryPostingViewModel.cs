using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class InventoryPostingViewModel : TInventoryPostingHeader
    {
        public InventoryPostingViewModel()
        {
            Lines = new List<InventoryPostingLineViewModel>();
        }
        public List<InventoryPostingLineViewModel> Lines { get; set; }
        public string TerminalId { get; set; }


    }

    public class InventoryPostingLineViewModel : TInventoryPostingLine
    {
        public InventoryPostingLineViewModel()
        {
            Lines = new List<TInventoryPostingLineSerial>();
        }
        public List<TInventoryPostingLineSerial> Lines { get; set; }
    }
}
