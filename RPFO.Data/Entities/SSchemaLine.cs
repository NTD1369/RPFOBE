using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SSchemaLine
    {
        public string SchemaId { get; set; }
        public string CompanyCode { get; set; }
        public int LineNum { get; set; }
        public string PromoId { get; set; }
        public string Description { get; set; }
        public int? Priority { get; set; }
        public string IsApply { get; set; }
        public string PromoType { get; set; }
        public string PromoTypeName { get; set; }


        public int VirtualIndex { get; set; }
    }
}
