
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
    public interface IInventoryPostingService
    {
        Task<GenericResult> GetAll(string companyCode);
        Task<GenericResult> GetByStore(string companyCode, string storeId); 
        Task<GenericResult> GetInventoryList(string CompanyCode, string StoreId, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy);
        Task<PagedList<TInventoryPostingHeader>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetById(string companyCode, string storeId, string Id);
        Task<GenericResult> Create(InventoryPostingViewModel model);
        Task<GenericResult> Update(InventoryPostingViewModel model);
        Task<GenericResult> Delete(string companyCode, string Id);
    }
}
