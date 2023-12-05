
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IShippingDivisionService
    {
        Task<GenericResult> GetAll(string CompanyCode, string FromDate, string ToDate); 
        Task<GenericResult> GetByCode(string CompanyCode, string Id);
        Task<GenericResult> Create(TShippingDivisionHeader model);
        Task<GenericResult> Update(TShippingDivisionHeader model);
        Task<GenericResult> GetDivisionToShip(string CompanyCode, string Id, string Date);
        Task<GenericResult> Delete(string Code);
      
    }
}
