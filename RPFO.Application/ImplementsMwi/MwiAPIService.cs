using Dapper;
using Newtonsoft.Json;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.EntitiesMWI;
using RPFO.Data.Infrastructure;
using RPFO.Data.OMSModels;
using RPFO.Data.Models;
using RPFO.Data.Repositories;
using RPFO.Utilities.Constants;
using RPFO.Utilities.Extensions;
using RPFO.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.ImplementsMwi
{
    public class MwiAPIService : IMwiAPIService
    {
        public string AccessToken { get; set; }

        private readonly IGenericRepository<SConnectSetting> _sConnectSetting;

        private string RootUrl = string.Empty;
        private string AuthUrl = string.Empty;
        private string GrantType = string.Empty;
        private string PublicKey = string.Empty;
        private string SecretKey = string.Empty;


        public MwiAPIService(IGenericRepository<SConnectSetting> connectSetting)
        {
            this._sConnectSetting = connectSetting;

            this.AccessToken = "";
        }

        private async Task<string> GetTokenAsync()
        {
            HttpClient client = new HttpClient();
            if (string.IsNullOrEmpty(this.AuthUrl))
            {
                GetConnectSettingJum();
            }
            client.BaseAddress = new Uri(this.AuthUrl);
            client.DefaultRequestHeaders.Accept.Clear();

            string requestUri = "/connect/token";
            var parameters = new Dictionary<string, string>
            {
                { "grant_type", this.GrantType },
                { "client_id", this.PublicKey },
                { "client_secret", this.SecretKey }
            };
            var content = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync(requestUri, content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseString))
            {
                throw new InvalidOperationException("Could not get authorization information from " + this.AuthUrl);
            }
            TokenModel tokenModel = new TokenModel();
            tokenModel = JsonConvert.DeserializeObject<TokenModel>(responseString);
            if (string.IsNullOrEmpty(tokenModel.AccessToken))
            {
                throw new InvalidOperationException("OMS Response data: " + responseString);
            }
            return tokenModel.AccessToken;
        }

        private async Task<HttpClient> GetHttpClient(string token = "")
        {
            var client = new HttpClient();
            if (string.IsNullOrEmpty(this.RootUrl))
            {
                GetConnectSettingJum();
            }
            client.BaseAddress = new Uri(this.RootUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(token))
            {
                var jwthandler = new JwtSecurityTokenHandler();
                var jwttoken = jwthandler.ReadToken(token);
                var expDate = jwttoken.ValidTo;
                if (expDate < DateTime.UtcNow.AddMinutes(1))
                    AccessToken = await GetTokenAsync();
            }
            else
            {
                AccessToken = await GetTokenAsync();
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
            return client;
        }

        private void GetConnectSettingJum()
        {
            List<SConnectSetting> items = null;
            try
            {
                var parameters = new DynamicParameters();
                items = _sConnectSetting.GetAll("USP_S_S_ConnectAPI_JUM", parameters, commandType: CommandType.StoredProcedure, gConnection: GConnection.MwiConnection);
            }
            catch { }

            if (items != null)
            {
                foreach (SConnectSetting item in items)
                {
                    if (item.SettingId == ConnectSettingKey.API_JA_AUTH_URL)
                    {
                        this.AuthUrl = Encryptor.DecryptString(item.SettingValue, AppConstants.TEXT_PHRASE);
                    }
                    else if (item.SettingId == ConnectSettingKey.API_JA_MID_URL)
                    {
                        this.RootUrl = Encryptor.DecryptString(item.SettingValue, AppConstants.TEXT_PHRASE);
                    }
                    else if (item.SettingId == ConnectSettingKey.API_JA_GRANT_TYPE)
                    {
                        this.GrantType = Encryptor.DecryptString(item.SettingValue, AppConstants.TEXT_PHRASE);
                    }
                    else if (item.SettingId == ConnectSettingKey.API_JA_CLIENT_ID)
                    {
                        this.PublicKey = Encryptor.DecryptString(item.SettingValue, AppConstants.TEXT_PHRASE);
                    }
                    else if (item.SettingId == ConnectSettingKey.API_JA_CLIENT_SECRET)
                    {
                        this.SecretKey = Encryptor.DecryptString(item.SettingValue, AppConstants.TEXT_PHRASE);
                    }
                }
            }

            //this.AuthUrl = "https://auth.jumparena.vn";
            //this.RootUrl = "https://mid-api.jumparena.vn";
            //this.GrantType = "client_credentials";
            //this.PublicKey = "Abeo_id_prod";
            //this.SecretKey = "123QWE!@#ASD";

            //this.AuthUrl = "https://auth-uat.jumparena.vn:4003";
            //this.RootUrl = "https://mid-api-uat.jumparena.vn:4002";
            //this.GrantType = "client_credentials";
            //this.PublicKey = "ABEO_Test";
            //this.SecretKey = "1q2w3e*";
        }

        public async Task<HttpResponseMessage> PushOrderOMSAsync(SaleOrderOMS saleOrder)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"OMS/push_order";
            var content = new StringContent(saleOrder.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateOrderOMSAsync(OrderUpdateOMS orderUpdate)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"OMS/update_order";
            var content = new StringContent(orderUpdate.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            return response;
        }

        public async Task<HttpResponseMessage> PushSharedDataOMSAsync(SharedDataModel dataModel)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"push_data";

            var content = new StringContent(dataModel.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            return response;
        }

        public async Task<HttpResponseMessage> GetCustomerListAsync(string name, string phoneNo, string customerId, string storeCode)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"TapTap/get_list_customer";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["name"] = name;
            query["customer_id"] = customerId;
            query["phonenumber"] = phoneNo;
            query["store_code"] = storeCode;
            string param = query.ToString();
            if (!string.IsNullOrEmpty(param))
            {
                requestUri += "?" + param;
            }
            var response = await client.GetAsync(requestUri);
            return response;
        }

        public async Task<HttpResponseMessage> GetCustomerInformationAsync(string phoneNo, string storeCode)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"TapTap/get_customer?phonenumber={phoneNo}&store_code={storeCode}&customer_id=";
            var response = await client.GetAsync(requestUri);
            return response;
        }

        public async Task<HttpResponseMessage> CreateCustomerFromVIGAsync(CustomerVIGModel customerModel)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"TapTap/create_customer";
            var content = new StringContent(customerModel.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateCustomerFromVIGAsync(CustomerVIGModel customerModel)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"TapTap/update_customer?customerid={customerModel.id}";
            var content = new StringContent(customerModel.ToJsonIgnoreNull(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(requestUri, content);
            return response;
        }

        public async Task<HttpResponseMessage> GetVoucherListFromVIGAsync(string customerid, string storeCode, string page, string size)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"TapTap/get_list_voucher";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["customerid"] = customerid;
            query["store_code"] = storeCode;
            query["page"] = page;
            query["size"] = size;
            string param = query.ToString();
            if (!string.IsNullOrEmpty(param))
            {
                requestUri += "?" + param;
            }
            var response = await client.GetAsync(requestUri);
            return response;
        }

        //public async Task<HttpResponseMessage> GetTAPTAPVoucherDetailFromVIGAsync(string customerid, string voucherid, string SourceID)
        //{
        //    HttpClient client = await GetHttpClient(AccessToken);
        //    string requestUri = $"TapTap/get_detail_voucher";
        //    var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
        //    query["customerid"] = customerid;
        //    query["voucherid"] = voucherid;
        //    //query["sourceID"] = SourceID;
        //    string param = query.ToString();
        //    if (!string.IsNullOrEmpty(param))
        //    {
        //        requestUri += "?" + param;
        //    }
        //    var response = await client.GetAsync(requestUri);
        //    return response;
        //}

        public async Task<HttpResponseMessage> ValidateTapTapVoucherAsync(string customerid, string voucherid, string storeCode)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"TapTap/validate_voucher";
            //var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            //query["voucherid"] = voucherid;
            //query["customerid"] = customerid;
            //query["sourceID"] = SourceID;
            //string param = query.ToString();
            //if (!string.IsNullOrEmpty(param))
            //{
            //    requestUri += "?" + param;
            //}
            //var response = await client.GetAsync(requestUri);
            //return response;
            TapTapVoucherRequest tapTapVoucher = new TapTapVoucherRequest
            {
                customer_id = customerid,
                voucher_code = voucherid,
                store_code = storeCode
            };
            var content = new StringContent(tapTapVoucher.ToJsonIgnoreNull(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            return response;
        }

        //public async Task<HttpResponseMessage> KeepTAPTAPVoucherAsync(OrderTapTapModel voucherModel)
        //{
        //    HttpClient client = await GetHttpClient(AccessToken);
        //    string requestUri = $"create_order_taptap";
        //    var parameters = new Dictionary<string, string>
        //    {
        //        { "OrderNumber", voucherModel.OrderNumber },
        //        { "CreateDate", voucherModel.CreateDate.ToString() },
        //        { "Voucher", voucherModel.Voucher.ToJson() },
        //        { "CustomerID", voucherModel.CustomerID },
        //        { "SourceID", voucherModel.SourceID }
        //    };
        //    var content = new StringContent(parameters.ToJson(), Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync(requestUri, content);
        //    return response;
        //}

        //public async Task<HttpResponseMessage> UseTAPTAPVoucherAsync(OrderTapTapModel voucherModel)
        //{
        //    HttpClient client = await GetHttpClient(AccessToken);
        //    string requestUri = $"completed_order_taptap";
        //    var content = new StringContent(voucherModel.ToJson(), Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync(requestUri, content);
        //    return response;
        //}

        //public async Task<HttpResponseMessage> CancelTAPTAPVoucherAsync(string transactionID, string customerID)
        //{
        //    HttpClient client = await GetHttpClient(AccessToken);
        //    string requestUri = $"cancel_order_taptap";
        //    var parameters = new Dictionary<string, string>
        //    {
        //        { "TransactionID", transactionID },
        //        { "CustomerID", customerID }
        //    };
        //    var content = new StringContent(parameters.ToJson(), Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync(requestUri, content);
        //    return response;
        //}

        public async Task<HttpResponseMessage> HoldTAPTAPVoucherAsync(string customerid, string voucherid, string storeCode, string transactionId)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"TapTap/hold_voucher";
            TapTapVoucherRequest tapTapVoucher = new TapTapVoucherRequest
            {
                customer_id = customerid,
                voucher_code = voucherid,
                store_code = storeCode,
                transaction_id = transactionId
            };
            var content = new StringContent(tapTapVoucher.ToJsonIgnoreNull(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            return response;
        }

        public async Task<HttpResponseMessage> UnHoldTAPTAPVoucherAsync(string customerid, string voucherid, string storeCode, string transactionId)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"TapTap/cancel_voucher";
            TapTapVoucherRequest tapTapVoucher = new TapTapVoucherRequest
            {
                customer_id = customerid,
                voucher_code = voucherid,
                store_code = storeCode,
                transaction_id = transactionId
            };
            var content = new StringContent(tapTapVoucher.ToJsonIgnoreNull(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            return response;
        }

        public async Task<HttpResponseMessage> RedeemTAPTAPVoucherAsync(string customerid, string voucherid, string storeCode, string transactionId)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            if (string.IsNullOrEmpty(transactionId))
            {
                transactionId = Guid.NewGuid().ToString();
            }
            string requestUri = $"TapTap/redeem_voucher";
            TapTapVoucherRequest tapTapVoucher = new TapTapVoucherRequest
            {
                customer_id = customerid,
                voucher_code = voucherid,
                store_code = storeCode,
                transaction_id = transactionId
            };
            var content = new StringContent(tapTapVoucher.ToJsonIgnoreNull(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateStatusAndPaymentOrderAsync(SaleOrderOMS saleOrder)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"OMS/update_order_payments";
            var content = new StringContent(saleOrder.ToJsonIgnoreNull(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            return response;
        }

        public async Task<HttpResponseMessage> GetCityLocationAsync()
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"masterdata/location/city";
            var response = await client.GetAsync(requestUri);
            return response;
        }

        public async Task<HttpResponseMessage> GetDistrictLocationAsync(string cityId)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"masterdata/location/district?cityid={cityId}";
            var response = await client.GetAsync(requestUri);
            return response;
        }

        public async Task<HttpResponseMessage> GetMemberCardAsync(string cardId)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"OMS/checkcardID?cardid={cardId}";
            var response = await client.GetAsync(requestUri);
            return response;
        }

        public async Task<HttpResponseMessage> PushMemberCardAsync(OMSCardModel cardModel)
        {
            HttpClient client = await GetHttpClient(AccessToken);
            string requestUri = $"OMS/push_cardid";
            var content = new StringContent(cardModel.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            return response;
        }
    }
}
