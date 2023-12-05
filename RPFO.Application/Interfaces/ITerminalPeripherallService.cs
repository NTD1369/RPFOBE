
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface ITerminalPeripherallService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Terminal, string IsSetup ); 
        Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Terminal, string PeripheralCode);
        Task<GenericResult> Create(MTerminalPeripherals model);
        Task<GenericResult> Update(MTerminalPeripherals model);
        Task<GenericResult> Delete(MTerminalPeripherals model);
        Task<GenericResult> Apply(MTerminalPeripherals model);
    }
}
