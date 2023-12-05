using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MFunction
    {
        public MFunction()
        {
           Items = new List<MFunction>();
        }
        public string FunctionId { get; set; }
        public string CompanyCode { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ParentId { get; set; }
        public string Icon { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string LicenseType { get; set; } 
        public int? OrderNo { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public bool? isShowMenu { get; set; }
        public bool? isParent { get; set; }
        public int? MenuOrder { get; set; }
        public List<MFunction> Items { get; set; }
    }
}
