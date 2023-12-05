
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
    public interface IStoreCurrencyService
    {
        Task<GenericResult> GetAll(string Company, string StoreId);
        Task<GenericResult> GetByStoreWExchangeRate(string CompanyCode, string StoreId);
        Task<GenericResult> GetByCode(string Company, string StoreId, string Code); 
        Task<GenericResult> Create(MStoreCurrency model);
        Task<GenericResult> Update(MStoreCurrency model);
        Task<GenericResult> Delete(string CompanyCode, string StoreId, string Currency);


    }
}
