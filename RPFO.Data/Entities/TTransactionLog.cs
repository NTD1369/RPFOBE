using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TTransactionLog
    {
        public string TransId { get; set; }
        public string CompanyCode { get; set; }
        public string SlocId { get; set; }
        public string ItemCode { get; set; }
        public string StoreId { get; set; }
        public DateTime? TransDate { get; set; }
        public string TransType { get; set; }
        public decimal? InQty { get; set; }
        public decimal? OutQty { get; set; }
        public string Uomcode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
    }
}
