using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MUserStore
    {
        public Guid UserId { get; set; }
        public string StoreId { get; set; }
    }
}
