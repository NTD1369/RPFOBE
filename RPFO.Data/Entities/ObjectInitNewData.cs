using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public class ObjectInitNewData
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string DBUser { get; set; }
        public string DBPassword { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public bool? IsServer { get; set; } = false;

    }
}
