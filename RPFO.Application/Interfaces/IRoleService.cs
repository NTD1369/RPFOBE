
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IRoleService
    {
        Task<GenericResult> GetAll();
        Task<PagedList<MRole>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string Code);
        
        Task<GenericResult> Create(MRole model);
        Task<GenericResult> Update(MRole model);
        Task<GenericResult> Delete(string Code);
    }
}
