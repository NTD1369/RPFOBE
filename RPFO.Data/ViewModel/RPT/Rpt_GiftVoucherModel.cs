using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel.RPT
{
    public class Rpt_GiftVoucherModel
    {
        public string GroupCode { get; set; }
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
        public string ItemID { get; set; } 
        public string ItemName { get; set; }
        public string BarCode { get; set; }
        public string DISTNUMBER { get; set; }
        public string SerialNum { get; set; }
        public string STA { get; set; }
        public string Status { get; set; }
        public string PosId { get; set; }

        public Nullable<DateTime> CREATEDON { get; set; }

        public Nullable<DateTime> EXPDATE { get; set; }
        public Nullable<DateTime> SaleDate { get; set; } 

    }
}
