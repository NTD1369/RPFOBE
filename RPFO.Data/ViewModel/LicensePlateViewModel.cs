using System;
using System.Collections.Generic;
using System.Text;
using RPFO.Data.Entities;

namespace RPFO.Data.ViewModel
{
    public class LicensePlateViewModel : LicensePlateHearder
    {
        public LicensePlateViewModel()
        {
            Lines = new List<LicensePlateLineViewModel>();
        }
        public List<LicensePlateLineViewModel> Lines { get; set; }
    }
    public class LicensePlateLineViewModel : LicensePlateLine
    {

    }
}
