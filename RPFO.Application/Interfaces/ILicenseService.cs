
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface ILicenseService
    {
        Task<GenericResult> GetAll(string CompanyCode); 
        Task<GenericResult> GetByLicense(string CompanyCode, string License); 
        Task<GenericResult> Create(SLicense model);
        //Task<GenericResult> Update(SLicenseType model); 
    }
}
