using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Constants;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using RPFO.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MWI.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private string CompanyCode;
        private readonly IRpfoAPIService RpfoAPI;
        private string LogPath;
        private double RangeDate = 15;

        public SalesOrderController(ISalesService salesService, IRpfoAPIService rpfoAPIService, IConfiguration config)
        {
            this._salesService = salesService;
            this.RpfoAPI = rpfoAPIService;
            this.CompanyCode = Encryptor.DecryptString(config.GetConnectionString("CompanyCode"), AppConstants.TEXT_PHRASE);
            this.LogPath = Encryptor.DecryptString(config.GetConnectionString("ApiLogPath"), AppConstants.TEXT_PHRASE);
            string rangeDate = Encryptor.DecryptString(config.GetConnectionString("mTKOtMEJaJs="), AppConstants.TEXT_PHRASE);
            if (!string.IsNullOrEmpty(rangeDate))
            {
                double.TryParse(rangeDate, out RangeDate);
            }
        }

        //[AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetSalesAsync(string storeId, string fromDate, string toDate)
        {
            GenericResult result = new GenericResult();

            DateTime fdtEtd = DateTime.Now;
            if (!string.IsNullOrEmpty(fromDate) && !DateTime.TryParseExact(fromDate.Trim(), StringFormatConst.SQLDateParam, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fdtEtd))
            {
                result.Success = false;
                result.Message = "From Date format must be " + StringFormatConst.SQLDateParam;
                return BadRequest(result);
            }

            DateTime tdtEtd = fdtEtd.AddDays(15);
            if (!string.IsNullOrEmpty(toDate) && !DateTime.TryParseExact(toDate.Trim(), StringFormatConst.SQLDateParam, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out tdtEtd))
            {
                result.Success = false;
                result.Message = "To Date format must be " + StringFormatConst.SQLDateParam;
                return BadRequest(result);
            }

            var checkRange = (tdtEtd - fdtEtd).TotalDays;
            if (checkRange > RangeDate)
            {
                result.Success = false;
                result.Message = $"You can search transactions within {RangeDate} days";
                return BadRequest(result);
            }

            try
            {
                var response = await RpfoAPI.GetSalesAsync(this.CompanyCode, storeId, fromDate, toDate);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericResult>(responseString);
                    if (res.Success)
                    {
                        result.Success = res.Success;

                        if (res.Data != null)
                        {
                            string content = res.Data.ToString();
                            result.Data = content.JsonToModel<List<RPFO.Data.ViewModels.SaleViewModel>>();
                            //result.Data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RPFO.Data.ViewModels.SaleViewModel>>(content);
                        }

                        result.Code = (int)System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        result = res;
                    }
                }
                else
                {
                    result.Message = "Internal error.";

                    return BadRequest(result);
                }
                //if (items != null)
                //{
                //    result.Success = true;
                //    result.Data = items;
                //    if (items.Count == 0)
                //    {
                //        result.Message = "Cannot found data.";
                //    }
                //}
                //else
                //{
                //    result.Message = "Cannot found data. (Exception)";
                //}
                return Ok(result);

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [HttpGet("GetSalesText")]
        public async Task<ContentResult> GetSalesNewAsync(string storeId, string fromDate, string toDate)
        {
            GenericResult result = new GenericResult();

            DateTime fdtEtd = DateTime.Now;
            if (!string.IsNullOrEmpty(fromDate) && !DateTime.TryParseExact(fromDate.Trim(), StringFormatConst.SQLDateParam, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fdtEtd))
            {
                result.Success = false;
                result.Message = "From Date format must be " + StringFormatConst.SQLDateParam;
                return Content(result.ToJson(), "text/plain", System.Text.Encoding.UTF8);
            }

            DateTime tdtEtd = fdtEtd.AddDays(15);
            if (!string.IsNullOrEmpty(toDate) && !DateTime.TryParseExact(toDate.Trim(), StringFormatConst.SQLDateParam, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out tdtEtd))
            {
                result.Success = false;
                result.Message = "To Date format must be " + StringFormatConst.SQLDateParam;
                return Content(result.ToJson(), "text/plain", System.Text.Encoding.UTF8);
            }

            var checkRange = (tdtEtd - fdtEtd).TotalDays;
            if (checkRange > RangeDate)
            {
                result.Success = false;
                result.Message = $"You can search transactions within {RangeDate} days";
                return Content(result.ToJson(), "text/plain", System.Text.Encoding.UTF8);
            }

            try
            {
                var response = await RpfoAPI.GetSalesAsync(this.CompanyCode, storeId, fromDate, toDate);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericResult>(responseString);
                    result.Success = res.Success;
                    if (res.Success)
                    {

                        if (res.Data != null)
                        {
                            string content = res.Data.ToString();
                            result.Data = content.JsonToModel<List<RPFO.Data.ViewModels.SaleViewModel>>();
                            //result.Data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RPFO.Data.ViewModels.SaleViewModel>>(content);
                        }

                        result.Code = (int)System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        result = res;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "Internal error.";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return Content(result.ToJson(), "text/plain", System.Text.Encoding.UTF8);
        }

        //[AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateSalesAsync([FromBody] TSalesViewModel sales)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (sales == null)
                {
                    result.Message = "Data must be not null.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSO", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(sales.StoreId))
                {
                    result.Message = "StoreId be not empty.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSO", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(sales.OMSId))
                {
                    result.Message = "OMSId be not null or empty.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSO", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (!string.IsNullOrEmpty(sales.CustomF1) && sales.CustomF1.Length > 50)
                {
                    result.Message = "CustomF1 not over 50 charactor.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (!string.IsNullOrEmpty(sales.CustomF3) && sales.CustomF3.Length > 50)
                {
                    result.Message = "CustomF3 not over 50 charactor.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                foreach (var item in sales.SalesLines)
                {
                    if (item.LineSerials != null && item.LineSerials.Count > 0)
                    {
                        if (item.Quantity == null || item.Quantity == 0)
                        {
                            result.Message = $"Item {item.ItemCode} does not exist quantity.";
                            LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSO", sales.ToJson() + "\n\n" + result.ToJson());
                            return BadRequest(result);
                        }
                        else
                        {
                            if (item.Quantity.Value - item.LineSerials.Count != 0)
                            {
                                result.Message = $"{item.ItemCode}: the serial number does not match the item quantity.";
                                LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSO", sales.ToJson() + "\n\n" + result.ToJson());
                                return BadRequest(result);
                            }
                        }
                    }
                }

                //LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSO", sales.ToJson());

                //if (sales.Payments == null || sales.Payments.Count == 0)
                //{
                //    result.Message = "Payments be not empty.";
                //    return BadRequest(result);

                //    ////tạm thời chưa cần payment, cần khi có mapping
                //    //sales.Payments = new List<RPFO.Data.Entities.TSalesPayment>();
                //    //sales.Payments.Add(new RPFO.Data.Entities.TSalesPayment()
                //    //{
                //    //    CompanyCode = this.CompanyCode,
                //    //    PaymentCode = "Cash",
                //    //    TotalAmt = sales.DocTotal,
                //    //    CreatedBy = "API-MWI",
                //    //});
                //}

                if (string.IsNullOrEmpty(sales.CompanyCode))
                {
                    sales.CompanyCode = this.CompanyCode;
                }
                sales.DocType = "SO";
                sales.POSType = POSType.Retail;
                sales.DataSource = "ECOM";

                //if (sales.CustomF3 == "CLASS")
                //{
                //    sales.CustomF2 = "CO";
                //}
                //else
                //{
                //    sales.CustomF2 = "ECOM";
                //}

                //  meeting 2021-07-05
                //sales.CardCode = sales.StoreId;
                var response = await RpfoAPI.CreateSalesOrders(sales);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericResult>(responseString);
                    if (res.Success)
                    {
                        result.Success = res.Success;
                        result.Data = res.Message;
                        result.Code = (int)System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        result = res;
                    }

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSO", sales.ToJson() + "\n\n" + res.Message);
                }
                else
                {
                    result.Message = "Internal error.";

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSO", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSO", sales.ToJson() + "\n\n" + result.ToJson() + "\n\n" + "Exception: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //[AllowAnonymous]
        [HttpPost("SOSpec")]
        public async Task<IActionResult> CreateSalesSpecAsync([FromBody] TSalesViewModel sales)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (sales == null)
                {
                    result.Message = "Data must be not null.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(sales.StoreId))
                {
                    result.Message = "StoreId be not empty.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(sales.SourceChanel))
                {
                    result.Message = "SourceChanel be not empty.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(sales.ContractNo))
                {
                    result.Message = "ContractNo be not empty.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (sales.ContractNo.Length > 50)
                {
                    result.Message = "ContractNo not over 50 charactor.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (!string.IsNullOrEmpty(sales.CustomF1) && sales.CustomF1.Length > 50)
                {
                    result.Message = "CustomF1 not over 50 charactor.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                //int customeF3 = 0;
                //if (string.IsNullOrEmpty(sales.CustomF3) || (!string.IsNullOrEmpty(sales.CustomF3) && !int.TryParse(sales.CustomF3, out customeF3)))
                //{
                //    result.Message = "CustomF3 be not empty and must be an integer.";
                //    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                //    return BadRequest(result);
                //}

                if (!string.IsNullOrEmpty(sales.CustomF3) && sales.CustomF3.Length > 50)
                {
                    result.Message = "CustomF3 not over 50 charactor.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                //LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson());

                //if (sales.Payments == null || sales.Payments.Count == 0)
                //{
                //    result.Message = "Payments be not empty.";
                //    return BadRequest(result);

                //    ////tạm thời chưa cần payment, cần khi có mapping
                //    //sales.Payments = new List<RPFO.Data.Entities.TSalesPayment>();
                //    //sales.Payments.Add(new RPFO.Data.Entities.TSalesPayment()
                //    //{
                //    //    CompanyCode = this.CompanyCode,
                //    //    PaymentCode = "Cash",
                //    //    TotalAmt = sales.DocTotal,
                //    //    CreatedBy = "API-MWI",
                //    //});
                //}

                if (string.IsNullOrEmpty(sales.CompanyCode))
                {
                    sales.CompanyCode = this.CompanyCode;
                }
                sales.DocType = "SOSpec";
                sales.POSType = POSType.Event;
                sales.DataSource = "ECOM";

                //if (customeF3 >= 30)
                //{
                //    sales.CustomF2 = "EVENTCO";
                //}
                //else
                //{
                //    sales.CustomF2 = "EVENTCENTER";
                //}

                var response = await RpfoAPI.CreateSalesOrders(sales);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericResult>(responseString);
                    if (res.Success)
                    {
                        result.Success = res.Success;
                        result.Data = res.Message;
                        result.Code = (int)System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        result = res;
                    }

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + res.Message);
                }
                else
                {
                    result.Message = "Internal error.";

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                return Ok(result);

            }
            catch (Exception ex)
            {
                LogUtils.WriteLogData(LogPath, "SalesOrders", "CreateSOSpec", sales.ToJson() + "\n\n" + result.ToJson() + "\n\n" + "Exception: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //[AllowAnonymous]
        [HttpPost("Cancel")]
        public async Task<IActionResult> CancelSalesOderAsync(string transId, string remark)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (string.IsNullOrEmpty(transId))
                {
                    result.Message = "'TransId' must be not empty.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CancelSO", transId + "\n\n" + remark + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                //var check = _salesService.CancelSalesOrder(this.CompanyCode, transId, remark);
                var response = await RpfoAPI.CancelSalesOrders(this.CompanyCode, transId, remark);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericResult>(responseString);
                    if (res.Success)
                    {
                        result.Success = res.Success;
                        result.Data = res.Message;
                        result.Code = (int)System.Net.HttpStatusCode.OK;
                        result.Message = "Cancel Sales Order Completed.";
                    }
                    else
                    {
                        result = res;
                    }
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CancelSO", transId + "\n\n" + remark + "\n\n" + result.ToJson());
                }
                else
                {
                    result.Message = "Internal error.";

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CancelSO", transId + "\n\n" + remark + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                return BadRequest(result);
            }
        }

        [HttpPost("UpdateTimeFrame")]
        public async Task<IActionResult> UpdateTimeFrameAsync([FromBody] IEnumerable<RPFO.Data.OMSModels.TimeFrameViewOMS> timeFrames)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (timeFrames == null || timeFrames.Count() <= 0)
                {
                    result.Message = "Body must be not empty.";
                    return BadRequest(result);
                }

                var response = await RpfoAPI.UpdateTimeFrame(timeFrames);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericResult>(responseString);
                    if (res.Success)
                    {
                        result.Success = res.Success;
                        result.Data = res.Message;
                        result.Code = (int)System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        result = res;
                    }

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "UpdateTimeFrame", timeFrames.ToJson() + "\n\n" + res.Message);
                }
                else
                {
                    result.Message = "Internal error.";

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "UpdateTimeFrame", timeFrames.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                LogUtils.WriteLogData(LogPath, "SalesOrders", "UpdateTimeFrame", timeFrames.ToJson() + "\n" + result.ToJson() + "\n\n" + "Exception: " + ex.Message);
                return BadRequest(result);
            }
        }

        [HttpPost("CloseOMSEvent")]
        public async Task<IActionResult> CloseOMSEvent([FromBody] CloseEventViewModel model)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (model == null)
                {
                    result.Message = "Body must be not empty.";
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(model.ContractNo))
                {
                    result.Message = "ContractNo must be not empty.";
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(model.CompanyCode))
                {
                    model.CompanyCode = this.CompanyCode;
                }

                var response = await RpfoAPI.CloseOMEvent(model);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericResult>(responseString);
                    if (res.Success)
                    {
                        result.Success = res.Success;
                        result.Data = res.Message;
                        result.Code = (int)System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        result = res;
                    }

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CloseOMSEvent", model.ToJson() + "\n\n" + res.Message);
                }
                else
                {
                    result.Message = "Internal error.";

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "CloseOMSEvent", model.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = "Exception: " + ex.Message;
                LogUtils.WriteLogData(LogPath, "SalesOrders", "CloseOMSEvent", model.ToJson() + "\n" + result.ToJson() + "\n\n" + "Exception: " + ex.Message);
                return BadRequest(result);
            }
        }

        //[AllowAnonymous]
        [HttpPost("SOHold")]
        public async Task<IActionResult> HoldSalesAsync([FromBody] TSalesViewModel sales)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (sales == null)
                {
                    result.Message = "Data must be not null.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "SOHold", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(sales.StoreId))
                {
                    result.Message = "StoreId be not empty.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "SOHold", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(sales.SourceChanel))
                {
                    result.Message = "SourceChanel be not empty.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "SOHold", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                //if (string.IsNullOrEmpty(sales.ContractNo))
                //{
                //    result.Message = "ContractNo be not empty.";
                //    LogUtils.WriteLogData(LogPath, "SalesOrders", "SOHold", sales.ToJson() + "\n\n" + result.ToJson());
                //    return BadRequest(result);
                //}

                if (sales.ContractNo.Length > 50)
                {
                    result.Message = "ContractNo not over 50 charactor.";
                    LogUtils.WriteLogData(LogPath, "SalesOrders", "SOHold", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                if (string.IsNullOrEmpty(sales.CompanyCode))
                {
                    sales.CompanyCode = this.CompanyCode;
                }
                sales.DocType = "SOHold";
                sales.POSType = POSType.Retail;
                sales.DataSource = "POS";
                //sales.CustomF2 = "SÀN";

                var response = await RpfoAPI.CreateSalesOrders(sales);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    GenericResult res = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericResult>(responseString);
                    if (res.Success)
                    {
                        result.Success = res.Success;
                        result.Data = res.Message;
                        result.Code = (int)System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        result = res;
                    }

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "SOHold", sales.ToJson() + "\n\n" + res.Message);
                }
                else
                {
                    result.Message = "Internal error.";

                    LogUtils.WriteLogData(LogPath, "SalesOrders", "SOHold", sales.ToJson() + "\n\n" + result.ToJson());
                    return BadRequest(result);
                }

                return Ok(result);

            }
            catch (Exception ex)
            {
                LogUtils.WriteLogData(LogPath, "SalesOrders", "SOHold", sales.ToJson() + "\n\n" + result.ToJson() + "\n\n" + "Exception: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
