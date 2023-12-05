using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MTax
    {
        public string TaxCode { get; set; }
        public string CompanyCode { get; set; }
        public string TaxName { get; set; }
        public decimal? TaxPercent { get; set; }
        public string TaxType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
    }
}
