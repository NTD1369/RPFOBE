using RPFO.Data.EntitiesMWI;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.InterfacesMwi
{
    public interface IRpfoAPIService
    {
        Task<HttpResponseMessage> GetCapacitiesAsync(DateTime transDate, int? quantity, string storeId, string storeAreaId, string timeFrameId);
        Task<HttpResponseMessage> GetItemStockAsync(string storeId, string slocId, string itemCode, string uomCode, string barCode, string serialNum);
        Task<HttpResponseMessage> GetTimeFrameAsync(string companyCode, string timeframe);
        Task<HttpResponseMessage> CreateSalesOrders(TSalesViewModel sales);
        Task<HttpResponseMessage> GetPaymentMethodAsync(string paymentMode, string storeId, string status);
        Task<HttpResponseMessage> UpdateTimeFrame(IEnumerable<Data.OMSModels.TimeFrameViewOMS> timeFrames);
        Task<HttpResponseMessage> CloseOMEvent(CloseEventViewModel model);
        Task<HttpResponseMessage> GetSalesAsync(string companyCode, string storeId, string fromDate, string toDate);
        Task<HttpResponseMessage> CancelSalesOrders(string companyCode, string transId, string reason);
        Task<HttpResponseMessage> WriteLogAsync(SaleViewModel orderModel, string type, string voucherType, string status);
    }
}
