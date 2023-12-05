
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public partial interface ISaleService
    {
        Task<List<TSalesHeader>> GetAll();
        Task<PagedList<TSalesHeader>> GetPagedList(UserParams userParams);
        Task<string> GetNewOrderCode(string CompanyCode, string StoreId);
        Task<TSalesHeader> GetById(string Id);
        Task<TSalesHeader> GetByUser(string User);
        Task<GenericResult> CreateSaleOrder(SaleViewModel model);
        Task<GenericResult> WriteLogRemoveBasket(SaleViewModel model);
        Task<GenericResult> Create(TSalesHeader model);
        Task<GenericResult> Update(SaleViewModel model);
        Task<GenericResult> Delete(string CompanyCode, string StoreId, string TransId);
        Task<GenericResult> CancelByTransID(string TransId);
        Task<List<TSalesLine>> GetLinesById(string Id);
        Task<GenericResult> UpdateStatusSO(string CompanyCode, string TransId, string Status, string Reason);
        Task<GenericResult> GetByType(string companycode, string storeId, string Type, string fromdate, string todate, string dataSource, string TransId, string Status, string SalesMan, string Keyword, string ViewBy, bool? includeDetail);
        Task<GenericResult> GetOrderById(string Id, string CompanyCode, string StoreId, bool? BreakByGroup = false);
        Task<GenericResult> GetCheckInById(string Id, string CompanyCode, string StoreId);
        Task<GenericResult> GetCheckOutById(string Id, string CompanyCode, string StoreId);
        Task<GenericResult> GetInvoiceInfor(string CompanyCode, string Phone, string TaxCode);
        Task<GenericResult> AddPayment(SaleViewModel model);
        Task<GenericResult> GetSummaryPayment(string TransId, string EventId, string CompanyCode, string StoreId);
        Task<GenericResult> GetSummaryPaymentByDate(string TransId, string EventId, string CompanyCode, string StoreId, DateTime Date);
        Task<List<TSalesHeader>> GetCheckOutList(string EventId, string CompanyCode, string StoreId, string ViewBy);
        Task<GenericResult> ConfirmSO(SaleViewModel model);
        Task<GenericResult> CancelSO(SaleViewModel model);
        Task<GenericResult> RejectSO(SaleViewModel model);
        Task<GenericResult> Import(DataImport model);
        Task<GenericResult> GetCheckoutOpentList(string companycode, string storeId, string Type, string fromdate, string todate, string ViewBy);
        Task<GenericResult> UpdateTimeFrame(List<TimeFrameLine> models);
        Task<GenericResult> CloseOMSEvent(CloseEventViewModel model);

        //Task<GenericResult> testPrint(string str, string filename, string width, string scale);

        Task<GenericResult> CheckOMSIDAlready(string CompanyCode, string OMSID);
        Task<GenericResult> GetTransIdByOMSID(string CompanyCode, string OMSID);
        Task<GenericResult> GetContractItem(string CompanyCode, string StoreId, string PlaceId, string ContractNo, string TransId, string ShiftId);
        Task<GenericResult> PrintReceiptAsync(string companyCode, string transId, string storeId, string printStatus, string size, string printName, bool? BreakByGroup);
        Task<GenericResult> ConfirmSOPayoo(SaleViewModel model);
        Task WriteFileLog(SaleViewModel model, bool? modelJson);
    }
}
