using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.Entities
{
 public  partial class LicensePlateHearder
    {
        public string TransId { get; set; }
        public string CompanyCode { get; set; }
        public string Contract { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Times { get; set; }
        public int TimesInDay { get; set; }
        public string Remark { get; set; }
        public string ItemCode { get; set; }
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
