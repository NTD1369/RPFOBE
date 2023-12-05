using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
   public  interface ISalesChannelService
    {
        Task<GenericResult> GetAll(string CompanyCode,  string Keyword);
        Task<GenericResult> GetByCode(string CompanyCode, string Key);
        Task<GenericResult> Create(MSalesChannel model);
        Task<GenericResult> Update(MSalesChannel model);
        Task<GenericResult> Delete(MSalesChannel model);
    }
}
