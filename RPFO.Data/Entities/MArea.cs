using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MArea
    {
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string ForeignName { get; set; }
        public string Status { get; set; }
        public string RegionCode { get; set; }
    }
}
