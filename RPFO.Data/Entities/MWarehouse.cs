using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MWarehouse
    {
        public string WhsCode { get; set; }
        public string CompanyCode { get; set; }
        public string WhsName { get; set; }
        public string StoreId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string WhsType { get; set; }
        public string DefaultSlocId { get; set; }
    }
}
