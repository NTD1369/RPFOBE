
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
    public interface IGeneralSettingService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<GenericResult> GetByStore(string CompanyCode, string StoreId);
        Task<GenericResult> GetGeneralSettingByStore(string CompanyCode, string StoreId);
        Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Code, Boolean? ischeck = null); 
        Task<GenericResult> Create(SGeneralSetting model);
        Task<GenericResult> Update(SGeneralSetting model);
        Task<GenericResult> UpdateList(List<SGeneralSetting> models);
        //Task<GenericResult> Import(DataImport model);
    }
}
