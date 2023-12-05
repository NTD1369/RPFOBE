using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public partial class SchemaViewModel : SPromoSchema
    {
        public SchemaViewModel()
        {
            this.SchemaLines = new List<SSchemaLine>();
            this.PromotionLines = new List<SPromoHeader>();
        }
        public List<SSchemaLine> SchemaLines { get; set; }
        public List<SPromoHeader> PromotionLines { get; set; }
    }
}
