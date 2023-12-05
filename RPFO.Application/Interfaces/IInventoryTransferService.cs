
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
    public interface IInventoryTransferService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<GenericResult> GetInventoryList(string CompanyCode, string StoreId, string FromSloc, string ToSloc, string DocType, string Status,
            DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy);
        Task<PagedList<TInventoryTransferHeader>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetById(string companyCode, string storeId, string Id);
        Task<GenericResult> Create(InventoryTransferViewModel model);
        Task<GenericResult> Update(InventoryTransferViewModel model);
        Task<GenericResult> Delete(string companyCode, string storeId, string Id);
    }
}
