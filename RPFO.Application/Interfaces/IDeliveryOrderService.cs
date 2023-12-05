
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
    public interface IDeliveryOrderService
    { 
        Task<GenericResult> GetNewOrderCode(string companyCode, string storeId);
        Task<GenericResult> GetById(string companycode, string storeId, string Id); 
        Task<GenericResult> Create(TDeliveryHeader model);
        Task<GenericResult> CreateByDate(string CompanyCode, string Date, string CreatedBy);
        Task<GenericResult> Update(TDeliveryHeader model); 
        //Task<GenericResult> UpdateStatus(GRPOViewModel model);
        Task<GenericResult> Delete(string companycode, string storeId, string Id);
        //Task<List<TGoodsReceiptPoline>> GetLinesById(string companycode, string storeId, string Id);
        Task<GenericResult> GetByType(string companyCode, string storeId, string fromdate, string todate, string TransId,
            string DeliveryBy, string key, string status, string ViewBy);
        //Task<GenericResult> GetOrderById(string Id, string CompanyCode, string StoreId);
    }
}
