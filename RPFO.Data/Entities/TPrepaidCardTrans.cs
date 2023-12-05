using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TPrepaidCardTrans
    {
        public string CompanyCode { get; set; }
        public string TransId { get; set; }
        public string PrepaidCardNo { get; set; }
        public string TransType { get; set; }
        public decimal? MainBalance { get; set; }
        public decimal? SubBalance { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
