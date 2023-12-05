using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SThirdPartyLog
    {
        public Guid Id { get; set; }
        public string TransId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Remark { get; set; }
        public List<SThirdPartyLogLine> Lines { get; set; } = new List<SThirdPartyLogLine>();


    }

    public partial class SThirdPartyLogLine
    {
        public string CompanyCode { get; set; }
        public string HeaderId { get; set; }
        public string StoreId { get; set; }
        public int LineId { get; set; }
        public string TransId { get; set; }
        public string JsonBody { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public string Key3 { get; set; }
        public string Key4 { get; set; }
        public string Key5 { get; set; }
        public string Status { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public string Remark { get; set; }
    }
}
