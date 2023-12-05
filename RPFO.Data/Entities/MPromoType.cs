using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MPromoType
    {
        public int PromoType { get; set; }
        public string TypeName { get; set; }
        public int? PriorityNo { get; set; }
        public string Status { get; set; }
    }
}
