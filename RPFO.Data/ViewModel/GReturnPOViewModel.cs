
using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public partial class GReturnPOViewModel : TGoodsReturnheader
    { 
        public List<TGoodsReturnline> Lines { get; set; } = new List<TGoodsReturnline>();
        public List<TGoodsReturnlineSerial> SerialLines { get; set; } = new List<TGoodsReturnlineSerial>();
    }
     

}
