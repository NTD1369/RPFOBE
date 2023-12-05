
using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public partial class ShiftViewModel : TShiftHeader
    {
         
        public List<TShiftLine> Lines { get; set; } = new List<TShiftLine>();
        
    }
}
