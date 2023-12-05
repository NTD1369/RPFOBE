
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
    public interface IGoodReceiptService
    {
        Task<GenericResult> GetAll(string companyCode);
        Task<GenericResult> GetByStore(string companyCode, string storeId);
        Task<GenericResult> GetGoodsReceiptList(string CompanyCode, string StoreId, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy);
        Task<PagedList<TGoodsReceiptHeader>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetById(string companyCode, string storeId, string Id);
        Task<GenericResult> Create(GoodReceiptViewModel model);
        Task<GenericResult> Update(GoodReceiptViewModel model);
        Task<GenericResult> Delete(string companyCode, string storeId, string Id);

        Task<GenericResult> Import(DataImport model);
    }
}
