
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
    public interface IKeyCapService
    {
        Task<GenericResult> GetAll(); 
        Task<GenericResult> GetByCode(string Id);
        //Task<GenericResult> LogoUpdate(string CompanyCode, string Url);
        Task<GenericResult> Create(MKeyCap model);
        Task<GenericResult> Update(MKeyCap model);
        Task<GenericResult> Delete(string Id);
        //Task<GenericResult> Import(DataImport model);
    }
}
