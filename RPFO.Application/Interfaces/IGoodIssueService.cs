
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
    public interface IGoodIssueService
    {
        Task<GenericResult> GetAll(string companyCode);
        Task<GenericResult> GetByStore(string companyCode, string storeId);
        Task<GenericResult> GetGoodsIssueList(string CompanyCode, string StoreId, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy);
        Task<PagedList<TGoodsIssueHeader>> GetPagedList(UserParams userParams);
        Task<GenericResult> GetById(string companyCode, string storeId, string Id);
        Task<GenericResult> Create(GoodsIssueViewModel model);
        Task<GenericResult> Update(GoodsIssueViewModel model);
        Task<GenericResult> Delete(string companyCode, string storeId, string Id);
    }
}
