
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IPeripherallService
    {
        Task<GenericResult> GetAll(string CompanyCode ); 
        Task<GenericResult> GetByCode(string CompanyCode,   string PeripheralCode);
        Task<GenericResult> Create(MPeripherals model);
        Task<GenericResult> Update(MPeripherals model);
        Task<GenericResult> Delete(MPeripherals model); 
    }
}
