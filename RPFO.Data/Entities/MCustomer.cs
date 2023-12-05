using RPFO.Data.OMSModels;
using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MCustomer : CustomerVIGModel
    {
        public string CompanyCode { get; set; }
        public string CustomerId { get; set; }
        public string CustomerGrpId { get; set; }
        public string CustomerName { get; set; }
        //public string Address { get; set; }
        //public string Email { get; set; }
        //public string Gender { get; set; }
        public string CardNo { get; set; }
        public string Phone { get; set; }
        //public DateTime? Dob { get; set; }
        public DateTime? JoinedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string CusType { get; set; }
        public string CustomerRank { get; set; }
        public string CustomerRankName { get; set; } 
        public decimal? RewardPoints { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public Boolean? DoNotAccumPoints { get; set; }
        public string CreatedByStore { get; set; }
        public double OBAmount { get; set; }
        public double OBPoint { get; set; }
        public string OBRecal { get; set; }
    }
}
