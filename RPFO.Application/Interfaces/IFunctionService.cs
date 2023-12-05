
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
    public interface IFunctionService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MFunction>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code);
       
        Task<GenericResult> Create(MFunction model);
        Task<GenericResult> Update(MFunction model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> GetNodeAll(string CompanyCode);
        Task<GenericResult> GetFunctionExpandAll(string CompanyCode, string userId);
        Task<GenericResult> UpdateMenuShow(List<MFunction> model);
        Task<GenericResult> GetFunctionMenuShow(string CompanyCode);
    }
}
