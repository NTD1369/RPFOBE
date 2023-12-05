
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
    public interface IStorePaymentService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<GenericResult> GetByStore(string CompanyCode, string StoreId, string CounterId, bool? IsSetup);
        Task<PagedList<MStorePayment>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code); 
        Task<GenericResult> Create(MStorePayment model);
        Task<GenericResult> Update(MStorePayment model);
        Task<GenericResult> Delete(string StoreId, string PaymentCode);
        Task<GenericResult> Import(DataImport model);
    }
}
