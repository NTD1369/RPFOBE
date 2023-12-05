
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IFormatConfigService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<SFormatConfig>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByStore(string CompanyCode, string StoreId);
        Task<GenericResult> GetByCode(string CompanyCode, string Code); 
        Task<GenericResult> Create(SFormatConfig model);
        Task<GenericResult> Update(SFormatConfig model);
        Task<GenericResult> Delete(string Code);
    }
}
