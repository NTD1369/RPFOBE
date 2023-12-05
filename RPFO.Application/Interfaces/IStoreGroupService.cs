
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
    public interface IStoreGroupService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MStoreGroup>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code); 
        Task<GenericResult> Create(MStoreGroup model);
        Task<GenericResult> Update(MStoreGroup model);
        Task<GenericResult> Delete(string CompanyCode, string Code);
        Task<GenericResult> Import(DataImport model);
    }
}
