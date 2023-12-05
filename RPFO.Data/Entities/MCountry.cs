using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MCountry
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string ForeignName { get; set; }
        public string AreaCode { get; set; }
    }
}
