
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
    public interface IExchangeRateService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Currency, DateTime? From, DateTime? To);
        Task<GenericResult> GetByCurrency(string CompanyCode, string StoreId, string CurrencyCode);
        Task<GenericResult> GetByDate(string CompanyCode, string StoreId, DateTime? Date);
        Task<GenericResult> GetExchangeRateByStore(string CompanyCode, string StoreId, string Currency);
        Task<GenericResult> GetExchangeRateIsNullByDate(string CompanyCode, string StoreId, DateTime? Date);
        Task<GenericResult> Create(MExchangeRate model);
        Task<GenericResult> Update(MExchangeRate model);
        Task<GenericResult> Delete(MExchangeRate model); 
    }
}
