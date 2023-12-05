
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface ICapacityService
    {
        Task<GenericResult> GetCapacity(string CompanyCode, DateTime TransDate, int? Quantity, string StoreId, string StoreAreaId, string TimeFrameId);
        Task<GenericResult> GetCapacityByStore(string CompanyCode, DateTime TransDate, int? Quantity, string StoreId);
        Task<GenericResult> GetCapacityAreaStore(string CompanyCode, string StoreId);
        Task<GenericResult> GetCapacityByAreaStore(string CompanyCode, string StoreId, string StoreAreaId);
        Task<GenericResult> GetCapacityFromTo(string CompanyCode, DateTime FromDate, DateTime ToDate, int? Quantity, string StoreId, string StoreAreaId, string TimeFrameId);
        Task<GenericResult> Create(MStoreCapacity model);
        Task<GenericResult> Update(MStoreCapacity model);
        Task<GenericResult> Delete(MStoreCapacity Code); 
        Task<GenericResult> Import(DataImport model);
    }
}
