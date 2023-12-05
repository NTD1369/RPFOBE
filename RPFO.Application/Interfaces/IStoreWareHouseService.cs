using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
   public interface IStoreWareHouseService
    {
        Task<GenericResult> GetByStoreID(string storeid);
        Task<GenericResult> Create(MStoreWarehouseModel model);
        Task<GenericResult> Update(MStoreWarehouseModel model);
        Task<GenericResult> Delete(string StoreID);
        Task<GenericResult> GetAll(string StoreID);
        Task<GenericResult> GetWhsByStore(string CompanyCode, string storeid);
    }
}
