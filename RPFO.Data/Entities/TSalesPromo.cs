using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TSalesPromo
    {
        public string TransId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string ItemCode { get; set; }
        public string BarCode { get; set; }
        public string RefTransId { get; set; }
        public string ApplyType { get; set; }
        public string ItemGroupId { get; set; }
        public string UomCode { get; set; }
        public decimal? Value { get; set; }
        public string PromoId { get; set; }
        public string PromoType { get; set; }
        public string PromoTypeLine { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public decimal? PromoPercent { get; set; }
        public decimal? PromoAmt { get; set; }

    }
}
