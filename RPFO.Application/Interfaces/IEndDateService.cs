
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
    public interface IEndDateService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Top);
        Task<GenericResult> GetEndDateList(string CompanyCode, string StoreId);
         
        Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Code);
        Task<GenericResult> EndDateSummary(string companyCode, string storeId, string transdate); 
        Task<GenericResult> EndDateSummaryByDepartment(string companyCode, string storeId, string Userlogin, string FDate, string TDate, string dailyId); 
        Task<GenericResult> EndDateSummaryPaymentPrint(string companyCode, string storeId, string dailyId); 
        Task<GenericResult> GetEndDateByDate(string CompanyCode, string StoreId, string Date);
        Task<GenericResult> GetEndDateByDailyId(string CompanyCode, string StoreId, string DailyId);
        Task<GenericResult> Create(TEndDate model);
        Task<GenericResult> Update(TEndDate model);
        Task<GenericResult> CheckCOUNTER_CONNECT(string CompanyCode, string StoreId, string Date);
        //Task<GenericResult> Import(DataImport model);
    }
}
