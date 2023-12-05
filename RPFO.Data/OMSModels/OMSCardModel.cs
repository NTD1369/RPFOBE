using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.OMSModels
{
    public class OMSCardModel
    {
        public string Ordernumber { get; set; }
        public string Source { get; set; }
        public List<Orderdetail> Orderdetails { get; set; }
    }

    public class Orderdetail
    {
        public string Itemcode { get; set; }
        public int Quantity { get; set; }
        public List<CardId> Cardids { get; set; }
    }

    public class CardId
    {
        public string Cardid { get; set; }
        public string Name { get; set; }
        public string Dob { get; set; }
        public string Phone { get; set; }
    }

}
