
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
    public interface IShippingService
    {
        Task<GenericResult> GetAll(string CompanyCode, string Keyword); 
        Task<GenericResult> GetByCode(string CompanyCode, string Id); 
        Task<GenericResult> Create(MShipping model);
        Task<GenericResult> Update(MShipping model);
        Task<GenericResult> Delete(MShipping model);
        //Task<GenericResult> Import(DataImport model);
    }
}
