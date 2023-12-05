
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IControlService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MControl>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code, string Function);
        Task<GenericResult> Create(MControl model);
        Task<GenericResult> Update(MControl model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> GetControlByFunction(string CompanyCode, string Function);
        Task<GenericResult> UpdateOrderNum(List<MControl> list);
    }
}
