using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class MMerchandiseCategory
    {
        public MMerchandiseCategory()
        {
            InverseMMerchandiseCategoryNavigation = new HashSet<MMerchandiseCategory>();
        }

        public string Mcid { get; set; }
        public string CompanyCode { get; set; }
        public string Mchier { get; set; }
        public string Mcname { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string BarcodePrefix { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public bool? isShow { get; set; }
        public int? OrderNum { get; set; }

        public virtual MMerchandiseCategory MMerchandiseCategoryNavigation { get; set; }
        public virtual ICollection<MMerchandiseCategory> InverseMMerchandiseCategoryNavigation { get; set; }
    }
}
