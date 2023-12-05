
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<List<TInvoiceHeader>> GetAll();
        Task<PagedList<TInvoiceHeader>> GetPagedList(UserParams userParams); 
        Task<string> GetNewOrderCode(string companyCode, string storeId);
        Task<TInvoiceHeader> GetById(string Id);
        Task<TInvoiceHeader> GetByUser(string User);
        Task<GenericResult> SaveImage(InvoiceViewModel model);
        Task<GenericResult> CreateInvoice(InvoiceViewModel model);
        Task<GenericResult> CreateInvoiceByTableType(InvoiceViewModel model);
        Task<GenericResult> Create(TInvoiceHeader model);
        Task<GenericResult> Update(TInvoiceHeader model);
        Task<GenericResult> Delete(string Code);
        Task<List<TInvoiceLine>> GetLinesById(string Id);
        Task<GenericResult> CheckARExistedBySoId(string SOId, string CompanyCode, string StoreId);
        Task<GenericResult> GetByType(string companycode, string storeId, string Type, string fromdate, string todate, string top);
        Task<GenericResult> GetOrderById(string Id, string CompanyCode, string StoreId);
        Task<GenericResult> GetCheckedPayment(string TransId, string EventId, string CompanyCode, string StoreId);
        Task<GenericResult> GetCheckOutList(string EventId, string CompanyCode, string StoreId);
        //Task<SaleViewModel> GetSummaryPaymentByDate(string TransId, string EventId, string CompanyCode, string StoreId, DateTime Date);

    }
}
