using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TGoodsIssueHeader
    {
        public string Invtid { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public decimal? TotalPayable { get; set; }
        public decimal? TotalDiscountAmt { get; set; }
        public decimal? TotalReceipt { get; set; }
        public decimal? TotalTax { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string IsCanceled { get; set; }
        public string Remark { get; set; }
        public string StoreName { get; set; }
        public string RefId { get; set; }
        public string MovementType { get; set; }
        public string ShiftId { get; set; }
        public string Type { get; set; }
        public string SAPNo { get; set; }

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
