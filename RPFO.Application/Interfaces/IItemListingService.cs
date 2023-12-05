
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
    public interface IItemListingService
    {
        Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ItemCode);
        Task<GenericResult> GetItemListingStore(string CompanyCode,  string ItemCode, string UserCode);
        Task<GenericResult> Create(MItemStoreListing model);
        Task<GenericResult> Update(MItemStoreListing model);
        Task<GenericResult> Delete(MItemStoreListing model); 
        //Task<GenericResult> GetRoundingMethod();
        //Task<GenericResult> Import(DataImport model);
    }
}
