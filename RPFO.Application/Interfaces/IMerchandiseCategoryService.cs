
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
    public interface IMerchandiseCategoryService
    {
        Task<GenericResult> GetAll(string companyCode);
        Task<GenericResult> GetByCompany(string companyCode, string mcid, string status, string keyword);
        Task<GenericResult> GetMerchandiseCategoryShow(string companyCode, string mcid, string status, string keyword);
        Task<PagedList<MMerchandiseCategory>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string companyCode, string Code); 
        Task<GenericResult> Create(MMerchandiseCategory model);
        Task<GenericResult> Update(MMerchandiseCategory model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> Import(DataImport model);
        Task<GenericResult> UpdateSetting(List<MMerchandiseCategory> model);
    }
}
