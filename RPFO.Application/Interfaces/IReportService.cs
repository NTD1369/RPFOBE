
using RPFO.Data.ViewModel.RPT;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IReportService
    {
        Task<GenericResult> Get_RPT_InventoryAudit(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_InventoryOnHand(string CompanyCode, string StoreId, string Userlogin);
        Task<GenericResult> Get_RPT_SalesStoreSummary(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_SalesTransactionDetail(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_Dash_SaleDetailTransactionByTop(string CompanyCode, string StoreId, string FromDate, string ToDate, string ViewType , string ViewBy ,int? Top);
        Task<GenericResult> Get_RPT_SOToDivision(string CompanyCode, string Date, string CusId, string TransId, bool? InComplete);
        Task<GenericResult> Get_RPT_SalesTransactionSummary(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_SalesTopProduct(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate, int? Top);
        Task<GenericResult> Get_RPT_SalesByHour(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_SalesByYear(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_SalesBySalesPerson(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_SalesTransactionPayment(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_Dashboard(string CompanyCode, string StoreId, string Userlogin, string Date); 
        Task<GenericResult> Get_RPT_LoadChartOrderPeriodByYear(string companyCode, string storeId, string userlogin, string year);
        Task<GenericResult> Get_RPT_SalesTransactionSummaryByDepartment(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate, string DailyId);
        Task<GenericResult> Get_RPT_LoadChartOrderPeriodByMonth(string companyCode, string storeId, string userlogin, string year, string month);

        Task<GenericResult> Get_RPT_LoadChartOrderPeriodByWeek(string companyCode, string storeId, string userlogin);
        Task<GenericResult> Get_RPT_SalesTransactionDetail_Return(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_SalesTransactionDetail_Ex(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_Rpt_GiftVoucher(string fromDate, string toDate, string OutletID);
        Task<GenericResult> Get_RPT_POSPromo(string fromDate, string toDate, string OutletID);
        Task<GenericResult> Get_RPT_InventoryPosting(string CompanyCode, string StoreId, string Userlogin, string fromDate, string toDate);
        Task<GenericResult> Get_RPT_InvoiceTransactionPayment(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_InvoiceTransactionDetail(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_InvoiceTransactionSummary(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> RPT_SYNC_ITEM_CMP(string CompanyCode, string FItem, string TItem, DateTime? FDate, DateTime? TDate);
        Task<GenericResult> RPT_SYNC_LISTING_CMP(string CompanyCode, string FItem, string TItem, DateTime? FDate, DateTime? TDate);
        Task<GenericResult> RPT_SYNC_PROMO_CMP(string CompanyCode, string FId, string TId, DateTime? FDate, DateTime? TDate);
        Task<GenericResult> RPT_SYNC_PRICE_CMP(string CompanyCode, string FItem, string TItem, DateTime? FDate, DateTime? TDate);
        Task<GenericResult> Get_RPT_POSPromoNew(string CompanyCode, string FDate, string TDate);
        Task<GenericResult> Get_RPT_InventorySerial(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);

        Task<GenericResult> Get_RPT_SalesbyItem(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
        Task<GenericResult> Get_RPT_VoucherCheckIn(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate, string Keyword);
        Task<GenericResult> Get_RPT_SalesEPAYDetail(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);

        Task<GenericResult> Get_RPT_ActionOnOrder(string CompanyCode, string StoreId, string TransId, string User, string Userlogin, string FDate, string TDate, string Type);
        Task<GenericResult> Get_RPT_CollectionDailyByCounter(string CompanyCode, string StoreId, string Userlogin, string Date);
        Task<GenericResult> Get_RPT_SyncDataStatusByIdoc(string CompanyCode, string IdocNum, string DataType);
        GenericResult Get_RPT_SyncDataStatusView(string CompanyCode, string StoreId, string Userlogin, string FDate, string TDate);
    }
}
