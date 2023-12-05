
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
    public interface ISalesPlanService
    {
        Task<GenericResult> GetItems(string CompanyCode, string Id, string Name, string Keyword, DateTime? FromDate, DateTime? ToDate);
        Task<GenericResult> GetItemById(string CompanyCode, string Id);
        Task<GenericResult> Create(MSalesPlanHeader model, Boolean? isUpdate);
        Task<GenericResult> Update(MSalesPlanHeader model);
        Task<GenericResult> Delete(MSalesPlanHeader model);
        //Task<GenericResult> Import(DataImport model);
    }
}
