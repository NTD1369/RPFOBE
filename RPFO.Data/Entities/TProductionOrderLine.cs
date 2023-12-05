using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
    public class TProductionOrderLine
    {
        public string PurchaseId { get; set; }
        public string CompanyCode { get; set; }
        public string LineId { get; set; }
        public string SlocId { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string BarCode { get; set; }
        public string UomCode { get; set; }
        public string UomName { get; set; }
        public string SapNo { get; set; }
        public decimal? BaseQty { get; set; }
        public decimal? BaseRatio { get; set; }
        public decimal? AddtitionalQty { get; set; }
        public decimal? PlannedQty { get; set; }
        public decimal? IssueId { get; set; }
        public decimal? Available { get; set; }
        public string WareHouse { get; set; }
        public string IssueMethod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? OpenQty { get; set; }
        public decimal? InStock { get; set; }
        public string Ordered { get; set; }
        public string Committed { get; set; }
        public string RowNo { get; set; }
        public string WIPAccount { get; set; }
        public string PhongBan { get; set; }
        public string CuaHang { get; set; }
        public string KenhBanHang { get; set; }
        public string Project { get; set; }
        public string ProductionTime { get; set; }
        public string AdditionalTime { get; set; }
        public string RunTime { get; set; }
        public string RequiredDay { get; set; }
        public string WaitingDay { get; set; }
        public string TotalDay { get; set; }
        public string ResourceAllocation { get; set; }
        public decimal? QtyInWhse { get; set; }
        public decimal? CommittedQtyInWhse { get; set; }
        public decimal? OrderedQtyInWhse { get; set; }
        public decimal? RouteSequence { get; set; }
        public string Status { get; set; }
        public decimal? CaculationProportion { get; set; }
        public string ProcurementDoc { get; set; }
        public string AllowProcurmtDoc { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public string CustomField6 { get; set; }
        public string CustomField7 { get; set; }
        public string CustomField8 { get; set; }
        public string CustomField9 { get; set; }
        public string CustomField10 { get; set; }
    }
}
