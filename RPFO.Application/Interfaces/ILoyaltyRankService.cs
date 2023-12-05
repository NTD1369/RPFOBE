
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
    public interface ILoyaltyRankService
    {
        Task<GenericResult> GetAll(string CompanyCode); 
        Task<GenericResult> GetByCode(string CompanyCode, string Code); 
        Task<GenericResult> Create(SLoyaltyRank model);
        Task<GenericResult> Update(SLoyaltyRank model);
        Task<GenericResult> Delete(string CompanyCode,string Code);
        //Task<GenericResult> Import(DataImport model);
    }
}
