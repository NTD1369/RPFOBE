
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IShiftService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<TShiftHeader>> GetPagedList(UserParams userParams);
        Task<string> GetNewShiftCode(string CompanyCode, string Id, string TerminalId);
        Task<GenericResult> GetByCode(string CompanyCode, string Code);
        //Task<List<TShiftHeader>> GetByUser(string User); 
        Task<GenericResult> Create(TShiftHeader model);
     
        Task<GenericResult> Update(EndShiftPrintViewModel model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> EndShift(TShiftHeader model);
        Task<GenericResult> LoadOpenShift(string companyCode, string storeId, string transdate,string UserId, string CounterId);
        Task<GenericResult> ShiftSummaryByDepartment(string companyCode, string storeId, string Userlogin, string FDate, string TDate, string dailyId, string shiftId);
        Task<GenericResult> GetEndShiftSummary(string companyCode, string storeId, string shiftId);
        Task<GenericResult> GetByStore(string CompanyCode, string StoreId, string top);
        //Task<GenericResult> EndDateSummary(string companyCode, string storeId, string transdate); 
        Task<GenericResult> GetOpenShiftSummary(string companyCode, string storeId, DateTime? Date);
    }
}
