
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
    public interface ICompanyService
    {
        Task<GenericResult> GetAll(); 
        Task<GenericResult> GetByCode(string Code);
        Task<GenericResult> LogoUpdate(string CompanyCode, string Url);
        Task<GenericResult> Create(MCompany model);
        Task<GenericResult> Update(MCompany model);
        //Task<GenericResult> Import(DataImport model);
    }
}
