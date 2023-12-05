using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MUser
    {
        public Guid UserId { get; set; }
        public string CompanyCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? CurrentDate { get; set; }
        public decimal? ExpiredNumber { get; set; }
        public int? NotifyShowOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string LastLoginStoreId { get; set; }
        public string LastLoginStoreLang { get; set; }
        public string Status { get; set; } 
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Position { get; set; }
        public string DefaultScreen { get; set; }
        public string DefEmployee { get; set; }
        public string DefCustomer { get; set; }

        public string DefStore { get; set; }
        public string QRBarcode { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
        public string License { get; set; }

        //public string PositionName { get; set; }
    }
}
