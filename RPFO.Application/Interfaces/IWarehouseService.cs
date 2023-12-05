
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
    public interface IWarehouseService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MWarehouse>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code);
        Task<GenericResult> GetByStore(string CompanyCode, string StoreId);
        Task<GenericResult> GetByUser(string User);  
        Task<GenericResult> Create(MWarehouse model);
        Task<GenericResult> Update(MWarehouse model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> GetWhsType();
        Task<GenericResult> Import(DataImport model);
        Task<GenericResult> GetWarehouseByWhsType(string CompanyCode, string StoreId);
    }
}
