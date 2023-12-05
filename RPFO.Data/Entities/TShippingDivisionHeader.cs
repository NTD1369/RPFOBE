using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class TShippingDivisionHeader
    {
        public string Id { get; set; } 
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string StoreId { get; set; } 
        public string StoreName { get; set; }
        public string ShiftId { get; set; }
        public string CusId { get; set; }
        public string CusGrpId { get; set; }
        public string ContractNo { get; set; }
        
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? DocDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Status { get; set; }
        public string IsCanceled { get; set; }
        public string Remarks { get; set; } 
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }


        public List<TShippingDivisionLine> Lines { get; set; }  = new List<TShippingDivisionLine>();
       
    }
}
