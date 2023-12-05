
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.Models;
using RPFO.Data.OMSModels;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IBankTerminalService
    {
        Task<GenericResult> GetAll(string CompanyCode); 
        Task<GenericResult> GetByCode(string CompanyCode, string Code); 
        Task<GenericResult> Create(MBankTerminal model);
        Task<GenericResult> Update(MBankTerminal model);
        Task<GenericResult> Delete(MBankTerminal model);

        Task<GenericResult> GetByCounter(string CompanyCode, string StoreId, string CounterId);
        //Task<GenericResult> Import(DataImport model);
        //Task<(string, string)> SendPaymentToTerminalAsync(Data.OMSModels.PaymentOMS payment);

        #region Bank Terminak Device (ECR)

        TerminalDataModel SendPaymentToTerminal(string type, string bankName, string portName, double amount, string invoiceNo, int timeOut, string orderId, out string message);

        //TerminalDataModel SendPaymentToTerminalAsync(double amount, int timeOut, out string message);
        TerminalDataModel TestReadData(string type, string bankName, string portName, double amount, string invoiceNo, int timeOut, out string message);
        bool CheckConnectBankTerminal(string bankName, string portName, out string message);

        #endregion

        #region Payoo

        (string, string) AssignNewRSAKey(int keySize, bool isPem = false);
        PayooResponseModel CreateOrderPayoo(PayooDataModel requestModel, out string message);
        PayooDataModel GetOrderPayoo(string orderCode, out string message);

        #endregion
    }
}
