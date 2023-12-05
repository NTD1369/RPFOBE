using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RPFO.API.Helpers;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace RPFO.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public partial class SaleController : ControllerBase
    {
        public readonly string NoPaymentTable = "NoPayment";
        public readonly string PaymentTable = "Payment";

        public readonly ISaleService _saleService;
        public readonly ITableInforService _tableInforService;

        private readonly ILogger<SaleController> _logger;
        SemaphoreWaiter _semaphoreWaiter = new SemaphoreWaiter();
        public SaleController(
            ILogger<SaleController> logger,
            ISaleService saleService,
            ITableInforService tableInforService)
        {
            _logger = logger;
            _saleService = saleService;
            _tableInforService = tableInforService;
        }
        //Task<GenericResult> GetInvoiceInfor(string CompanyCode)
        [HttpGet]
        [Route("GetInvoiceInfor")]
        public async Task<GenericResult> GetInvoiceInfor(string CompanyCode, string Phone, string TaxCode)
        {
            return await _saleService.GetInvoiceInfor(CompanyCode, Phone, TaxCode);
        }
        [HttpGet]
        [Route("getNewOrderNum")]
        public async Task<string> getNewOrderNum(string companyCode, string StoreCode)
        {
            return await _saleService.GetNewOrderCode(companyCode, StoreCode);
        }       

        [HttpGet]
        [Route("GetOrderById")]
        public async Task<GenericResult> GetOrderById(string id, string companycode, string storeid)
        {
            return await _saleService.GetOrderById(id, companycode, storeid);
        }
        [HttpGet]
        [Route("GetCheckInById")]
        public async Task<GenericResult> GetCheckInById(string id, string companycode, string storeid)
        {
            return await _saleService.GetCheckInById(id, companycode, storeid);
        }       
        
        [HttpGet]
        [Route("GetCheckOutById")]
        public async Task<GenericResult> GetCheckOutById(string id, string companycode, string storeid)
        {
            return await _saleService.GetCheckOutById(id, companycode, storeid);
        }
        [HttpGet]
        [Route("GetSummaryPayment")]
        public async Task<GenericResult> GetSummaryPayment(string TransId, string EvenId, string CompanyCode, string StoreId)
        {
            return await _saleService.GetSummaryPayment(TransId, EvenId, CompanyCode, StoreId);
        }

        [HttpGet]
        [Route("GetSummaryPaymentByDate")]
        public async Task<GenericResult> GetSummaryPaymentByDate(string TransId, string EvenId, string CompanyCode, string StoreId, DateTime Date)
        {
            return await _saleService.GetSummaryPaymentByDate(TransId, EvenId, CompanyCode, StoreId, Date);
        }
        //Task<List<TSalesHeader>> GetCheckouOpentList(string companycode, string storeId, string Type, string fromdate, string todate)
        [HttpGet]
        [Route("GetByType")]

        public async Task<GenericResult> GetByType(string companycode, string storeId, string Type, string fromdate, string todate, string dataSource, string TransId, string Status, string SalesMan, string Keyword, string ViewBy, bool? includeDetail)
        {
            return await _saleService.GetByType(companycode, storeId, Type, fromdate, todate, dataSource, TransId, Status, SalesMan, Keyword, ViewBy, includeDetail);
        }
        [HttpGet]
        [Route("CheckOMSIDAlready")]

        public async Task<GenericResult> CheckOMSIDAlready(string companycode, string OMSID)
        {
            return await _saleService.CheckOMSIDAlready(companycode, OMSID);
        }
        [HttpGet]
        [Route("GetTransIdByOMSID")]
        public async Task<GenericResult> GetTransIdByOMSID(string companycode, string OMSID)
        {
            return await _saleService.GetTransIdByOMSID(companycode, OMSID);
        }

        //Task<GenericResult> CheckOMSIDAlready(string CompanyCode, string OMSID)
        [HttpGet]
        [Route("GetCheckoutOpentList")]

        public async Task<GenericResult> GetCheckoutOpentList(string companycode, string storeId, string Type, string fromdate, string todate, string ViewBy)
        {
            return await _saleService.GetCheckoutOpentList(companycode, storeId, Type, fromdate, todate, ViewBy);
        }

        //[AllowAnonymous]
        [HttpPost]
        [Route("CreateSaleOrder")]
        public async Task<GenericResult> CreateSaleOrder(SaleViewModel model)
        {
            //GenericResult result = new GenericResult();
            //using (_semaphoreWaiter.WaitForLinuxDocker())
            //{
            // Call backend service here
            //var response = await _controlService.GetAll();
            //return Ok(response);
            return await _saleService.CreateSaleOrder(model);
            //}
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("CreateSaleOrderByTableType")]
        public async Task<GenericResult> CreateSaleOrderByTableType(SaleViewModel model)
        {

            // Đối với kiểu SalesType = Table
            // Cập nhật lại Status = H, Payment = null
            // Kiểm tra order thì nó có nằm trong 1 group table không?                   

            var orderInGroupTable = await _saleService.CheckOrderInGroupTable(model);
            model.CustomF2 = orderInGroupTable ?? "";

            model.CustomF4 = model.Payments != null && model.Payments.Any() ? string.Empty : "NoPaymentOfTable";
            var result = await _saleService.CreateSaleOrderByTableType(model);
            await _saleService.UpdatePaymentForTable(model);

            SaleViewModel modelWriteResult = new SaleViewModel();
            modelWriteResult.Logs = new List<OrderLogModel>();
            OrderLogModel logItem = new OrderLogModel();

            logItem.Type = "Order";
            logItem.Action = "Create";
            logItem.Result = result.Success.ToString();
            logItem.Value = model.OrderId.HasValue ? model.OrderId.ToString() : "";
            logItem.CustomF1 = String.IsNullOrEmpty(result.Message) ? "" : result.Message;
            logItem.CustomF2 = "";
            logItem.CustomF3 = "";
            logItem.CustomF4 = "";
            logItem.CustomF5 = "";
            logItem.CustomF6 = "";
            logItem.CustomF7 = "";
            logItem.CustomF8 = "";
            logItem.CustomF9 = "";
            logItem.CustomF10 = "";
            logItem.CreatedBy = model.CreatedBy;
            logItem.Time = DateTime.Now;
            logItem.StoreId = model.StoreId;
            logItem.CompanyCode = model.CompanyCode;

            if (result.Success)
            {
                var data = result.Data as SaleViewModel;
                logItem.TransId = data.TransId.ToString();
                modelWriteResult.TransId = data.TransId.ToString();
                await _saleService.WriteFileLog(model, true);

                if (data.SalesType.ToLower() == "table" && data.IsCanceled == "N")
                {
                    await _saleService.AutoPrintSetting(model);
                }
            }
            else
            {
                var data = result.Data as SaleViewModel;
                if (data != null && !string.IsNullOrEmpty(data.TransId))
                {
                    logItem.TransId = data.TransId.ToString();
                    modelWriteResult.TransId = data.TransId.ToString();
                }
                else
                {
                    logItem.TransId = model.OrderId.ToString();
                    modelWriteResult.TransId = model.OrderId.ToString();
                }
            }

            modelWriteResult.CompanyCode = model.CompanyCode;
            modelWriteResult.StoreId = model.StoreId;
            modelWriteResult.ShiftId = model.ShiftId;
            modelWriteResult.CreatedBy = model.CreatedBy;
            modelWriteResult.TerminalId = model.TerminalId;
            modelWriteResult.Logs.Add(logItem);
            await _saleService.WriteLogRemoveBasket(modelWriteResult);

            //Force garbage collection.
            GC.Collect();

            // Wait for all finalizers to complete before continuing.
            GC.WaitForPendingFinalizers();

            return result;
        }

        [HttpPost]
        [Route("WriteLogRemoveBasket")]
        public async Task<GenericResult> WriteLogRemoveBasket(SaleViewModel model)
        {
            return await _saleService.WriteLogRemoveBasket(model);
        }
        //[HttpPost]
        //[Route("WriteFileLog")]
        //public async Task WriteFileLog(SaleViewModel model, bool? modelJson)
        //{
        //    return _saleService.WriteFileLog(model, modelJson);
        //}

        [HttpPut]
        [Route("UpdateTimeFrame")]
        public async Task<GenericResult> UpdateTimeFrame(List<TimeFrameLine> models)
        {

            //using (_semaphoreWaiter.WaitForLinuxDocker())
            //{
            // Call backend service here
            //var response = await _controlService.GetAll();
            //return Ok(response);
            return await _saleService.UpdateTimeFrame(models);
            //}
        }

        [HttpPost]
        [Route("ConfirmSO")]
        public async Task<GenericResult> ConfirmSO(SaleViewModel model)
        {
            return await _saleService.ConfirmSO(model);
        }
        [HttpPost]
        [Route("CreateMultipleOrder")]
        public async Task<GenericResult> CreateMultipleOrder(List<SaleViewModel> models)
        {
            GenericResult resultX = new GenericResult();
            List<string> OrderList = new List<string>();
            string CompanyCode = "";
            string StoreId = "";
            string TableId = "";
            string PlaceId = "";
            string RefId = "";
            try
            {
                resultX.Success = true;
                foreach (var order in models)
                {
                    order.OrderId = Guid.NewGuid();
                    if (string.IsNullOrEmpty(RefId))
                        RefId = string.IsNullOrEmpty(order.CustomF5) ? "" : order.CustomF5 == order.TransId ? order.CustomF5 : "";
                    order.TransId = "";
                    order.CustomF5 = "";
                    var result = _saleService.CreateSaleOrderByTableType(order).Result;
                    CompanyCode = order.CompanyCode;
                    StoreId = order.StoreId;
                    TableId = order.ContractNo;
                    PlaceId = order.CustomF1;
                    if (result.Success)
                    {
                        await Task.Delay(4 * 1000);
                        OrderList.Add(result.Message);
                    }
                    else
                    {
                        resultX = result;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                resultX.Success = false;
                resultX.Message = ex.Message;
            }
            if (resultX.Success == false)
            {
                foreach (string Code in OrderList)
                {
                    _saleService.Delete(CompanyCode, StoreId, Code);
                }

            }
            else
            {
                string transList = "";
                foreach (string Code in OrderList)
                {
                    transList += "'" + Code + "',";
                }
                if (!string.IsNullOrEmpty(transList))
                {
                    transList = transList.Substring(0, transList.Length - 1);
                }
                transList = "(" + transList + ")";
                if (string.IsNullOrEmpty(RefId))
                {
                    await _saleService.CancelTableExclude(CompanyCode, StoreId, TableId, PlaceId, transList);
                }
                else
                {
                    await _saleService.CancelByTransID(RefId);
                }
            }
            return resultX;
        }

        [HttpPost]
        [Route("CancelSO")]
        public async Task<GenericResult> CancelSO(SaleViewModel model)
        {
            return await _saleService.CancelSO(model);
        }

        [HttpPut]
        [Route("UpdateStatusSO")]
        public async Task<GenericResult> UpdateStatusSO(string CompanyCode, string TransId, string Status, string Reason)
        {
            return await _saleService.UpdateStatusSO(CompanyCode, TransId, Status, Reason);
            //return await _saleService.RejectSO(model);
        }
        [HttpPost]
        [Route("RejectSO")]
        public async Task<GenericResult> RejectSO(SaleViewModel model)
        {
            return await _saleService.RejectSO(model);
        }
        [HttpPost]
        [Route("AddPayment")]
        public async Task<GenericResult> AddPayment(SaleViewModel model)
        {
            return await _saleService.AddPayment(model);
        }
        [HttpGet]
        [Route("GetPagedList")]
        public async Task<IActionResult> GetPagedList([FromQuery] UserParams userParams)
        {
            var data = await _saleService.GetPagedList(userParams);
            Response.AddPagination(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
            return Ok(data);
        }

        [HttpPost]
        [Route("Import")]
        public async Task<GenericResult> Import(DataImport models)
        {
            return await _saleService.Import(models);
        }

        [HttpPost]
        [Route("CloseOMSEvent")]
        public async Task<GenericResult> CloseOMSEvent(CloseEventViewModel model)
        {
            return await _saleService.CloseOMSEvent(model);
        }

        //[AllowAnonymous]
        //[HttpPost]
        //[Route("testPrint")]
        //public async Task<GenericResult> testPrint(string htmlStr, string filename, string width, string scale)
        //{

        //    return await _saleService.testPrint(htmlStr , filename, width, scale);

        //}

        [HttpGet]
        [Route("PrintReceipt")]
        public async Task<IActionResult> PrintRecipt(string companyCode, string transId, string storeId, string printStatus, string size, string printName, bool? BreakByGroup)
        {
            GenericResult result = await _saleService.PrintReceiptAsync(companyCode, transId, storeId, printStatus, size, printName, BreakByGroup);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                if (result.Code == -1)
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            //return Ok("Version not support print from API");
        }

        [HttpPost]
        [Route("ConfirmSOPayoo")]
        public async Task<GenericResult> ConfirmSOPayoo(SaleViewModel model)
        {
            return await _saleService.ConfirmSOPayoo(model);
        }
    }
}
