using Newtonsoft.Json;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class TimeFrameViewModel
    {
        public string CompanyCode { get; set; }
        public string TimeFrameId { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
         
    }
   
}
