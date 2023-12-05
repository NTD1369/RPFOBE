
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
    public interface IEmployeeSalesTargetSummaryService
    {
        Task<GenericResult> GetAll(string CompanyCode, string Employee, string Position, DateTime? FromDate, DateTime? ToDate, string ViewType);
        //Task<GenericResult> GetByCode(string CompanyCode, string Code);  
        Task<GenericResult> Create(TEmployeeSalesTargetSummary model);
        Task<GenericResult> CreateByList(List<TEmployeeSalesTargetSummary> models);
        Task<GenericResult> Update(TEmployeeSalesTargetSummary model);
        Task<GenericResult> Delete(TEmployeeSalesTargetSummary model);
        //Task<GenericResult> Import(DataImport model);
    }
}
