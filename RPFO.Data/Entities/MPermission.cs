using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MPermission
    {
        public string CompanyCode { get; set; }
        public Guid PermissionId { get; set; }
        public Guid RoleId { get; set; }
        public string FunctionId { get; set; }
        public string ControlId { get; set; }
        public string Permissions { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
