using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class GoodsIssueViewModel : TGoodsIssueHeader
    {
    
        public GoodsIssueViewModel()
        {
            Lines = new List<GoodIssueLineViewModel>();
        }
        public List<GoodIssueLineViewModel> Lines { get; set; }
        public string TerminalId { get; set; }


    }

    public class GoodIssueLineViewModel : TGoodsIssueLine
    {
        public GoodIssueLineViewModel()
        {
            SerialLines = new List<TGoodsIssueLineSerial>();
            Lines = new List<GoodIssueLineViewModel>();
        }
        public List<GoodIssueLineViewModel> Lines { get; set; }
        public List<TGoodsIssueLineSerial> SerialLines { get; set; }
    }
}
