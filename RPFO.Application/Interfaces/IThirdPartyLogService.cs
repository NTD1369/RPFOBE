
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
    public interface IThirdPartyLogService
    {
        Task<GenericResult> GetAll(string companyCode, string storeId); 
        Task<GenericResult> GetById(string companyCode, string storeId, string transId);
        Task<GenericResult> Create(SThirdPartyLog model);
        Task<GenericResult> Update(SThirdPartyLog model);
        //Task<GenericResult> Delete(string companyCode, string storeId, string Id);
    }
}
