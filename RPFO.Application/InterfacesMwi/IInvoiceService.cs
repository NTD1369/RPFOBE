using RPFO.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Application.InterfacesMwi
{
    public interface IInvoiceService
    {
        List<TInvoiceViewModel> GetInvoices(string companyCode, string transId);
        string CreateInvoice(TInvoiceViewModel invoice, out string msg);
    }
}
