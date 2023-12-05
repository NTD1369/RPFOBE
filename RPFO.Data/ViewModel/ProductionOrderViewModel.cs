using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class ProductionOrderViewModel:TProductionOrderHeader
    {
        public List<TProductionOrderLine> Lines { get; set; } = new List<TProductionOrderLine>();
       
    }
}
