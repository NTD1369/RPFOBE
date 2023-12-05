
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
    public interface IGRPOService
    {
        Task<GenericResult> GetAll(string CompanyCode);
        Task<PagedList<TGoodsReceiptPoheader>> GetPagedList(UserParams userParams);
        Task<string> GetNewOrderCode(string companyCode, string storeId);
        Task<TGoodsReceiptPoheader> GetById(string companycode, string storeId, string Id); 
        Task<GenericResult> Create(GRPOViewModel model); 
        Task<GenericResult> Update(TGoodsReceiptPoheader model); 
        Task<GenericResult> UpdateStatus(GRPOViewModel model);
        Task<GenericResult> Delete(string companycode, string storeId, string Code);
        Task<List<TGoodsReceiptPoline>> GetLinesById(string companycode, string storeId, string Id);
        Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status);
        Task<GenericResult> GetOrderById(string Id, string CompanyCode, string StoreId);
    }
}
