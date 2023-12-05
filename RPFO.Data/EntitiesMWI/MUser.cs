using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.EntitiesMWI
{
    public partial class MUser
    {
        public string CompanyCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string LastLoginStoreId { get; set; }
        public string LastLoginStoreLang { get; set; }
        public string Status { get; set; }
    }
}
