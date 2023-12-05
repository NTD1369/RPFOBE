using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
  public  interface IPurchaseRequestService
    {
        Task<List<TPurchaseRequestHeader>> GetAll();
        Task<PagedList<TPurchaseRequestHeader>> GetPagedList(UserParams userParams);
        Task<string> GetNewOrderCode(string companyCode, string storeId);
        Task<TPurchaseRequestHeader> GetById(string Id);
        Task<TPurchaseRequestHeader> GetByUser(string User);
        Task<GenericResult> SavePO(PurchaseRequestViewModel model);
        Task<GenericResult> UpdateStatus(PurchaseRequestViewModel model);
        Task<GenericResult> UpdateCancel(PurchaseRequestViewModel model);
        //Task<GenericResult> Update(PurchaseRequestViewModel model);
        Task<GenericResult> Delete(string Code);
        Task<List<TPurchaseRequestLine>> GetLinesById(string Id);
        Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status);
        Task<PurchaseRequestViewModel> GetOrderById(string Id, string CompanyCode, string StoreId);

        Task<string> GetLastPricePO(string companyCode, string storeId, string ItemCode, string UomCode, string Barcode);
        Task<GenericResult> GetSalesPeriod(AverageNumberSaleModel model);
    }
}
