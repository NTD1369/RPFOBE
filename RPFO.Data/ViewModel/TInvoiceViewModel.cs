using RPFO.Data.EntitiesMWI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class TInvoiceViewModel : TInvoiceHeader
    {
        public List<TInvoiceLine> InvoiceLines { get; set; }
    }
}
