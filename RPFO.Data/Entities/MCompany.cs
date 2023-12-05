using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MCompany
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string ForeignName { get; set; }
        public string ShortName { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string Phone  { get; set; }
        public string TaxCode { get; set; }
        public string Email { get; set; } 
        public string Status { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool? CheckUserStatus { get; set; }

        public string CustomerF1 { get; set; }
        public string CustomerF2 { get; set; }
        public string CustomerF3 { get; set; }
        public string CustomerF4 { get; set; }
        public string CustomerF5 { get; set; }
        public string CustomerF6 { get; set; }
        public string CustomerF7 { get; set; }
        public string CustomerF8 { get; set; }
        public string CustomerF9 { get; set; }
        public string CustomerF10 { get; set; }
         
    }
}
