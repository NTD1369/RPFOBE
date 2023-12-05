
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IPickupAmountService
    {
        Task<GenericResult> GetAll(string CompanyCode); 
        Task<GenericResult> GetItems(string CompanyCode, string StoreId, string DailyId, string CounterId, string ShiftId, string PickupBy, string CreatedBy, DateTime? FDate, DateTime? TDate, string Id);
        Task<GenericResult> GetPickupAmountLst(string CompanyCode, string StoreId, string DailyId, string ShiftId, string IsSales, DateTime? FDate, DateTime? TDate); 
        Task<GenericResult> Create(TPickupAmount model);
        Task<GenericResult> Update(TPickupAmount model);
        Task<GenericResult> Delete(TPickupAmount model);

        Task<GenericResult> GetItem(string CompanyCode, string StoreId, string DailyId, string CounterId, string ShiftId, string Id, string NumOfList);
    }
}
