using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SDatasourceEdit
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string DataSource { get; set; }
        public string Field { get; set; }
        public string CanEdit { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
       
    }
}
