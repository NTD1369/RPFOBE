
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IDevisionService
    {
        Task<GenericResult> GetAll(string CompanyCode, string FromDate, string ToDate); 
        Task<GenericResult> GetByCode(string CompanyCode, string Id);
        Task<GenericResult> Create(TDivisionHeader model);
        Task<GenericResult> CreateByList(List<TDivisionHeader> model);
        Task<GenericResult> Update(TDivisionHeader model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> GetDetailDivision(string CompanyCode, string Id);


    }
}
