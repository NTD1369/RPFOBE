
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface ICustomerGroupService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<GenericResult> GetAllViewModel(string CompanyCode);
        Task<PagedList<MCustomerGroup>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetByCode(string CompanyCode, string Code); 
        Task<GenericResult> Create(MCustomerGroup model);
        Task<GenericResult> Update(MCustomerGroup model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> Import(DataImport model);
    }
}
