using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TGoodsReceiptLine
    {
        public string Invtid { get; set; }
        public string CompanyCode { get; set; }
        public string LineId { get; set; }
        public string KeyId { get { return ItemCode + UomCode + BarCode; } }
        public string ItemCode { get; set; }
        public string SlocId { get; set; }
        public string BarCode { get; set; }
        public string UomCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? LineTotal { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? CurrencyRate { get; set; }
        public string TaxCode { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmt { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

    }
}
