 
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public partial class BasketPaymentViewModel  
    {
        public string Id { get; set; } 
        public string refNum { get; set; }
        public string RefNumber { get; set; }
        public decimal paymentDiscount { get; set; }
        public decimal paymentTotal { get; set; }
        public decimal paymentCharged { get; set; }
        public int LineNum { get; set; }
        public bool? IsRequireRefNum { get; set; }
        public decimal? MainBalance { get; set; }
        public decimal? SubBalance { get; set; }
    }
}
