using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public class TProductionOrderHeader
    {
        public string PurchaseId { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string Type { get; set; }
        public string SapStatus { get; set; }
        public string ProductNo { get; set; }
        public string Description { get; set; }
        public string UomCode { get; set; }
        public string UomName { get; set; }
        public decimal? Quantity { get; set; }
        public string WarehouseCode { get; set; }
        public int? Priority { get; set; }
        public string RoutingDateCalculation { get; set; }
        public bool? IsProductItem { get; set; }
        public string SapNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? DocDueDate { get; set; }
        public string Origin { get; set; }
        public string LinkedTo { get; set; }
        public string LinkedOrder { get; set; }
        public string Customer { get; set; }
        public string DistrRule { get; set; }
        public string Project { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Comment { get; set; }
        public string IsCanceled { get; set; }
        public string Status { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string DocEntry { get; set; }

    }
}
