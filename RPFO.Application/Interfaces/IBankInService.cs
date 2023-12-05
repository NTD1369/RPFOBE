
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
    public interface IBankInService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string DailyId ); 
        Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string DailyId, string Id);
        //Task<GenericResult> LogoUpdate(string CompanyCode, string Url);
        Task<GenericResult> Create(TBankIn model);
        Task<GenericResult> Update(TBankIn model);
        Task<GenericResult> Delete(TBankIn model);
        //Task<GenericResult> Import(DataImport model);
    }
}
