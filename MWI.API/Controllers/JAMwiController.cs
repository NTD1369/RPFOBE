using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.CodeParser;
using DevExpress.DataAccess.Native.Web;
using DevExpress.Printing.Utils.DocumentStoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MWI.API.Helpers;
using Newtonsoft.Json;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.OMSModels;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using RPFO.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RPFO.Utilities.Constants.AppConstants;

namespace MWI.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JAMwiController : ControllerBase
    {
        private IMwiAPIService _mwiAPIService;
        private string LogPath;
        private IRpfoAPIService _rpfoAPIService;
        private ISalesService _saleService;
        //IConfiguration _config;

        public JAMwiController(IMwiAPIService mwiAPIService, IRpfoAPIService rpfoAPIService, IConfiguration config, ISalesService saleService)
        {
            this._mwiAPIService = mwiAPIService;
            this._rpfoAPIService = rpfoAPIService;
            //this._config = config;
            this.LogPath = Encryptor.DecryptString(config.GetConnectionString("ApiLogPath"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            _saleService = saleService;
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpPost("PushOrder")]
        public async Task<IActionResult> PushOrderOMS([FromBody] SaleOrderOMS saleOrder)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                //LogUtils.WriteLogData(LogPath, "JAMwi", "PushOrder_" + saleOrder.OrderID, saleOrder.ToJson());

                var response = await _mwiAPIService.PushOrderOMSAsync(saleOrder);
                var responseString = await response.Content.ReadAsStringAsync();

                //LogUtils.WriteLogData(LogPath, "JAMwi", "PushOrder_" + saleOrder.OrderID, $"{saleOrder.ToJson()}\n\nResponse: {response.StatusCode}\n{responseString}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                    return Ok(result);
                }
                else
                {
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        result = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                    }
                    else
                    {
                        result.success = false;
                        result.code = (int)response.StatusCode;
                        result.Msg = "Response data does not exist content.";
                    }

                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        [OnlyABEOWebhookAttribute]
        [HttpPost("UpdateOrder")]
        public async Task<IActionResult> UpdateOrderOMS([FromBody] OrderUpdateOMS orderUpdate)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                /*
                 * {
                      "sourceID": "4",
                      "orderID": "48294884",
                      "orderStatusID": "15",
                      "paymentStatusID": "30",
                      "deliveryStatusID": "20"
                    }
                 */

                //LogUtils.WriteLogData(LogPath, "JAMwi", "PushOrder", orderUpdate.ToJson());

                var response = await _mwiAPIService.UpdateOrderOMSAsync(orderUpdate);
                var responseString = await response.Content.ReadAsStringAsync();

                //LogUtils.WriteLogData(LogPath, "JAMwi", "UpdateOrder_" + orderUpdate.OrderID, $"{orderUpdate.ToJson()}\n\nResponse: {response.StatusCode}\n{responseString}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                    return Ok(result);
                }
                else
                {
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        result = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                    }
                    else
                    {
                        result.success = false;
                        result.code = (int)response.StatusCode;
                        result.Msg = "Response data does not exist content.";
                    }

                    return BadRequest(result);
                }
                //return Ok(responseString);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        [OnlyABEOWebhookAttribute]
        [HttpPost("PushData")]
        public async Task<IActionResult> PushSharedDataOMS([FromBody] SharedDataModel dataModel)
        {
            GenericResult result = new GenericResult();
            try
            {
                var response = await _mwiAPIService.PushSharedDataOMSAsync(dataModel);
                var responseString = await response.Content.ReadAsStringAsync();
                return Ok(responseString);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [HttpGet("Customers")]
        public async Task<IActionResult> GetCustomerList(string name, string phoneNo, string customerId, string storeCode)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                var response = await _mwiAPIService.GetCustomerListAsync(name, phoneNo, customerId, storeCode);
                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                if (rsModel != null)
                {
                    if (rsModel.Status == 0 && rsModel.Msg.ToLower() == "null")
                    {
                        result.Msg = "Unable Connect To CRM System";
                        return Ok(result);
                    }
                    else
                    {
                        return Ok(responseString);
                    }
                }
                else
                {
                    result.Status = 0;
                    result.Msg = "Unable Connect To CRM System";
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpGet("CustomerInfo")]
        public async Task<IActionResult> GetCustomerInformation(string phoneNo, string storeCode)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                //  84937662362
                var response = await _mwiAPIService.GetCustomerInformationAsync(phoneNo, storeCode);
                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                if (rsModel != null)
                {
                    if (rsModel.Status == 0 && rsModel.Msg.ToLower() == "null")
                    {
                        result.success = false;
                        result.Msg = "Unable Connect To CRM System";
                        return Ok(result);
                    }
                    else
                    {
                        return Ok(responseString);
                    }
                }
                else
                {
                    result.success = false;
                    result.Status = 0;
                    result.Msg = "Unable Connect To CRM System";
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                result.Msg = "Exception: " + ex.Message;
                if (ex.InnerException != null)
                {
                    result.Msg += " InnerException: " + ex.InnerException.Message;
                }
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomerFromVIG([FromBody] CustomerVIGModel customerModel)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                /*  Data Test
                 *  {
                        "firstname": "Giang",
                        "lastname": "Nguyen",
                        "email": "test1@gmail.com",
                        "mobile": "84937662363",
                        "gender": "male",
                        "dob": "2000-09-20",
                        "createdon": "2020-09-20T10:00:00",
                        "createdby": "ja q7",
                        "registeredstore": "JAPOS",
                        "group": "Group",
                        "address": "Address",
                        "district": "district",
                        "city": "City",
                        "membertype": "member_type",
                        "description": "description",
                        "residental_type": "residental_type",
                        "acquisition_channel": "acquisition_channel",
                        "sourceofcustomers": "source_of_customers",
                        "promotiontracker": "1",
                        "referencename": "reference_name",
                        "referenceemail": "reference_email",
                        "referencemobile": "reference_mobile",
                        "waiverskill": "Waiver_Skill",
                        "socialaccount": "Social_account",
                        "familymember": [
                            {
                                "name": "Giang",
                                "email": "test1@gmail.com",
                                "mobile": "0931209944",
                                "gender": "male",
                                "dob": "2000-09-20",
                                "waiverrelationship": "waiver_relationship"
                            },
                            {
                                "name": "Giang",
                                "email": "test1@gmail.com",
                                "mobile": "0931209944",
                                "gender": "male",
                                "dob": "2000-09-20",
                                "waiverrelationship": "waiver_relationship"
                            }
                        ]
                    }
                 * 
                 */
                customerModel.created_on = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                foreach (var line in customerModel.family_member)
                {
                    //line.Dob = DateTime.UtcNow.ToString("yyyy-MM-dd");
                    line.Dob = line.Dob.Substring(0, Math.Min(10, line.Dob.Length));
                }
                var response = await _mwiAPIService.CreateCustomerFromVIGAsync(customerModel);
                var responseString = await response.Content.ReadAsStringAsync();
                //return Ok(responseString);
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                if (rsModel != null)
                {
                    if (rsModel.Status == 0 && rsModel.Msg.ToLower() == "null")
                    {
                        result.Msg = "Unable Connect To CRM System";
                        return Ok(result);
                    }
                    else
                    {
                        return Ok(responseString);
                    }
                }
                else
                {
                    result.Status = 0;
                    result.Msg = "Unable Connect To CRM System";
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpPost("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomerFromVIG([FromBody] CustomerVIGModel customerModel)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                //LogUtils.WriteLogData(LogPath, "JAMwi", "UpdateCustomer", customerModel.ToJson());

                customerModel.updated_on = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                foreach (var line in customerModel.family_member)
                {
                    //line.Dob = DateTime.UtcNow.ToString("yyyy-MM-dd");
                    line.Dob = line.Dob.Substring(0, Math.Min(10, line.Dob.Length));
                }
                var response = await _mwiAPIService.UpdateCustomerFromVIGAsync(customerModel);
                var responseString = await response.Content.ReadAsStringAsync();
                //return Ok(responseString);
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                if (rsModel != null)
                {
                    if (rsModel.Status == 0 && rsModel.Msg.ToLower() == "null")
                    {
                        result.Msg = "Unable Connect To CRM System";
                        return Ok(result);
                    }
                    else
                    {
                        return Ok(responseString);
                    }
                }
                else
                {
                    result.Status = 0;
                    result.Msg = "Unable Connect To CRM System";
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpGet("Vouchers")]
        public async Task<IActionResult> GetVoucherListFromVIG(string customerid, string storeCode, string page, string size)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                // data test
                //  0703535463
                //customerid: c0a86d9c-7641-1e06-8176-4b9d57e602d7
                //customerid: c0a89fb5-76ff-1e03-8177-05562e08002b
                var response = await _mwiAPIService.GetVoucherListFromVIGAsync(customerid, storeCode, page, size);
                var responseString = await response.Content.ReadAsStringAsync();
                return Ok(responseString);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[HttpGet("VoucherDetail")]
        //public async Task<IActionResult> GetTAPTAPVoucherDetailFromVIG(string customerid, string voucherid, string sourceID)
        //{
        //    JAMwiResponseModel result = new JAMwiResponseModel();
        //    try
        //    {
        //        //  customerid: c0a86d9c-7641-1e06-8176-4b9d57e602d7
        //        //  voucherid = 3661486487
        //        //  sourceID = 1
        //        var response = await _mwiAPIService.GetTAPTAPVoucherDetailFromVIGAsync(customerid, voucherid, sourceID);
        //        var responseString = await response.Content.ReadAsStringAsync();
        //        return Ok(responseString);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Msg = ex.Message;
        //        return BadRequest(result);
        //    }
        //}

        [OnlyABEOWebhookAttribute]
        //[AllowAnonymous]
        [HttpGet("ValidateVoucher")]
        public async Task<IActionResult> ValidateTapTapVoucher(string customerid, string voucherid, string storeCode)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                var response = await _mwiAPIService.ValidateTapTapVoucherAsync(customerid, voucherid, storeCode);
                var responseString = await response.Content.ReadAsStringAsync();

                LogUtils.WriteLogData(LogPath, "JAMwi", "ValidateVoucher_" + voucherid, $"CustomerId: {customerid}\nVoucherId: {voucherid}\nStoreCode: {storeCode}" +
                    $"\n\nResponse: {responseString}");

                return Ok(responseString);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[HttpPost("KeepVoucher")]
        //public async Task<IActionResult> KeepTAPTAPVoucher([FromBody] OrderTapTapModel voucherModel)
        //{
        //    JAMwiResponseModel result = new JAMwiResponseModel();
        //    try
        //    {
        //        /*{
        //          "orderNumber": "48294883",
        //          "createDate": "2021-01-19T10:37:51.854Z",
        //          "voucher": ["3661486487"],
        //          "customerID": "c0a86d9c-7641-1e06-8176-4b9d57e602d7",
        //          "sourceID": "1"
        //        }*/
        //        var response = await _mwiAPIService.KeepTAPTAPVoucherAsync(voucherModel);
        //        var responseString = await response.Content.ReadAsStringAsync();
        //        return Ok(responseString);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Msg = ex.Message;
        //        return BadRequest(result);
        //    }
        //}

        //[HttpPost("UseVoucher")]
        //public async Task<IActionResult> UseTAPTAPVoucher([FromBody] OrderTapTapModel voucherModel)
        //{
        //    JAMwiResponseModel result = new JAMwiResponseModel();
        //    try
        //    {
        //        var response = await _mwiAPIService.UseTAPTAPVoucherAsync(voucherModel);
        //        var responseString = await response.Content.ReadAsStringAsync();
        //        return Ok(responseString);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Msg = ex.Message;
        //        return BadRequest(result);
        //    }
        //}

        //[HttpPost("CancelVoucher")]
        //public async Task<IActionResult> CancelTAPTAPVoucher([FromBody] OrderTapTapModel voucherModel)
        //{
        //    JAMwiResponseModel result = new JAMwiResponseModel();
        //    try
        //    {
        //        //{
        //        //    "customerID": "c0a86d9c-7641-1e06-8176-4b9d57e602d7",
        //        //  "transactionID": "c0a88b25-76f5-1988-8177-1e24166e00bb"
        //        //}
        //        var response = await _mwiAPIService.CancelTAPTAPVoucherAsync(voucherModel.TransactionID, voucherModel.CustomerID);
        //        var responseString = await response.Content.ReadAsStringAsync();
        //        return Ok(responseString);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Msg = ex.Message;
        //        return BadRequest(result);
        //    }
        //}
        [OnlyABEOWebhookAttribute]
        //[AllowAnonymous]
        [HttpPost("HoldVoucher")]
        public async Task<IActionResult> HoldTAPTAPVoucher(string customerid, string voucherid, string storeCode, string transactionId)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                var response = await _mwiAPIService.HoldTAPTAPVoucherAsync(customerid, voucherid, storeCode, transactionId);
                var responseString = await response.Content.ReadAsStringAsync();
                return Ok(responseString);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpPost("UnholdVoucher")]
        public async Task<IActionResult> UnholdTAPTAPVoucher(string customerid, string voucherid, string storeCode, string transactionId)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                var response = await _mwiAPIService.UnHoldTAPTAPVoucherAsync(customerid, voucherid, storeCode, transactionId);
                var responseString = await response.Content.ReadAsStringAsync();
                return Ok(responseString);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpPost("RedeemVoucher")]
        public async Task<IActionResult> RedeemTAPTAPVoucher(string customerid, string voucherid, string storeCode, string transactionId, string createdBy, string voucherName, string cusPhone, string cusName)
        {
            if (string.IsNullOrEmpty(createdBy))
            {
                createdBy = "MWISystem";
            }
            if (string.IsNullOrEmpty(voucherName))
            {
                voucherName = "";

            }
            if (string.IsNullOrEmpty(cusPhone))
            {
                cusPhone = "";
            }
            if (string.IsNullOrEmpty(cusName))
            {
                cusName = "";
            }
            OMSResponseModel result = new OMSResponseModel();

            string VoucherRedeemType = "Redeem";
            if (string.IsNullOrEmpty(transactionId))
            {
                VoucherRedeemType = "Redeem Member";
            }
            SaleViewModel modelRpfoLog = new SaleViewModel
            {
                CompanyCode = "CP001",
                TransId = transactionId,
                StoreId = storeCode,
                ShiftId = "",
                CreatedBy = createdBy,
                TerminalId = "MWILog",
                CusId = customerid,
                Phone = cusPhone,
                CusName = cusName
            };

            RPFO.Data.Entities.TSalesRedeemVoucher voucher = new RPFO.Data.Entities.TSalesRedeemVoucher()
            {
                VoucherCode = voucherid,
                Name = voucherName
            };

            modelRpfoLog.Vouchers = new List<RPFO.Data.Entities.TSalesRedeemVoucher>
                {
                    voucher
                };

            try
            {
                var response = await _mwiAPIService.RedeemTAPTAPVoucherAsync(customerid, voucherid, storeCode, transactionId);
                var responseString = await response.Content.ReadAsStringAsync();

                LogUtils.WriteLogData(LogPath, "JAMwi", "RedeemVoucher_" + voucherid, $"CustomerId: {customerid}\nVoucherId: {voucherid}\nStoreCode: {storeCode}\nTransId: {transactionId}\nCreatedBy: {createdBy}\nVoucherName: {voucherName}\nCusPhone: {cusPhone}\nCusName: {cusName}" +
                    $"\n\nResponse: {responseString}");

                //Redeem Member
                //Redeem
                // 2023-04-18 Lập thêm ghi log về bên POS
                //System.Net.Http.HttpResponseMessage responseLog = await _rpfoAPIService.WriteLogAsync(modelRpfoLog, "RedeemVoucher", VoucherRedeemType, "Success");
                //string responseStringRpfo = await responseLog.Content.ReadAsStringAsync();

                var resLog = _saleService.RpfoWriteLog(modelRpfoLog, "RedeemVoucher", VoucherRedeemType, "Success", out string msg);
                if (!resLog && !string.IsNullOrEmpty(msg))
                {
                    LogUtils.WriteLogData(LogPath, "JAMwi", $"RedeemVoucher_{voucherid}_RpfoLog", $"ModelLog: {modelRpfoLog.ToJson()}\nStatus: Success\nMessage: {msg}");
                }

                return Ok(responseString);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;

                LogUtils.WriteLogData(LogPath, "JAMwi", "RedeemVoucher_" + voucherid, $"CustomerId: {customerid}\nVoucherId: {voucherid}\nStoreCode: {storeCode}\nTransId: {transactionId}\nCreatedBy: {createdBy}\nVoucherName: {voucherName}\nCusPhone: {cusPhone}\nCusName: {cusName}" +
                    $"\n\nResult: {result.ToJson()}");

                // 2023-04-18 Lập thêm ghi log về bên POS 
                //System.Net.Http.HttpResponseMessage responseLog = await _rpfoAPIService.WriteLogAsync(modelRpfoLog, "RedeemVoucher", VoucherRedeemType, "Failed");
                //string responseStringRpfo = await responseLog.Content.ReadAsStringAsync();
                //LogUtils.WriteLogData(LogPath, "JAMwi", $"RedeemVoucher_{voucherid}_RpfoLog", $"Request: {modelRpfoLog.ToJson()}\nResponse: {responseStringRpfo}");

                var resLog = _saleService.RpfoWriteLog(modelRpfoLog, "RedeemVoucher", VoucherRedeemType, "Failed", out string msg);
                if (!resLog && !string.IsNullOrEmpty(msg))
                {
                    LogUtils.WriteLogData(LogPath, "JAMwi", $"RedeemVoucher_{voucherid}_RpfoLog", $"ModelLog: {modelRpfoLog.ToJson()}\nStatus: Failed\nMessage: {msg}");
                }

                return BadRequest(result);
            }
        }

        ////[HttpPost("WriteLogPOS")]
        //public async Task<IActionResult> WriteRedeemLogPOS(string Type, string CompanyCode, string StoreId, string CreatedBy, string VoucherCode, string VoucherName, string CustomerId, string Result, string Message, string CusPhone, string CusName)
        //{
        //    OMSResponseModel result = new OMSResponseModel();
        //    try
        //    {
        //        SaleViewModel modelWriteResult = new SaleViewModel();
        //        modelWriteResult.Logs = new List<OrderLogModel>();

        //        string transId = Guid.NewGuid().ToString();

        //        modelWriteResult.CompanyCode = CompanyCode;
        //        modelWriteResult.StoreId = StoreId;
        //        modelWriteResult.ShiftId = "";
        //        modelWriteResult.CreatedBy = CreatedBy;
        //        modelWriteResult.TerminalId = "System";

        //        OrderLogModel RowRequest = new OrderLogModel();
        //        RowRequest.Type = Type;
        //        RowRequest.Action = "Request";
        //        RowRequest.Result = "";
        //        RowRequest.Value = VoucherCode;
        //        RowRequest.CustomF1 = "";
        //        RowRequest.CustomF2 = "";
        //        RowRequest.CustomF3 = "";
        //        RowRequest.CustomF4 = "";
        //        RowRequest.CustomF5 = "";
        //        RowRequest.CustomF6 = "";
        //        RowRequest.CustomF7 = "";
        //        RowRequest.CustomF8 = "";
        //        RowRequest.CustomF9 = "";
        //        RowRequest.CustomF10 = "";
        //        RowRequest.CreatedBy = CreatedBy;
        //        RowRequest.Time = DateTime.Now;
        //        RowRequest.StoreId = StoreId;
        //        RowRequest.CompanyCode = CompanyCode;
        //        RowRequest.TerminalId = "MWILog";
        //        RowRequest.TransId = transId;


        //        modelWriteResult.Logs.Add(RowRequest);

        //        OrderLogModel RowRedeem = new OrderLogModel();
        //        RowRedeem.Type = Type;
        //        RowRedeem.Action = "Redeem Member";
        //        RowRedeem.Result = Result;
        //        RowRedeem.Value = VoucherCode;
        //        RowRedeem.CustomF1 = VoucherName;
        //        RowRedeem.CustomF2 = CustomerId;
        //        RowRedeem.CustomF3 = StoreId;
        //        RowRedeem.CustomF4 = Message;
        //        RowRedeem.CustomF5 = CusPhone;
        //        RowRedeem.CustomF6 = CusName;
        //        RowRedeem.CustomF7 = "";
        //        RowRedeem.CustomF8 = "";
        //        RowRedeem.CustomF9 = "";
        //        RowRedeem.CustomF10 = "";
        //        RowRedeem.CreatedBy = CreatedBy;
        //        RowRedeem.Time = DateTime.Now;
        //        RowRedeem.StoreId = StoreId;
        //        RowRedeem.CompanyCode = CompanyCode;
        //        RowRedeem.TransId = transId;
        //        RowRedeem.TerminalId = "MWILog";

        //        modelWriteResult.Logs.Add(RowRedeem);

        //        var response = await _rpfoAPIService.WriteLogAsync(modelWriteResult);
        //        var responseString = await response.Content.ReadAsStringAsync();



        //        return Ok(responseString);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Msg = ex.Message;
        //        return BadRequest(result);
        //    }
        //}


        [HttpPost("UpdateOrderPayment")]
        public async Task<IActionResult> UpdateStatusAndPaymentOrder([FromBody] SaleOrderOMS orderModel)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                var response = await _mwiAPIService.UpdateStatusAndPaymentOrderAsync(orderModel);
                var responseString = await response.Content.ReadAsStringAsync();
                return Ok(responseString);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpGet("City")]
        public async Task<IActionResult> GetCityLocation()
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                var response = await _mwiAPIService.GetCityLocationAsync();
                var responseString = await response.Content.ReadAsStringAsync();
                return Ok(responseString);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpGet("District")]
        public async Task<IActionResult> GetDistrictLocation(string cityId)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                var response = await _mwiAPIService.GetDistrictLocationAsync(cityId);
                var responseString = await response.Content.ReadAsStringAsync();
                return Ok(responseString);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpGet("MemberCard")]
        public async Task<IActionResult> GetMemberCard(string cardId)
        {
            OMSResponseModel result = new();
            //if (!Helpers.ApiFactory.CheckAbeoOnly(HttpContext, _config, out System.Net.Http.HttpResponseMessage httpResponse))
            //{
            //    result.Status = (int)httpResponse.StatusCode;
            //    result.success = false;
            //    result.Msg = httpResponse.Content.ReadAsStringAsync().Result;
            //    result.message = httpResponse.Content.ReadAsStringAsync().Result;
            //    return Unauthorized(result);
            //}
            try
            {
                var response = await _mwiAPIService.GetMemberCardAsync(cardId);
                var responseString = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, responseString);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [OnlyABEOWebhookAttribute]
        [HttpPost("MemberCard")]
        public async Task<IActionResult> PushMemberCard([FromBody] OMSCardModel cardModel)
        {
            OMSResponseModel result = new();
            //if (!Helpers.ApiFactory.CheckAbeoOnly(HttpContext, _config, out System.Net.Http.HttpResponseMessage httpResponse))
            //{
            //    result.Status = (int)httpResponse.StatusCode;
            //    result.success = false;
            //    result.Msg = httpResponse.Content.ReadAsStringAsync().Result;
            //    result.message = httpResponse.Content.ReadAsStringAsync().Result;
            //    return Unauthorized(result);
            //}
            try
            {
                var response = await _mwiAPIService.PushMemberCardAsync(cardModel);
                var responseString = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);

                LogUtils.WriteLogData(LogPath, "JAMwi", "MemberCard_" + cardModel.Ordernumber, $"{cardModel.ToJson()}\n\nResponse: {response.StatusCode}\n{responseString}");

                return StatusCode((int)response.StatusCode, result);
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return BadRequest(result);
            }
        }
    }
}
