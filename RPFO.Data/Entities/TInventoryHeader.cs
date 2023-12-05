using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TInventoryHeader
    {
        public string Invtid { get; set; }
        public string CompanyCode { get; set; }
        public string DocType { get; set; }
        public string RefInvtid { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string IsCanceled { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public string FromStore { get; set; }
        public string FromStoreName { get; set; }
        public string ToStore { get; set; }
        public string ToStoreName { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public string RefId { get; set; }
        public string TransitWhs { get; set; }
        public string FromWhs { get; set; }
        public string ToWhs { get; set; }

        public string ShiftId { get; set; }

        //Thêm vào để control được source Location
        public string Source { get; set; }
        public string TerminalId { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
    }
}
