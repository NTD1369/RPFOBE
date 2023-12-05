using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public partial class TReturnHeader
    {

        public string PurchaseId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string DocStatus { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string InvoiceAddress { get; set; }
        public string TaxCode { get; set; }
        public decimal VATTotal { get; set; }
        public decimal DocTotal { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string IsCanceled { get; set; }
        public DateTime? SyncDate { get; set; }
        public string SyncSource { get; set; }
        public string DocEntry { get; set; }
        public string DataSource { get; set; }
        public string SAPNo { get; set; }
        public string PRNum { get; set; }

    }
}
