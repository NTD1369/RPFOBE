
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
    public interface IStoreAreaService
    {
        Task<GenericResult> GetAll(string companyCode);
        Task<GenericResult> GetStoreAreaCapacity(string companyCode, string StoreId);
        Task<PagedList<MStoreArea>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string companyCode, string Code);
        Task<GenericResult> Create(MStoreArea model);
        Task<GenericResult> Update(MStoreArea model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> Import(DataImport model);
    }
}
