using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.EntitiesMWI
{
    public partial class MItemUom
    {
        public string ItemCode { get; set; }
        public string Uomcode { get; set; }
        public decimal? Factor { get; set; }
        public string BarCode { get; set; }
        public string Qrcode { get; set; }
        public string Status { get; set; }
    }
}
