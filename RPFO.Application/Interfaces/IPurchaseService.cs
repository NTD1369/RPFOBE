
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
    public interface IPurchaseService
    {
        Task<List<TPurchaseOrderHeader>> GetAll();
        Task<PagedList<TPurchaseOrderHeader>> GetPagedList(UserParams userParams);
        Task<string> GetNewOrderCode(string companyCode, string storeId);
        Task<TPurchaseOrderHeader> GetById(string Id);
        Task<TPurchaseOrderHeader> GetByUser(string User);
        Task<GenericResult> SavePO(PurchaseOrderViewModel model);
        Task<GenericResult> UpdateStatus(PurchaseOrderViewModel model);
        //Task<GenericResult> Update(PurchaseOrderViewModel model);
        Task<GenericResult> Delete(string Code);
        Task<List<TPurchaseOrderLine>> GetLinesById(string Id);
        Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status);
        Task<PurchaseOrderViewModel> GetOrderById(string Id, string CompanyCode, string StoreId);

        Task<string> GetLastPricePO(string companyCode, string storeId, string ItemCode, string UomCode, string Barcode);
    }
}
