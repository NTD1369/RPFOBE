using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MImage
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string Type { get; set; }
        public string Num { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public DateTime CreateOn { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
    }
}
