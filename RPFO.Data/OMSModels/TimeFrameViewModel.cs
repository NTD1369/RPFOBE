using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.OMSModels
{
    public class TimeFrameViewOMS
    {
        public string TransId { get; set; }
        public string OMSId { get; set; }      //  OMSId
        public string OMSLineId { get; set; }  //  OMSLineId
        public string ItemCode { get; set; }    //  ItemCode
        public string TimeFrame { get; set; }   //  TimeFrame
        //public decimal Quantiy { get; set; }    // khỏi
    }
}
