using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MRegion
    {
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public string ForeignName { get; set; }
        public string CountryCode { get; set; }
        public string Status { get; set; }
    }
}
