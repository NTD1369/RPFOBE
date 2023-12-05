
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
    public interface IInventoryCountingService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<GenericResult> GetByStore(string CompanyCode, string StoreId);
        Task<PagedList<TInventoryCountingHeader>> GetPagedList(UserParams userParams);
        Task<GenericResult> InventoryCountingToCounted(string companyCode, string storeId, string id);
        Task<GenericResult> GetInventoryList(string CompanyCode, string StoreId, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword);
        Task<GenericResult> GetById(string companyCode, string storeId, string Id);
        Task<GenericResult> Create(InventoryCountingViewModel model);
        Task<GenericResult> Update(InventoryCountingViewModel model);
        Task<GenericResult> Delete(string companyCode, string Id);
        Task<GenericResult> GetInventoryCounted(string CompanyCode, string StoreId, DateTime? FrDate, DateTime? ToDate, string Keyword);
    }
}
