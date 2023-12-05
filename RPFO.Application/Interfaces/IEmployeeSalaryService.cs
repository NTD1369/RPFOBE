
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
    public interface IEmployeeSalaryService
    {
        Task<GenericResult> GetAll(string CompanyCode, string Employee, string Id, DateTime? FromDate, DateTime? ToDate, string ViewType); 
        //Task<GenericResult> GetByCode(string CompanyCode, string Code);  
        Task<GenericResult> Create(MEmployeeSalary model);
        Task<GenericResult> Update(MEmployeeSalary model);
        Task<GenericResult> Delete(MEmployeeSalary model);
        //Task<GenericResult> Import(DataImport model);
    }
}
