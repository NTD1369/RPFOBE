using DevExpress.Printing.Utils.DocumentStoring;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.Entities;
using RPFO.Data.EntitiesMWI;
using RPFO.Data.Models;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static RPFO.Utilities.Constants.AppConstants;

namespace RPFO.Application.ImplementsMwi
{
    public class RpfoAPIService : IRpfoAPIService
    {
        private readonly IConfiguration Config;

        private string RootUrl { get; set; }
        private string CompanyCode { get; set; }
        public string AccessToken { get; set; }
        private string AuthenUserName { get; set; }
        private string AuthenPassword { get; set; }

        public RpfoAPIService(IConfiguration config)
        {
            this.Config = config;
            this.RootUrl = Utilities.Helpers.Encryptor.DecryptString(Config.GetConnectionString("RPFOHost"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            this.CompanyCode = Utilities.Helpers.Encryptor.DecryptString(Config.GetConnectionString("CompanyCode"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);

            string authenString = Utilities.Helpers.Encryptor.DecryptString(Config.GetConnectionString("RPFOAuth"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (!string.IsNullOrEmpty(authenString))
            {
                this.AuthenUserName = authenString.Split(' ').FirstOrDefault();
                this.AuthenPassword = authenString.Split(' ').LastOrDefault();
            }
        }

        private async Task<HttpClient> GetHttpClientAsync(string token = "")
        {
            var client = new HttpClient();
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

        private async Task<string> GetTokenAsync()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.RootUrl);
            client.DefaultRequestHeaders.Accept.Clear();

            string requestUri = "/api/Auth/Login";
            string contentString = "{ \"UserName\" : \"" + this.AuthenUserName + "\", \"Password\" : \"" + this.AuthenPassword + "\" }";
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            var responseString = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseString))
            {
                throw new InvalidOperationException("Could not get authorization information from " + this.RootUrl);
            }
            TokenModel tokenModel = responseString.JsonToModel<TokenModel>(); //JsonConvert.DeserializeObject<TokenModel>(responseString);
            string token;
            if (string.IsNullOrEmpty(tokenModel.AccessToken) && string.IsNullOrEmpty(tokenModel.Token))
            {
                throw new InvalidOperationException("OMS Response data: " + responseString);
            }
            else if (!string.IsNullOrEmpty(tokenModel.AccessToken))
            {
                token = tokenModel.AccessToken;
            }
            else
            {
                token = tokenModel.Token;
            }
            return token;
        }

        public async Task<HttpResponseMessage> GetCapacitiesAsync(DateTime transDate, int? quantity, string storeId, string storeAreaId, string timeFrameId)
        {
            HttpClient client = await GetHttpClientAsync();
            string requestUri = "api/Capacity/GetCapacity";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["CompanyCode"] = this.CompanyCode;
            query["TransDate"] = transDate.ToString("yyyy-MM-dd");
            if (quantity != null)
            {
                query["Quantity"] = quantity.Value.ToString();
            }
            query["StoreId"] = storeId;
            query["StoreAreaId"] = storeAreaId;
            query["TimeFrameId"] = timeFrameId;
            string param = query.ToString();
            if (!string.IsNullOrEmpty(param))
            {
                requestUri += "?" + param;
            }
            var response = await client.GetAsync(requestUri);

            return response;
        }

        public async Task<HttpResponseMessage> GetItemStockAsync(string storeId, string slocId, string itemCode, string uomCode, string barCode, string serialNum)
        {
            HttpClient client = await GetHttpClientAsync();
            string requestUri = "api/Item/ItemStock";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["companyCode"] = this.CompanyCode;
            query["storeId"] = storeId;
            query["slocId"] = slocId;
            query["itemCode"] = itemCode;
            query["uomCode"] = uomCode;
            query["barCode"] = barCode;
            query["serialNum"] = serialNum;
            string param = query.ToString();
            if (!string.IsNullOrEmpty(param))
            {
                requestUri += "?" + param;
            }
            var response = await client.GetAsync(requestUri);

            return response;
        }

        public async Task<HttpResponseMessage> GetTimeFrameAsync(string companyCode, string timeframeId)
        {
            HttpClient client = await GetHttpClientAsync();
            string requestUri = "api/TimeFrame/GetTimeFrame";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["companyCode"] = companyCode;
            query["timeframeId"] = timeframeId;
            string param = query.ToString();
            if (!string.IsNullOrEmpty(param))
            {
                requestUri += "?" + param;
            }
            var response = await client.GetAsync(requestUri);

            return response;
        }

        public async Task<HttpResponseMessage> CreateSalesOrders(TSalesViewModel sales)
        {
            //HttpClient client = await GetHttpClientAsync();
            //string requestUri = "api/Sale/CreateSaleOrder";

            Data.ViewModels.SaleViewModel saleView = new Data.ViewModels.SaleViewModel
            {
                TransId = sales.PostransId,
                CompanyCode = sales.CompanyCode,
                StoreId = sales.StoreId,
                ContractNo = sales.ContractNo,
                //StoreName = "",//
                //ShiftId = "",//
                CusId = sales.CardCode,
                //CusIdentifier = "",//
                //TotalAmount = sales.DocTotal,
                //TotalPayable = sales.DocTotal,
                TotalDiscountAmt = sales.DiscSum,
                TotalReceipt = sales.TotalReceipt,
                //AmountChange = 0,//
                PaymentDiscount = sales.PaymentDiscount,//
                TotalTax = sales.VatSum,
                DiscountType = "Discount Amount",//
                DiscountAmount = sales.DiscSum,//
                DiscountRate = sales.DiscPrcnt,
                CreatedOn = DateTime.Now,
                CreatedBy = "MWI.API",
                ModifiedOn = DateTime.Now,
                ModifiedBy = "MWI.API",
                Status = "H",//sales.DocStatus,
                IsCanceled = sales.IsCanceled,
                Remarks = sales.Remarks,
                //SalesPerson = "",//
                SalesMode = "SALES",
                //RefTransId = "",//
                //ManualDiscount = "",//
                //SalesType = "",//
                DataSource = sales.DataSource,
                POSType = sales.POSType,
                CusName = sales.CustomerName,
                Phone = sales.PhoneNo,
                CusAddress = sales.CustomerAddress,
                RefTransId = sales.EcomTransId,
                Lines = new List<Data.ViewModels.TSalesLineViewModel>(),
                SerialLines = new List<Data.ViewModels.TSalesLineSerialViewModel>(),
                Payments = sales.Payments,
                PromoLines = new List<Data.ViewModels.TSalesPromoViewModel>(),
                Invoice = new Data.Entities.TSalesInvoice()
                {
                    TransId = sales.PostransId,
                    CompanyCode = sales.CompanyCode,
                    StoreId = sales.StoreId,
                    //StoreName = "",
                    Name = sales.InvoiceFullName,
                    CustomerName = sales.CardName,
                    TaxCode = sales.InvoiceTaxCode,
                    Email = sales.InvoiceEmail,
                    Address = sales.InvoiceAddress,
                    Phone = sales.InvoicePhone,
                    //Remark=""
                },
                OMSId = sales.OMSId,
                Chanel = sales.SourceChanel,
                CustomF1 = sales.CustomF1,
                CustomF2 = sales.CustomF2,
                CustomF3 = sales.CustomF3,
            };

            decimal totalDiscountLine = 0;
            foreach (TSalesLineView line in sales.SalesLines)
            {
                if (line.LineSerials != null && line.LineSerials.Count > 0)
                {
                    decimal quantity = line.Quantity ?? 1;
                    decimal disSumAver = (line.DiscSum ?? 0) / quantity;
                    decimal lineTotalAver = (line.LineTotal ?? 0) / quantity;
                    foreach (var lineSerial in line.LineSerials)
                    {
                        Data.ViewModels.TSalesLineViewModel soLine = new Data.ViewModels.TSalesLineViewModel
                        {
                            CompanyCode = sales.CompanyCode,
                            TransId = line.PostransId,
                            LineId = line.LineId.ToString(),
                            Status = line.LineStatus,
                            ItemCode = line.ItemCode,
                            UomCode = line.UomCode,
                            BarCode = line.BarCode,
                            Quantity = 1,
                            OpenQty = 1,
                            Price = line.Price,
                            DiscountRate = line.DiscPrcnt,
                            DiscountAmt = disSumAver,
                            TaxCode = line.TaxCode,
                            TaxRate = line.TaxRate,
                            TaxAmt = line.TaxAmt,
                            Remark = line.Remark,
                            PromoId = line.PromoId,
                            PromoType = line.PromoType,
                            StoreAreaId = line.StoreAreaId,
                            //TimeFrameId = line.TimeFrameId,
                            AppointmentDate = line.AppointmentDate,
                            LineTotal = lineTotalAver,
                            Description = line.Description,
                            BaseTransId = line.EcomTransId,
                            BaseLine = line.EcomLineId,
                            MemberDate = line.MemberDate,
                            MemberValue = line.MemberValue,
                            StartDate = line.StartDate,
                            EndDate = line.EndDate,
                            BomId = line.BomId,
                            IsPromo = line.IsPromo,
                            IsSerial = line.IsSerial,
                            IsVoucher = line.IsVoucher,
                            TimeFrameId = line.TimeFrameId,
                            Duration = line.Duration,
                            SerialNum = lineSerial.Serial,
                            Phone = lineSerial.Phone,
                            Name = lineSerial.Name
                        };

                        totalDiscountLine += disSumAver;

                        saleView.Lines.Add(soLine);
                    }
                }
                else
                {
                    Data.ViewModels.TSalesLineViewModel soLine = new Data.ViewModels.TSalesLineViewModel
                    {
                        CompanyCode = sales.CompanyCode,
                        TransId = line.PostransId,
                        LineId = line.LineId.ToString(),
                        Status = line.LineStatus,
                        ItemCode = line.ItemCode,
                        UomCode = line.UomCode,
                        BarCode = line.BarCode,
                        Quantity = line.Quantity,
                        OpenQty = line.OpenQty,
                        Price = line.Price,
                        DiscountRate = line.DiscPrcnt,
                        DiscountAmt = line.DiscSum,
                        TaxCode = line.TaxCode,
                        TaxRate = line.TaxRate,
                        TaxAmt = line.TaxAmt,
                        Remark = line.Remark,
                        PromoId = line.PromoId,
                        PromoType = line.PromoType,
                        StoreAreaId = line.StoreAreaId,
                        //TimeFrameId = line.TimeFrameId,
                        AppointmentDate = line.AppointmentDate,
                        LineTotal = line.LineTotal,
                        Description = line.Description,
                        BaseTransId = line.EcomTransId,
                        BaseLine = line.EcomLineId,
                        MemberDate = line.MemberDate,
                        MemberValue = line.MemberValue,
                        StartDate = line.StartDate,
                        EndDate = line.EndDate,
                        BomId = line.BomId,
                        IsPromo = line.IsPromo,
                        IsSerial = line.IsSerial,
                        IsVoucher = line.IsVoucher,
                        TimeFrameId = line.TimeFrameId,
                        Duration = line.Duration,
                    };

                    totalDiscountLine += line.DiscSum ?? 0;
                    //if (line.TimeFrames != null && line.TimeFrames.Count > 0)
                    //{
                    //    var timeFrame = line.TimeFrames.FirstOrDefault();
                    //    soLine.TimeFrameId = timeFrame.TimeFrameId;
                    //}

                    saleView.Lines.Add(soLine);
                }
            }

            //  docTotal là giá sau khuyến mãi
            saleView.TotalPayable = sales.DocTotal;


            if (saleView.TotalDiscountAmt == null)
            {
                saleView.TotalDiscountAmt = totalDiscountLine;
            }
            else
            {
                saleView.TotalDiscountAmt += totalDiscountLine;
            }

            saleView.TotalAmount = sales.DocTotal + saleView.TotalDiscountAmt;

            //saleView.TotalPayable = saleView.TotalAmount - saleView.TotalDiscountAmt;

            saleView.Payments = sales.Payments;
            saleView.Deliveries = sales.Deliveries;

            if (saleView.Payments != null && saleView.Payments.Count > 0)
            {
                for (int i = 0; i < saleView.Payments.Count; i++)
                {
                    saleView.Payments[i].ChargableAmount = saleView.Payments[i].TotalAmt;
                    saleView.Payments[i].CollectedAmount = saleView.Payments[i].TotalAmt;
                    saleView.Payments[i].PaymentMode = "ECOM";
                }
            }

            if (saleView.Deliveries != null && saleView.Deliveries.Count > 0)
            {
                for (int i = 0; i < saleView.Deliveries.Count; i++)
                {
                    if (string.IsNullOrEmpty(saleView.Deliveries[i].CompanyCode))
                    {
                        saleView.Deliveries[i].CompanyCode = this.CompanyCode;
                    }
                }
            }

            HttpClient client = await GetHttpClientAsync();
            string requestUri = "api/Sale/CreateSaleOrder";
            var content = new StringContent(saleView.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);

            return response;
        }

        public async Task<HttpResponseMessage> GetPaymentMethodAsync(string paymentMode, string storeId, string status)
        {
            HttpClient client = await GetHttpClientAsync();
            string requestUri = "api/PaymentMethod/MwiGet";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["companyCode"] = this.CompanyCode;
            query["paymentCode"] = paymentMode;
            query["storeId"] = storeId;
            query["status"] = status;
            string param = query.ToString();
            if (!string.IsNullOrEmpty(param))
            {
                requestUri += "?" + param;
            }
            var response = await client.GetAsync(requestUri);

            return response;
        }

        public async Task<HttpResponseMessage> WriteLogAsync(SaleViewModel orderModel, string type, string voucherType, string status)
        {
            try
            {
                orderModel.Logs = new List<OrderLogModel>();

                string transId = Guid.NewGuid().ToString();

                orderModel.CompanyCode = CompanyCode;

                TSalesRedeemVoucher voucher = orderModel.Vouchers[0];

                OrderLogModel RowRequest = new OrderLogModel
                {
                    Type = type,
                    Action = "Request",
                    Result = "",
                    Value = voucher.VoucherCode,
                    CustomF1 = "",
                    CustomF2 = "",
                    CustomF3 = "",
                    CustomF4 = "",
                    CustomF5 = "",
                    CustomF6 = "",
                    CustomF7 = "",
                    CustomF8 = "",
                    CustomF9 = "",
                    CustomF10 = "",
                    CreatedBy = orderModel.CreatedBy,
                    Time = DateTime.Now,
                    StoreId = orderModel.StoreId,
                    CompanyCode = CompanyCode,
                    TerminalId = orderModel.TerminalId,
                    TransId = transId
                };


                orderModel.Logs.Add(RowRequest);

                OrderLogModel RowRedeem = new OrderLogModel
                {
                    Type = type,
                    Action = voucherType,
                    Result = status,
                    Value = voucher.VoucherCode,
                    CustomF1 = voucher.Name,
                    CustomF2 = orderModel.CusId,
                    CustomF3 = orderModel.StoreId,
                    CustomF4 = "",
                    CustomF5 = orderModel.Phone,
                    CustomF6 = orderModel.CusName,
                    CustomF7 = "",
                    CustomF8 = "",
                    CustomF9 = "",
                    CustomF10 = "",
                    CreatedBy = orderModel.CreatedBy,
                    Time = DateTime.Now,
                    StoreId = orderModel.StoreId,
                    CompanyCode = CompanyCode,
                    TransId = transId,
                    TerminalId = orderModel.TerminalId
                };

                orderModel.Logs.Add(RowRedeem);

                HttpClient client = await GetHttpClientAsync();
                string requestUri = "api/Sale/WriteLogRemoveBasket";

                var content = new StringContent(orderModel.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                return response;
            }
            catch
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.FailedDependency);
            }
        }

        public async Task<HttpResponseMessage> UpdateTimeFrame(IEnumerable<Data.OMSModels.TimeFrameViewOMS> timeFrames)
        {
            HttpClient client = await GetHttpClientAsync();
            string requestUri = "api/Sale/UpdateTimeFrame";

            var content = new StringContent(timeFrames.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);

            return response;
        }

        public async Task<HttpResponseMessage> CloseOMEvent(CloseEventViewModel model)
        {
            HttpClient client = await GetHttpClientAsync();
            string requestUri = "api/Sale/CloseOMSEvent";

            var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);

            return response;
        }

        public async Task<HttpResponseMessage> GetSalesAsync(string companyCode, string storeId, string fromDate, string toDate)
        {
            HttpClient client = await GetHttpClientAsync();
            string requestUri = "api/Sale/GetByType";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["companycode"] = companyCode;
            query["storeId"] = storeId;
            query["fromdate"] = fromDate;
            query["todate"] = toDate;
            query["includeDetail"] = "true";
            query["viewBy"] = "admin";
            string param = query.ToString();
            if (!string.IsNullOrEmpty(param))
            {
                requestUri += "?" + param;
            }
            var response = await client.GetAsync(requestUri);

            return response;
        }

        public async Task<HttpResponseMessage> CancelSalesOrders(string companyCode, string transId, string reason)
        {
            HttpClient client = await GetHttpClientAsync();
            string requestUri = "api/Sale/UpdateStatusSO";

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["CompanyCode"] = companyCode;
            query["TransId"] = transId;
            query["Status"] = "Canceled";
            query["Reason"] = reason;
            string param = query.ToString();
            if (!string.IsNullOrEmpty(param))
            {
                requestUri += "?" + param;
            }

            //var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(requestUri, null);

            return response;
        }
    }
}
