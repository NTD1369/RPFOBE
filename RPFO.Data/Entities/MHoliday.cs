using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MHoliday
    {
        public string CompanyCode { get; set; }
        public string HldCode { get; set; }
        public DateTime? StrDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Rmrks { get; set; }
        public string Status { get; set; }
    }
}


 