
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
    public interface IPaymentMethodService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MPaymentMethod>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Code);
        Task<GenericResult> GetByStore(string CompanyCode, string StoreId); 
        Task<GenericResult> Create(MPaymentMethod model);
        Task<GenericResult> Update(MPaymentMethod model);
        Task<GenericResult> Delete(string Code);
        Task<PagedList<StorePaymentViewModel>> GetByStorePagedList(UserParams userParams);

        Task<GenericResult> Import(DataImport model);
        Task<GenericResult> GetPaymentType();
        Task<List<MPaymentMethod>> GetMPayments(string companyCode, string paymentCode, string storeId, string status);
    }
}
