using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TBankIn
    {
        public Guid? Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string DailyId { get; set; }
        public int? LineNum { get; set; }
        public string Currency { get; set; }
        public decimal? FCAmt { get; set; }
        public decimal? Rate { get; set; }
        public decimal? BankInAmt { get; set; }
        public string RefNum { get; set; }
        public string RefNum2 { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; } 
        public DateTime? DocDate { get; set; }
    }
}
