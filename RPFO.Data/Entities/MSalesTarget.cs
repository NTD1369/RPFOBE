﻿using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    //public partial class SSalesPlanType
    //{ 
    //    public string CompanyCode { get; set; }
    //    public string Code { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string Status { get; set; }
    //}
    public partial class MSalesPlanHeader
    {
        public string Id { get; set; }
        public string CompanyCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        //public string SetupType { get; set; }
        //public string SetupValue { get; set; } 
        //public int? Priority { get; set; }
        //public string FilterBy { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }


        public List<MSalesPlanLine> Lines { get; set; } = new List<MSalesPlanLine>();
    }

    public partial class MSalesPlanLine
    {
        public Guid Id { get; set; }
        public string CompanyCode { get; set; }
        public string StoreId { get; set; }
        public string SalesPlanId { get; set; }
        public int? LineNum { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public string SetupType { get; set; }
        public string SetupValue { get; set; }
        public decimal? Target { get; set; }
        public string CommissionType { get; set; }
        public decimal? CommissionValue { get; set; }
        //public int? Priority { get; set; }
        public string FilterBy { get; set; }
        public string Remark { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; } 
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}


 