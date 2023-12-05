
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface ICurrencyRoundingOffService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId); 
        Task<GenericResult> GetById(string CompanyCode, string StoreId, string Id); 
        Task<GenericResult> Create(SCurrencyRoundingOff model);
        Task<GenericResult> Update(SCurrencyRoundingOff model);
        Task<GenericResult> Delete(SCurrencyRoundingOff model); 
 
    }
}
