using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MRole
    {
        public Guid RoleId { get; set; }
        public string CompanyCode { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Status { get; set; }
        public string DefaultScreen { get; set; }
    }
}
