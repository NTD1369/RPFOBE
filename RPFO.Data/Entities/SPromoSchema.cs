using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SPromoSchema
    {
        public string SchemaId { get; set; }
        public string CompanyCode { get; set; }
        public string SchemaName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string AllowChain { get; set; }
    }
}
