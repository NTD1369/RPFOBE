
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
    public interface ITaxService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MTax>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code);
        Task<GenericResult> GetByUser(string User);  
        Task<GenericResult> Create(MTax model);
        Task<GenericResult> Update(MTax model);
        Task<GenericResult> Import(DataImport model);
    }
}
