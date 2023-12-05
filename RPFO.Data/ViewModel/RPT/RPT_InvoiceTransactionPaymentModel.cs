using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel.RPT
{
    public class RPT_InvoiceTransactionPaymentModel
    {
        public string InvoiceType { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string TransId { get; set; }
        public string StoreName { get; set; }
        public string PaymentCode { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? ChargableAmount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }

    }
}
