
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
    public interface IUserStoreService
    {
        Task<GenericResult> GetAll();
        Task<PagedList<MUserStore>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string Code); 
        Task<GenericResult> Create(MUserStore model);
        Task<GenericResult> Update(MUserStore model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> Import(DataImport model);
    }
}
