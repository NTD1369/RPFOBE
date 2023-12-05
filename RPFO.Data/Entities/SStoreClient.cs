using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SStoreClient
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string Name { get; set; }
        public string LocalIP { get; set; }
        public string PublicIP { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; }
        public string PoleName { get; set; }
        public string PoleBaudRate { get; set; }
        public string PoleParity { get; set; }
        public string PoleDataBits { get; set; }
        public string PoleStopBits { get; set; }
        public string PoleHandshake { get; set; }
        public string PrintSize { get; set; }
        public string PrintName { get; set; }
        public List<SClientDisallowance> Disallowances { get; set; } = new List<SClientDisallowance>();
      
    }
}
