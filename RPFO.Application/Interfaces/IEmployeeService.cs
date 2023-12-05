
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
    public interface IEmployeeService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MEmployee>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code);
        Task<GenericResult> GetByStore(string CompanyCode, string StoreCode, bool? CheckAvailable);
        Task<GenericResult> GetByUser(string CompanyCode, string User);
        Task<GenericResult> GetByUser(string User); 
        Task<GenericResult> Create(MEmployee model);
        Task<GenericResult> Update(MEmployee model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> Import(DataImport model);
    }
}
