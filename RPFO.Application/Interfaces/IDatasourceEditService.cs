
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IDatasourceEditService
    {
        Task<GenericResult> GetAll(string CompanyCode, string DataSource); 
        Task<GenericResult> GetByCode(string CompanyCode, string DataSource, string Code );
        Task<GenericResult> Create(SDatasourceEdit model);
        Task<GenericResult> Update(SDatasourceEdit model);
        Task<GenericResult> Delete(string CompanyCode, string Code);
      
    }
}
