using RPFO.Data.OMSModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.InterfacesMwi
{
    public interface IMwiAPIService
    {
        Task<HttpResponseMessage> PushOrderOMSAsync(SaleOrderOMS saleOrder);
        Task<HttpResponseMessage> UpdateOrderOMSAsync(OrderUpdateOMS orderUpdate);
        Task<HttpResponseMessage> PushSharedDataOMSAsync(SharedDataModel dataModel);
        Task<HttpResponseMessage> GetCustomerListAsync(string name, string phoneNo, string customerId, string storeCode);
        Task<HttpResponseMessage> GetCustomerInformationAsync(string phoneNo, string storeCode);
        Task<HttpResponseMessage> CreateCustomerFromVIGAsync(CustomerVIGModel customerModel);
        Task<HttpResponseMessage> UpdateCustomerFromVIGAsync(CustomerVIGModel customerModel);
        Task<HttpResponseMessage> GetVoucherListFromVIGAsync(string customerid, string storeCode, string page, string size);
        //Task<HttpResponseMessage> GetTAPTAPVoucherDetailFromVIGAsync(string customerid, string voucherid, string SourceID);
        Task<HttpResponseMessage> ValidateTapTapVoucherAsync(string customerid, string voucherid, string storeCode);
        //Task<HttpResponseMessage> KeepTAPTAPVoucherAsync(OrderTapTapModel voucherModel);
        //Task<HttpResponseMessage> UseTAPTAPVoucherAsync(OrderTapTapModel voucherModel);
        //Task<HttpResponseMessage> CancelTAPTAPVoucherAsync(string transactionID, string customerID);
        Task<HttpResponseMessage> HoldTAPTAPVoucherAsync(string customerid, string voucherid, string storeCode, string transactionId);
        Task<HttpResponseMessage> UnHoldTAPTAPVoucherAsync(string customerid, string voucherid, string storeCode, string transactionId);
        Task<HttpResponseMessage> RedeemTAPTAPVoucherAsync(string customerid, string voucherid, string storeCode, string transactionId);
        Task<HttpResponseMessage> UpdateStatusAndPaymentOrderAsync(SaleOrderOMS saleOrder);
        Task<HttpResponseMessage> GetCityLocationAsync();
        Task<HttpResponseMessage> GetDistrictLocationAsync(string cityId);
        Task<HttpResponseMessage> GetMemberCardAsync(string cardId);
        Task<HttpResponseMessage> PushMemberCardAsync(OMSCardModel cardModel);
    }
}
