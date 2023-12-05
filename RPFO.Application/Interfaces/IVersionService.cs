
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
    public interface IVersionService
    {
        Task<GenericResult> GetAll(string CompanyCode); 
        Task<GenericResult> GetByCode(string CompanyCode, string Version); 
        Task<GenericResult> Create(SVersion model);
        Task<GenericResult> Update(SVersion model);
        Task<GenericResult> Delete(SVersion Code);

        //Task<GenericResult> Import(DataImport model);
    }
}
