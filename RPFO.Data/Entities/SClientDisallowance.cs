using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public class SClientDisallowance
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string CounterId { get; set; }
        public string Permission { get; set; }
        public string FunctionId { get; set; }
        public string Function { get; set; }
        public string ControlId { get; set; }
        public string Control { get; set; }
        public string Remark { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
 
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }

    }
}
