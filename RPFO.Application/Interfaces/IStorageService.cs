
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
    public interface IStorageService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MStorage>> GetPagedList(UserParams userParams); 
        Task<GenericResult> GetByStore(string Store, string CompanyCode); 
        Task<GenericResult> Create(MStorage model);
        Task<GenericResult> Update(MStorage model);
        Task<GenericResult> Import(DataImport model);
    }
}
