
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
    public interface ITimeFrameService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MTimeFrame>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string company, string code);
        Task<GenericResult> GetTimeFrame(string company, string code);  
        Task<GenericResult> Create(MTimeFrame model);
        Task<GenericResult> Update(MTimeFrame model);
        Task<GenericResult> Import(DataImport model);
    }
}
