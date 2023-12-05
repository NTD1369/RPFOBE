
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
    public interface IShortcutService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<GenericResult> GetByCode(string CompanyCode, string Code);
        Task<GenericResult> GetByFunction(string CompanyCode, string FunctionCode);
        Task<GenericResult> Create(MShortcutKeyboard model);
        Task<GenericResult> Update(MShortcutKeyboard model);
        Task<GenericResult> Delete(string CompanyCode, string Code); 
           
    }
}
