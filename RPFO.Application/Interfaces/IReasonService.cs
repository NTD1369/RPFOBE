
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IReasonService
    {
        Task<GenericResult> GetAll(string CompanyCode); 
        Task<GenericResult> GetByCode(string CompanyCode, string Code );
        Task<GenericResult> Create(MReason model);
        Task<GenericResult> Update(MReason model);
        Task<GenericResult> Delete(string CompanyCode, string Code);
         
    }
}
