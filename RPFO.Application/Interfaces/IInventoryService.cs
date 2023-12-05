
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RPFO.Data.ViewModels;

namespace RPFO.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<GenericResult> GetInventoryList(string CompanyCode, string FromStore, string ToStore, string DocType, string Status,
            DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy);
        Task<PagedList<TInventoryHeader>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetById(string companyCode, string storeId, string Id);
        Task<GenericResult> Create(InventoryViewModel model);
        Task<GenericResult> Update(InventoryViewModel model);
        Task<GenericResult> Delete(string companyCode, string storeId, string Id);
        Task<GenericResult> GetTranferNotify(string CompanyCode, string StoreId);
        Task<GenericResult> CheckitemImport(List<ItemModel> models);
        Task<GenericResult> Cancel(InventoryViewModel model);
    }
}
