using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class LoyaltyPointTransferModel
    {
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string SendCustomerId { get; set; }
        public string SendCustomerName { get; set; }
        public string SendCardNumber { get; set; }
        public string RecivedCustomerId { get; set; }
        public string RecivedCustomerName { get; set; }
        public string RecivedCardNumber { get; set; }
        public DateTime TransDate { get; set; }
        //public string TransType { get; set; }
        public double TransPoint { get; set; }
        public string CreatedBy { get; set; }
    }
}
