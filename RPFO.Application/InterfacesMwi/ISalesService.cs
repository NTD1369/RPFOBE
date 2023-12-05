using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Application.InterfacesMwi
{
    public interface ISalesService
    {
        //List<TSalesViewModel> GetSales(string companyCode, string transId);
        string CreateSales(TSalesViewModel sales, out string msg);
        bool UpdateDocStatus(string companyCode, string transId, string status);
        bool CancelSalesOrder(string companyCode, string transId, string remark);
        bool RpfoWriteLog(SaleViewModel orderModel, string type, string voucherType, string status, out string message);
    }
}
