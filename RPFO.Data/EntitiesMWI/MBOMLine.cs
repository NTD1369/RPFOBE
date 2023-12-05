using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.EntitiesMWI
{
    public partial class MBomline
    {
        public string Bomid { get; set; }
        //public Guid Id { get; set; }
        //public string KeyId { get { return ItemCode + UomCode; } }
        public string ItemCode { get; set; }
        //public string CompanyCode { get; set; }
        public string ItemName { get; set; }
        public string UomCode { get; set; }
        public decimal? Quantity { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public string ModifiedBy { get; set; }
        //public DateTime? ModifiedOn { get; set; }
        //public string Status { get; set; }
        public bool? IsOption { get; set; }
        public string OptionGroup { get; set; }
        public string TriggerStatus { get; set; }
        public string TriggerSystem { get; set; }
    }
}
