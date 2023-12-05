
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
    public interface IPriceListService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ItemCode);
        Task<GenericResult> GetAllId(string CompanyCode);
        Task<PagedList<MPriceList>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code); 
        Task<GenericResult> Create(MPriceList model);
        Task<GenericResult> Update(MPriceList model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> Import(DataImport model);
    }
}
