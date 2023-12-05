
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
    public interface ICustomerService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<MCustomer>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code);
        Task<GenericResult> GetByCompany(string CompanyCode, string Type, string CustomerGrpId, string CustomerId, string Status
           , string Keyword, string CustomerName, string CustomerRank, string Address, string Phone, DateTime? DOB,decimal? Display=null);
        Task<GenericResult> Create(MCustomer model);
        Task<GenericResult> Update(MCustomer model);
        Task<GenericResult> Delete(string Code);

        Task<GenericResult> Import(DataImport model);
    }
}
