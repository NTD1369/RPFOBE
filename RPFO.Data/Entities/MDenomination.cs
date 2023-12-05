using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MDenomination
    {
        public Guid Id { get; set; }
        public string Currency { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public bool? ShowOnPayment { get; set; }
        public bool? ShowOnDiscount { get; set; }
    }
}
