using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.EntitiesMWI
{
    public partial class TSalesHeader
    {
        public string PostransId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string ContractNo { get; set; }
        public string DocEntry { get; set; }
        public string DocStatus { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string DocCur { get; set; }
        public decimal? DocRate { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal? DiscSum { get; set; }
        public decimal? VatSum { get; set; }
        public decimal? DocTotal { get; set; }
        public string IsCanceled { get; set; }
        public string Remarks { get; set; }
        public string DocType { get; set; }
        public string POSType { get; set; }
    }
}
