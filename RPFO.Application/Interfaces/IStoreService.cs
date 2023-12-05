
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IStoreService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MStore>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetStoreListByUser(string User);
        Task<GenericResult> GetByCode(string CompanyCode, string Code);
        Task<GenericResult> GetByUser(string User); 
        Task<GenericResult> Create(MStore model);
        Task<GenericResult> Update(MStore model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> Import(DataImport model);
        Task<GenericResult> GetStoreByUserWhsType(string User);
        Task<GenericResult> GetAllByWhstype(string CompanyCode);
    }
}
