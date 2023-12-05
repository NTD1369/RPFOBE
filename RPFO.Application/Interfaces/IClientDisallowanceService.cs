
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
    public interface IClientDisallowanceService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Id, string CounterId, string Keyword); 
        Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Id);
        //Task<GenericResult> LogoUpdate(string CompanyCode, string Url);
        Task<GenericResult> Create(SClientDisallowance model);
        Task<GenericResult> Update(SClientDisallowance model);
        Task<GenericResult> Delete(SClientDisallowance model); 
       
    }
}
