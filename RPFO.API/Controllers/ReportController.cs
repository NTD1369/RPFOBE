using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RPFO.API.Errors;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModel.RPT;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;

namespace RPFO.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        IReportService _reportService;
        private readonly IConfiguration _config;
        private readonly ILogger<ReportController> _logger;

        public ReportController(ILogger<ReportController> logger, IConfiguration config, IReportService reportService)
        {
            _logger = logger;
            _config = config;
            _reportService = reportService;
        }


        [HttpGet]
        [Route("Get_RPT_InventoryAudit")]
        public async Task<GenericResult> Get_RPT_InventoryAudit(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_InventoryAudit(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_InventoryOnHand")]
        public async Task<GenericResult> Get_RPT_InventoryOnHand(string companyCode, string storeId, string userlogin)
        {
            var result = await _reportService.Get_RPT_InventoryOnHand(companyCode, storeId, userlogin);
            return result;
        }


        [HttpGet]
        [Route("Get_RPT_SalesStoreSummary")]
        public async Task<GenericResult> Get_RPT_SalesStoreSummary(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesStoreSummary(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_SalesTransactionDetail")]
        public async Task<GenericResult> Get_RPT_SalesTransactionDetail(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesTransactionDetail(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }
        [HttpGet]
        [Route("Get_Dash_SaleDetailTransactionByTop")]
        public async Task<GenericResult> Get_Dash_SaleDetailTransactionByTop(string CompanyCode, string StoreId, string FromDate, string ToDate, string ViewType , string ViewBy ,int? Top)
        {
            var result = await _reportService.Get_Dash_SaleDetailTransactionByTop( CompanyCode,  StoreId,  FromDate,  ToDate,  ViewType ,  ViewBy , Top);
            return result;
        }
        //string CompanyCode, string Date, string CusId, string TransId, bool? InComplete)
        [HttpGet]
        [Route("Get_RPT_SOToDivision")]
        public async Task<GenericResult> Get_RPT_SOToDivision(string CompanyCode, string Date, string CusId, string TransId, bool? InComplete)
        {
            var result = await _reportService.Get_RPT_SOToDivision(CompanyCode, Date, CusId, TransId, InComplete);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_SalesTransactionDetail_Return")]
        public async Task<GenericResult> Get_RPT_SalesTransactionDetail_Return(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesTransactionDetail_Return(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_SalesTransactionDetail_Ex")]
        public async Task<GenericResult> Get_RPT_SalesTransactionDetail_Ex(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesTransactionDetail_Ex(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_SalesTransactionSummary")]
        public async Task<GenericResult> Get_RPT_SalesTransactionSummary(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesTransactionSummary(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_SalesTopProduct")]
        public async Task<GenericResult> Get_RPT_SalesTopProduct(string companyCode, string storeId, string userlogin, string fromDate, string toDate, int? top)
        {
            var result = await _reportService.Get_RPT_SalesTopProduct(companyCode, storeId, userlogin, fromDate, toDate, top);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_SalesByHour")]
        public async Task<GenericResult> Get_RPT_SalesByHour(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesByHour(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_SalesByYear")]
        public async Task<GenericResult> Get_RPT_SalesByYear(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesByYear(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_VoucherCheckIn")]
        public async Task<GenericResult> Get_RPT_VoucherCheckIn(string companyCode, string storeId, string userlogin, string fromDate, string toDate, string keyword)
        {
            var result = await _reportService.Get_RPT_VoucherCheckIn(companyCode, storeId, userlogin, fromDate, toDate, keyword);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_SalesBySalesPerson")]
        public async Task<GenericResult> Get_RPT_SalesBySalesPerson(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesBySalesPerson(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_SalesTransactionPayment")]
        public async Task<GenericResult> Get_RPT_SalesTransactionPayment(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesTransactionPayment(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }
     
        [HttpGet]
        [Route("Get_RPT_SalesTransactionSummaryByDepartment")]
        public async Task<GenericResult> Get_RPT_SalesTransactionSummaryByDepartment(string companyCode, string storeId, string userlogin, string fromDate, string toDate, string DailyId)
        {
            var result = await _reportService.Get_RPT_SalesTransactionSummaryByDepartment(companyCode, storeId, userlogin, fromDate, toDate, DailyId);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_Dashboard")]
        public async Task<GenericResult> Get_RPT_Dashboard(string companyCode, string storeId, string userlogin, string date)
        {
            var result = await _reportService.Get_RPT_Dashboard(companyCode, storeId, userlogin, date);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_LoadChartOrderPeriodByYear")]
        public async Task<GenericResult> Get_RPT_LoadChartOrderPeriodByYear(string companyCode, string storeId, string userlogin, string year)
        {
            var result = await _reportService.Get_RPT_LoadChartOrderPeriodByYear(companyCode, storeId, userlogin, year);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_LoadChartOrderPeriodByMonth")]
        public async Task<GenericResult> Get_RPT_LoadChartOrderPeriodByMonth(string companyCode, string storeId, string userlogin, string year, string month)
        {
            var result = await _reportService.Get_RPT_LoadChartOrderPeriodByMonth(companyCode, storeId, userlogin, year, month);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_LoadChartOrderPeriodByWeek")]
        public async Task<GenericResult> Get_RPT_LoadChartOrderPeriodByWeek(string companyCode, string storeId, string userlogin)
        {
            var result = await _reportService.Get_RPT_LoadChartOrderPeriodByWeek(companyCode, storeId, userlogin);
            return result;
        }
        [HttpGet]
        [Route("Get_Rpt_GiftVoucher")]
        public async Task<GenericResult> Get_Rpt_GiftVoucher(string fromDate, string toDate, string OutletID)
        {
            var result = await _reportService.Get_Rpt_GiftVoucher(fromDate, toDate, OutletID);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_POSPromo")]
        public async Task<GenericResult> Get_RPT_POSPromo(string fromDate, string toDate, string OutletID)
        {
            var result = await _reportService.Get_RPT_POSPromo(fromDate, toDate, OutletID);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_SyncDataStatusByIdoc")]
        public async Task<GenericResult> Get_RPT_SyncDataStatusByIdoc(string CompanyCode, string IdocNum, string DataType)
        {
            var result = await _reportService.Get_RPT_SyncDataStatusByIdoc(CompanyCode, IdocNum, DataType);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_InventoryPosting")]
        public async Task<GenericResult> Get_RPT_InventoryPosting(string CompanyCode, string StoreId, string Userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_InventoryPosting(CompanyCode, StoreId, Userlogin, fromDate, toDate);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_InvoiceTransactionDetail")]
        public async Task<GenericResult> Get_RPT_InvoiceTransactionDetail(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_InvoiceTransactionDetail(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_InvoiceTransactionSummary")]
        public async Task<GenericResult> Get_RPT_InvoiceTransactionSummary(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_InvoiceTransactionSummary(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_InvoiceTransactionPayment")]
        public async Task<GenericResult> Get_RPT_InvoiceTransactionPayment(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_InvoiceTransactionPayment(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_POSPromoNew")]
        public async Task<GenericResult> Get_RPT_POSPromoNew(string companyCode, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_POSPromoNew(companyCode, fromDate, toDate);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_InventorySerial")]
        public async Task<GenericResult> Get_RPT_InventorySerial(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_InventorySerial(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }

        //[AllowAnonymous]
        [HttpGet]
        [Route("Get_RPT_SalesbyItem")]
        public async Task<GenericResult> Get_RPT_SalesbyItem(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesbyItem(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_SalesEPAYDetail")]
        public async Task<GenericResult> Get_RPT_SalesEPAYDetail(string companyCode, string storeId, string userlogin, string fromDate, string toDate)
        {
            var result = await _reportService.Get_RPT_SalesEPAYDetail(companyCode, storeId, userlogin, fromDate, toDate);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_ActionOnOrder")]
        public async Task<GenericResult> Get_RPT_ActionOnOrder(string companyCode, string storeId, string TransId, string User,  string userlogin, string fromDate, string toDate, string Type)
        {
            var result = await _reportService.Get_RPT_ActionOnOrder(companyCode, storeId, TransId,  User, userlogin, fromDate, toDate, Type);
            return result;
        }
     
        [HttpGet]
        [Route("Get_RPT_CollectionDailyByCounter")]
        public async Task<GenericResult> Get_RPT_CollectionDailyByCounter(string companyCode, string storeId,   string userlogin, string Date)
        {
            var result = await _reportService.Get_RPT_CollectionDailyByCounter(companyCode, storeId,  userlogin, Date);
            return result;
        }

        [HttpGet]
        [Route("Get_SYNC_ITEM_CMP")]
        public async Task<GenericResult> Get_SYNC_ITEM_CMP(string CompanyCode, string FItem, string TItem, DateTime? FDate, DateTime? TDate)
        {
            var result = await _reportService.RPT_SYNC_ITEM_CMP(CompanyCode, FItem, TItem,  FDate, TDate );
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_SYNC_LISTING_CMP")]
        public async Task<GenericResult> Get_RPT_SYNC_LISTING_CMP(string CompanyCode, string FItem, string TItem, DateTime? FDate, DateTime? TDate)
        {
            var result = await _reportService.RPT_SYNC_LISTING_CMP(CompanyCode, FItem, TItem, FDate, TDate);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_SYNC_PROMO_CMP")]
        public async Task<GenericResult> Get_RPT_SYNC_PROMO_CMP(string CompanyCode, string FId, string TId, DateTime? FDate, DateTime? TDate)
        {
            var result = await _reportService.RPT_SYNC_PROMO_CMP(CompanyCode, FId, TId, FDate, TDate);
            return result;
        }

        [HttpGet]
        [Route("Get_RPT_SYNC_PRICE_CMP")]
        public async Task<GenericResult> Get_RPT_SYNC_PRICE_CMP(string CompanyCode, string FItem, string TItem, DateTime? FDate, DateTime? TDate)
        {
            var result = await _reportService.RPT_SYNC_PRICE_CMP(CompanyCode, FItem, TItem,FDate, TDate);
            return result;
        }
        [HttpGet]
        [Route("Get_RPT_SyncDataStatusView")]
        public GenericResult Get_RPT_SyncDataStatusView(string CompanyCode, string StoreId, string Userlogin, string fromDate, string toDate)
        {
            var result = _reportService.Get_RPT_SyncDataStatusView(CompanyCode, StoreId, Userlogin, fromDate, toDate);
            return result;
        }

        //[AllowAnonymous]
        [HttpPost]
        [Route("Export_RPT_SalesTransactionDetail")]
        public async Task<ActionResult> Export_RPT_SalesTransactionDetail(string companyCode, string storeId, string userlogin, string fromDate, string toDate, List<HeaderModel> header)
        {
            var data = await _reportService.Get_RPT_SalesTransactionDetail(companyCode, storeId, userlogin, fromDate, toDate);
            try
            {
                List<RPT_SalesTransactionDetailModel> list = (List<RPT_SalesTransactionDetailModel>)data.Data;
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Main sheet");

                    var currentColums = 1;
                    header.ForEach((item) =>
                    {
                        var currentRow = 1;
                        worksheet.Cell(currentRow, currentColums).Value = item.Name;
                        currentRow += 1;
                        foreach (var sale in list)
                        {
                            if (item.Key.ToLower() == "CompanyCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CompanyCode;
                            if (item.Key.ToLower() == "StoreId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreId;
                            if (item.Key.ToLower() == "StoreName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreName;
                            if (item.Key.ToLower() == "SLocId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SLocId;
                            if (item.Key.ToLower() == "ContractNo".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ContractNo;
                            if (item.Key.ToLower() == "CusIdentifier".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CusIdentifier;
                            if (item.Key.ToLower() == "ShiftId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ShiftId;
                            if (item.Key.ToLower() == "CusId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CusId;
                            if (item.Key.ToLower() == "HeaderStatus".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.HeaderStatus;
                            if (item.Key.ToLower() == "IsCanceled".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.IsCanceled;
                            if (item.Key.ToLower() == "Remarks".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Remarks;
                            if (item.Key.ToLower() == "SalesPerson".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SalesPerson;
                            if (item.Key.ToLower() == "HeaderSalesMode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.HeaderSalesMode;
                            if (item.Key.ToLower() == "RefTransId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.RefTransId;
                            if (item.Key.ToLower() == "ManualDiscount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ManualDiscount;
                            if (item.Key.ToLower() == "DataSource".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DataSource;
                            if (item.Key.ToLower() == "POSType".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.POSType;
                            if (item.Key.ToLower() == "Phone".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Phone;
                            if (item.Key.ToLower() == "CusName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CusName;
                            if (item.Key.ToLower() == "CusAddress".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CusAddress;
                            if (item.Key.ToLower() == "TransId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TransId;
                            if (item.Key.ToLower() == "LineId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.LineId;
                            if (item.Key.ToLower() == "ItemCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCode;
                            if (item.Key.ToLower() == "BarCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.BarCode;
                            if (item.Key.ToLower() == "UomCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.UOMCode;
                            if (item.Key.ToLower() == "Quantity".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Quantity;
                            if (item.Key.ToLower() == "Price".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Price;
                            if (item.Key.ToLower() == "LineTotalBefDiscount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.LineTotalBefDiscount;
                            if (item.Key.ToLower() == "LineTotal".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.LineTotal;
                            if (item.Key.ToLower() == "FinalLineTotal".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.FinalLineTotal;
                            if (item.Key.ToLower() == "DiscountType".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DiscountType;
                            if (item.Key.ToLower() == "LineDiscountAmt".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.LineDiscountAmt;
                            if (item.Key.ToLower() == "DiscountAmt".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DiscountAmt;
                            if (item.Key.ToLower() == "DiscountRate".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DiscountRate;
                            if (item.Key.ToLower() == "LineDiscountRate".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.LineDiscountRate;
                            if (item.Key.ToLower() == "CreatedBy".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CreatedBy;
                            if (item.Key.ToLower() == "CreatedOn".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CreatedOn;
                            if (item.Key.ToLower() == "ModifiedBy".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ModifiedBy;
                            if (item.Key.ToLower() == "ModifiedOn".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ModifiedOn;
                            if (item.Key.ToLower() == "Status".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Status;
                            if (item.Key.ToLower() == "Remark".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Remark;
                            if (item.Key.ToLower() == "PromoId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PromoId;
                            if (item.Key.ToLower() == "PromoName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PromoName;
                            if (item.Key.ToLower() == "PromoType".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PromoType;
                            if (item.Key.ToLower() == "PromoPercent".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PromoPercent;
                            if (item.Key.ToLower() == "PromoBaseItem".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PromoBaseItem;
                            if (item.Key.ToLower() == "SalesMode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SalesMode;
                            if (item.Key.ToLower() == "TaxRate".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TaxRate;
                            if (item.Key.ToLower() == "TaxAmt".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TaxAmt;
                            if (item.Key.ToLower() == "TaxCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TaxCode;
                            if (item.Key.ToLower() == "MinDepositAmt".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.MinDepositAmt;
                            if (item.Key.ToLower() == "MinDepositPercent".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.MinDepositPercent;
                            if (item.Key.ToLower() == "DeliveryType".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DeliveryType;
                            if (item.Key.ToLower() == "POSService".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.POSService;
                            if (item.Key.ToLower() == "StoreAreaId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreAreaId;
                            if (item.Key.ToLower() == "TimeFrameId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TimeFrameId;
                            if (item.Key.ToLower() == "AppointmentDate".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.AppointmentDate;
                            if (item.Key.ToLower() == "BomID".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.BomID;
                            if (item.Key.ToLower() == "PromoPrice".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PromoPrice;
                            if (item.Key.ToLower() == "PromoLineTotal".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PromoLineTotal;
                            if (item.Key.ToLower() == "Description".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Description;
                            if (item.Key.ToLower() == "PrepaidCardNo".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PrepaidCardNo;
                            if (item.Key.ToLower() == "CustomField1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField1;
                            if (item.Key.ToLower() == "CustomField2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField2;
                            if (item.Key.ToLower() == "CustomField3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField3;
                            if (item.Key.ToLower() == "CustomField4".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField4;
                            if (item.Key.ToLower() == "CustomField5".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField5;
                            if (item.Key.ToLower() == "CustomField6.ToLower()")
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField6;
                            if (item.Key.ToLower() == "CustomField7".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField7;
                            if (item.Key.ToLower() == "CustomField8".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField8;
                            if (item.Key.ToLower() == "CustomField9".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField9;
                            if (item.Key.ToLower() == "CustomField10".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField10;
                            if (item.Key.ToLower() == "DefaultPrice".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DefaultPrice;
                            if (item.Key.ToLower() == "CusGrpId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CusGrpId;
                            if (item.Key.ToLower() == "PriceListId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PriceListId;
                            if (item.Key.ToLower() == "ItemGroupName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemGroupName;
                            if (item.Key.ToLower() == "ItemCategory1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory1;
                            if (item.Key.ToLower() == "ItemCategory2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory2;
                            if (item.Key.ToLower() == "ItemCategory3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory3;
                            if (item.Key.ToLower() == "Reason".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Reason;
                            if (item.Key.ToLower() == "CounterId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CounterId;
                            if (item.Key.ToLower() == "CusGrpDesc".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CusGrpDesc;
                            if (item.Key.ToLower() == "ItemGroupId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemGroupId;
                            if (item.Key.ToLower() == "SyncMWIStatus".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SyncMWIStatus;
                            if (item.Key.ToLower() == "OrgQuantity".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.OrgQuantity;
                            if (item.Key.ToLower() == "OMSId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.OMSId;
                            if (item.Key.ToLower() == "SyncMWIDate".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SyncMWIDate;
                            if (item.Key.ToLower() == "HeaderDiscount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.HeaderDiscount;
                            if (item.Key.ToLower() == "HeaderDiscountAmt".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.HeaderDiscountAmt;
                            if (item.Key.ToLower() == "SaleCatergoryID".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCatergoryID;
                            if (item.Key.ToLower() == "SaleCategoryID".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryID;
                            if (item.Key.ToLower() == "SaleCategoryID1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryID1;
                            if (item.Key.ToLower() == "SaleCategoryID2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryID2;
                            if (item.Key.ToLower() == "SaleCategoryID3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryID3;
                            if (item.Key.ToLower() == "SaleCategoryID4".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryID4;
                            if (item.Key.ToLower() == "SaleCategoryID5".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryID5;
                            if (item.Key.ToLower() == "SaleCaterogyName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCaterogyName;
                            if (item.Key.ToLower() == "SaleCategoryName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryName;
                            if (item.Key.ToLower() == "SaleCategoryName1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryName1;
                            if (item.Key.ToLower() == "SaleCategoryName2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryName2;
                            if (item.Key.ToLower() == "SaleCategoryName3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryName3;
                            if (item.Key.ToLower() == "SaleCategoryName4".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SaleCategoryName4;

                            currentRow++;

                        }
                        currentColums++;
                    });


                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }
            catch (Exception)
            {
                using (var workbook = new XLWorkbook())
                {
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }



            //Task<GenericResult> Get_RPT_POSPromoNew(string CompanyCode, string FDate, string TDate)
        }
        //[AllowAnonymous]
        [HttpPost]
        [Route("Export_RPT_SalesStoreSummary")]
        public async Task<ActionResult> Export_RPT_SalesStoreSummary(string companyCode, string storeId, string userlogin, string fromDate, string toDate, List<HeaderModel> header)
        {
            var data = await _reportService.Get_RPT_SalesStoreSummary(companyCode, storeId, userlogin, fromDate, toDate);
            try
            {
                List<RPT_SalesStoreSummaryModel> list = (List<RPT_SalesStoreSummaryModel>)data.Data;
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Main sheet");

                    var currentColums = 1;
                    header.ForEach((item) =>
                    {
                        var currentRow = 1;
                        worksheet.Cell(currentRow, currentColums).Value = item.Name;
                        currentRow += 1;
                        foreach (var sale in list)
                        {
                            if (item.Key.ToLower() == "CompanyCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CompanyCode;
                            if (item.Key.ToLower() == "StoreId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreId;
                            if (item.Key.ToLower() == "StoreName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreName;
                            if (item.Key.ToLower() == "TotalAmount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalAmount;
                            if (item.Key.ToLower() == "TotalPayable".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalPayable;
                            if (item.Key.ToLower() == "TotalDiscountAmt".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalDiscountAmt;
                            if (item.Key.ToLower() == "TotalReceipt".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalReceipt;
                            if (item.Key.ToLower() == "AmountChange".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.AmountChange;
                            if (item.Key.ToLower() == "PaymentDiscount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PaymentDiscount;
                            if (item.Key.ToLower() == "TotalTax".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalTax;
                            if (item.Key.ToLower() == "SalesMode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SalesMode;
                            if (item.Key.ToLower() == "CustomField7".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField7;
                            if (item.Key.ToLower() == "CountNo".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CountNo;
                            if (item.Key.ToLower() == "RoundingOff".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.RoundingOff;
                            if (item.Key.ToLower() == "AvgTotal".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.AVGTotal;

                            currentRow++;

                        }
                        currentColums++;
                    });


                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }
            catch (Exception)
            {
                using (var workbook = new XLWorkbook())
                {
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }



            //Task<GenericResult> Get_RPT_POSPromoNew(string CompanyCode, string FDate, string TDate)
        }
        [HttpPost]
        [Route("Export_RPT_SalesTransactionSummary")]
        public async Task<ActionResult> Export_RPT_SalesTransactionSummary(string companyCode, string storeId, string userlogin, string fromDate, string toDate, List<HeaderModel> header)
        {
            var data = await _reportService.Get_RPT_SalesTransactionSummary(companyCode, storeId, userlogin, fromDate, toDate);
            try
            {
                List<RPT_SalesTransactionSummaryModel> list = (List<RPT_SalesTransactionSummaryModel>)data.Data;
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Main sheet");

                    var currentColums = 1;
                    header.ForEach((item) =>
                    {
                        var currentRow = 1;
                        worksheet.Cell(currentRow, currentColums).Value = item.Name;
                        currentRow += 1;
                        foreach (var sale in list)
                        {
                            if (item.Key.ToLower() == "TransId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TransId;
                            if (item.Key.ToLower() == "CompanyCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CompanyCode;
                            if (item.Key.ToLower() == "StoreId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreId;
                            if (item.Key.ToLower() == "StoreName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreName;
                            if (item.Key.ToLower() == "ContractNo".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ContractNo;
                            if (item.Key.ToLower() == "ShiftId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ShiftId;
                            if (item.Key.ToLower() == "CusId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CusId;
                            if (item.Key.ToLower() == "CusIdentifier".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CusIdentifier;
                            if (item.Key.ToLower() == "TotalAmount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalAmount;
                            if (item.Key.ToLower() == "TotalPayable".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalPayable;
                            if (item.Key.ToLower() == "TotalDiscountAmt".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalDiscountAmt;
                            if (item.Key.ToLower() == "TotalReceipt".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalReceipt;
                            if (item.Key.ToLower() == "AmountChange".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.AmountChange;
                            if (item.Key.ToLower() == "PaymentDiscount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PaymentDiscount;
                            if (item.Key.ToLower() == "TotalTax".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalTax;
                            if (item.Key.ToLower() == "DiscountType".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DiscountType;
                            if (item.Key.ToLower() == "DiscountAmount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DiscountAmount;
                            if (item.Key.ToLower() == "DiscountRate".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DiscountRate;
                            if (item.Key.ToLower() == "CreatedOn".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CreatedOn;
                            if (item.Key.ToLower() == "CreatedBy".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CreatedBy;
                            if (item.Key.ToLower() == "ModifiedOn".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ModifiedOn;
                            if (item.Key.ToLower() == "ModifiedBy".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ModifiedBy;
                            if (item.Key.ToLower() == "Status".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Status;
                            if (item.Key.ToLower() == "IsCanceled".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.IsCanceled;
                            if (item.Key.ToLower() == "Remarks".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Remarks;
                            if (item.Key.ToLower() == "SalesPerson".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SalesPerson;
                            if (item.Key.ToLower() == "SalesMode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SalesMode;
                            if (item.Key.ToLower() == "RefTransId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.RefTransId;
                            if (item.Key.ToLower() == "ManualDiscount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ManualDiscount;
                            if (item.Key.ToLower() == "CustomField7".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField7;
                            if (item.Key.ToLower() == "RoundingOff".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.RoundingOff;
                            if (item.Key.ToLower() == "CounterId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CounterId;
                            if (item.Key.ToLower() == "TotalReceiptByContractNo".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalReceiptByContractNo;


                            currentRow++;

                        }
                        currentColums++;
                    });


                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }
            catch (Exception)
            {
                using (var workbook = new XLWorkbook())
                {
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }



            //Task<GenericResult> Get_RPT_POSPromoNew(string CompanyCode, string FDate, string TDate)
        }
        [HttpPost]
        [Route("Export_RPT_SalesTransactionPayment")]
        public async Task<ActionResult> Export_RPT_SalesTransactionPayment(string companyCode, string storeId, string userlogin, string fromDate, string toDate, List<HeaderModel> header)
        {
            var data = await _reportService.Get_RPT_SalesTransactionPayment(companyCode, storeId, userlogin, fromDate, toDate);
            try
            {
                List<RPT_SalesTransactionPaymentModel> list = (List<RPT_SalesTransactionPaymentModel>)data.Data;
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Main sheet");

                    var currentColums = 1;
                    header.ForEach((item) =>
                    {
                        var currentRow = 1;
                        worksheet.Cell(currentRow, currentColums).Value = item.Name;
                        currentRow += 1;
                        foreach (var sale in list)
                        {
                            if (item.Key.ToLower() == "CompanyCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CompanyCode;
                            if (item.Key.ToLower() == "StoreId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreId;
                              if (item.Key.ToLower() == "TransId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TransId;
                            if (item.Key.ToLower() == "StoreName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreName;
                            if (item.Key.ToLower() == "TransId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TransId;
                            if (item.Key.ToLower() == "PaymentCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PaymentCode;
                            if (item.Key.ToLower() == "TotalAmt".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TotalAmt;
                            if (item.Key.ToLower() == "ChargableAmount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ChargableAmount;
                            if (item.Key.ToLower() == "Currency".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Currency;
                            if (item.Key.ToLower() == "FCAmount".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.FCAmount;
                            if (item.Key.ToLower() == "CreatedOn".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CreatedOn;
                            if (item.Key.ToLower() == "CreatedBy".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CreatedBy;
                            if (item.Key.ToLower() == "ModifiedOn".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ModifiedOn;
                            if (item.Key.ToLower() == "ModifiedBy".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ModifiedBy;
                            if (item.Key.ToLower() == "Forfeit".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Forfeit;
                            if (item.Key.ToLower() == "RefNumber".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.RefNumber;
                            if (item.Key.ToLower() == "CustomF1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomF1;
                            if (item.Key.ToLower() == "CustomF2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomF2;
                            if (item.Key.ToLower() == "CustomF3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomF3;
                            if (item.Key.ToLower() == "CustomF4".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomF4;
                            if (item.Key.ToLower() == "CustomF5".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomF5;
                            if (item.Key.ToLower() == "ShiftId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ShiftId;
                            if (item.Key.ToLower() == "CounterId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CounterId;
                            if (item.Key.ToLower() == "ModifiedOn".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ModifiedOn;

                            currentRow++;

                        }
                        currentColums++;
                    });


                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }
            catch (Exception)
            {
                using (var workbook = new XLWorkbook())
                {
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }



            //Task<GenericResult> Get_RPT_POSPromoNew(string CompanyCode, string FDate, string TDate)
        }
        [HttpPost]
        [Route("Export_RPT_InventoryAudit")]
        public async Task<ActionResult> Export_RPT_InventoryAudit(string companyCode, string storeId, string userlogin, string fromDate, string toDate, List<HeaderModel> header)
        {
            var data = await _reportService.Get_RPT_InventoryAudit(companyCode, storeId, userlogin, fromDate, toDate);
            try
            {
                List<RPT_InventoryAuditModel> list = (List<RPT_InventoryAuditModel>)data.Data;
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Main sheet");

                    var currentColums = 1;
                    header.ForEach((item) =>
                    {
                        var currentRow = 1;
                        worksheet.Cell(currentRow, currentColums).Value = item.Name;
                        currentRow += 1;
                        foreach (var sale in list)
                        {
                            if (item.Key.ToLower() == "CompanyCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CompanyCode;
                            if (item.Key.ToLower() == "ItemCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCode;
                            if (item.Key.ToLower() == "UOMCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.UOMCode;
                            if (item.Key.ToLower() == "SlocId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SlocId;
                            if (item.Key.ToLower() == "StoreId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreId;
                            if (item.Key.ToLower() == "StoreName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreName;
                            if (item.Key.ToLower() == "BeginQty".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.BeginQty;
                            if (item.Key.ToLower() == "InQty".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.InQty;
                            if (item.Key.ToLower() == "OutQty".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.OutQty;
                            if (item.Key.ToLower() == "EndQty".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.EndQty;
                            if (item.Key.ToLower() == "ProductId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ProductId;
                            if (item.Key.ToLower() == "VariantId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.VariantId;
                            if (item.Key.ToLower() == "Status".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Status;
                            if (item.Key.ToLower() == "CapacityValue".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CapacityValue;
                            if (item.Key.ToLower() == "ItemGroupId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemGroupId;
                            if (item.Key.ToLower() == "SalesTaxCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SalesTaxCode;
                            if (item.Key.ToLower() == "PurchaseTaxCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PurchaseTaxCode;
                            if (item.Key.ToLower() == "ItemName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemName;
                            if (item.Key.ToLower() == "ItemDescription".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemDescription;
                            if (item.Key.ToLower() == "ItemCategory_1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory_1;
                            if (item.Key.ToLower() == "ItemCategory_2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory_2;
                            if (item.Key.ToLower() == "ItemCategory_3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory_3;
                            if (item.Key.ToLower() == "ForeignName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ForeignName;
                            if (item.Key.ToLower() == "InventoryUOM".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.InventoryUOM;
                            if (item.Key.ToLower() == "ImageURL".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ImageURL;
                            if (item.Key.ToLower() == "ImageLink".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ImageLink;
                            if (item.Key.ToLower() == "Mcid".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.MCId;
                            if (item.Key.ToLower() == "CustomField1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField1;
                            if (item.Key.ToLower() == "CustomField2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField2;
                            if (item.Key.ToLower() == "CustomField3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField3;
                            if (item.Key.ToLower() == "CustomField4".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField4;
                            if (item.Key.ToLower() == "CustomField5".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField5;
                            if (item.Key.ToLower() == "CustomField6".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField6;
                            if (item.Key.ToLower() == "CustomField7".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField7;
                            if (item.Key.ToLower() == "CustomField8".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField8;
                            if (item.Key.ToLower() == "CustomField9".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField9;
                            if (item.Key.ToLower() == "CustomField10".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField10;
                            if (item.Key.ToLower() == "DefaultPrice".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DefaultPrice;
                            if (item.Key.ToLower() == "IsSerial".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.IsSerial;
                            if (item.Key.ToLower() == "IsBOM".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.IsBOM;
                            if (item.Key.ToLower() == "ValidFrom".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ValidFrom;
                            if (item.Key.ToLower() == "ValidTo".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ValidTo;


                            currentRow++;

                        }
                        currentColums++;
                    });


                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }
            catch (Exception)
            {
                using (var workbook = new XLWorkbook())
                {
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }



            //Task<GenericResult> Get_RPT_POSPromoNew(string CompanyCode, string FDate, string TDate)
        }
        [HttpPost]
        [Route("Export_RPT_InventoryPosting")]
        public async Task<ActionResult> Export_RPT_InventoryPosting(string companyCode, string storeId, string userlogin, string fromDate, string toDate, List<HeaderModel> header)
        {
            var data = await _reportService.Get_RPT_InventoryPosting(companyCode, storeId, userlogin, fromDate, toDate);
            try
            {
                List<RPT_InventoryPostingModel> list = (List<RPT_InventoryPostingModel>)data.Data;
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Main sheet");

                    var currentColums = 1;
                    header.ForEach((item) =>
                    {
                        var currentRow = 1;
                        worksheet.Cell(currentRow, currentColums).Value = item.Name;
                        currentRow += 1;
                        foreach (var sale in list)
                        {
                            if (item.Key.ToLower() == "CompanyCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CompanyCode;
                            if (item.Key.ToLower() == "TransType".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TransType;
                            if (item.Key.ToLower() == "SlocId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SlocId;
                            if (item.Key.ToLower() == "TransDate".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TransDate;
                            if (item.Key.ToLower() == "TransId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.TransId;
                            if (item.Key.ToLower() == "ItemCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCode;
                            if (item.Key.ToLower() == "UomCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.UOMCode;
                            if (item.Key.ToLower() == "StoreId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreId;
                            if (item.Key.ToLower() == "InQty".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.InQty;
                            if (item.Key.ToLower() == "OutQty".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.OutQty;
                            if (item.Key.ToLower() == "ProductId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ProductId;
                            if (item.Key.ToLower() == "VariantId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.VariantId;
                            if (item.Key.ToLower() == "Status".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Status;
                            if (item.Key.ToLower() == "CapacityValue".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CapacityValue;
                            if (item.Key.ToLower() == "ItemGroupId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemGroupId;
                            if (item.Key.ToLower() == "SalesTaxCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SalesTaxCode;
                            if (item.Key.ToLower() == "PurchaseTaxCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PurchaseTaxCode;
                            if (item.Key.ToLower() == "ItemName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemName;
                            if (item.Key.ToLower() == "ItemDescription".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemDescription;
                            if (item.Key.ToLower() == "ItemCategory_1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory_1;
                            if (item.Key.ToLower() == "ItemCategory_2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory_2;
                            if (item.Key.ToLower() == "ItemCategory_3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory_3;
                            if (item.Key.ToLower() == "ForeignName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ForeignName;
                            if (item.Key.ToLower() == "InventoryUOM".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.InventoryUOM;
                            if (item.Key.ToLower() == "ImageURL".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ImageURL;
                            if (item.Key.ToLower() == "ImageLink".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ImageLink;
                            if (item.Key.ToLower() == "Remark".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Remark;
                            if (item.Key.ToLower() == "MCId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.MCId;
                            if (item.Key.ToLower() == "CustomField1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField1;
                            if (item.Key.ToLower() == "CustomField2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField2;
                            if (item.Key.ToLower() == "CustomField3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField3;
                            if (item.Key.ToLower() == "CustomField4".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField4;
                            if (item.Key.ToLower() == "CustomField5".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField5;
                            if (item.Key.ToLower() == "CustomField6".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField6;
                            if (item.Key.ToLower() == "CustomField7".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField7;
                            if (item.Key.ToLower() == "CustomField8".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField8;
                            if (item.Key.ToLower() == "CustomField9".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField9;
                            if (item.Key.ToLower() == "CustomField10".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField10;
                            if (item.Key.ToLower() == "DefaultPrice".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DefaultPrice;
                            if (item.Key.ToLower() == "IsSerial".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.IsSerial;
                            if (item.Key.ToLower() == "IsBOM".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.IsBOM;
                            if (item.Key.ToLower() == "ValidFrom".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ValidFrom;
                            if (item.Key.ToLower() == "ValidTo".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ValidTo;
                            if (item.Key.ToLower() == "MovementTypeCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.MovementTypeCode;
                            if (item.Key.ToLower() == "MovementTypeName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.MovementTypeName;
                            currentRow++;

                        }
                        currentColums++;
                    });


                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }
            catch (Exception)
            {
                using (var workbook = new XLWorkbook())
                {
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }



            //Task<GenericResult> Get_RPT_POSPromoNew(string CompanyCode, string FDate, string TDate)
        }
        [HttpPost]
        [Route("Export_RPT_InventoryOnHand")]
        public async Task<ActionResult> Export_RPT_InventoryOnHand(string companyCode, string storeId, string userlogin, string fromDate, string toDate, List<HeaderModel> header)
        {
            var data = await _reportService.Get_RPT_SalesStoreSummary(companyCode, storeId, userlogin, fromDate, toDate);
            try
            {
                List<RPT_InventoryOnHandModel> list = (List<RPT_InventoryOnHandModel>)data.Data;
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Main sheet");

                    var currentColums = 1;
                    header.ForEach((item) =>
                    {
                        var currentRow = 1;
                        worksheet.Cell(currentRow, currentColums).Value = item.Name;
                        currentRow += 1;
                        foreach (var sale in list)
                        {
                            if (item.Key.ToLower() == "CompanyCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CompanyCode;
                            if (item.Key.ToLower() == "SlocId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SlocId;
                            if (item.Key.ToLower() == "ItemCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCode;
                            if (item.Key.ToLower() == "UomCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.UOMCode;
                            if (item.Key.ToLower() == "StoreId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreId;
                            if (item.Key.ToLower() == "StoreName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.StoreName;
                            if (item.Key.ToLower() == "Quantity".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Quantity;
                            if (item.Key.ToLower() == "ProductId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ProductId;
                            if (item.Key.ToLower() == "VariantId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.VariantId;
                            if (item.Key.ToLower() == "CreatedBy".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CreatedBy;
                            if (item.Key.ToLower() == "CreatedOn".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CreatedOn;
                            if (item.Key.ToLower() == "ModifiedBy".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ModifiedBy;
                            if (item.Key.ToLower() == "ModifiedOn".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ModifiedOn;
                            if (item.Key.ToLower() == "Status".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.Status;
                            if (item.Key.ToLower() == "CapacityValue".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CapacityValue;
                            if (item.Key.ToLower() == "ItemGroupId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemGroupId;
                            if (item.Key.ToLower() == "SalesTaxCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.SalesTaxCode;
                            if (item.Key.ToLower() == "PurchaseTaxCode".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.PurchaseTaxCode;
                            if (item.Key.ToLower() == "ItemName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemName;
                            if (item.Key.ToLower() == "ItemDescription".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemDescription;
                            if (item.Key.ToLower() == "ItemCategory_1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory_1;
                            if (item.Key.ToLower() == "ItemCategory_2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory_2;
                            if (item.Key.ToLower() == "ItemCategory_3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ItemCategory_3;
                            if (item.Key.ToLower() == "ForeignName".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ForeignName;
                            if (item.Key.ToLower() == "InventoryUOM".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.InventoryUOM;
                            if (item.Key.ToLower() == "ImageURL".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ImageURL;
                            if (item.Key.ToLower() == "ImageLink".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ImageLink;
                            if (item.Key.ToLower() == "MCId".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.MCId;
                            if (item.Key.ToLower() == "CustomField1".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField1;
                            if (item.Key.ToLower() == "CustomField2".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField2;
                            if (item.Key.ToLower() == "CustomField3".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField3;
                            if (item.Key.ToLower() == "CustomField4".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField4;
                            if (item.Key.ToLower() == "CustomField5".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField5;
                            if (item.Key.ToLower() == "CustomField6".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField6;
                            if (item.Key.ToLower() == "CustomField7".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField7;
                            if (item.Key.ToLower() == "CustomField8".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField8;
                            if (item.Key.ToLower() == "CustomField9".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField9;
                            if (item.Key.ToLower() == "CustomField10".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.CustomField10;
                            if (item.Key.ToLower() == "DefaultPrice".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.DefaultPrice;
                            if (item.Key.ToLower() == "IsSerial".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.IsSerial;
                            if (item.Key.ToLower() == "IsBOM".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.IsBOM;
                            if (item.Key.ToLower() == "ValidFrom".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ValidFrom;
                            if (item.Key.ToLower() == "ValidTo".ToLower())
                                worksheet.Cell(currentRow, currentColums).Value = sale.ValidTo;

                            currentRow++;

                        }
                        currentColums++;
                    });


                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }
            catch (Exception)
            {
                using (var workbook = new XLWorkbook())
                {
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "users.xlsx");
                    }
                }
            }



            //Task<GenericResult> Get_RPT_POSPromoNew(string CompanyCode, string FDate, string TDate)
        }
    }
}
