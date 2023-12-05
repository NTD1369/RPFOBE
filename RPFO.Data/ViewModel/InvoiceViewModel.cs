
using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    public partial class InvoiceViewModel : TInvoiceHeader
    {
        public InvoiceViewModel()
        {
            Lines = new List<TInvoiceLineViewModel>();
            Payments = new List<TInvoicePayment>();
        }
      
        public MCustomer Customer { get; set; } 
        public List<TInvoiceLineViewModel> Lines { get; set; }
        public List<TInvoiceLineSerialViewModel> SerialLines { get; set; }
        public List<TInvoicePromoViewModel> PromoLines { get; set; }
        public List<TInvoicePayment> Payments { get; set; }  
    }
  
    public class TInvoiceLineViewModel : TInvoiceLine
    {
        public string ItemName { get; set; }
        public string UomName { get; set; }
        public List<TInvoiceLineViewModel> Lines { get; set; }
        public List<TInvoiceLineSerialViewModel> SerialLines { get; set; }
    }
    public class TInvoicePromoViewModel : TInvoicePromo
    {
        public string ItemName { get; set; }
        public string UomName { get; set; }
    }
    public class TInvoiceLineSerialViewModel : TInvoiceLineSerial
    {
        public string ItemName { get; set; }
        public string UomName { get; set; }
    }
}
