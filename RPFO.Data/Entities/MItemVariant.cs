using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MItemVariant
    {
        public Guid VariantId { get; set; }
        public string Description { get; set; } 
        public string Status { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; } 
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public List<MItemVariantBuy> BuyList { get; set; } = new List<MItemVariantBuy>();
        public List<MItemVariantMap> MapList { get; set; } = new List<MItemVariantMap>();


    }
    public partial class MItemVariantBuy
    {
        public string KeyId { get { return LineCode + LineUom; } }

        public Guid VariantId { get; set; }
        public int LineNum { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }
        public string LineUom { get; set; }
        public string CollectType { get; set; }
        public string SelectionType { get; set; } //
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; } 
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
    }
    public partial class MItemVariantMap
    {
        public string KeyId { get { return LineCode + LineUom; } }

        public Guid VariantId { get; set; }
        public int LineNum { get; set; } 
        public string LineCode { get; set; }
        public string LineName { get; set; }

        public string LineUom { get; set; }
        public string CollectType { get; set; } // Là Group / Item / Category / Collection
        public string SelectionType { get; set; } // Loại lựa chọn Single/Multi
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
    }
}
