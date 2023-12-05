
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface ILicenseTypeService
    {
        Task<List<SLicenseType>> GetAll();
        Task<PagedList<SLicenseType>> GetPagedList(UserParams userParams);
        Task<SLicenseType> GetByCode(string Code);
        Task<List<SLicenseType>> GetByUser(string User); 
        Task<GenericResult> Create(SLicenseType model);
        Task<GenericResult> Update(SLicenseType model);
        Task<GenericResult> Delete(string Code);
    }
}
