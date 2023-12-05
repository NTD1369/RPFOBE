using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.OMSModels
{
    public class OMSResponseModel
    {
        public int Status { get; set; }
        public string Msg { get; set; }
        public string MsgVN { get; set; }
        public object Data { get; set; }

        public int? code { get; set; }
        public bool? success { get; set; }
        public string message { get; set; }

        public string token { get; set; }
    }
}
