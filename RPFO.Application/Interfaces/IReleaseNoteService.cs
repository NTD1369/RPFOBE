
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
    public interface IReleaseNoteService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<SReleaseNote>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Id, string Version); 
        Task<GenericResult> Create(SReleaseNote model);
        Task<GenericResult> Update(SReleaseNote model);
        Task<GenericResult> Delete(SReleaseNote Code);

        //Task<GenericResult> Import(DataImport model);
    }
}
