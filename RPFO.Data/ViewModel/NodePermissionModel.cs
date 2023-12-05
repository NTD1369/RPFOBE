using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class NodePermissionModel
    { 
        public string RoleId { get; set; }
        public string FunctionId { get; set; }
        public string ControlId { get; set; }
        public string ColumnName { get; set; }
        public bool ColumnValue { get; set; }
    }
}
