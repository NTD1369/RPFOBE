using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public partial class TLoyaltyLog
    {
        public string TransId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public DateTime TransDate { get; set; }
        public string TransType { get; set; }
        public double InPoint { get; set; }
        public double OutPoint { get; set; }
        public double InAmt { get; set; }
        public double OutAmt { get; set; }
        public double PointRatio { get; set; }
        public double AmountRatio { get; set; }
        public string CusRank { get; set; }
        public double RankFactor { get; set; }
    }
}
