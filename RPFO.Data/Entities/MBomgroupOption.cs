using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MBomgroupOption
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int? MaxValue { get; set; }
        public string Status { get; set; }
    }
}
