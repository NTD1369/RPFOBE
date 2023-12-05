
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
    public interface IItemStorageService
    {
        Task<GenericResult> GetAll(string companyCode, string StoreId, string SlocId, string ItemCode, string Uomcode);
        Task<GenericResult> GetByCode(string companyCode, string StoreId, string SlocId, string ItemCode, string Uomcode);
        Task<GenericResult> Create(TItemStorage model);
        Task<GenericResult> Update(TItemStorage model);
        Task<GenericResult> Delete(string Code);
        Task<GenericResult> Import(DataImport model);
    }
}
