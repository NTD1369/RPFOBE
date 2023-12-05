using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public partial class InActivePromoViewModel 
    {
        public string CompanyCode { get; set; }
        public string PromoLineType { get; set; }
        public string PromoId { get; set; }
        public int LineNum { get; set; }
        public string InActive { get; set; }
    }
}
