using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
   public partial class TSalesRedeemMenber
    {
        public string CompanyCode { get; set; }
        public string TransId { get; set; }
        public string InvoiceId { get; set; }
        public string ItemCode { get; set; }
        public string SerialNum { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TimesInDay { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public string UomCode { get; set; }

    }
}
