
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
//using RPFO.Data.OMSModels;
using RPFO.Data.Models;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
//using RPFO.Data.JAMwiModels;
using RPFO.Data.OMSModels;
using static ESCPOS.Commands;
using ESCPOS;
using ESCPOS.Utils;
using System.IO;
using RPFO.Utilities.Helpers;

namespace RPFO.Application.Implements
{
    public class SaleService : ISaleService
    {
        private readonly IGenericRepository<TSalesHeader> _saleHeaderRepository;
        private readonly IGenericRepository<TSalesLine> _saleLineRepository;
        private readonly IGenericRepository<TSalesPayment> _salepaymentLineRepository;
        private readonly IGenericRepository<MCustomer> _customerRepository;
        private readonly IGenericRepository<MBomline> _bomLineRepository;
        private readonly IGenericRepository<SLog> _logRepository;
         
        //private readonly BOMService _bomRepository;
        private readonly ILoyaltyService _loyaltyService;
        private readonly IBankTerminalService _bankTerminalService;
        private readonly IDeliveryInforService _deliveryInforService;
        private readonly IGeneralSettingService _settingService;
        private readonly IPrepaidCardService _prepaidCardService;
        private readonly IThirdPartyLogService _log3Service;
        private readonly IPaymentMethodService _paymentService;
        private readonly string MWIClient = "";
        private readonly string PrefixSO = "";
        private readonly string PrefixAR = "";
        private readonly string TerminalID = "";
        private readonly string MerchantID = "";
        private IShiftService _shiftService;
        private IInvoiceService _invoiceService;
        private IPermissionService _permissionService;
        private TimeSpan timeQuickAction = TimeSpan.FromSeconds(15);
        private IResponseCacheService cacheService;
        private string PrefixCacheActionSO = "QASO-{0}-{1}";

        //
        IMapper _mapper;
        public SaleService(IGenericRepository<TSalesHeader> saleHeaderRepository, IPermissionService permissionService,
            IGenericRepository<SLog> logRepository, IDeliveryInforService deliveryInforService, IBankTerminalService bankTerminalService,
            IGenericRepository<TSalesPayment> salepaymentLineRepository, IConfiguration config, IInvoiceService invoiceService,
           IGenericRepository<MBomline> bomLineRepository, IGenericRepository<TSalesLine> saleLineRepository, IPaymentMethodService paymentService,
           IGeneralSettingService settingService, IPrepaidCardService prepaidCardService, IThirdPartyLogService log3Service, ILoyaltyService loyaltyService, IGenericRepository<MCustomer> customerRepository, IMapper mapper, IShiftService shiftService, IResponseCacheService responseCacheService /*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _saleHeaderRepository = saleHeaderRepository;
            _saleLineRepository = saleLineRepository;
            _invoiceService = invoiceService;
            _deliveryInforService = deliveryInforService;
            _salepaymentLineRepository = salepaymentLineRepository;
            _customerRepository = customerRepository;
            _bomLineRepository = bomLineRepository;
            _bankTerminalService = bankTerminalService;
            _mapper = mapper;
            _permissionService = permissionService;
            _paymentService = paymentService;
            _settingService = settingService;
            _loyaltyService = loyaltyService;
            _logRepository = logRepository;
            _shiftService = shiftService;
            _log3Service = log3Service;
            _prepaidCardService = prepaidCardService;
            MWIClient = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("MWIService"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            PrefixSO = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixSO"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            PrefixAR = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixAR"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            TerminalID = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("TerminalID"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            MerchantID = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("MerchantID"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            this.cacheService = responseCacheService;

            string timeCache = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("TimeCacheAction"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (!string.IsNullOrEmpty(timeCache) && double.TryParse(timeCache, out double timeAction))
            {
                timeQuickAction = TimeSpan.FromSeconds(timeAction);
            }
        }

        public async Task<List<MBomline>> getBOMLine(string itemCode)
        {


            try
            {
                List<MBomline> lines = await _bomLineRepository.GetAllAsync($"select * from M_BOMLine with (nolock) where BOMId=N'{itemCode}'", null, commandType: CommandType.Text);
                return lines;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private async Task<GenericResult> GetPrepaidCard(string CompanyCode, string Code)
        {
            return await _prepaidCardService.GetByCode(CompanyCode, Code);

        }
        public Task<GenericResult> Create(TSalesHeader model)
        {
            throw new NotImplementedException();
        }
        public class ResultModel
        {
            public int ID { get; set; }
            public string Message { get; set; }
        }
        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(this.MWIClient);
            //client.DefaultRequestHeaders.Accept.Clear();

            return client;
        }
        public async Task<GenericResult> GetInvoiceInfor(string CompanyCode, string Phone, string TaxCode)
        {
            GenericResult result = new GenericResult();

            if (CompanyCode == null)
            {
                result.Success = false;
                result.Message = "Company Code not null.";
                return result;
            }

            using (IDbConnection db = _saleHeaderRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    //var data = db.QueryAsync<TSalesInvoice>($"", null, commandType: CommandType.Text);
                    string query = $"select distinct * from T_SalesInvoice where CompanyCode= N'{CompanyCode}' and isnull(CustomerName,'')<>'' and isnull(Phone,'')<>' {Phone}' and isnull(TaxCode,'')<>''";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                    return result;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    return result;

                }
            }
        }
        public async Task<GenericResult> CheckOMSIDAlready(string CompanyCode, string OMSID)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (string.IsNullOrEmpty(OMSID))
                {
                    result.Success = false;
                    result.Message = "OMS Id Can't null. Please input OMS Id";
                    return result;
                }
                if (OMSID.Length >= 9)
                {
                    result.Success = false;
                    result.Message = "OMS Id length invalid. Please input another Id";
                    return result;
                }
                var Ecomparameters = new DynamicParameters();
                Ecomparameters.Add("CompanyCode", CompanyCode);
                Ecomparameters.Add("EcomId", OMSID);
                var socheck = await _saleHeaderRepository.GetAllAsync("USP_S_T_SalesEcom", Ecomparameters, commandType: CommandType.StoredProcedure);
                if (socheck != null && socheck.Count > 0)
                {
                    result.Success = false;
                    result.Message = $"Can't add order. {socheck[0].RefTransId} has existed. POS ID: " + socheck.FirstOrDefault().TransId;
                }
                else
                {
                    result.Success = true;

                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;


            }
            return result;

        }

        public async Task<GenericResult> GetTransIdByOMSID(string CompanyCode, string OMSID)
        {
            GenericResult result = new GenericResult();
            try
            {
                if (string.IsNullOrEmpty(OMSID))
                {
                    result.Success = false;
                    result.Message = "OMS Id Can't null. Please input OMS Id";
                    return result;
                }

                var Ecomparameters = new DynamicParameters();
                Ecomparameters.Add("CompanyCode", CompanyCode);
                Ecomparameters.Add("OMSId", OMSID);
                var socheck = _saleHeaderRepository.GetScalar("USP_GetTransIdByOMSId", Ecomparameters, commandType: CommandType.StoredProcedure);
                if (string.IsNullOrEmpty(socheck))
                {
                    result.Success = false;
                    result.Message = "Can't found transaction id created with oms id: " + OMSID;
                }
                else
                {
                    result.Success = true;
                    result.Message = socheck;
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;


            }
            return result;

        }

        public async Task<GenericResult> CreateSaleOrderWithoutPayment(SaleViewModel model)
        {
            GenericResult result = new GenericResult();
            //result.Success = false;
            //result.Message = "Test";
            //return result;

            if (model.CompanyCode == null || string.IsNullOrEmpty(model.CompanyCode))
            {
                result.Success = false;
                result.Message = "Company Code not null.";
                return result;
            }
            if (model.Lines == null || model.Lines.Count() == 0)
            {
                result.Success = false;
                result.Message = "Doc line not null.";
                return result;
            }
            if (model.StoreId == null || string.IsNullOrEmpty(model.StoreId))
            {
                result.Success = false;
                result.Message = " Store not null.";
                return result;
            }
            if (string.IsNullOrEmpty(model.CusId))
            {
                result.Success = false;
                result.Message = " Customer not null.";
                return result;
            }
            try
            {
                if (string.IsNullOrEmpty(model.ContractNo))
                {
                    if (model.TotalAmount - model.TotalDiscountAmt > model.TotalReceipt && model.SalesMode != "HOLD")
                    {
                        result.Success = false;
                        result.Message = "Please check your amount.";
                        return result;
                    }

                }

                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var parameters = new DynamicParameters();
                                string key = "";
                                if (!string.IsNullOrEmpty(model.TransId))
                                {

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.TransId;
                                }
                                else
                                {

                                    key = _saleHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixSO}',N'{model.CompanyCode}', N'{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.TransId = key;
                                }
                                if (model.SalesMode != null && model.SalesMode == "Return")
                                {
                                    string checkResult = _saleHeaderRepository.GetScalar($"USP_Check_ReturnOrder N'{model.CompanyCode}', N'{model.StoreId}', N'{model.TransId}',N'{model.SalesType}',N'{model.SalesMode}'", null, commandType: CommandType.Text);
                                    if (checkResult == "0")
                                    {
                                        result.Success = false;
                                        result.Message = "Can't return order. Because the order date is not valid.";
                                        return result;
                                    }

                                }
                                string itemList = "";
                                foreach (var line in model.Lines)
                                {

                                    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                }
                                string defaultWhs = _saleHeaderRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);

                                //string querycheck = $"USP_I_T_SalesLine_CheckNegative N'{model.CompanyCode}', N'{itemList}'";
                                //var resultCheck = db.Query(querycheck, null, commandType: CommandType.Text);
                                //if(resultCheck.ToList().Count > 0)
                                //{
                                //    var line = resultCheck.ToList()[0] as ResultModel;
                                //    if (line.ID != 0)
                                //    {
                                //        result.Success = false;
                                //        result.Message = line.Message;
                                //        return result;
                                //    }

                                //}


                                //Create and fill-up master table data
                                parameters = new DynamicParameters();

                                parameters.Add("TransId", model.TransId, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("ShiftId", model.ShiftId);
                                parameters.Add("ContractNo", model.ContractNo);
                                parameters.Add("CusId", model.CusId);
                                parameters.Add("CusGrpId", model.CusGrpId);
                                parameters.Add("CusIdentifier", model.CusIdentifier);

                                parameters.Add("TotalAmount", model.TotalAmount);
                                parameters.Add("TotalPayable", model.TotalPayable);
                                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                parameters.Add("TotalReceipt", model.TotalReceipt);
                                parameters.Add("AmountChange", model.AmountChange);
                                parameters.Add("PaymentDiscount", model.PaymentDiscount);

                                parameters.Add("TotalTax", model.TotalTax);
                                parameters.Add("DiscountType", model.DiscountType);
                                parameters.Add("DiscountAmount", model.DiscountAmount);
                                parameters.Add("DiscountRate", model.DiscountRate);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "C" : model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("Remarks", model.Remarks);
                                parameters.Add("SalesPerson", model.SalesPerson);
                                parameters.Add("SalesMode", model.SalesMode);
                                parameters.Add("RefTransId", model.RefTransId);
                                parameters.Add("ManualDiscount", model.ManualDiscount);
                                parameters.Add("SalesType", model.SalesType);
                                parameters.Add("DataSource", model.DataSource);
                                parameters.Add("POSType", model.POSType);
                                parameters.Add("Phone", model.Phone);
                                parameters.Add("CusName", model.CusName);
                                parameters.Add("CusAddress", model.CusAddress);
                                parameters.Add("Reason", model.Reason);
                                parameters.Add("OMSId", model.OMSId);
                                parameters.Add("Chanel", model.Chanel);
                                parameters.Add("TerminalId", model.TerminalId);
                                parameters.Add("StartTime", model.StartTime == null ? DateTime.Now : model.StartTime);

                                parameters.Add("RewardPoints", model.RewardPoints);
                                parameters.Add("ExpiryDate", model.ExpiryDate == null ? DateTime.Now : model.ExpiryDate);
                                parameters.Add("DocDate", model.DocDate == null ? DateTime.Now : model.DocDate);
                                parameters.Add("CustomF1", model.CustomF1);
                                parameters.Add("CustomF2", model.CustomF2);
                                parameters.Add("CustomF3", model.CustomF3);
                                parameters.Add("CustomF4", model.CustomF4);
                                parameters.Add("CustomF5", model.CustomF5);
                                if (!string.IsNullOrEmpty(model.LuckyNo))
                                {
                                    parameters.Add("LuckyNo", model.LuckyNo);
                                }

                                //_saleHeaderRepository.Insert("InsertSaleHeader", parameters, commandType: CommandType.StoredProcedure);

                                //Insert record in master table. Pass transaction parameter to Dapper.
                                var affectedRows = db.Execute("USP_I_T_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //Get the Id newly created for master table record.
                                //If this is not an Identity, use different method here
                                //newId = Convert.ToInt64(connection.ExecuteScalar<object>("SELECT @@IDENTITY", null, transaction: transaction));

                                //Create and fill-up detail table data
                                //Use suitable loop as you want to insert multiple records.
                                //for(......)
                                int stt = 0;

                                foreach (var line in model.Lines)
                                {
                                    stt++;

                                    parameters = new DynamicParameters();
                                    parameters.Add("TransId", key, DbType.String);
                                    parameters.Add("LineId", stt);
                                    line.LineId = stt.ToString();
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Uomcode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("Price", line.Price);
                                    parameters.Add("LineTotal", line.LineTotal);
                                    parameters.Add("DiscountType", string.IsNullOrEmpty(line.DiscountType) && string.IsNullOrEmpty(line.PromoType) ? line.PromoType : line.DiscountType);
                                    parameters.Add("DiscountAmt", !line.DiscountAmt.HasValue && line.PromoDisAmt.HasValue ? line.PromoDisAmt : line.DiscountAmt);
                                    parameters.Add("DiscountRate", !line.DiscountRate.HasValue && line.PromoPercent.HasValue ? line.PromoPercent : line.DiscountRate);
                                    parameters.Add("CreatedBy", line.CreatedBy);
                                    if (string.IsNullOrEmpty(line.SlocId))
                                    {

                                        line.SlocId = defaultWhs;
                                    }
                                    parameters.Add("ModifiedBy", null);
                                    parameters.Add("ModifiedOn", null);
                                    parameters.Add("PromoId", line.PromoId);
                                    parameters.Add("PromoType", line.PromoType);

                                    parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "C" : model.Status);
                                    parameters.Add("Remark", line.Remark);
                                    parameters.Add("PromoPercent", line.PromoPercent);
                                    parameters.Add("PromoBaseItem", line.PromoBaseItem);
                                    parameters.Add("SalesMode", line.SalesMode);
                                    parameters.Add("Remarks", line.Remark);
                                    parameters.Add("TaxRate", line.TaxRate);
                                    line.TaxAmt = ((line.Price * line.Quantity) + line.DiscountAmt == null ? 1 : line.DiscountAmt) * line.TaxRate / 100;
                                    parameters.Add("TaxAmt", line.TaxAmt);
                                    parameters.Add("TaxCode", line.TaxCode);
                                    parameters.Add("SlocId", line.SlocId);
                                    parameters.Add("MinDepositAmt", line.MinDepositAmt);
                                    parameters.Add("MinDepositPercent", line.MinDepositPercent);
                                    parameters.Add("DeliveryType", line.DeliveryType);
                                    parameters.Add("Posservice", line.Posservice);
                                    parameters.Add("StoreAreaId", line.StoreAreaId);
                                    parameters.Add("TimeFrameId", line.TimeFrameId);
                                    parameters.Add("Duration", line.Duration);
                                    parameters.Add("AppointmentDate", line.AppointmentDate);
                                    parameters.Add("BomId", line.BomId);
                                    parameters.Add("PromoPrice", line.PromoPrice);
                                    parameters.Add("PromoLineTotal", line.PromoLineTotal);
                                    parameters.Add("BaseLine", line.BaseLine);
                                    parameters.Add("BaseTransId", line.BaseTransId);
                                    parameters.Add("OpenQty", line.OpenQty);
                                    parameters.Add("PromoDisAmt", line.PromoDisAmt);
                                    parameters.Add("IsPromo", line.IsPromo);
                                    parameters.Add("IsSerial", line.IsSerial);
                                    parameters.Add("IsVoucher", line.IsVoucher);
                                    parameters.Add("Description", line.Description);
                                    parameters.Add("PrepaidCardNo", line.PrepaidCardNo);
                                    parameters.Add("MemberDate", line.MemberDate);
                                    parameters.Add("MemberValue", line.MemberValue);
                                    parameters.Add("StartDate", line.StartDate);
                                    parameters.Add("EndDate", line.EndDate);
                                    parameters.Add("ItemType", line.ItemType);
                                    parameters.Add("WeightScaleBarcode", line.WeightScaleBarcode);
                                    parameters.Add("StoreId", model.StoreId);
                                    parameters.Add("BookletNo", line.BookletNo);
                                    db.Execute("usp_I_T_SalesLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                }
                                int sttLine = 0;
                                if (model.SerialLines != null && model.SerialLines.Count > 0)
                                {
                                    foreach (var line in model.SerialLines)
                                    {
                                        sttLine++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("TransId", key, DbType.String);
                                        parameters.Add("LineId", Guid.NewGuid());
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", line.ItemCode);
                                        if (string.IsNullOrEmpty(line.SlocId))
                                        {
                                            line.SlocId = defaultWhs;
                                        }
                                        parameters.Add("SerialNum", line.SerialNum);
                                        parameters.Add("Uomcode", line.UomCode);
                                        parameters.Add("SLocId", line.SlocId);
                                        parameters.Add("Quantity", line.Quantity);
                                        parameters.Add("Status", line.Status);
                                        parameters.Add("CreatedBy", line.CreatedBy);
                                        parameters.Add("OpenQty", line.OpenQty);
                                        parameters.Add("BaseLine", line.BaseLine);
                                        parameters.Add("BaseTransId", line.BaseTransId);
                                        parameters.Add("LineNum", sttLine);
                                        parameters.Add("Prefix", line.Prefix);
                                        parameters.Add("Phone", line.Phone);
                                        parameters.Add("Name", line.Name);
                                        parameters.Add("CustomF1", line.CustomF1);
                                        parameters.Add("CustomF2", line.CustomF2);
                                        parameters.Add("StoreId", model.StoreId);
                                        db.Execute("USP_I_T_SalesLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }
                                if (model.PromoLines != null && model.PromoLines.Count > 0)
                                {
                                    foreach (var line in model.PromoLines)
                                    {
                                        stt++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("TransId", key, DbType.String);
                                        //parameters.Add("LineId", stt);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", line.ItemCode);
                                        parameters.Add("BarCode", line.BarCode);
                                        parameters.Add("Uomcode", line.UomCode);
                                        parameters.Add("RefTransId", line.RefTransId);
                                        parameters.Add("ApplyType", line.ApplyType);
                                        parameters.Add("ItemGroupId", line.ItemGroupId);
                                        parameters.Add("Value", line.Value);
                                        parameters.Add("PromoId", line.PromoId);
                                        parameters.Add("PromoType", line.PromoType);
                                        parameters.Add("PromoTypeLine", line.PromoTypeLine);
                                        parameters.Add("Status", line.Status);
                                        parameters.Add("CreatedBy", line.CreatedBy);
                                        parameters.Add("PromoAmt", line.PromoAmt);
                                        parameters.Add("PromoPercent", line.PromoPercent);
                                        parameters.Add("StoreId", model.StoreId);
                                        db.Execute("USP_I_T_SalesPromo", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    }
                                }
                                stt = 0;
                                if (model.Payments != null && model.Payments.Count > 0)
                                {
                                    foreach (var payment in model.Payments)
                                    {
                                        stt++;
                                        if (string.IsNullOrEmpty(payment.Currency))
                                        {
                                            string CurrencyStr = $"select CurrencyCode from M_Store with (nolock) where StoreId =N'{model.StoreId}' and CompanyCode =N'{model.CompanyCode}' ";
                                            string Currency = _saleHeaderRepository.GetScalar(CurrencyStr, null, commandType: CommandType.Text);
                                            payment.Currency = Currency;
                                        }
                                        parameters = new DynamicParameters();
                                        parameters.Add("PaymentCode", payment.PaymentCode, DbType.String);
                                        parameters.Add("CompanyCode", model.CompanyCode);

                                        parameters.Add("TransId", key);
                                        parameters.Add("LineId", stt);
                                        parameters.Add("TotalAmt", payment.TotalAmt == null ? payment.ChargableAmount : payment.TotalAmt);
                                        parameters.Add("ReceivedAmt", payment.ReceivedAmt);
                                        parameters.Add("PaidAmt", payment.PaidAmt);
                                        parameters.Add("Currency", payment.Currency);
                                        if ((payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0) > 0 && (payment.ChangeAmt == 0 || payment.ChangeAmt == null))
                                        {
                                            payment.ChangeAmt = (payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0);
                                        }
                                        parameters.Add("ChangeAmt", payment.ChangeAmt);
                                        parameters.Add("PaymentMode", payment.PaymentMode);
                                        parameters.Add("CardType", payment.CardType);
                                        parameters.Add("CardHolderName", payment.CardHolderName);
                                        parameters.Add("CardNo", payment.CardNo);
                                        parameters.Add("VoucherBarCode", payment.VoucherBarCode);
                                        parameters.Add("VoucherSerial", payment.VoucherSerial);
                                        parameters.Add("CreatedBy", payment.CreatedBy);
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                        parameters.Add("Status", payment.Status);
                                        parameters.Add("ChargableAmount", payment.ChargableAmount);
                                        parameters.Add("PaymentDiscount", payment.PaymentDiscount);
                                        parameters.Add("CollectedAmount", payment.CollectedAmount);
                                        parameters.Add("RefNumber", payment.RefNumber);
                                        parameters.Add("DataSource", payment.DataSource);
                                        parameters.Add("Currency", payment.Currency);
                                        parameters.Add("Rate", payment.Rate);
                                        parameters.Add("FCAmount", payment.FCAmount);
                                        parameters.Add("ShiftId", model.ShiftId);
                                        parameters.Add("CardExpiryDate", payment.CardExpiryDate);
                                        parameters.Add("AdjudicationCode", payment.AdjudicationCode);
                                        parameters.Add("AuthorizationDateTime", payment.AuthorizationDateTime);
                                        parameters.Add("StoreId", model.StoreId);
                                        db.Execute("USP_I_T_SalesPayment", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }

                                //foreach(var prepaidTrans in model.PrepaidLines)
                                //{
                                //    parameters = new DynamicParameters();
                                //    parameters.Add("TransId", key, DbType.String);
                                //    parameters.Add("LineId", stt);
                                //    parameters.Add("CompanyCode", model.CompanyCode);
                                //    parameters.Add("TransId", prepaidTrans.TransId);
                                //    parameters.Add("PepaidCardNo", prepaidTrans.PepaidCardNo);
                                //    parameters.Add("TransType", prepaidTrans.TransType);
                                //    parameters.Add("MainBalance", prepaidTrans.MainBalance);
                                //    parameters.Add("SubBlance", prepaidTrans.SubBlance);
                                //    parameters.Add("LineTotal", line.LineTotal);
                                //    parameters.Add("DiscountType", line.DiscountType);
                                //    db.Execute("usp_I_T_SalesLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //}    
                                //if (model.Vouchers != null && model.Vouchers.Count > 0)
                                //{
                                //    int sttVoucher = 0;
                                //    foreach (var prepaidTrans in model.Vouchers)
                                //    {
                                //        sttVoucher++;
                                //        parameters = new DynamicParameters();
                                //        parameters.Add("Id", Guid.NewGuid(), DbType.String);
                                //        parameters.Add("TransId", key, DbType.String);
                                //        parameters.Add("LineId", sttVoucher);
                                //        parameters.Add("CompanyCode", model.CompanyCode);
                                //        parameters.Add("TransId", prepaidTrans.TransId);
                                //        parameters.Add("PepaidCardNo", prepaidTrans.PepaidCardNo);
                                //        parameters.Add("TransType", prepaidTrans.TransType);
                                //        parameters.Add("MainBalance", prepaidTrans.MainBalance);
                                //        parameters.Add("SubBlance", prepaidTrans.SubBlance);
                                //        parameters.Add("LineTotal", line.LineTotal);
                                //        parameters.Add("DiscountType", line.DiscountType);
                                //        db.Execute("usp_I_T_SalesLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //    }
                                //}
                                if (model.Invoice != null)
                                {
                                    parameters = new DynamicParameters();

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId, DbType.String);
                                    parameters.Add("StoreId", model.StoreId);
                                    parameters.Add("StoreName", model.StoreName);
                                    parameters.Add("CustomerName", model.Invoice.CustomerName);
                                    parameters.Add("Name", model.Invoice.Name);
                                    parameters.Add("TaxCode", model.Invoice.TaxCode);
                                    parameters.Add("Email", model.Invoice.Email);
                                    parameters.Add("Address", model.Invoice.Address);
                                    parameters.Add("Phone", model.Invoice.Phone);
                                    parameters.Add("Remark", model.Invoice.Remark);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    db.Execute("USP_I_T_SalesInvoice", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                }

                                if (model.DataSource == "POS" && model.Status != "H")
                                {
                                    //Tạo Delivery
                                    TSalesDelivery delivery = new TSalesDelivery();
                                    delivery.TransId = key;
                                    delivery.CompanyCode = model.CompanyCode;
                                    delivery.DeliveryFee = 0;
                                    delivery.DeliveryMethod = "Giao tai cua hang";
                                    delivery.DeliveryType = "NONE";
                                    parameters = new DynamicParameters();

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId, DbType.String);
                                    parameters.Add("DeliveryType", delivery.DeliveryType);
                                    parameters.Add("DeliveryMethod", delivery.DeliveryMethod);
                                    parameters.Add("DeliveryFee", delivery.DeliveryFee);

                                    db.Execute("USP_I_T_Sales_Delivery", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    //Tạo Invoice

                                    InvoiceViewModel invoice = new InvoiceViewModel();
                                    invoice = _mapper.Map<InvoiceViewModel>(model);
                                    invoice.TransId = "";
                                    invoice.RefTransId = model.TransId;
                                    foreach (var line in invoice.Lines)
                                    {
                                        line.BaseLine = int.Parse(line.LineId);
                                        line.BaseTransId = model.TransId;
                                    }
                                    foreach (var line in invoice.Payments)
                                    {
                                        line.RefTransId = model.TransId;
                                    }
                                    result = await CreateInvoice(invoice, db, tran);
                                    if (result.Success == false)
                                    {
                                        tran.Rollback();
                                        return result;
                                    }

                                }
                                else
                                {
                                    if (model.Deliveries != null && model.Deliveries.Count > 0)
                                    {
                                        foreach (var delivery in model.Deliveries)
                                        {
                                            //TSalesDelivery delivery = new TSalesDelivery();
                                            //delivery.TransId = key;
                                            //delivery.CompanyCode = model.CompanyCode;
                                            //delivery.DeliveryFee = 0;
                                            //delivery.DeliveryMethod = "Giao tai cua hang";
                                            //delivery.DeliveryType = "NONE";
                                            parameters = new DynamicParameters();

                                            parameters.Add("CompanyCode", model.CompanyCode);
                                            parameters.Add("TransId", model.TransId, DbType.String);
                                            parameters.Add("DeliveryType", delivery.DeliveryType);
                                            parameters.Add("DeliveryMethod", delivery.DeliveryMethod);
                                            parameters.Add("DeliveryFee", delivery.DeliveryFee);
                                            db.Execute("USP_I_T_Sales_Delivery", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        }
                                    }
                                }


                                result.Success = true;
                                result.Message = key;
                                tran.Commit();


                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //throw new NotImplementedException();
        }
        public Document MapSOtoDocument(SaleViewModel model)
        {
            //GenericResult result = new GenericResult();

            Document document = new Document();
            document.DocDate = DateTime.Now;
            document.CardCode = model.CusId;
            document.TransId = model.TransId;
            document.StoreId = model.StoreId;
            document.CardName = model.CusName;
            document.NumAtCard = model.Phone;

            //document.DocDate = DateTime.;
            //document.CardGroup = basket.customer.customerGrpId;
            // currencyCode: string="";
            // storeType: string="";
            // listType: string="";
            // formatConfigId: string="";
            // whsCode: string="";
            //document.DocCurrency = model.Cu;
            document.UCompanyCode = model.CompanyCode;
            document.DocumentLines = new List<DocumentLine>();
            document.UCreatedBy = model.CreatedBy;
            foreach (var line in model.Lines)
            {
                DocumentLine lineDo = new DocumentLine();
                lineDo.ItemCode = line.ItemCode;
                lineDo.Quantity = (double)line.Quantity;
                lineDo.DiscountPercent = line.DiscountRate == null ? 0 : (double)line.DiscountRate;
                //lineDo.Currency = line.Cu
                lineDo.UnitPrice = (double)line.Price;
                lineDo.UoMCode = line.UomCode;
                lineDo.BarCode = line.BarCode;
                lineDo.LineTotal = (double)line.LineTotal;

                lineDo.ItemType = line.ItemType;
                lineDo.PrepaidCardNo = line.PrepaidCardNo;
                lineDo.MemberValue = line.MemberValue;
                lineDo.StartDate = line.StartDate;
                lineDo.EndDate = line.EndDate;

                document.DocumentLines.Add(lineDo);
            }

            return document;
        }

        public async Task<string> CheckPaymentList<T>(IEnumerable<T> l1, IEnumerable<T> l2)
        {
            string resultStr = "";
            foreach (var item in l2)
            {
                if (!l1.Contains(item))
                {
                    resultStr += item + ",";

                }
            }
            if (resultStr.Length > 0)
            {
                resultStr = resultStr.Substring(0, resultStr.Length - 1);
            }
            return resultStr;
        }
        public class ResultCheck
        {
            public decimal? TotalAmount { get; set; }
            public decimal? SumLine { get; set; }
            public decimal? SumQty { get; set; }
        }

        public async Task<string> CheckOrderData(string CompanyCode, string StoreId, string TransId, decimal? TotalAmount, decimal? LinesCount, decimal? QuantitySum)
        {
            string result = "";
            using (IDbConnection db = _saleHeaderRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string queryGetDatOrderCheck = $"select top 1 TotalAmount, Count(t2.LineId) SumLine, SUM(t2.Quantity) SumQty " +
                       $" from T_SalesHeader t1 left join T_SalesLine t2 on t1.CompanyCode = t2.CompanyCode and t1.StoreId = t2.StoreId and t1.TransId = t2.TransId " +
                       $" where t1.TransId = N'{TransId}' and t1.CompanyCode = N'{CompanyCode}' and t1.StoreId = N'{StoreId}' " +
                       $" group by t1.TotalAmount ";
                    var queryGetDatOrderCheckData = await db.QueryAsync<ResultCheck>(queryGetDatOrderCheck, null, commandType: CommandType.Text, commandTimeout: 3600);
                    //var queryGetDatOrderCheckData = await db.GetAsync(queryGetDatOrderCheck, null, commandType: CommandType.Text);
                    if (queryGetDatOrderCheckData != null)
                    {
                        var dataX = queryGetDatOrderCheckData as ResultCheck;
                        if (dataX !=null && dataX.TotalAmount == TotalAmount && dataX.SumLine == LinesCount && dataX.SumQty == QuantitySum)
                        {
                            result = TransId;
                        }
                    }

                }
                catch (Exception ex)
                {
                    result = "";
                    //result.Data = failedlist;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            //result.Data = failedlist;
            return result;


        }
        public async Task<GenericResult> CheckBasketId(string CompanyCode, string StoreId, string OrderId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var OrderIdparameters = new DynamicParameters();
                OrderIdparameters.Add("CompanyCode", CompanyCode);
                OrderIdparameters.Add("StoreId", StoreId);
                OrderIdparameters.Add("OrderId", OrderId);

                var socheck = await _saleHeaderRepository.GetAllAsync("USP_S_T_SalesHeaderByOrderId", OrderIdparameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                if (socheck != null && socheck.Count > 0)
                {
                    var firstTran = socheck.FirstOrDefault();
                    result.Success = true;
                    result.Message = firstTran.TransId;
                }
                
            }
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        } 
        public async Task<GenericResult> CreateSaleOrder(SaleViewModel model)
        {
            GenericResult result = new GenericResult();
            //var jsonM = model.ToJson();
            //RPFO.Utilities.Helpers.LogUtils.WriteLogData("C:\\RPFO.API.Log\\", "SalesOrders", "CreateSO", model.ToJson());
            try
            {
                if (string.IsNullOrEmpty(model.DataSource))
                {
                    result.Success = false;
                    result.Message = "Data Source can't null.";
                    return result;
                }
                if(model.Payments!= null && model.Payments.Count > 0)
                {
                    var paymentOfStore = await _paymentService.GetByStore(model.CompanyCode, model.StoreId);
                    if (paymentOfStore != null && paymentOfStore.Success)
                    {
                        var payments = paymentOfStore.Data as List<StorePaymentViewModel>;
                        if (payments != null && payments.Count > 0)
                        {
                            var PaymentInStore = payments.Select(x => x.PaymentCode).Distinct();
                            var PaymentInData = model.Payments.Select(x => x.PaymentCode).Distinct();
                            string checkStr = await CheckPaymentList(PaymentInStore, PaymentInData);
                            if (!string.IsNullOrEmpty(checkStr))
                            {
                                result.Success = false;
                                result.Message = "Payments:" + checkStr + " not in existed in Store: " + model.StoreId;
                                return result;
                            }

                            if(model.IsCanceled == "C" || model.IsCanceled == "Y")
                            {
                                foreach(var payment in model.Payments)
                                {
                                    var paymentMaster = payments.Where(x => x.PaymentCode == payment.PaymentCode).FirstOrDefault();
                                    if(paymentMaster!=null)
                                    {
                                        if(paymentMaster.RejectVoid == true)
                                        {
                                            result.Success = false;
                                            result.Message = "Payments: " + paymentMaster.PaymentDesc + " reject void.";
                                            return result;
                                        }    
                                    }    
                                }    
                            }    
                        }
                    }
                }    
               
                var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                if (settingData.Success)
                {

                    SettingList = settingData.Data as List<GeneralSettingStore>;


                }

                //  Minh đã xin phép Lập    (2021-11-23)
                if (string.IsNullOrEmpty(model.TransId) && model.DataSource == "GRAB")
                {
                    //model.Status = "C";
                    //model.CollectedStatus = "Closed";
                    var Ecomparameters = new DynamicParameters();
                    Ecomparameters.Add("CompanyCode", model.CompanyCode);
                    Ecomparameters.Add("OMSId", model.OMSId);
                    string socheck = _saleHeaderRepository.GetScalar("USP_GetTransIdByOMSId", Ecomparameters, commandType: CommandType.StoredProcedure);
                    if (!string.IsNullOrEmpty(socheck))
                    {
                        model.TransId = socheck;
                    }
                }

                if (model.DataSource.ToLower() == "pos" && model.Status.ToLower() != "h" && model.Status.ToLower() != "hold")
                {
                    var Data = await _shiftService.GetByCode(model.CompanyCode, model.ShiftId);
                    var shift = Data.Data as ShiftViewModel;
                    if (shift == null || shift.Status == "C")
                    {
                        result.Success = false;
                        result.Message = "Shift " + model.ShiftId + " not exist or has closed.";
                        return result;
                    }
                    else
                    {
                        //var created = shift.CreatedOn.Value.ToString("yyyy/MM/dd");
                        //var now = DateTime.Now.ToString("yyyy/MM/dd");
                        //if (DateTime.Parse(created) > DateTime.Parse(now))
                        //{
                        //    result.Success = false;
                        //    result.Message = "Can't completed order. Shift " + model.ShiftId + " not valid. Please logout";
                        //    return result;
                        //}

                    }
                    var salesZeroValue = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "SalesZeroValue").FirstOrDefault();
                    if (salesZeroValue != null && (salesZeroValue.SettingValue == "false" || salesZeroValue.SettingValue == "0"))
                    {
                        var checkItem = model.Lines.Where(x => x.Price == 0 && x.IsPromo != "1" && (x.BomId == null || x.BomId == "")).ToList();
                        if (checkItem != null && checkItem.Count > 0)
                        {
                            result.Success = false;
                            result.Message = "Can't completed order. Price of item " + checkItem[0].ItemName + " invalid";
                            return result;
                        }
                    }

                    //decimal TotalDiscount = model.TotalDiscountAmt ?? 0;
                    //decimal DiscountInLine = 0;
                    //foreach(var line in model.Lines.Where(x=>  x.IsPromo != "1"  && (string.IsNullOrEmpty(x.BomId) ? "" : x.BomId) == ""))
                    //{
                    //    DiscountInLine += line.LineTotalDisIncludeHeader.HasValue ? line.LineTotalDisIncludeHeader.Value : 0;
                    //}    
                    //if(TotalDiscount != DiscountInLine)
                    //{
                    //    result.Success = false;
                    //    result.Message = "Total discount invalid. Please check discount in bill or reorder. Thank you.";
                    //    return result;
                    //}    

                }
                if (string.IsNullOrEmpty(model.POSType))
                {
                    model.POSType = "";
                }
                //if (model.POSType != "E" && (model.DataSource.ToLower() == "pos" && model.Status != "H") || (model.DataSource.ToLower() != "pos" && model.POSType != "E"))
                if (model.DataSource.ToLower() == "pos" && string.IsNullOrEmpty(model.ContractNo) && model.Status.ToLower() != "h" && model.Status.ToLower() != "hold")
                {
                    if (model.TotalPayable != 0)
                    {
                        if (model.Payments == null || model.Payments.Count <= 0)
                        {
                            result.Success = false;
                            result.Message = "Payments can't null. Please check bill payments.";
                            return result;
                        }

                    }
                }

                if (model.CompanyCode == null || string.IsNullOrEmpty(model.CompanyCode))
                {
                    result.Success = false;
                    result.Message = "Company Code cannot null.";
                    return result;
                }
                if (model.Lines == null || model.Lines.Count() == 0)
                {
                    result.Success = false;
                    result.Message = "Doc line cannot null.";
                    return result;
                }
                if (model.StoreId == null || string.IsNullOrEmpty(model.StoreId))
                {
                    result.Success = false;
                    result.Message = "Store cannot null.";
                    return result;
                }
                if (string.IsNullOrEmpty(model.CusId))
                {
                    result.Success = false;
                    result.Message = "Customer cannot null.";
                    return result;
                }
                if (string.IsNullOrEmpty(model.SalesMode))
                {
                    result.Success = false;
                    result.Message = "SalesMode cannot null.";
                    return result;
                }
                if (model.Payments != null && model.Payments.Count > 0 && model.SalesMode.ToLower() != "return" && model.SalesMode.ToLower() != "ex")
                {
                    if (model.IsCanceled != "C")
                    {
                        var salesLineNegative = model.Lines.Where(x => !x.AllowSalesNegative.HasValue ? false : x.AllowSalesNegative == true).ToList();
                        if(salesLineNegative.Count <= 0)
                        {
                            var payment = model.Payments.Where(x => x.CollectedAmount <= 0).ToList();
                            if (payment != null && payment.Count > 0)
                            {
                                result.Success = false;
                                result.Message = "Please complete progress payment. Can't payment with value 0";
                                return result;
                            }
                        }    
                        
                    }

                }

                if (!string.IsNullOrEmpty(model.OrderId.ToString()))
                {
                    var OrderIdparameters = new DynamicParameters();
                    OrderIdparameters.Add("CompanyCode", model.CompanyCode);
                    OrderIdparameters.Add("StoreId", model.StoreId);
                    OrderIdparameters.Add("OrderId", model.OrderId);

                    var socheck = await _saleHeaderRepository.GetAllAsync("USP_S_T_SalesHeaderByOrderId", OrderIdparameters, commandType: CommandType.StoredProcedure);
                    if (socheck != null && socheck.Count > 0)
                    {
                        var firstTran = socheck.FirstOrDefault();
                        if (firstTran != null)
                        {
                            result.Success = false;
                            result.Message = "Order has been created or processing. Trans Id: " + firstTran.TransId + " Order Id: " + model.OrderId;
                            string checkTransId = await CheckOrderData(model.CompanyCode, model.StoreId, model.TransId, model.TotalAmount, model.Lines.Count(), model.Lines.Sum(x => x.Quantity));
                            if (!string.IsNullOrEmpty(checkTransId))
                            {
                                result.Data = checkTransId;
                            }
                            string Folder = Path.Combine(
                             Directory.GetCurrentDirectory(),
                             "wwwroot", "Logs");
                            if (!Directory.Exists(Folder))
                                Directory.CreateDirectory(Folder);
                            string filename = firstTran.TransId + "Double";
                            var path = Path.Combine(
                                       Directory.GetCurrentDirectory(),
                                       "wwwroot/Logs", "");
                            LogUtils.WriteLogData(path, "", filename, model.ToJson());
                            return result;
                        }

                    }
                }
                if (model.SalesMode.ToLower() == "return" || ((model.SalesMode.ToLower() == "ex" || model.SalesMode.ToLower() == "exchange") && model.TotalPayable < 0))
                {
                    decimal? numOfPayment = 0;
                    foreach (var line in model.Payments)
                    {
                        numOfPayment += line.CollectedAmount;
                    }
                    if (Math.Abs((decimal)numOfPayment) != Math.Abs((decimal)model.TotalPayable))
                    {
                        result.Success = false;
                        result.Message = "Please check return amount. Return amount can't different collected amount.";
                        return result;
                    }

                }
                //if (model.SalesMode.ToLower() == "return")
                //{
                //    decimal? numOfPayment = 0;
                //    foreach (var line in model.Payments)
                //    {
                //        numOfPayment += line.CollectedAmount;
                //    }
                //    if (Math.Abs((decimal)model.TotalPayable) > Math.Abs((decimal)numOfPayment) )
                //    {
                //        result.Success = false;
                //        result.Message = "Please check return amount. Total amount can't less than return amount.";
                //        return result;
                //    }
                //    //var ItemCannotCancel = model.Lines.Where(line => string.IsNullOrEmpty( line.ItemType.ToLower() == "pn" || line.ItemType.ToLower() == "tp" || line.ItemType.ToLower() == "pin" || line.ItemType.ToLower() == "bp" || line.ItemType.ToLower() == "topup");
                //    //if (ItemCannotCancel != null && ItemCannotCancel.Count() > 0)
                //    //{
                //    //    result.Success = false;
                //    //    result.Message = "Order (PIN/TopUp) can't return.";
                //    //    return result;
                //    //}
                //}
                if(string.IsNullOrEmpty(model.IsCanceled))
                {
                    model.IsCanceled = "N";
                }    
                if (model.IsCanceled == "C" && model.Status != "H" && model.Status != "Hold")
                {
                    string userCheck = model.ApprovalId ?? model.CreatedBy;
                    var checkCancel = await  _permissionService.CheckFunctionByUserName(model.CompanyCode, userCheck, "Spc_CancelOrder", "", "I");
                    if(checkCancel!=null && checkCancel.Success)
                    {

                    }   
                    else
                    {
                        return checkCancel;
                    }    
                }
                if (model.DataSource.ToLower() != "pos")
                {
                    if (string.IsNullOrEmpty(model.TotalAmount.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Amount. Total Amount can't null";
                        return result;
                    }
                    if (string.IsNullOrEmpty(model.TotalPayable.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Payable. Total Payable can't null";
                        return result;
                    }
                    if (string.IsNullOrEmpty(model.TotalDiscountAmt.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Discount Amount. Total Discount Amount can't null";
                        return result;
                    }
                    if (string.IsNullOrEmpty(model.TotalReceipt.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Receipt. Total Receipt can't null";
                        return result;
                    }
                    if (string.IsNullOrEmpty(model.PaymentDiscount.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Payment Discount. Payment Discount can't null";
                        return result;
                    }
                    if (string.IsNullOrEmpty(model.TotalTax.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Tax. Total Tax can't null";
                        return result;
                    }
                    //var listLine = model.Lines.Where(x => x.IsSerial == true || x.IsVoucher == true).ToList();
                    //if(listLine!=null && listLine.Count > 0)
                    //{
                    //    var checkValidSerialNum = listLine.Where(x=>x.lin)
                    //}    
                    if (model.Lines != null)
                    {
                        var qtyCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.Quantity.ToString()) || x.Quantity == 0).ToList();
                        string mes = "";
                        if (qtyCheck != null && qtyCheck.Count > 0)
                        {
                            foreach (var line in qtyCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Quantity of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }
                        var priceCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.Price.ToString())).ToList();

                        if (priceCheck != null && priceCheck.Count > 0)
                        {
                            foreach (var line in priceCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Price of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }
                        var lineTotalCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.LineTotal.ToString())).ToList();

                        if (lineTotalCheck != null && lineTotalCheck.Count > 0)
                        {
                            foreach (var line in lineTotalCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Line Total of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }



                        var itemCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.ItemCode.ToString())).ToList();

                        if (itemCodeCheck != null && itemCodeCheck.Count > 0)
                        {
                            result.Success = false;
                            result.Message = "Please Input Item Code";
                            return result;
                        }
                        var DiscountRateCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.DiscountRate.ToString())).ToList();

                        if (DiscountRateCheck != null && DiscountRateCheck.Count > 0)
                        {
                            foreach (var line in DiscountRateCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Discount Rate of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }

                        var TaxRateCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.TaxRate.ToString())).ToList();

                        if (TaxRateCheck != null && TaxRateCheck.Count > 0)
                        {
                            foreach (var line in TaxRateCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Tax Rate of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }
                        var TaxAmtCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.TaxAmt.ToString())).ToList();

                        if (TaxAmtCheck != null && TaxAmtCheck.Count > 0)
                        {
                            foreach (var line in TaxAmtCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Tax Amt of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }
                        //var TaxCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.TaxCode)).ToList();

                        //if (TaxCodeCheck != null && TaxCodeCheck.Count > 0)
                        //{
                        //    foreach (var line in TaxCodeCheck)
                        //    {
                        //        mes += line.ItemCode + ", ";
                        //    }
                        //    if (!string.IsNullOrEmpty(mes))
                        //    {
                        //        result.Success = false;
                        //        result.Message = "Please Input Tax Code of items: " + mes.Substring(0, mes.Length - 2);
                        //        return result;
                        //    }
                        //}



                        var UOMCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.UomCode)).ToList();

                        if (UOMCodeCheck != null && UOMCodeCheck.Count > 0)
                        {
                            foreach (var line in UOMCodeCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input UOM Code of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }
                    }

                }
                if (model.Lines != null)
                {
                    var qtyCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.Quantity.ToString()) || x.Quantity == 0).ToList();
                    string mes = "";
                    if (qtyCheck != null && qtyCheck.Count > 0)
                    {
                        foreach (var line in qtyCheck)
                        {
                            mes += line.ItemCode + ", ";
                        }
                        if (!string.IsNullOrEmpty(mes))
                        {
                            result.Success = false;
                            result.Message = "Please Input Quantity of items: " + mes.Substring(0, mes.Length - 2);
                            return result;
                        }
                    }
                }
                if (model.IsCanceled != "C")
                {
                    var TaxCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.BomId) && string.IsNullOrEmpty(x.TaxCode)).ToList();

                    if (TaxCodeCheck != null && TaxCodeCheck.Count > 0)
                    {
                        string mes = "";
                        foreach (var line in TaxCodeCheck)
                        {
                            mes += line.ItemCode + ", ";
                        }
                        if (!string.IsNullOrEmpty(mes))
                        {
                            result.Success = false;
                            result.Code = 701;
                            result.Message = "Tax Code of items: " + mes.Substring(0, mes.Length - 2) + " null.";
                            return result;
                        }
                    }
                }


                if (model.DataSource.ToLower() == "pos" && model.IsCanceled != "C" && model.Payments != null && model.Payments.Count > 0)
                {
                    decimal? numOfPayment = 0;
                    decimal? numOfLine = 0;
                    foreach (var line in model.Payments)
                    {
                        if (string.IsNullOrEmpty(line.PaymentMode))
                        {
                            line.PaymentMode = "";
                        }
                    }

                    foreach (var line in model.Payments.Where(x => x.PaymentMode.ToLower() != "change"))
                    {
                        numOfPayment += line.CollectedAmount;// * (line.Rate ?? 1);
                    }
                    var lineExcludeBOM = model.Lines.Where(x => string.IsNullOrEmpty(x.BomId) && x.IsPromo != "Y" && x.IsPromo != "1");
                    decimal discountLine = 0;
                    foreach (var line in lineExcludeBOM)
                    {
                        if(!string.IsNullOrEmpty(line.DiscountType) && !line.DiscountType.Contains("Bonus"))
                        {
                            decimal discountNum = line.DiscountAmt == null ? 0 : (decimal)line.DiscountAmt;
                            //line.LineTotal.Value
                            decimal linetotal = line.Quantity.Value * line.Price.Value;
                            var lineDiscount = Math.Abs(line.DiscountAmt ?? 0);
                            if (model.SalesMode.ToLower() == "return")
                            {
                                numOfLine += Math.Abs(linetotal) - Math.Abs(discountNum);
                            }
                            else
                            {
                                if (linetotal < 0)
                                {
                                    numOfLine += -(Math.Abs(linetotal) - Math.Abs(discountNum));
                                    lineDiscount = -lineDiscount;
                                }
                                else
                                {
                                    numOfLine += linetotal - Math.Abs(discountNum);
                                }

                            }
                            discountLine += lineDiscount;
                        }    
                       
                    }
                    decimal discountTotal = model.TotalDiscountAmt == null ? 0 : (decimal)model.TotalDiscountAmt;
                    decimal roudingoff = model.RoundingOff ?? 0;
                    // Sửa - thành + để test xem sao
                    decimal totalPayable = Math.Abs((decimal)numOfLine) - Math.Abs(discountTotal) + roudingoff;
                    decimal maxPercent = totalPayable - (totalPayable * 5) / 100;
                    if (Math.Abs((decimal)numOfPayment) < maxPercent) //&& model.SalesMode.ToLower() != "return"
                    {
                        result.Success = false;
                        result.Message = "501: Please check bill and amount. Collected Amount: " + numOfPayment + ", Total Payable " + totalPayable;
                        return result;
                    }
                    decimal discountCheck = Math.Round(discountLine + Math.Abs(model.DiscountAmount ?? 0), 0);
                    if ((Math.Round(Math.Abs(discountTotal), 0) + 1) < discountCheck)
                    {
                        result.Success = false;
                        result.Message = "502: Please check bill and amount. Discount Total: " + discountTotal + ", Discount Line " + discountCheck;
                        return result;
                    }
                    //if (model.SalesMode.ToLower() == "return")
                    //{

                    //}    
                }
               
                if (model.DataSource.ToLower() == "pos" && model.IsCanceled != "C" && string.IsNullOrEmpty(model.ContractNo))
                {
                    decimal roudingoff = model.RoundingOff ?? 0;
                    var TotalAmount = Math.Abs((model.TotalAmount ?? 0) - roudingoff) ;
                    var TotalDiscount = Math.Abs(model.TotalDiscountAmt.Value);
                    var TotalPayable = Math.Abs(model.TotalPayable.Value);
                    decimal perTotalPayable = Math.Abs(model.TotalPayable.Value) * 5 / 100;


                    if (TotalAmount - TotalDiscount > TotalPayable + perTotalPayable && model.SalesMode.ToLower() == "sales" && model.Status != "H")
                    {
                        result.Success = false;
                        result.Message = "101: Please check your receipt amount. Total Amount: " + model.TotalAmount.Value.ToString("C2") + " Total Discount Amt: " + model.TotalDiscountAmt.Value.ToString("C2") + " TotalPayable: " + model.TotalPayable.Value.ToString("C2") + " TotalReceipt " + model.TotalReceipt.Value.ToString("C2");
                        return result;
                    }
                    if (model.TotalPayable > model.TotalReceipt && model.SalesMode.ToLower() == "sales" && model.Status != "H")
                    {
                        result.Success = false;
                        result.Message = "102: Please check your receipt amount. Total Payable: " + model.TotalPayable.Value.ToString("C2") + " Total Receipt: " + model.TotalReceipt.Value.ToString("C2") + " TotalPayable: " + model.TotalPayable.Value.ToString("C2") + " TotalReceipt " + model.TotalReceipt.Value.ToString("C2");
                        return result;
                    }
                    if (TotalAmount - TotalDiscount > TotalPayable && model.SalesMode.ToLower() != "sales" && model.Status == "H")
                    {
                        result.Success = false;
                        result.Message = "103: Please check your receipt amount. Total Amount: " + model.TotalAmount.Value.ToString("C2") + " Total Discount Amt: " + model.TotalDiscountAmt.Value.ToString("C2") + "  TotalPayable: " + model.TotalPayable.Value.ToString("C2") + " TotalReceipt " + model.TotalReceipt.Value.ToString("C2");
                        return result;
                    }
                    //if (model.Payments == null || (model.SalesMode.ToLower() == "sales" && model.Status != "H" && model.Payments.Count() == 0) || (model.Payments.Count == 0 && model.SalesMode.ToLower() != "sales" && model.Status != "H" && model.SalesMode != "EX"))
                    //{
                    //    result.Success = false;
                    //    result.Message = "Payment list not null.";
                    //    return result;
                    //}
                }
                
                List<string> holdList = new List<string>();
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {

                            string key = "";
                            try
                            {
                                var parameters = new DynamicParameters();
                                if ((model.DataSource.ToLower() != "pos" && model.POSType.ToLower() == "e"))
                                {
                                    foreach (var line in model.Lines)
                                    {
                                        string getItemType = $"select CustomField1 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                        string itemType = _saleHeaderRepository.GetScalar(getItemType, null, commandType: CommandType.Text);
                                        if (string.IsNullOrEmpty(itemType))
                                        {
                                            result.Success = false;
                                            result.Message = "Please check master data (Item Type ) with your admin. Item " + line.ItemCode;
                                            return result;
                                        }
                                        else
                                        {
                                            if (model.POSType.ToLower() == "e" && (itemType.ToLower() == "class" || itemType.ToLower() == "member" || itemType.ToLower() == "voucher" || itemType.ToLower() == "card"))
                                            {
                                                result.Success = false;
                                                result.Message = "Event can't order item " + line.ItemCode + " b/c item in " + itemType.ToLower() + " group";
                                                return result;
                                            }
                                        }
                                        //if()
                                    }
                                    //var ItemBan = model.Lines.Where(x=>x.ItemType)
                                }
                                if(string.IsNullOrEmpty(model.IsCanceled))
                                {
                                    model.IsCanceled = "N";
                                }    
                                if (model.SalesMode.ToLower() == "return" || model.IsCanceled.ToLower() == "c")
                                {
                                    var ItemCannotCancel = model.Lines.Where(line => !string.IsNullOrEmpty(line.ItemType) && (line.ItemType.ToLower() == "pn" || line.ItemType.ToLower() == "tp" || line.ItemType.ToLower() == "pin" || line.ItemType.ToLower() == "bp" || line.ItemType.ToLower() == "topup"));
                                    if (ItemCannotCancel != null && ItemCannotCancel.Count() > 0)
                                    {
                                        result.Success = false;
                                        result.Message = "Order (PIN/TopUp) can't cancel / return.";
                                        return result;
                                    }

                                }

                                if (model.IsCanceled == "C" && (model.Status.ToLower() == "h" || model.Status.ToLower() == "hold"))
                                {
                                    //xxx
                                    //Status = 'C',
                                    string queryUpdate = $"Update T_SalesHeader set  IsCanceled = N'Y', CollectedStatus = N'Canceled' , ModifiedBy= N'{model.CreatedBy}' , ModifiedOn= N'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' where TransId=N'{model.RefTransId}' and CompanyCode=N'{model.CompanyCode}' and StoreId=N'{model.StoreId}'";
                                    db.Execute(queryUpdate, null, commandType: CommandType.Text, transaction: tran);
                                    result.Success = true;
                                    result.Message = model.RefTransId;
                                    result.Data = model;
                                    tran.Commit();
                                    return result;
                                }

                                foreach (var line in model.Lines)
                                {
                                    string getItemType = $"select CustomField1 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                    string itemType = _saleHeaderRepository.GetScalar(getItemType, null, commandType: CommandType.Text);
                                    if (string.IsNullOrEmpty(itemType))
                                    {
                                        result.Success = false;
                                        result.Message = "Please check master data (Item Type ) with your admin. Item " + line.ItemCode;
                                        return result;
                                    }
                                    else
                                    {
                                        if (itemType.ToLower() == "class" || itemType.ToLower() == "member")
                                        {
                                            if (string.IsNullOrEmpty(line.StartDate.ToString()) || string.IsNullOrEmpty(line.EndDate.ToString()))
                                            {
                                                result.Success = false;
                                                result.Message = "Member Class Start Date / End Date can't null";
                                                return result;
                                            }

                                        }
                                        else if (itemType.ToLower() == "card")
                                        {
                                            if (string.IsNullOrEmpty(line.StartDate.ToString()) || string.IsNullOrEmpty(line.EndDate.ToString()))
                                            {
                                                result.Success = false;
                                                result.Message = "Card Member Start Date / End Date can't null";
                                                return result;
                                            }

                                        }

                                        string getCapacity = $"select CustomField8 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                        var capaValue = _saleHeaderRepository.GetScalar(getCapacity, null, commandType: CommandType.Text);
                                        if (!string.IsNullOrEmpty(capaValue))
                                        {
                                            if (string.IsNullOrEmpty(line.TimeFrameId))
                                            {
                                                result.Success = false;
                                                result.Message = "Time Frame Id can't null";
                                                return result;
                                            }
                                            if (string.IsNullOrEmpty(line.AppointmentDate.ToString()))
                                            {
                                                result.Success = false;
                                                result.Message = "Appointment Date can't null";
                                                return result;
                                            }

                                            if (!string.IsNullOrEmpty(line.StoreAreaId))
                                            {
                                                string queryCheckStoreArea = $" [USP_CheckStoreAreaInStoreCapacity] N'{model.CompanyCode}', N'{model.StoreId}',N'{line.StoreAreaId}'";
                                                var AreaCount = _saleHeaderRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
                                                if (AreaCount == "0")
                                                {
                                                    result.Success = false;
                                                    result.Message = "Store Area Id does not match Store Capacity. Please check your data input";
                                                    return result;
                                                }
                                            }
                                            if (string.IsNullOrEmpty(line.StoreAreaId))
                                            {
                                                string queryCheckStoreArea = $" [USP_S_StoreAreaIdByStore] N'{model.CompanyCode}', N'{model.StoreId}'";
                                                var AreaId = _saleHeaderRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
                                                if (string.IsNullOrEmpty(AreaId))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Store Area Id can't null. Please check capacity setup";
                                                    return result;
                                                }
                                                else
                                                {
                                                    line.StoreAreaId = AreaId;
                                                }

                                            }
                                        }
                                    }


                                }

                                if (!string.IsNullOrEmpty(model.TransId))
                                {

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.TransId;
                                }
                                else
                                {
                                    key = _saleHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixSO}',N'{model.CompanyCode}', N'{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.TransId = key;
                                }


                                if (model.DataSource.ToLower() != "pos")
                                {
                                    var Ecomparameters = new DynamicParameters();
                                    Ecomparameters.Add("CompanyCode", model.CompanyCode);
                                    Ecomparameters.Add("EcomId", model.OMSId);
                                    if (string.IsNullOrEmpty(model.IsCanceled))
                                    {
                                        model.IsCanceled = "N";
                                    }
                                    if (model.IsCanceled != "C")
                                    {
                                        var socheck = await _saleHeaderRepository.GetAllAsync("USP_S_T_SalesEcom", Ecomparameters, commandType: CommandType.StoredProcedure);
                                        if (socheck != null && socheck.Count > 0)
                                        {
                                            result.Success = false;
                                            result.Message = $"Can't add order. {model.RefTransId} has existed. POS ID: " + socheck.FirstOrDefault().TransId;
                                            return result;
                                        }
                                    }

                                    List<TSalesLineViewModel> ListA = new List<TSalesLineViewModel>();
                                    foreach (var line in model.Lines)
                                    {
                                        string getItemType = $"select CustomField1 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                        string itemType = _saleHeaderRepository.GetScalar(getItemType, null, commandType: CommandType.Text);
                                        if (string.IsNullOrEmpty(itemType))
                                        {
                                            result.Success = false;
                                            result.Message = "Please check master data (Item Type ) with your admin. Item " + line.ItemCode;
                                            return result;
                                        }
                                        else
                                        {
                                            if (itemType.ToLower() == "class" || itemType.ToLower() == "member")
                                            {
                                                if (string.IsNullOrEmpty(line.StartDate.ToString()) || string.IsNullOrEmpty(line.EndDate.ToString()))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Member Class StartDate/EndDate can't null";
                                                    return result;
                                                }
                                            }
                                            else if (itemType.ToLower() == "card")
                                            {
                                                if (string.IsNullOrEmpty(line.StartDate.ToString()) || string.IsNullOrEmpty(line.EndDate.ToString()))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Card Member StartDate/EndDate can't null";
                                                    return result;
                                                }

                                            }

                                            string getCapacity = $"select CustomField8 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                            var capaValue = _saleHeaderRepository.GetScalar(getCapacity, null, commandType: CommandType.Text);
                                            if (!string.IsNullOrEmpty(capaValue))
                                            {
                                                if (string.IsNullOrEmpty(line.TimeFrameId))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Time Frame Id can't null";
                                                    return result;
                                                }
                                                if (string.IsNullOrEmpty(line.AppointmentDate.ToString()))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Appointment Date can't null";
                                                    return result;
                                                }

                                                if (!string.IsNullOrEmpty(line.StoreAreaId))
                                                {
                                                    string queryCheckStoreArea = $" [USP_CheckStoreAreaInStoreCapacity] N'{model.CompanyCode}', N'{model.StoreId}',N'{line.StoreAreaId}'";
                                                    var AreaCount = _saleHeaderRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
                                                    if (AreaCount == "0")
                                                    {
                                                        result.Success = false;
                                                        result.Message = "Store Area Id does not match Store Capacity. Please check your data input";
                                                        return result;
                                                    }
                                                }
                                                if (string.IsNullOrEmpty(line.StoreAreaId))
                                                {
                                                    string queryCheckStoreArea = $" [USP_S_StoreAreaIdByStore] N'{model.CompanyCode}', N'{model.StoreId}'";
                                                    var AreaId = _saleHeaderRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
                                                    if (string.IsNullOrEmpty(AreaId))
                                                    {
                                                        result.Success = false;
                                                        result.Message = "Store Area Id can't null. Please check capacity setup";
                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        line.StoreAreaId = AreaId;
                                                    }

                                                }
                                            }
                                        }

                                        var bomLines = await getBOMLine(line.ItemCode);

                                        if (bomLines != null && bomLines.Count > 0)
                                        {
                                            foreach (var bomLineX in bomLines)
                                            {
                                                TSalesLineViewModel salesLine = new TSalesLineViewModel();
                                                salesLine.ItemCode = bomLineX.ItemCode;
                                                salesLine.Description = bomLineX.ItemName;
                                                salesLine.Price = 0;
                                                salesLine.UomCode = bomLineX.UomCode;
                                                salesLine.Quantity = line.Quantity * bomLineX.Quantity;
                                                salesLine.LineTotal = 0;
                                                salesLine.Status = "C";
                                                salesLine.BomId = line.ItemCode;
                                                ListA.Add(salesLine);
                                            }

                                        }
                                    }

                                    model.Lines.AddRange(ListA);
                                }

                              
                                if (model.IsCanceled == "C")
                                {
                                    string querycheck = $"select isnull(count(*),0) from T_SalesHeader with (nolock) where RefTransId = N'{model.RefTransId}' and CompanyCode = N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'";
                                    string num = _saleHeaderRepository.GetScalar(querycheck, null, commandType: CommandType.Text);
                                    if (int.Parse(num) > 0)
                                    {
                                        result.Success = false;
                                        result.Message = "Can't cancel order. Because the order have return/exchange.";
                                        return result;
                                    }
                                    var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "VoidOrder").FirstOrDefault();
                                    if (setting != null && setting.SettingValue == "BeforeSyncData")
                                    {
                                        var orderGetData = await GetOrderById(model.RefTransId, model.CompanyCode, model.StoreId);
                                        var orderGet = orderGetData.Data as SaleViewModel;
                                        if (orderGet.SyncMWIStatus == "Y")
                                        {
                                            result.Success = false;
                                            result.Message = "The order cannot be canceled because the order has been synced with MWI.";
                                            return result;
                                        }
                                    }
                                }

                                if (model.SalesMode != null && model.SalesMode == "Return")
                                {
                                    string checkResult = _saleHeaderRepository.GetScalar($"USP_Check_ReturnOrder N'{model.CompanyCode}', N'{model.StoreId}', N'{model.TransId}',N'{model.SalesType}',N'{model.SalesMode}'", null, commandType: CommandType.Text);
                                    if (checkResult == "0")
                                    {
                                        result.Success = false;
                                        result.Message = "Can't return order. Because the order date is not valid.";
                                        return result;
                                    }
                                }
                                string itemList = "";
                                string defaultWhs = _saleHeaderRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);

                                foreach (var line in model.Lines)
                                {
                                    if (string.IsNullOrEmpty(line.SlocId))
                                    {
                                        line.SlocId = defaultWhs;
                                    }
                                    //if(string.IsNullOrEmpty(line.BomId))
                                    //{
                                    if (string.IsNullOrEmpty(line.TimeFrameId) && line.Quantity > 0)
                                    {
                                        itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                    }
                                    //}     
                                }
                                if (model.SalesMode.ToLower() != "return")
                                {
                                    DynamicParameters newParameters = new DynamicParameters();
                                    newParameters.Add("CompanyCode", model.CompanyCode);
                                    newParameters.Add("ListLine", itemList);
                                    var resultCheck = db.Query<ResultModel>($"USP_I_T_SalesLine_CheckNegative", newParameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    if (resultCheck != null && resultCheck.ToList().Count > 0)
                                    {
                                        var line = resultCheck.FirstOrDefault();
                                        if (line != null && line.ID != 0)
                                        {
                                            result.Success = false;
                                            result.Message = line.Message;
                                            return result;
                                        }
                                    }
                                }

                                string keyCache = string.Format(PrefixCacheActionSO, model.StoreId, model.TerminalId);
                                string storeCache = cacheService.GetCachedData<string>(keyCache);
                                if (string.IsNullOrEmpty(storeCache))
                                {
                                    cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
                                }
                                else
                                {
                                    result.Success = false;
                                    result.Message = "Your actions are too fast and too dangerous. Please wait for your order to be completed.";
                                    return result;
                                }

                                //Create and fill-up master table data

                                parameters.Add("TransId", model.TransId, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("ShiftId", model.ShiftId);
                                parameters.Add("ContractNo", model.ContractNo);
                                parameters.Add("CusId", model.CusId);
                                parameters.Add("CusIdentifier", model.CusIdentifier);
                                parameters.Add("RoundingOff", model.RoundingOff);
                                parameters.Add("TotalAmount", model.TotalAmount);
                                parameters.Add("TotalPayable", model.TotalPayable);
                                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                parameters.Add("TotalReceipt", model.TotalReceipt);
                                parameters.Add("AmountChange", model.AmountChange);
                                parameters.Add("PaymentDiscount", model.PaymentDiscount);
                                parameters.Add("CusGrpId", model.CusGrpId);
                                if (!string.IsNullOrEmpty(model.OrderId.ToString()))
                                {
                                    parameters.Add("OrderId", model.OrderId);
                                }


                                parameters.Add("TotalTax", model.TotalTax);
                                parameters.Add("DiscountType", model.DiscountType);
                                parameters.Add("DiscountAmount", model.DiscountAmount);
                                parameters.Add("DiscountRate", model.DiscountRate);
                                parameters.Add("CreatedBy", model.CreatedBy);

                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("Remarks", model.Remarks);
                                parameters.Add("SalesPerson", model.SalesPerson);
                                parameters.Add("SalesPersonName", model.SalesPersonName);
                                parameters.Add("SalesMode", model.SalesMode);
                                parameters.Add("RefTransId", model.RefTransId);
                                parameters.Add("ManualDiscount", model.ManualDiscount);
                                parameters.Add("SalesType", model.SalesType);
                                parameters.Add("DataSource", model.DataSource);
                                parameters.Add("POSType", model.POSType);
                                parameters.Add("Phone", model.Phone);
                                parameters.Add("CusName", model.CusName);
                                parameters.Add("CusAddress", model.CusAddress);
                                parameters.Add("Reason", model.Reason);
                                parameters.Add("OMSId", model.OMSId);
                                parameters.Add("Chanel", model.Chanel);
                                parameters.Add("TerminalId", model.TerminalId);
                                parameters.Add("ShortOrderID", model.ShortOrderID);
                                parameters.Add("MerchantId", model.MerchantId);
                                parameters.Add("PromoId", model.PromoId);
                                parameters.Add("ApprovalId", model.ApprovalId);
                                parameters.Add("StartTime", model.StartTime == null ? DateTime.Now.AddMinutes(-1) : model.StartTime);
                                parameters.Add("RewardPoints", model.RewardPoints);
                                parameters.Add("ExpiryDate", model.ExpiryDate == null ? DateTime.Now : model.ExpiryDate);
                                parameters.Add("DocDate", model.DocDate == null ? DateTime.Now : model.DocDate);
                                parameters.Add("CustomF1", model.CustomF1);
                                parameters.Add("CustomF2", model.CustomF2);
                                parameters.Add("CustomF3", model.CustomF3);
                                parameters.Add("CustomF4", model.CustomF4);
                                parameters.Add("CustomF5", model.CustomF5);
                                if (!string.IsNullOrEmpty(model.LuckyNo))
                                {
                                    parameters.Add("LuckyNo", model.LuckyNo);
                                }

                                if (model.SalesMode.ToLower() == "return" || model.SalesMode.ToLower() == "ex" || model.SalesMode.ToLower() == "exchange")
                                {
                                    parameters.Add("CollectedStatus", "Closed");
                                }
                                else
                                {
                                    if ((model.DataSource == "POS" && model.Status.ToLower() != "h" && model.Status.ToLower() != "hold") || (model.DataSource != "POS" && ( model.IsCanceled == "C" || model.Status == "C")))
                                    {
                                        model.Status = "C";
                                        parameters.Add("CollectedStatus", "Completed");
                                    }
                                    else
                                    {
                                        parameters.Add("CollectedStatus", "Hold");
                                    }

                                }
                                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "C" : model.Status);
                                //_saleHeaderRepository.Insert("InsertSaleHeader", parameters, commandType: CommandType.StoredProcedure);


                                //Insert record in master table. Pass transaction parameter to Dapper.
                                var affectedRows = db.Execute("USP_I_T_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //Get the Id newly created for master table record.
                                //If this is not an Identity, use different method here
                                //newId = Convert.ToInt64(connection.ExecuteScalar<object>("SELECT @@IDENTITY", null, transaction: transaction));

                                //Create and fill-up detail table data
                                //Use suitable loop as you want to insert multiple records.
                                //for(......)
                                int stt = 0;

                                SThirdPartyLog log = new SThirdPartyLog();
                                log.TransId = model.TransId;
                                log.Type = "S4Voucher";
                                log.StoreId = model.StoreId;
                                log.Remark = "";
                                log.CompanyCode = model.CompanyCode;
                                log.CreatedBy = model.CreatedBy;
                                log.Lines = new List<SThirdPartyLogLine>();

                                List<EpayModel> epayList = new List<EpayModel>();
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    if (line.Quantity.HasValue && Math.Abs(line.Quantity.Value) > 0)
                                    {
                                        parameters = new DynamicParameters();
                                        if (line.ItemType == null)
                                        {
                                            line.ItemType = "";
                                        }
                                        parameters.Add("TransId", key, DbType.String);
                                        parameters.Add("LineId", stt);
                                        line.LineId = stt.ToString();
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", line.ItemCode);
                                        parameters.Add("BarCode", line.BarCode);
                                        parameters.Add("Uomcode", line.UomCode);
                                        parameters.Add("Quantity", line.Quantity);
                                        parameters.Add("Price", line.Price);
                                        parameters.Add("LineTotal", line.LineTotal);
                                        parameters.Add("DiscountType", string.IsNullOrEmpty(line.DiscountType) && string.IsNullOrEmpty(line.PromoType) ? line.PromoType : line.DiscountType);
                                        parameters.Add("DiscountAmt", !line.DiscountAmt.HasValue && line.PromoDisAmt.HasValue ? line.PromoDisAmt : line.DiscountAmt);
                                        parameters.Add("DiscountRate", !line.DiscountRate.HasValue && line.PromoPercent.HasValue ? line.PromoPercent : line.DiscountRate);
                                        parameters.Add("CreatedBy", model.CreatedBy);
                                        if (string.IsNullOrEmpty(line.SlocId))
                                        {

                                            line.SlocId = defaultWhs;
                                        }
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                        parameters.Add("PromoId", line.PromoId);
                                        parameters.Add("PromoType", line.PromoType);
                                        parameters.Add("Status", line.Status);
                                        parameters.Add("Remark", line.Remark);
                                        parameters.Add("PromoPercent", line.PromoPercent);
                                        parameters.Add("PromoBaseItem", line.PromoBaseItem);
                                        parameters.Add("SalesMode", line.SalesMode);
                                        parameters.Add("Remarks", line.Remark);
                                        parameters.Add("TaxRate", line.TaxRate);
                                        line.TaxAmt = (line.LineTotal * (line.TaxRate.HasValue ? line.TaxRate : 1)) / 100;
                                        parameters.Add("TaxAmt", line.TaxAmt);
                                        parameters.Add("TaxCode", line.TaxCode);
                                        parameters.Add("SlocId", line.SlocId);
                                        parameters.Add("MinDepositAmt", line.MinDepositAmt);
                                        parameters.Add("MinDepositPercent", line.MinDepositPercent);
                                        parameters.Add("DeliveryType", line.DeliveryType);
                                        parameters.Add("Posservice", line.Posservice);
                                        parameters.Add("StoreAreaId", line.StoreAreaId);
                                        parameters.Add("TimeFrameId", line.TimeFrameId);
                                        parameters.Add("Duration", line.Duration);
                                        parameters.Add("AppointmentDate", line.AppointmentDate);
                                        parameters.Add("BomId", line.BomId);
                                        parameters.Add("PromoPrice", line.PromoPrice);
                                        parameters.Add("PromoLineTotal", line.PromoLineTotal);
                                        parameters.Add("BaseLine", line.BaseLine);
                                        parameters.Add("BaseTransId", line.BaseTransId);
                                        parameters.Add("OpenQty", line.OpenQty);
                                        parameters.Add("PromoDisAmt", line.PromoDisAmt);
                                        parameters.Add("IsPromo", line.IsPromo);
                                        parameters.Add("IsSerial", line.IsSerial);
                                        parameters.Add("IsVoucher", line.IsVoucher);
                                        parameters.Add("Description", line.Description);
                                        parameters.Add("PrepaidCardNo", line.PrepaidCardNo);
                                        parameters.Add("MemberDate", line.MemberDate);
                                        parameters.Add("MemberValue", line.MemberValue);
                                        parameters.Add("StartDate", line.StartDate);
                                        parameters.Add("EndDate", line.EndDate);
                                        parameters.Add("ItemType", line.ItemType);
                                        parameters.Add("SerialNum", line.SerialNum);
                                        parameters.Add("Name", line.Name);
                                        parameters.Add("Phone", line.Phone);
                                        parameters.Add("ItemTypeS4", line.ItemTypeS4);



                                        if (model.DataSource == "POS" && model.IsCanceled != "C" && (line.ItemType == "Member" || line.ItemType == "Class"))
                                        {
                                            TSalesLineSerialViewModel newSerialLine = new TSalesLineSerialViewModel();
                                            newSerialLine.ItemCode = line.ItemCode;
                                            newSerialLine.UomCode = line.UomCode;
                                            newSerialLine.SlocId = line.SlocId;
                                            newSerialLine.SerialNum = line.SerialNum;
                                            newSerialLine.Name = line.Name;
                                            newSerialLine.Phone = line.Phone;
                                            newSerialLine.Quantity = line.Quantity;
                                            newSerialLine.Status = "A";
                                            newSerialLine.OpenQty = 0;
                                            newSerialLine.BaseLine = null;
                                            newSerialLine.BaseTransId = null;
                                            model.SerialLines.Add(newSerialLine);
                                        }

                                        if (model.DataSource == "POS" && model.Status != "H" && (line.ItemType.ToLower() == "pn" || line.ItemType.ToLower() == "tp" || line.ItemType.ToLower() == "pin" || line.ItemType.ToLower() == "bp" || line.ItemType.ToLower() == "topup"))
                                        {

                                            if (model.SalesMode == "SALES" && (line.ItemType.ToLower() == "pn" || line.ItemType.ToLower() == "pin"))
                                            {
                                                log.Type = "Epay";
                                                for (int iP = 1; iP <= line.Quantity; iP++)
                                                {

                                                    SThirdPartyLogLine lineLog = new SThirdPartyLogLine();
                                                    lineLog.CompanyCode = model.CompanyCode;
                                                    lineLog.TransId = model.TransId;
                                                    lineLog.Key1 = line.ProductId;
                                                    lineLog.Key2 = (line.Price * 100).ToString();
                                                    lineLog.CustomF1 = line.ItemType;

                                                    lineLog.StartTime = DateTime.Now;
                                                    //var jsonString = acttiveVoucher.ToJson();
                                                    //lineLog.JsonBody = jsonString;
                                                    var resultEpay = await EpayPINOrder(TerminalID, MerchantID, line.Price * 100, model.StoreId, line.ProductId, model.OrderId.ToString() + "-" + line.LineId);
                                                    if (resultEpay != null && resultEpay.success.Value)
                                                    {
                                                        var data = JsonConvert.DeserializeObject<EpayResult>(resultEpay.Data.ToString());
                                                        //parameters.Add("Custom3", data.transRef);
                                                        //parameters.Add("Custom4", data.pinNumber);
                                                        TSalesLineSerialViewModel newSerialLine = new TSalesLineSerialViewModel();
                                                        newSerialLine.ItemCode = line.ItemCode;
                                                        newSerialLine.UomCode = line.UomCode;
                                                        newSerialLine.SlocId = line.SlocId;
                                                        newSerialLine.SerialNum = data.pinNumber;
                                                        newSerialLine.Name = line.Name;
                                                        newSerialLine.Phone = line.Phone;
                                                        newSerialLine.Quantity = 1;
                                                        newSerialLine.Status = "A";
                                                        newSerialLine.OpenQty = 1;
                                                        newSerialLine.BaseLine = null;
                                                        newSerialLine.BaseTransId = data.transRef; 
                                                        newSerialLine.CustomF1 = data.pinExpiryDate;
                                                        newSerialLine.Prefix = "PIN";
                                                        newSerialLine.CustomF2 = line.ProductId;
                                                        newSerialLine.CustomF3 = data.customField1;
                                                        model.SerialLines.Add(newSerialLine);

                                                        EpayModel epay = new EpayModel();
                                                        epay.terminalID = TerminalID;
                                                        epay.merchantID = MerchantID;
                                                        epay.amount = int.Parse(line.LineTotal.Value.ToString());
                                                        epay.operatorID = model.StoreId;
                                                        epay.product = line.ProductId;
                                                        epay.source = line.ItemType;
                                                        epay.transRef = data.transRef;
                                                        lineLog.EndTime = DateTime.Now;
                                                        lineLog.Key3 = data.transRef;
                                                        lineLog.Status = "True";
                                                        log.Lines.Add(lineLog);
                                                        epayList.Add(epay);
                                                    }
                                                    else
                                                    {
                                                        lineLog.EndTime = DateTime.Now;
                                                        lineLog.Remark = resultEpay.message;
                                                        lineLog.Status = "False";
                                                        log.Lines.Add(lineLog);
                                                        tran.Rollback();
                                                        result.Success = false;
                                                        result.Message = "Epay Message: " + resultEpay.message;
                                                        var resultVoid = await VoidEpay(epayList, model.OrderId.ToString() + "-" + line.LineId);
                                                        var listVoid = resultVoid.Data as List<SThirdPartyLogLine>;
                                                        foreach (var voidResult in listVoid)
                                                        {

                                                            voidResult.CompanyCode = model.CompanyCode;
                                                            voidResult.TransId = model.TransId;
                                                            voidResult.CustomF1 = "Void";
                                                        }
                                                        return result;
                                                    }

                                                }

                                            }
                                            if (line.ItemType.ToLower() == "tp" || line.ItemType.ToLower() == "topup" || line.ItemType.ToLower() == "bp")
                                            {
                                                //line.ItemCode
                                                SThirdPartyLogLine lineLog = new SThirdPartyLogLine();
                                                lineLog.CompanyCode = model.CompanyCode;
                                                lineLog.TransId = model.TransId;
                                                lineLog.Key1 = line.ProductId;
                                                lineLog.Key2 = (line.Price * 100).ToString();
                                                lineLog.Key3 = line.Custom1.ToString();
                                                lineLog.CustomF1 = line.ItemType;
                                                lineLog.StartTime = DateTime.Now;

                                                var resultEpay = await EpayTopupOrder(TerminalID, MerchantID, line.LineTotal * 100, model.StoreId, line.ProductId, line.Custom1, model.OrderId.ToString() + "-" + line.LineId);
                                                if (resultEpay != null && resultEpay.success.Value)
                                                {
                                                    var data = JsonConvert.DeserializeObject<EpayResult>(resultEpay.Data.ToString());
                                                    line.Custom3 = data.transRef;
                                                    line.Custom4 = data.customField1;
                                                    //parameters. ("Custom3", );
                                                    //TSalesLineSerialViewModel newSerialLine = new TSalesLineSerialViewModel();
                                                    //newSerialLine.ItemCode = line.ProductId;
                                                    //newSerialLine.UomCode = line.UomCode;
                                                    //newSerialLine.SlocId = line.SlocId;
                                                    //newSerialLine.SerialNum = data.pinNumber;
                                                    //newSerialLine.Name = line.Name;
                                                    //newSerialLine.Phone = line.Phone;
                                                    //newSerialLine.Quantity = 1;
                                                    //newSerialLine.Status = "A";
                                                    //newSerialLine.OpenQty = 1;
                                                    //newSerialLine.BaseLine = null;
                                                    //newSerialLine.BaseTransId = data.transRef;
                                                    ////newSerialLine.CustomF1 = data.pinExpiryDate;
                                                    ////newSerialLine.CustomF2 = line.ProductId;
                                                    //model.SerialLines.Add(newSerialLine);
                                                    EpayModel epay = new EpayModel();
                                                    epay.terminalID = TerminalID;
                                                    epay.merchantID = MerchantID;
                                                    epay.amount = int.Parse(line.LineTotal.Value.ToString());
                                                    epay.operatorID = model.StoreId;
                                                    epay.product = line.ProductId;
                                                    epay.source = line.ItemType;
                                                    epay.accountNo = line.Custom1;
                                                    epay.transRef = data.transRef;
                                                    epayList.Add(epay);
                                                    lineLog.EndTime = DateTime.Now;
                                                    lineLog.Status = "True";
                                                    log.Lines.Add(lineLog);
                                                }
                                                else
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = resultEpay.message;
                                                    lineLog.EndTime = DateTime.Now;
                                                    lineLog.Remark = "Epay Message: " + resultEpay.message;
                                                    lineLog.Status = "False";
                                                    log.Lines.Add(lineLog);
                                                    var resultVoid = await VoidEpay(epayList, model.OrderId.ToString() + "-" + line.LineId);
                                                    var listVoid = resultVoid.Data as List<SThirdPartyLogLine>;
                                                    foreach (var voidResult in listVoid)
                                                    {

                                                        voidResult.CompanyCode = model.CompanyCode;
                                                        voidResult.TransId = model.TransId;
                                                        voidResult.CustomF1 = "Void";
                                                    }
                                                    return result;
                                                }
                                            }


                                        }
                                        parameters.Add("Custom1", line.Custom1);
                                        parameters.Add("Custom2", line.Custom2);
                                        parameters.Add("Custom3", line.Custom3);
                                        parameters.Add("Custom4", line.Custom4);
                                        parameters.Add("Custom5", line.Custom5);
                                        parameters.Add("ProductId", line.ProductId);
                                        parameters.Add("LineTotalBefDis", line.LineTotalBefDis);
                                        parameters.Add("LineTotalDisIncludeHeader", line.LineTotalDisIncludeHeader);
                                        parameters.Add("PriceListId", line.PriceListId);
                                        parameters.Add("WeightScaleBarcode", line.WeightScaleBarcode);
                                        parameters.Add("StoreId", model.StoreId);
                                        parameters.Add("BookletNo", line.BookletNo);

                                        //_saleHeaderRepository.GetConnection().Get("",); 
                                        db.Execute("usp_I_T_SalesLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);


                                    }

                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                }
                                int sttLine = 0;


                                List<S4VoucherDetail> s4VoucherActiveList = new List<S4VoucherDetail>();

                                if (model.SerialLines != null && model.SerialLines.Count > 0)
                                {
                                    foreach (var line in model.SerialLines)
                                    {
                                        sttLine++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("TransId", key, DbType.String);
                                        parameters.Add("LineId", Guid.NewGuid());
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", line.ItemCode);
                                        if (string.IsNullOrEmpty(line.SlocId))
                                        {
                                            line.SlocId = defaultWhs;
                                        }
                                        parameters.Add("SerialNum", line.SerialNum);
                                        parameters.Add("Uomcode", line.UomCode);
                                        parameters.Add("SLocId", line.SlocId);
                                        parameters.Add("Quantity", line.Quantity ?? 1);
                                        parameters.Add("Status", line.Status);
                                        parameters.Add("CreatedBy", model.CreatedBy);
                                        parameters.Add("OpenQty", line.OpenQty);
                                        parameters.Add("BaseLine", line.BaseLine);
                                        parameters.Add("BaseTransId", line.BaseTransId);
                                        parameters.Add("LineNum", sttLine);

                                        parameters.Add("Prefix", line.Prefix);
                                        parameters.Add("Phone", line.Phone);
                                        parameters.Add("Name", line.Name);
                                        parameters.Add("CustomF1", line.CustomF1);
                                        parameters.Add("CustomF2", line.CustomF2);
                                        if(!string.IsNullOrEmpty(line.CustomF3) )
                                        {
                                            parameters.Add("CustomF3", line.CustomF3);
                                        }
                                        if (!string.IsNullOrEmpty(line.CustomF4))
                                        {
                                            parameters.Add("CustomF4", line.CustomF4);
                                        }
                                        if (!string.IsNullOrEmpty(line.CustomF5))
                                        {
                                            parameters.Add("CustomF5", line.CustomF5);
                                        }
                                         
                                        var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "SerialCheck").FirstOrDefault();
                                        if (setting != null && line.Prefix != "PIN" && line.Prefix != "PN" && setting.SettingValue == "MWI.S4SV")
                                        {

                                            S4VoucherDetail acttiveVoucher = new S4VoucherDetail();
                                            acttiveVoucher.customername = model.CusName;
                                            acttiveVoucher.customeraddress = model.CusAddress;
                                            acttiveVoucher.identificationcard = model.CusId;
                                            acttiveVoucher.actionsdate = DateTime.Now.ToString("yyyyMMdd");
                                            acttiveVoucher.validfrom = DateTime.Now.ToString("yyyyMMdd");
                                            acttiveVoucher.plantcode = model.StoreId;
                                            acttiveVoucher.transactionid = key;
                                            acttiveVoucher.phonenumber = model.Phone;
                                            acttiveVoucher.serialnumber = line.SerialNum;
                                            acttiveVoucher.materialnumber = line.ItemCode;
                                            acttiveVoucher.salesvalue = line.Price.ToString();
                                            acttiveVoucher.statuscode = "ACTI";
                                            if (!string.IsNullOrEmpty(line.SapBonusBuyId))
                                            {
                                                acttiveVoucher.bonusbuyid = line.SapBonusBuyId.Split(',')[0];
                                            }

                                            //s4VoucherActiveList.Add(acttiveVoucher);

                                            var jsonString = acttiveVoucher.ToJson();
                                            SThirdPartyLogLine lineLog = new SThirdPartyLogLine();
                                            lineLog.CompanyCode = model.CompanyCode;
                                            lineLog.TransId = model.TransId;

                                            lineLog.JsonBody = jsonString;
                                            lineLog.Key1 = line.SerialNum;
                                            lineLog.Key2 = acttiveVoucher.bonusbuyid;
                                            lineLog.StartTime = DateTime.Now;
                                            var resultHold = await ServayUpdateVoucher(acttiveVoucher);

                                            if (resultHold.success.Value)
                                            {
                                                lineLog.Status = resultHold.success.Value.ToString();
                                                lineLog.EndTime = DateTime.Now;


                                                var listData = JsonConvert.DeserializeObject<List<S4VoucherDetail>>(resultHold.Data.ToString());

                                                var data = listData.FirstOrDefault();
                                                if (data != null)
                                                {

                                                    if (data.statuscode != acttiveVoucher.statuscode)
                                                    {
                                                        //tran.Rollback(); 
                                                        result.Success = false;
                                                        result.Message = data.statusmessage;
                                                        lineLog.Remark = "S4 Message: " + data.statusmessage;
                                                        //return result;
                                                    }
                                                    string date = "";
                                                    if (!string.IsNullOrEmpty(data.todate) && data.todate != "00000000")
                                                    {
                                                        date = data.todate.Substring(0, 4) + "/" + data.todate.Substring(4, 2) + "/" + data.todate.Substring(6, 2);
                                                        line.ExpDate = DateTime.Parse(date);
                                                    }

                                                }
                                                log.Lines.Add(lineLog);
                                            }
                                            else
                                            {
                                                throw new Exception(resultHold.Msg);
                                            }

                                        }
                                        parameters.Add("ExpDate", line.ExpDate);
                                        parameters.Add("StoreId", model.StoreId);

                                        string queryUpdate = $"USP_I_T_SalesLineSerial N'{model.TransId}',N'{ Guid.NewGuid()}',N'{model.CompanyCode}',N'{line.ItemCode}',N'{line.SerialNum}'" +
                                            $",N'{line.SlocId}',N'{line.Quantity}',N'{line.UomCode}',N'{model.CreatedBy}',N'{line.Status}',N'{line.OpenQty}', N'{line.BaseLine}'" +
                                            $",N'{line.BaseTransId}',N'{sttLine}',N'{line.Prefix}',N'{line.Phone}',N'{line.Name}',N'{line.CustomF1}',N'{line.CustomF2}',N'{line.ExpDate}',N'{line.StoreId}'";

                                        db.Execute("USP_I_T_SalesLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }

                                if (s4VoucherActiveList != null && s4VoucherActiveList.Count > 0 &&   model.Status != "H")
                                {
                                    var resultHold = await ServayUpdateVouchers(s4VoucherActiveList);
                                    //var jsonString = acttiveVoucher.ToJson();
                                    //SThirdPartyLogLine lineLog = new SThirdPartyLogLine();
                                    //lineLog.CompanyCode = model.CompanyCode;
                                    //lineLog.TransId = model.TransId;

                                    //lineLog.JsonBody = jsonString;
                                    //lineLog.Key1 = line.SerialNum;
                                    //lineLog.Key2 = acttiveVoucher.bonusbuyid;
                                    //lineLog.StartTime = DateTime.Now;
                                    if (resultHold.success.Value)
                                    {
                                        //lineLog.Status = resultHold.success.Value.ToString();
                                        //lineLog.EndTime = DateTime.Now;


                                        var listData = JsonConvert.DeserializeObject<List<S4VoucherDetail>>(resultHold.Data.ToString());

                                        var data = listData.FirstOrDefault();
                                        if (data != null)
                                        {

                                            //if (data.statuscode != acttiveVoucher.statuscode)
                                            //{
                                            //    //tran.Rollback(); 
                                            //    result.Success = false;
                                            //    result.Message = data.statusmessage;
                                            //    //lineLog.Remark = "S4 Message: " + data.statusmessage;
                                            //    //return result;
                                            //}
                                            string date = "";
                                            if (!string.IsNullOrEmpty(data.todate) && data.todate != "00000000")
                                            {
                                                date = data.todate.Substring(0, 4) + "/" + data.todate.Substring(4, 2) + "/" + data.todate.Substring(6, 2);
                                                //line.ExpDate = DateTime.Parse(date);
                                            }

                                        }
                                        //log.Lines.Add(lineLog);
                                    }
                                    else
                                    {
                                        throw new Exception(resultHold.Msg);
                                    }
                                }
                                if (log.Lines != null && log.Lines.Count > 0)
                                {
                                    await _log3Service.Create(log);
                                }

                                if (model.PromoLines != null && model.PromoLines.Count > 0)
                                {
                                    foreach (var line in model.PromoLines)
                                    {
                                        if (!string.IsNullOrEmpty(line.PromoId))
                                        {
                                            stt++;
                                            string[] splt = line.PromoId.Split(",");
                                            //foreach(var voucherCheck in model.VoucherApply)
                                            //{
                                            //    if(voucherCheck.discount_code===splt.)
                                            //}    
                                            //var voucher = model.VoucherApply.Where(x => splt.Any(y=>y.Contains(x.discount_code))).FirstOrDefault();
                                            var voucher = model.VoucherApply.Where(x => splt.Any(y => y == x.discount_code)).FirstOrDefault();
                                            parameters = new DynamicParameters();
                                            parameters.Add("TransId", key, DbType.String);
                                            //parameters.Add("LineId", stt);
                                            parameters.Add("CompanyCode", model.CompanyCode);
                                            parameters.Add("ItemCode", line.ItemCode);
                                            parameters.Add("BarCode", line.BarCode);
                                            parameters.Add("Uomcode", line.UomCode);

                                            if (voucher != null)
                                            {
                                                parameters.Add("RefTransId", voucher.voucher_code);
                                                parameters.Add("ApplyType", "Ecom");
                                            }
                                            else
                                            {
                                                parameters.Add("RefTransId", line.RefTransId);
                                                parameters.Add("ApplyType", line.ApplyType);
                                            }
                                            parameters.Add("ItemGroupId", line.ItemGroupId);
                                            parameters.Add("Value", line.Value);
                                            parameters.Add("PromoId", line.PromoId);
                                            parameters.Add("PromoType", line.PromoType);
                                            parameters.Add("PromoTypeLine", line.PromoTypeLine);
                                            parameters.Add("Status", line.Status);
                                            parameters.Add("CreatedBy", model.CreatedBy);
                                            parameters.Add("PromoAmt", line.PromoAmt);
                                            parameters.Add("PromoPercent", line.PromoPercent);
                                            parameters.Add("StoreId", model.StoreId);
                                            //USP_I_T_SalesPromo
                                            //USP_U_T_SalesLineSerial
                                            db.Execute("USP_I_T_SalesPromo", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                        }
                                    }
                                }
                                stt = 0;
                                var document = MapSOtoDocument(model);
                                if (model.SalesType == null)
                                {
                                    model.SalesType = "";
                                }

                                document.UDiscountAmount = (double)(model.DiscountAmount ?? 0);
                                if (model.Payments != null && model.Payments.Count > 0)
                                {
                                    foreach (var payment in model.Payments)
                                    {
                                        stt++;
                                        if (string.IsNullOrEmpty(payment.Currency))
                                        {
                                            string CurrencyStr = $"select CurrencyCode from M_Store with (nolock) where StoreId =N'{model.StoreId}' and CompanyCode =N'{model.CompanyCode}' ";
                                            string Currency = _saleHeaderRepository.GetScalar(CurrencyStr, null, commandType: CommandType.Text);
                                            payment.Currency = Currency;
                                        }
                                        var getPayment = await _paymentService.GetByCode(model.CompanyCode, model.StoreId, payment.PaymentCode);
                                        if (getPayment.Success)
                                        {
                                            var paymentCheck = getPayment.Data as MPaymentMethod;
                                            if (paymentCheck != null && paymentCheck.RequireTerminal.HasValue && paymentCheck.RequireTerminal.Value)
                                            {
                                                if (string.IsNullOrEmpty(payment.CustomF4))
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = "Payment method " + payment.PaymentCode + ": Bank terminal required.";
                                                    return result;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tran.Rollback();
                                            return getPayment;
                                        }
                                        //tran.Rollback();
                                        parameters = new DynamicParameters();
                                        parameters.Add("PaymentCode", payment.PaymentCode, DbType.String);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("TransId", key);
                                        parameters.Add("LineId", stt);
                                        parameters.Add("TotalAmt", payment.TotalAmt == null ? payment.ChargableAmount : payment.TotalAmt);
                                        parameters.Add("ReceivedAmt", payment.ReceivedAmt);
                                        parameters.Add("PaidAmt", payment.PaidAmt);
                                        parameters.Add("Currency", payment.Currency);
                                        if (Math.Abs((payment.CollectedAmount ?? 0)) - Math.Abs((payment.ChargableAmount ?? 0)) > 0 && (payment.ChangeAmt == 0 || payment.ChangeAmt == null))
                                        {
                                            payment.ChangeAmt = (payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0);
                                        }
                                        parameters.Add("ChangeAmt", payment.ChangeAmt);
                                        parameters.Add("Forfeit", payment.Forfeit);
                                        parameters.Add("ForfeitCode", payment.ForfeitCode);
                                        parameters.Add("PaymentMode", payment.PaymentMode);
                                        parameters.Add("CardType", payment.CardType);
                                        parameters.Add("CardHolderName", payment.CardHolderName);
                                        parameters.Add("CardNo", payment.CardNo);
                                        parameters.Add("VoucherBarCode", payment.VoucherBarCode);
                                        parameters.Add("VoucherSerial", payment.VoucherSerial);
                                        parameters.Add("CreatedBy", model.CreatedBy);
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                        parameters.Add("Status", payment.Status);
                                        parameters.Add("ChargableAmount", payment.ChargableAmount);
                                        parameters.Add("PaymentDiscount", payment.PaymentDiscount);
                                        parameters.Add("CollectedAmount", payment.CollectedAmount);
                                      
                                        parameters.Add("DataSource", payment.DataSource);
                                        parameters.Add("Currency", payment.Currency);
                                        parameters.Add("Rate", payment.Rate);
                                        parameters.Add("FCAmount", payment.FCAmount);
                                        parameters.Add("ShiftId", model.ShiftId);
                                        parameters.Add("CardExpiryDate", payment.CardExpiryDate);
                                        parameters.Add("AdjudicationCode", payment.AdjudicationCode);
                                        parameters.Add("AuthorizationDateTime", payment.AuthorizationDateTime);
                                        parameters.Add("TerminalId", model.TerminalId);
                                        parameters.Add("RoundingOff", payment.RoundingOff);
                                        parameters.Add("FCRoundingOff", payment.FCRoundingOff);
                                   
                                        parameters.Add("StoreId", model.StoreId);
                                        if (!string.IsNullOrEmpty(payment.CardNo) && model.Status != "H")
                                        {
                                            var prepaidCardData = await GetPrepaidCard(model.CompanyCode, payment.CardNo);
                                            var prepaidCar = prepaidCardData.Data as MPrepaidCard;
                                            if(prepaidCar!= null)
                                            {
                                                decimal main = prepaidCar.MainBalance == null ? 0 : prepaidCar.MainBalance.Value;
                                                decimal sub = prepaidCar.SubBalance == null ? 0 : prepaidCar.SubBalance.Value;
                                                if (main + sub <= 0 || payment.CollectedAmount > main + sub)
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = "Balance of Card No " + payment.CardNo + " not available.";
                                                    return result;
                                                }
                                            }    
                                           

                                        }

                                        if (model.IsCanceled == "C" && model.Status != "H" && payment.CustomF2 == "E")
                                        {

                                            if (payment.CustomF3 == "EWallet")
                                            {
                                                decimal? value = payment.CollectedAmount.Value * 100;

                                                var CancelResult = await EpayVoidPaymentOrder(TerminalID, MerchantID, value, model.StoreId, "", payment.RefNumber, model.OrderId?.ToString());
                                                if (CancelResult.Success)
                                                {
                                                    //var resultData = CancelResult.Data as EpayModel;
                                                    payment.CustomF1 = CancelResult.Message.ToString();
                                                   
                                                }
                                                else
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = "Cancel Sarawak failed. Message " + CancelResult.Message;
                                                    return result;
                                                }
                                            }
                                            if (payment.CustomF3 == "Sarawak")
                                            {
                                                var CancelResult = await ServaySarawakRefund("M100004203", model.OMSId, payment.CustomF1, "https://abeoinc.com", payment.CollectedAmount);
                                                if (CancelResult != null)
                                                {
                                                    if (CancelResult.success.HasValue && !CancelResult.success.Value)
                                                    {
                                                        tran.Rollback();
                                                        result.Success = false;
                                                        result.Message = "Cancel Sarawak failed. Message " + CancelResult.message;
                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        payment.CustomF1 = CancelResult.Data.ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = "Cancel Sarawak failed.";
                                                    return result;
                                                }
                                            }
                                             
                                        }
                                        if (model.IsCanceled == "C" && model.Status != "H" && !string.IsNullOrEmpty(payment.PaymentMode) && payment.PaymentMode == "BankTerminal")
                                        {
                                            if (payment.PaymentMode == "BankTerminal")
                                            {
                                                
                                                string message = "";
                                                string comName = "COM1";

                                                Data.Models.TerminalDataModel response = _bankTerminalService.TestReadData("5", payment.CustomF5, comName, (double)payment.CollectedAmount.Value, payment.RefNumber, 60, out message);
                                                if (response != null)
                                                {
                                                    if (!string.IsNullOrEmpty(message))
                                                    {
                                                        tran.Rollback();
                                                        result.Data = response;
                                                        result.Success = false;
                                                        result.Message = message;
                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        payment.RefNumber = response.InvoiceNumber;
                                                        //result.Data = response;
                                                        //result.Success = true;
                                                    }
                                                }
                                                else
                                                {
                                                    tran.Rollback();
                                                    result.Message = message;
                                                    result.Success = false;
                                                    return result;
                                                }
                                            }
                                        }
                                        
                                        parameters.Add("RefNumber", payment.RefNumber);
                                        parameters.Add("CustomF1", payment.CustomF1);
                                        parameters.Add("CustomF2", payment.CustomF2);
                                        parameters.Add("CustomF3", payment.CustomF3);
                                        parameters.Add("CustomF4", payment.CustomF4);
                                        parameters.Add("CustomF5", payment.CustomF5);
                                        db.Execute("USP_I_T_SalesPayment", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                        //if(payment.PaymentCode == "TapTap")
                                        //{
                                        //    var resultHold = await HoldVoucher(payment.RefNumber, model.CusId, model.StoreId, key);
                                        //    if (resultHold.Status != 0)
                                        //    {
                                        //        //if (holdList != null && holdList.Count > 0)
                                        //        //{
                                        //        //    foreach (var voucher in holdList)
                                        //        //    {
                                        //        //        await UnholdVoucher(voucher, model.CusId, model.StoreId, key);
                                        //        //    }
                                        //        //}
                                        //        //tran.Rollback();
                                        //        //result.Success = false;
                                        //        //result.Message = resultHold.Msg;
                                        //        //return result; 
                                        //        throw new Exception(resultHold.Msg);
                                        //    }    
                                        //    holdList.Add(payment.RefNumber);
                                        //}

                                        //if (model.SalesType.ToLower() != "ex" && model.SalesType.ToLower() != "return")
                                        //{
                                        //}
                                        if (model.Status != "H" && payment.PaymentCode == "Point")
                                        {
                                            //document.UDiscountAmount += payment.CollectedAmount == null ? 0 : (double)payment.CollectedAmount;
                                            //db.Execute($"USP_UpdateLoyaltyPoint N'{model.CompanyCode}' ,N'{model.CusId}' , N'{payment.RefNumber}'", parameters, commandType: CommandType.Text, transaction: tran);
                                            //_loyaltyService.InsertLoyaltyLog(true, document, 0, double.Parse(payment.RefNumber), double.Parse(payment.CollectedAmount.ToString()), out string _);

                                            double outPoint = double.Parse(payment.RefNumber);
                                            double outAmt = double.Parse(payment.CollectedAmount.ToString());
                                            if (outAmt < 0)
                                            {
                                                outPoint *= -1;
                                            }
                                            document.UDiscountAmount += payment.CollectedAmount == null ? 0 : (double)payment.CollectedAmount;
                                            //db.Execute($"USP_UpdateLoyaltyPoint N'{model.CompanyCode}' ,N'{model.CusId}' , N'{outPoint}'", parameters, commandType: CommandType.Text, transaction: tran);
                                            _loyaltyService.InsertLoyaltyLog(true, document, 0, outPoint, outAmt, out string _);
                                        }

                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }
                                //foreach(var prepaidTrans in model.PrepaidLines)
                                //{
                                //    parameters = new DynamicParameters();
                                //    parameters.Add("TransId", key, DbType.String);
                                //    parameters.Add("LineId", stt);
                                //    parameters.Add("CompanyCode", model.CompanyCode);
                                //    parameters.Add("TransId", prepaidTrans.TransId);
                                //    parameters.Add("PepaidCardNo", prepaidTrans.PepaidCardNo);
                                //    parameters.Add("TransType", prepaidTrans.TransType);
                                //    parameters.Add("MainBalance", prepaidTrans.MainBalance);
                                //    parameters.Add("SubBlance", prepaidTrans.SubBlance);
                                //    parameters.Add("LineTotal", line.LineTotal);
                                //    parameters.Add("DiscountType", line.DiscountType);
                                //    db.Execute("usp_I_T_SalesLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //}    
                                //if (model.Vouchers != null && model.Vouchers.Count > 0)
                                //{
                                //    int sttVoucher = 0;
                                //    foreach (var prepaidTrans in model.Vouchers)
                                //    {
                                //        sttVoucher++;
                                //        parameters = new DynamicParameters();
                                //        parameters.Add("Id", Guid.NewGuid(), DbType.String);
                                //        parameters.Add("TransId", key, DbType.String);
                                //        parameters.Add("LineId", sttVoucher);
                                //        parameters.Add("CompanyCode", model.CompanyCode);
                                //        parameters.Add("TransId", prepaidTrans.TransId);
                                //        parameters.Add("PepaidCardNo", prepaidTrans.PepaidCardNo);
                                //        parameters.Add("TransType", prepaidTrans.TransType);
                                //        parameters.Add("MainBalance", prepaidTrans.MainBalance);
                                //        parameters.Add("SubBlance", prepaidTrans.SubBlance);
                                //        parameters.Add("LineTotal", line.LineTotal);
                                //        parameters.Add("DiscountType", line.DiscountType);
                                //        db.Execute("usp_I_T_SalesLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //    }
                                //}

                                if (model.Invoice != null)
                                {
                                    parameters = new DynamicParameters();

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId, DbType.String);
                                    parameters.Add("StoreId", model.StoreId);
                                    parameters.Add("StoreName", model.StoreName);
                                    parameters.Add("CustomerName", model.Invoice.CustomerName);
                                    parameters.Add("Name", model.Invoice.Name);
                                    parameters.Add("TaxCode", model.Invoice.TaxCode);
                                    parameters.Add("Email", model.Invoice.Email);
                                    parameters.Add("Address", model.Invoice.Address);
                                    parameters.Add("Phone", model.Invoice.Phone);
                                    parameters.Add("Remark", model.Invoice.Remark);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    db.Execute("USP_I_T_SalesInvoice", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                }
                                if (model.Delivery != null)
                                {
                                    parameters = new DynamicParameters();

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId, DbType.String);
                                    parameters.Add("StoreId", model.StoreId);
                                    parameters.Add("StoreName", model.StoreName);
                                    parameters.Add("DeliveryPartner", model.Delivery.DeliveryPartner);
                                    parameters.Add("DeliveryId", model.Delivery.DeliveryId);
                                    parameters.Add("Email", model.Delivery.Email);
                                    parameters.Add("Address", model.Delivery.Address);
                                    parameters.Add("Phone", model.Delivery.Phone);
                                    parameters.Add("Remark", model.Delivery.Remark);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    db.Execute("USP_I_T_SalesDelivery", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                }
                                if (model.DataSource == "POS" && model.Status != "H")
                                {
                                    //Tạo Delivery

                                    //Get default delivery
                                    var defaultDeliver = await _deliveryInforService.GetDefault(model.CompanyCode);
                                    if (defaultDeliver.Success == true && defaultDeliver.Data != null)
                                    {
                                        var defData = defaultDeliver.Data as MDeliveryInfor;
                                        TSalesDelivery delivery = new TSalesDelivery();
                                        delivery.TransId = key;
                                        delivery.CompanyCode = model.CompanyCode;
                                        delivery.DeliveryFee = defData.DeliveryFee;
                                        delivery.DeliveryMethod = defData.DeliveryId;
                                        delivery.DeliveryType = defData.DeliveryType;
                                        parameters = new DynamicParameters();

                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("TransId", model.TransId, DbType.String);
                                        parameters.Add("DeliveryType", delivery.DeliveryType);
                                        parameters.Add("DeliveryMethod", delivery.DeliveryMethod);
                                        parameters.Add("DeliveryFee", delivery.DeliveryFee);

                                        db.Execute("USP_I_T_Sales_Delivery", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    }

                                    //Tạo Invoice

                                    InvoiceViewModel invoice = new InvoiceViewModel();
                                    invoice = _mapper.Map<InvoiceViewModel>(model);
                                    invoice.TransId = "";
                                    invoice.RefTransId = model.TransId;
                                    foreach (var line in invoice.Lines)
                                    {
                                        if (!string.IsNullOrEmpty(line.LineId))
                                        {
                                            line.BaseLine = int.Parse(line.LineId);
                                            line.BaseTransId = model.TransId;
                                        }
                                    }
                                    foreach (var line in invoice.Payments)
                                    {
                                        line.RefTransId = model.TransId;
                                    }
                                    result = await CreateInvoice(invoice, db, tran);
                                    if (result.Success == false)
                                    {
                                        tran.Rollback();
                                        return result;
                                    }

                                }
                                else
                                {
                                    if (model.Deliveries != null && model.Deliveries.Count > 0)
                                    {
                                        foreach (var delivery in model.Deliveries)
                                        {
                                            //TSalesDelivery delivery = new TSalesDelivery();
                                            //delivery.TransId = key;
                                            //delivery.CompanyCode = model.CompanyCode;
                                            //delivery.DeliveryFee = 0;
                                            //delivery.DeliveryMethod = "Giao tai cua hang";
                                            //delivery.DeliveryType = "NONE";
                                            parameters = new DynamicParameters();

                                            parameters.Add("CompanyCode", model.CompanyCode);
                                            parameters.Add("TransId", model.TransId, DbType.String);
                                            parameters.Add("DeliveryType", delivery.DeliveryType);
                                            parameters.Add("DeliveryMethod", delivery.DeliveryMethod);
                                            parameters.Add("DeliveryFee", delivery.DeliveryFee);
                                            db.Execute("USP_I_T_Sales_Delivery", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        }
                                    }
                                }

                                if (model.IsCanceled == "C")
                                {
                                    db.Execute($"Update T_SalesHeader set IsCanceled = 'Y', CollectedStatus = 'Canceled' where TransId=N'{model.RefTransId}' and CompanyCode=N'{model.CompanyCode}' and StoreId=N'{model.StoreId}'", null, commandType: CommandType.Text, transaction: tran);
                                }
                                if (model.VoucherApply != null && model.VoucherApply.Count > 0)
                                {
                                    List<string> listRd = new List<string>();
                                    foreach (var voucher in model.VoucherApply)
                                    {
                                        if (voucher.source == "MWI.S4SV")
                                        {
                                            if (!listRd.Contains(voucher.serialnumber))
                                            {
                                                S4VoucherDetail voucherRedeem = new S4VoucherDetail();
                                                voucherRedeem.customername = model.CusName;
                                                voucherRedeem.customeraddress = model.CusAddress;
                                                voucherRedeem.identificationcard = model.CusId;
                                                voucherRedeem.actionsdate = DateTime.Now.ToString("yyyyMMdd");
                                                voucherRedeem.plantcode = model.StoreId;
                                                voucherRedeem.transactionid = key;
                                                voucherRedeem.phonenumber = model.Phone;
                                                voucherRedeem.serialnumber = voucher.serialnumber;
                                                voucherRedeem.materialnumber = voucher.materialnumber;
                                                voucherRedeem.salesvalue = voucher.discount_value;

                                                //voucherRedeem.materialnumber = voucher.;
                                                voucherRedeem.statuscode = "REDE";

                                                var resultHold = await ServayUpdateVoucher(voucherRedeem);
                                                if (resultHold.success.Value)
                                                {
                                                    //var listData = resultHold.Data as List<S4VoucherDetail>;
                                                    var listData = JsonConvert.DeserializeObject<List<S4VoucherDetail>>(resultHold.Data.ToString());
                                                    var data = listData.FirstOrDefault();
                                                    if (data != null)
                                                    {
                                                        if (data.statuscode != voucherRedeem.statuscode)
                                                        {
                                                            tran.Rollback();
                                                            result.Success = false;
                                                            result.Message = "S4 Message: " + data.statusmessage;
                                                            return result;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception(resultHold.Msg);
                                                }
                                                //if (resultHold.Status != 0)
                                                //{
                                                //    throw new Exception(resultHold.Msg);
                                                //}
                                                listRd.Add(voucher.serialnumber);
                                            }

                                            //holdList.Add(voucher.voucher_code);
                                        }
                                        else
                                        {
                                            var resultHold = await HoldVoucher(voucher.voucher_code, model.CusId, model.StoreId, key);
                                            if (resultHold.Status != 0)
                                            {
                                                throw new Exception(resultHold.Msg);
                                            }
                                            holdList.Add(voucher.voucher_code);
                                        }

                                    }
                                }

                                //  2021-11-29
                                //bool canAddCard = false;
                                var memberCardValue = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "MemberCard").FirstOrDefault();
                                if (memberCardValue != null && (memberCardValue.SettingValue == "true" || memberCardValue.SettingValue == "1"))
                                {
                                    List<string> lstCardNo = document.DocumentLines.Where(x => x.ItemType.ToLower() == "card" && !string.IsNullOrEmpty(x.PrepaidCardNo)).Select(x => x.PrepaidCardNo).ToList();
                                    if (lstCardNo.Count > 0)
                                    {
                                        string qryCheckCard = $"SELECT CardNo From S_CustomerCard WHERE CustomerId <> '{document.CardCode}' AND CardNo IN ('{string.Join("','", lstCardNo)}')";
                                        var cardNo = db.Query<string>(qryCheckCard, commandType: CommandType.Text).ToList();
                                        if (cardNo != null && cardNo.Count > 0)
                                        {
                                            tran.Rollback();
                                            result.Success = false;
                                            result.Message = $"Card number '{string.Join("','", cardNo)}' is already owned by another customer.";
                                            return result;
                                        }
                                        else
                                        {
                                            //canAddCard = true;
                                            bool memberCard = _loyaltyService.InsertUpdateMemberCard(document, out string msg);
                                            if (!memberCard)
                                            {
                                                tran.Rollback();
                                                result.Success = false;
                                                result.Message = msg;
                                                return result;
                                            }
                                        }
                                    }
                                }
                                if (model.Status.ToLower() != "h" && model.Status.ToLower() != "hold")
                                {
                                    var luckyDraw = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "LuckyDraw").FirstOrDefault();
                                    if (luckyDraw != null && (luckyDraw.SettingValue == "true" || luckyDraw.SettingValue == "1"))
                                    {
                                        string LuckyNo = _loyaltyService.GetLuckyNo(document, out string _);

                                        if (!string.IsNullOrEmpty(LuckyNo))
                                        {
                                            db.Execute($"Update T_SalesHeader set LuckyNo = '{LuckyNo}' where TransId = N'{model.TransId}' and CompanyCode = N'{model.CompanyCode}' and StoreId= N'{model.StoreId}'", null, commandType: CommandType.Text, transaction: tran);
                                        }
                                    }
                                }

                                tran.Commit();


                                //if (model.SalesType.ToLower() != "ex" && model.SalesType.ToLower() != "return")
                                //{
                                //}
                                var loyaltySystem = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "Loyalty").FirstOrDefault();
                                //string loyaltySystem = $"select SettingValue from S_GeneralSetting with (nolock) where SettingId ='Loyalty' and CompanyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}' ";
                                //loyaltySystem = _saleHeaderRepository.GetScalar(loyaltySystem, null, commandType: CommandType.Text);
                                if (model.Status.ToLower() != "h" && model.Status.ToLower() != "hold")
                                {
                                    if (loyaltySystem != null && (loyaltySystem.SettingValue == "true" || loyaltySystem.SettingValue == "1"))
                                    {
                                        if (string.IsNullOrEmpty(document.NumAtCard))
                                        {
                                            document.NumAtCard = model.CusId;
                                        }
                                        var point = _loyaltyService.ApplyLoyalty(document, out string _);
                                        model.RewardPoints = point;
                                    }

                                }


                                result.Success = true;
                                result.Message = key;
                                model.TransId = key;
                                result.Data = model;
                                var writeLog = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "WriteLog").FirstOrDefault();
                                if (writeLog != null && (writeLog.SettingValue == "LocalFile" || writeLog.SettingValue == "All"))
                                {
                                    await WriteFileLog(model);
                                }
                                if (writeLog != null && (writeLog.SettingValue == "Database" || writeLog.SettingValue == "All"))
                                {
                                    await WriteLog(model);

                                }
                              

                            }
                            catch (Exception ex)
                            {
                                if (holdList != null && holdList.Count > 0)
                                {
                                    foreach (var voucher in holdList)
                                    {
                                        var resultRedeem = await UnholdVoucher(voucher, model.CusId, model.StoreId, key);

                                    }
                                }
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //throw new NotImplementedException();
        }
        public async Task WriteLog(SaleViewModel model)
        {
            int lineNum = 0;
            var logs = model.Logs;
            if (logs != null && logs.Count > 0)
            {

                string query = "";
                foreach (var line in logs)
                {
                    lineNum++;

                    //query += "Go"; 
                    //query += $"[dbo].[USP_I_S_Log] N'{model.CompanyCode}',N'{model.StoreId}',N'{line.Type}',N'{model.TransId}',N'{lineNum.ToString()}'" +
                    //    $",N'{line.Action}',N'{line.Time.Value.ToString("yyyy/MM/dd HH:mm:ss:fff")}',N'{line.Value}',N'{line.Result}',N'{line.CustomF1}',N'{line.CustomF2}',N'{line.CustomF3}',N'{line.CustomF4}',N'{line.CustomF5}',N'{model.CreatedBy}'; {Environment.NewLine} ";


                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("StoreId", model.StoreId);
                    parameters.Add("Type", line.Type);
                    parameters.Add("TransId", model.TransId, DbType.String);
                    parameters.Add("LineNum", lineNum.ToString(), DbType.String);
                    parameters.Add("Action", line.Action, DbType.String);
                    parameters.Add("Time", line.Time);
                    parameters.Add("Value", line.Value);
                    parameters.Add("Result", line.Result);
                    parameters.Add("CustomF1", line.CustomF1);
                    parameters.Add("CustomF2", line.CustomF2);
                    parameters.Add("CustomF3", line.CustomF3);
                    parameters.Add("CustomF4", line.CustomF4);
                    parameters.Add("CustomF5", line.CustomF5);
                    parameters.Add("CreatedBy", model.CreatedBy);
                    _logRepository.Insert("USP_I_S_Log", parameters, commandType: CommandType.StoredProcedure);


                }
                //if (query.Length > 0)
                //{
                //    if (db.State == ConnectionState.Closed)
                //        db.Open();
                //    using (var tran = db.BeginTransaction())
                //    {
                //        try
                //        {
                //            db.Execute(query, null, commandType: CommandType.Text, transaction: tran);
                //            tran.Commit();
                //        }
                //        catch (Exception ex)
                //        {
                //            //return null
                //            tran.Rollback();
                //        }

                //    }
                //}

            }

            //using (IDbConnection db = _saleHeaderRepository.GetConnection())
            //{
            //    try
            //    {


            //    }
            //    catch (Exception ex)
            //    {
            //        //return null
            //        //tran.Rollba();
            //    }


            //}

            //return result;
        }
        public async Task WriteFileLog(SaleViewModel model)
        {
            int lineNum = 0;
            var logs = model.Logs;
            if (logs != null && logs.Count > 0)
            {

                //string query = "";
                //string[] lines = new string[];
                foreach (var line in logs)
                {
                    lineNum++;
                    line.LineNum = lineNum;
                }
                //// Set a variable to the Documents path.
                //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //File.AppendAllLines(Path.Combine(docPath, "WriteFile.txt"), logs);
                string Folder= Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", "Logs");
                if (!Directory.Exists(Folder))
                    Directory.CreateDirectory(Folder);
                string filename =  model.TransId;
                var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot/Logs", "");

                LogUtils.WriteLogData(path,"", filename, logs.ToJson());
            }
          

        }
        public async Task<GenericResult> VoidEpay(List<EpayModel> list, string TransId )
        {
            GenericResult result = new GenericResult();
            try
            {
                List<SThirdPartyLogLine> resultData = new List<SThirdPartyLogLine>();
                foreach (var epay in list)
                {
                    if (epay.source.ToLower() == "pin" || epay.source.ToLower() == "pn")
                    {
                        var resultEpay = await EpayVoidPINOrder(epay.terminalID, epay.merchantID, epay.amount, epay.operatorID, epay.product, epay.transRef, TransId);
                        //resultData.Add(resultEpay);
                        //GenericResult rex= new GenericResult();
                        //rex.Data = resultEpay.Data;
                        //rex.Success = resultEpay.success.Value; 
                        //resultData.Add(rex);

                        SThirdPartyLogLine lineLogVoid = new SThirdPartyLogLine();
                        //lineLogVoid.CompanyCode = model.CompanyCode;
                        //lineLogVoid.TransId = model.TransId;
                        lineLogVoid.Key1 = epay.product;
                        lineLogVoid.Key2 = epay.amount.ToString();
                        lineLogVoid.Key3 = epay.operatorID.ToString();
                        lineLogVoid.Key4 = epay.transRef.ToString();
                        resultData.Add(lineLogVoid);
                    }
                    if (epay.source.ToLower() == "tp" || epay.source.ToLower() == "topup")
                    {
                        //line.ItemCode
                        var resultEpay = await EpayVoidTopupOrder(epay.terminalID, epay.merchantID, epay.amount, epay.operatorID, epay.product, epay.transRef, TransId);
                        //resultData.Add(resultEpay);
                        SThirdPartyLogLine lineLogVoid = new SThirdPartyLogLine();
                        //lineLogVoid.CompanyCode = model.CompanyCode;
                        //lineLogVoid.TransId = model.TransId;
                        lineLogVoid.Key1 = epay.product;
                        lineLogVoid.Key2 = epay.amount.ToString();
                        lineLogVoid.Key3 = epay.operatorID.ToString();
                        lineLogVoid.Key4 = epay.transRef.ToString();
                        resultData.Add(lineLogVoid);
                    }
                    if (epay.source.ToLower() == "bp")
                    {
                        var resultEpay = await EpayVoidPaymentOrder(epay.terminalID, epay.merchantID, epay.amount, epay.operatorID, epay.product, epay.transRef, TransId);
                        //resultData.Add(resultEpay);
                        SThirdPartyLogLine lineLogVoid = new SThirdPartyLogLine();
                        //lineLogVoid.CompanyCode = model.CompanyCode;
                        //lineLogVoid.TransId = model.TransId;
                        lineLogVoid.Key1 = epay.product;
                        lineLogVoid.Key2 = epay.amount.ToString();
                        lineLogVoid.Key3 = epay.operatorID.ToString();
                        lineLogVoid.Key4 = epay.transRef.ToString();
                        resultData.Add(lineLogVoid);
                    }

                }
                //HttpClient client = GetHttpClient();
                //string requestUri = "JAMwi/ReedemVoucher?customerid=" + customerid + "&voucherid=" + voucher + "&storeCode=" + storeCode + "&transactionId=" + transactionId;
                //var model = new { customerid = customerid, voucherid = voucher, storeCode = storeCode, transactionId = transactionId };
                //var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                //var response = await client.PostAsync(requestUri, content);
                ////return response;
                ////var response = await _mwiAPIService.HoldTAPTAPVoucherAsync(customerid, voucherid, storeCode, transactionId);
                //var responseString = await response.Content.ReadAsStringAsync();
                result.Success = true;
                result.Data = resultData;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
        }
        public async Task<OMSResponseModel> ReedemVoucher(string voucher, string customerid, string storeCode, string transactionId)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                HttpClient client = GetHttpClient();
                string requestUri = "JAMwi/ReedemVoucher?customerid=" + customerid + "&voucherid=" + voucher + "&storeCode=" + storeCode + "&transactionId=" + transactionId;
                var model = new { customerid = customerid, voucherid = voucher, storeCode = storeCode, transactionId = transactionId };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);
                //return response;
                //var response = await _mwiAPIService.HoldTAPTAPVoucherAsync(customerid, voucherid, storeCode, transactionId);
                var responseString = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return result;
            }
        }

        //public async Task<HttpResponseMessage> ReedemVoucher(string voucher, string customerid, string storeCode, string transactionId)
        //{
        //    HttpClient client = GetHttpClient();
        //    string requestUri = "JAMwi/ReedemVoucher?customerid=" + customerid + "&voucherid=" + voucher + "&storeCode=" + storeCode + "&transactionId=" + transactionId;
        //    var model = new { customerid = customerid, voucherid = voucher, storeCode = storeCode, transactionId = transactionId };
        //    var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync(requestUri, content);
        //    return response;
        //}
        public async Task<OMSResponseModel> HoldVoucher(string voucher, string customerid, string storeCode, string transactionId)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                HttpClient client = GetHttpClient();
                string requestUri = "JAMwi/HoldVoucher?customerid=" + customerid + "&voucherid=" + voucher + "&storeCode=" + storeCode + "&transactionId=" + transactionId;
                var model = new { customerid = customerid, voucherid = voucher, storeCode = storeCode, transactionId = transactionId };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);
                //return response;
                //var response = await _mwiAPIService.HoldTAPTAPVoucherAsync(customerid, voucherid, storeCode, transactionId);
                var responseString = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return result;
            }
        }

        public async Task<OMSResponseModel> ServayUpdateVoucher(S4VoucherDetail model)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                HttpClient client = GetHttpClient();

                client.DefaultRequestHeaders.Add("X-ABEO-SECRET", "6f59e16f523e4525b4278b5dbd61c624");

                string requestUri = "/api/S4MWI/UpdateVoucher";
                //var model = new { customerid = customerid, voucherid = voucher, storeCode = storeCode, transactionId = transactionId };
                var modelJson = model.ToJson();
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);
                //return response;
                //var response = await _mwiAPIService.HoldTAPTAPVoucherAsync(customerid, voucherid, storeCode, transactionId);
                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                return rsModel;
                //return result;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                result.Status = -1;
                return result;
            }
        }
        public async Task<OMSResponseModel> ServayUpdateVouchers(List<S4VoucherDetail> models)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {

                models = models.Where(x => x.vouchercategory != "SPV").ToList(); 
                HttpClient client = GetHttpClient();

                client.DefaultRequestHeaders.Add("X-ABEO-SECRET", "6f59e16f523e4525b4278b5dbd61c624");

                string requestUri = "/api/S4MWI/UpdateVouchers";
                //var model = new { customerid = customerid, voucherid = voucher, storeCode = storeCode, transactionId = transactionId };
                var modelJson = models.ToJson();
                var content = new StringContent(models.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);
                //return response;
                //var response = await _mwiAPIService.HoldTAPTAPVoucherAsync(customerid, voucherid, storeCode, transactionId);
                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                return rsModel;
                //return result;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                result.Status = -1;
                return result;
            }
        }

        public async Task<OMSResponseModel> ServaySarawakRefund(string merchantId, string merOrderNo, string orderNo, string notifyURL, decimal? refundAmt)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                HttpClient client = GetHttpClient();
                ///api/Auth/Login
                string requestUriAuth = "/api/Auth/Login";
                var modelAuth = new { userName = "user_test", password = "1234" };
                var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                var responseAuth = await client.PostAsync(requestUriAuth, contentAuth);
                var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                var rsModelAuth = JsonConvert.DeserializeObject<OMSResponseModel>(responseStringAuth);
                //var rsModel = JsonConvert.DeserializeObject<JAMwiResponseModel>(responseString);


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string AccessToken = rsModelAuth.token;
                // "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6InVzZXJfdGVzdCIsIm5iZiI6MTYzMDkzMjEzMCwiZXhwIjoxNjMwOTMzOTMwLCJpYXQiOjE2MzA5MzIxMzB9.gbVpefc4uhPrfuTlBx3H80OYAhzHaS458vT9Dvq_A4t5KxKnItToxbOco67rKTvehCu5CHh4YEVpocnOj2qwVg";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                string requestUri = "/api/SarawakPay/CreateOrderRefund";
                var model = new
                {
                    merchantId = merchantId,
                    merOrderNo = merOrderNo,
                    orderNo = orderNo,
                    notifyURL = notifyURL,
                    refundAmt = Math.Abs(refundAmt.Value)
                };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);
                return rsModel;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                result.Status = -1;
                return result;
            }
        }
        public async Task<OMSResponseModel> EpayPINOrder(string terminalID, string merchantID, decimal? amount, string operatorID, string product, string transId)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                HttpClient client = GetHttpClient();
                ///api/Auth/Login
                string requestUriAuth = "/api/Auth/Login";
                var modelAuth = new { userName = "user_test", password = "1234" };
                var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                var responseAuth = await client.PostAsync(requestUriAuth, contentAuth);
                var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                var rsModelAuth = JsonConvert.DeserializeObject<OMSResponseModel>(responseStringAuth);
                //var rsModel = JsonConvert.DeserializeObject<JAMwiResponseModel>(responseString);


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string AccessToken = rsModelAuth.token;// "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6InVzZXJfdGVzdCIsIm5iZiI6MTYzMDkzMjEzMCwiZXhwIjoxNjMwOTMzOTMwLCJpYXQiOjE2MzA5MzIxMzB9.gbVpefc4uhPrfuTlBx3H80OYAhzHaS458vT9Dvq_A4t5KxKnItToxbOco67rKTvehCu5CHh4YEVpocnOj2qwVg";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                string requestUri = "/api/Epay/PINProcess";
                var model = new
                {
                    terminalID = terminalID,
                    merchantID = merchantID,
                    amount = int.Parse(amount.Value.ToString()),
                    operatorID = operatorID,
                    product = product,
                    transId = transId
                };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);

                return rsModel;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                result.Status = -1;
                return result;
            }
        }

        public async Task<OMSResponseModel> EpayVoidPINOrder(string terminalID, string merchantID, decimal? amount, string operatorID, string product, string transRef, string transId)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                HttpClient client = GetHttpClient();
                ///api/Auth/Login
                string requestUriAuth = "/api/Auth/Login";
                var modelAuth = new { userName = "user_test", password = "1234" };
                var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                var responseAuth = await client.PostAsync(requestUriAuth, contentAuth);
                var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                var rsModelAuth = JsonConvert.DeserializeObject<OMSResponseModel>(responseStringAuth);
                //var rsModel = JsonConvert.DeserializeObject<JAMwiResponseModel>(responseString);


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string AccessToken = rsModelAuth.token;// "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6InVzZXJfdGVzdCIsIm5iZiI6MTYzMDkzMjEzMCwiZXhwIjoxNjMwOTMzOTMwLCJpYXQiOjE2MzA5MzIxMzB9.gbVpefc4uhPrfuTlBx3H80OYAhzHaS458vT9Dvq_A4t5KxKnItToxbOco67rKTvehCu5CHh4YEVpocnOj2qwVg";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                string requestUri = "/api/Epay/VoidPINProcess";
                var model = new
                {
                    terminalID = terminalID,
                    merchantID = merchantID,
                    amount = int.Parse(amount.Value.ToString()),
                    operatorID = operatorID,
                    product = product,
                    transRef = transRef,
                    transId = transId
                };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);

                return rsModel;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                result.Status = -1;
                return result;
            }
        }


        public class EpayResult
        {
            //
            public string transRef { get; set; }
            public string pinNumber { get; set; }
            public string pinExpiryDate { get; set; }
            //Topup 

            //Payment
            public string productId { get; set; }
            public string customField1 { get; set; }
            public string customField2 { get; set; }
            public string customField3 { get; set; }
            public string customField4 { get; set; }
            public string customField5 { get; set; }

        }

        public class EpayModel
        {
            public string source { get; set; }
            public string terminalID { get; set; }
            public string merchantID { get; set; }
            public int amount { get; set; }
            public string operatorID { get; set; }
            public string product { get; set; }
            public string accountNo { get; set; }
            public string transRef { get; set; }
            public string customField1 { get; set; }
            public string customField2 { get; set; }
            public string customField3 { get; set; }
            public string customField4 { get; set; }
            public string customField5 { get; set; }

        }
        public async Task<OMSResponseModel> EpayTopupOrder(string terminalID, string merchantID, decimal? amount, string operatorID, string product, string accountNo, string transId)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                HttpClient client = GetHttpClient();
                ///api/Auth/Login
                string requestUriAuth = "/api/Auth/Login";
                var modelAuth = new { userName = "user_test", password = "1234" };
                var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                var responseAuth = await client.PostAsync(requestUriAuth, contentAuth);
                var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                var rsModelAuth = JsonConvert.DeserializeObject<OMSResponseModel>(responseStringAuth);
                //var rsModel = JsonConvert.DeserializeObject<JAMwiResponseModel>(responseString);


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string AccessToken = rsModelAuth.token;// "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6InVzZXJfdGVzdCIsIm5iZiI6MTYzMDkzMjEzMCwiZXhwIjoxNjMwOTMzOTMwLCJpYXQiOjE2MzA5MzIxMzB9.gbVpefc4uhPrfuTlBx3H80OYAhzHaS458vT9Dvq_A4t5KxKnItToxbOco67rKTvehCu5CHh4YEVpocnOj2qwVg";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                string requestUri = "/api/Epay/TopupProcessFlow";
                var model = new
                {
                    terminalID = terminalID,
                    merchantID = merchantID,
                    amount = int.Parse(amount.ToString()),
                    operatorID = operatorID,
                    product = product,
                    accountNo = accountNo,
                    transId = transId
                };

                var modelJs = model.ToJson();
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);

                return rsModel;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                result.Status = -1;
                return result;
            }
        }
        public async Task<GenericResult> EpayVoidTopupOrder(string terminalID, string merchantID, decimal? amount, string operatorID, string product, string transRef, string transId)
        {
            GenericResult result = new GenericResult();
            try
            {
                HttpClient client = GetHttpClient();
                ///api/Auth/Login
                string requestUriAuth = "/api/Auth/Login";
                var modelAuth = new { userName = "user_test", password = "1234" };
                var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                var responseAuth = await client.PostAsync(requestUriAuth, contentAuth);
                var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                var rsModelAuth = JsonConvert.DeserializeObject<OMSResponseModel>(responseStringAuth);
                //var rsModel = JsonConvert.DeserializeObject<JAMwiResponseModel>(responseString);


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string AccessToken = rsModelAuth.token;// "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6InVzZXJfdGVzdCIsIm5iZiI6MTYzMDkzMjEzMCwiZXhwIjoxNjMwOTMzOTMwLCJpYXQiOjE2MzA5MzIxMzB9.gbVpefc4uhPrfuTlBx3H80OYAhzHaS458vT9Dvq_A4t5KxKnItToxbOco67rKTvehCu5CHh4YEVpocnOj2qwVg";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                string requestUri = "/api/Epay/VoidTopUpProcessFlow";
                var model = new
                {
                    terminalID = terminalID,
                    merchantID = merchantID,
                    amount = int.Parse(amount.Value.ToString()),
                    operatorID = operatorID,
                    product = product,
                    transRef = transRef,
                    transId = transId
                };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<GenericResult>(responseString);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }


        public async Task<GenericResult> EpayPaymentOrder(string terminalID, string merchantID, decimal? amount, string operatorID, string product, string accountNo, string transId)
        {
            GenericResult result = new GenericResult();
            try
            {
                HttpClient client = GetHttpClient();
                ///api/Auth/Login
                string requestUriAuth = "/api/Auth/Login";
                var modelAuth = new { userName = "user_test", password = "1234" };
                var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                var responseAuth = await client.PostAsync(requestUriAuth, contentAuth);
                var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                var rsModelAuth = JsonConvert.DeserializeObject<OMSResponseModel>(responseStringAuth);
                //var rsModel = JsonConvert.DeserializeObject<JAMwiResponseModel>(responseString);


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string AccessToken = rsModelAuth.token;// "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6InVzZXJfdGVzdCIsIm5iZiI6MTYzMDkzMjEzMCwiZXhwIjoxNjMwOTMzOTMwLCJpYXQiOjE2MzA5MzIxMzB9.gbVpefc4uhPrfuTlBx3H80OYAhzHaS458vT9Dvq_A4t5KxKnItToxbOco67rKTvehCu5CHh4YEVpocnOj2qwVg";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                string requestUri = "/api/Epay/PaymentProcessFlow";
                var model = new
                {
                    terminalID = terminalID,
                    merchantID = merchantID,
                    amount = int.Parse(amount.Value.ToString()),
                    operatorID = operatorID,
                    product = product, // Không cần nhập cũng được
                    accountNo = accountNo,
                    transId = transId
                };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<GenericResult>(responseString);


            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;

            }
            return result;
        }
        public async Task<GenericResult> EpayVoidPaymentOrder(string terminalID, string merchantID, decimal? amount, string operatorID, string product, string transRef, string transId)
        {
            GenericResult result = new GenericResult();
            try
            {
                HttpClient client = GetHttpClient();
                ///api/Auth/Login
                string requestUriAuth = "/api/Auth/Login";
                var modelAuth = new { userName = "user_test", password = "1234" };
                var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                var responseAuth = await client.PostAsync(requestUriAuth, contentAuth);
                var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                var rsModelAuth = JsonConvert.DeserializeObject<OMSResponseModel>(responseStringAuth);
                //var rsModel = JsonConvert.DeserializeObject<JAMwiResponseModel>(responseString);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string AccessToken = rsModelAuth.token;// "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6InVzZXJfdGVzdCIsIm5iZiI6MTYzMDkzMjEzMCwiZXhwIjoxNjMwOTMzOTMwLCJpYXQiOjE2MzA5MzIxMzB9.gbVpefc4uhPrfuTlBx3H80OYAhzHaS458vT9Dvq_A4t5KxKnItToxbOco67rKTvehCu5CHh4YEVpocnOj2qwVg";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                string requestUri = "/api/Epay/VoidPaymentProcessFlow";


                int Value = (int)amount.Value;
                var model = new
                {
                    terminalID = terminalID,
                    merchantID = merchantID,
                    amount = Math.Abs(Value),
                    operatorID = operatorID,
                    product = product, // Không cần nhập cũng được
                    transRef = transRef,
                    transId = transId
                };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<GenericResult>(responseString);

                return rsModel;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;

            }
            return result;
        }
        public async Task<GenericResult> EpayRefundPaymentOrder(string terminalID, string merchantID, decimal? amount, string operatorID, string product, string transRef, string transId)
        {
            GenericResult result = new GenericResult();
            try
            {
                HttpClient client = GetHttpClient();
                ///api/Auth/Login
                string requestUriAuth = "/api/Auth/Login";
                var modelAuth = new { userName = "user_test", password = "1234" };
                var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                var responseAuth = await client.PostAsync(requestUriAuth, contentAuth);
                var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                var rsModelAuth = JsonConvert.DeserializeObject<OMSResponseModel>(responseStringAuth);
                //var rsModel = JsonConvert.DeserializeObject<JAMwiResponseModel>(responseString);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string AccessToken = rsModelAuth.token;// "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6InVzZXJfdGVzdCIsIm5iZiI6MTYzMDkzMjEzMCwiZXhwIjoxNjMwOTMzOTMwLCJpYXQiOjE2MzA5MzIxMzB9.gbVpefc4uhPrfuTlBx3H80OYAhzHaS458vT9Dvq_A4t5KxKnItToxbOco67rKTvehCu5CHh4YEVpocnOj2qwVg";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                string requestUri = "/api/Epay/RefundPaymentProcessFlow";
                var model = new
                {
                    terminalID = terminalID,
                    merchantID = merchantID,
                    amount = int.Parse(amount.Value.ToString()),
                    operatorID = operatorID,
                    product = product, // Không cần nhập cũng được
                    transRef = transRef,
                    transId = transId
                };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<GenericResult>(responseString);


            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<OMSResponseModel> GrabAcceptRejectOrder(string TransId, string Status)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                HttpClient client = GetHttpClient();
                ///api/Auth/Login
                string requestUriAuth = "/api/Auth/Login";
                var modelAuth = new { userName = "user_test", password = "1234" };
                var contentAuth = new StringContent(modelAuth.ToJson(), Encoding.UTF8, "application/json");
                var responseAuth = await client.PostAsync(requestUriAuth, contentAuth);
                var responseStringAuth = await responseAuth.Content.ReadAsStringAsync();
                var rsModelAuth = JsonConvert.DeserializeObject<OMSResponseModel>(responseStringAuth);
                //var rsModel = JsonConvert.DeserializeObject<JAMwiResponseModel>(responseString);


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string AccessToken = rsModelAuth.token;// "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJ1bmlxdWVfbmFtZSI6InVzZXJfdGVzdCIsIm5iZiI6MTYzMDkzMjEzMCwiZXhwIjoxNjMwOTMzOTMwLCJpYXQiOjE2MzA5MzIxMzB9.gbVpefc4uhPrfuTlBx3H80OYAhzHaS458vT9Dvq_A4t5KxKnItToxbOco67rKTvehCu5CHh4YEVpocnOj2qwVg";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                string requestUri = "/api/order/AcceptRejectOrder?OrderId=" + TransId + "&toState=" + Status;
                var model = new
                {
                    //merchantId = merchantId,
                    //merOrderNo = merOrderNo,
                    //orderNo = orderNo,
                    //notifyURL = notifyURL,
                    //refundAmt = Math.Abs(refundAmt.Value)
                };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                var responseString = await response.Content.ReadAsStringAsync();
                var rsModel = JsonConvert.DeserializeObject<OMSResponseModel>(responseString);

                return rsModel;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                result.Status = -1;
                return result;
            }
        }
        public async Task<OMSResponseModel> UnholdVoucher(string voucher, string customerid, string storeCode, string transactionId)
        {
            OMSResponseModel result = new OMSResponseModel();
            try
            {
                HttpClient client = GetHttpClient();
                string requestUri = "JAMwi/UnholdVoucher?customerid=" + customerid + "&voucherid=" + voucher + "&storeCode=" + storeCode + "&transactionId=" + transactionId;
                var model = new { customerid = customerid, voucherid = voucher, storeCode = storeCode, transactionId = transactionId };
                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);
                //return response;
                //var response = await _mwiAPIService.HoldTAPTAPVoucherAsync(customerid, voucherid, storeCode, transactionId);
                var responseString = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
                return result;
            }
        }
        //public async Task<HttpResponseMessage> UnholdVoucher(string voucher, string customerid, string storeCode, string transactionId)
        //{
        //    HttpClient client = GetHttpClient();
        //    string requestUri = "JAMwi/UnholdVoucher?customerid=" + customerid + "&voucherid=" + voucher + "&storeCode=" + storeCode + "&transactionId=" + transactionId;
        //    var model = new { customerid = customerid, voucherid = voucher, storeCode = storeCode, transactionId = transactionId };
        //    var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync(requestUri, content);
        //    return response;
        //}
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<SaleViewModelResultViewModel> resultlist = new List<SaleViewModelResultViewModel>();
            try
            {
                TShiftHeader shiftHeader = new TShiftHeader();
                shiftHeader.StoreId = model.StoreId;
                shiftHeader.CompanyCode = model.CompanyCode;
                shiftHeader.Status = "O";
                GenericResult shiftResult = await _shiftService.Create(shiftHeader);
                if (shiftResult.Success)
                {
                    foreach (var item in model.SO)
                    {
                        item.CreatedBy = model.CreatedBy;
                        item.CompanyCode = model.CompanyCode;
                        item.ShiftId = shiftResult.Message;
                        item.TransId = "";
                        var itemResult = await CreateSaleOrderWithoutPayment(item);

                        SaleViewModelResultViewModel itemRs = new SaleViewModelResultViewModel();
                        itemRs = _mapper.Map<SaleViewModelResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                        resultlist.Add(itemRs);
                    }
                    await _shiftService.EndShift(shiftHeader);
                }
                else
                {
                    return shiftResult;
                }

                result.Success = true;
                result.Data = resultlist;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                //result.Data = failedlist;
            }
            return result;
        }

        public async Task<GenericResult> CreateInvoice(InvoiceViewModel model, IDbConnection db, IDbTransaction tran)
        {
            GenericResult result = new GenericResult();

            if (model.Lines == null || model.Lines.Count() == 0)
            {
                result.Success = false;
                result.Message = "Doc line not null.";
                return result;
            }
            if (string.IsNullOrEmpty(model.StoreId))
            {
                result.Success = false;
                result.Message = "Store not null.";
                return result;
            }

            try
            {

                var parameters = new DynamicParameters();
                string key = "";
                if (!string.IsNullOrEmpty(model.TransId))
                {

                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("TransId", model.TransId);
                    parameters.Add("StoreId", model.StoreId);
                    var delAffectedRows = db.Execute("USP_D_InvoiceHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                    key = model.TransId;
                }
                else
                {
                    key = _saleHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixAR}',N'{model.CompanyCode}',N'{model.StoreId}')", null, commandType: CommandType.Text);
                    model.TransId = key;
                }
                string itemList = "";
                foreach (var line in model.Lines)
                {

                    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                }
                //string querycheck = $"USP_I_T_InvoiceLine_CheckNegative N'{model.CompanyCode}', N'{itemList}'";
                //var resultCheck = db.Query(querycheck, null, commandType: CommandType.Text);
                //if(resultCheck.ToList().Count > 0)
                //{
                //    var line = resultCheck.ToList()[0] as ResultModel;
                //    if (line.ID != 0)
                //    {
                //        result.Success = false;
                //        result.Message = line.Message;
                //        return result;
                //    }

                //}


                //Create and fill-up master table data


                parameters.Add("TransId", model.TransId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("StoreName", model.StoreName);
                parameters.Add("ContractNo", model.ContractNo);
                parameters.Add("ShiftId", model.ShiftId);
                parameters.Add("CusId", model.CusId);
                parameters.Add("CusIdentifier", model.CusIdentifier);

                parameters.Add("TotalAmount", model.TotalAmount);
                parameters.Add("TotalPayable", model.TotalPayable);
                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                parameters.Add("TotalReceipt", model.TotalReceipt);
                parameters.Add("AmountChange", model.AmountChange);
                parameters.Add("PaymentDiscount", model.PaymentDiscount);

                parameters.Add("TotalTax", model.TotalTax);
                parameters.Add("DiscountType", model.DiscountType);
                parameters.Add("DiscountAmount", model.DiscountAmount);
                parameters.Add("DiscountRate", model.DiscountRate);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "C" : model.Status);
                parameters.Add("IsCanceled", model.IsCanceled);
                parameters.Add("Remarks", model.Remarks);
                parameters.Add("SalesPerson", model.SalesPerson);
                parameters.Add("SalesPersonName", model.SalesPersonName);
                parameters.Add("SalesMode", model.SalesMode);
                parameters.Add("RefTransId", model.RefTransId);
                parameters.Add("ManualDiscount", model.ManualDiscount);
                parameters.Add("SalesType", model.SalesType);
                parameters.Add("DataSource", model.DataSource);
                parameters.Add("POSType", model.POSType);
                parameters.Add("InvoiceType", model.InvoiceType);
                parameters.Add("Phone", model.Phone);
                parameters.Add("CusAddress", model.CusAddress);
                parameters.Add("CusName", model.CusName);
                parameters.Add("Reason", model.Reason);
                parameters.Add("Chanel", model.Chanel);
                parameters.Add("TerminalId", model.TerminalId);
                //_invoiceHeaderRepository.Insert("InsertSaleHeader", parameters, commandType: CommandType.StoredProcedure);

                //Insert record in master table. Pass transaction parameter to Dapper.
                var affectedRows = db.Execute("USP_I_T_InvoiceHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                //Get the Id newly created for master table record.
                //If this is not an Identity, use different method here
                //newId = Convert.ToInt64(connection.ExecuteScalar<object>("SELECT @@IDENTITY", null, transaction: transaction));

                //Create and fill-up detail table data
                //Use suitable loop as you want to insert multiple records.
                //for(......)
                int stt = 0;
                foreach (var line in model.Lines)
                {
                    stt++;
                    parameters = new DynamicParameters();
                    parameters.Add("TransId", key, DbType.String);
                    parameters.Add("LineId", stt);
                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("ItemCode", line.ItemCode);
                    parameters.Add("BarCode", line.BarCode);
                    parameters.Add("Uomcode", line.UomCode);
                    parameters.Add("Quantity", string.IsNullOrEmpty(line.Quantity.ToString()) ? null : line.Quantity);
                    parameters.Add("Price", string.IsNullOrEmpty(line.Price.ToString()) ? null : line.Price);
                    parameters.Add("LineTotal", string.IsNullOrEmpty(line.LineTotal.ToString()) ? null : line.LineTotal);
                    parameters.Add("DiscountType", line.DiscountType);
                    parameters.Add("DiscountAmt", string.IsNullOrEmpty(line.DiscountAmt.ToString()) ? null : line.DiscountAmt);
                    parameters.Add("DiscountRate", string.IsNullOrEmpty(line.DiscountRate.ToString()) ? null : line.DiscountRate);
                    parameters.Add("CreatedBy", line.CreatedBy);
                    parameters.Add("Status", line.Status);
                    parameters.Add("PromoId", line.PromoId);
                    parameters.Add("PromoType", line.PromoType);
                    parameters.Add("Remark", line.Remark);
                    parameters.Add("PromoPercent", string.IsNullOrEmpty(line.PromoPercent.ToString()) ? null : line.PromoPercent);
                    parameters.Add("PromoBaseItem", line.PromoBaseItem);
                    parameters.Add("SalesMode", line.SalesMode);
                    parameters.Add("TaxRate", string.IsNullOrEmpty(line.TaxRate.ToString()) ? null : line.TaxRate);
                    parameters.Add("TaxAmt", string.IsNullOrEmpty(line.TaxAmt.ToString()) ? null : line.TaxAmt);
                    parameters.Add("TaxCode", line.TaxCode);
                    parameters.Add("SlocId", line.SlocId);
                    parameters.Add("MinDepositAmt", string.IsNullOrEmpty(line.MinDepositAmt.ToString()) ? null : line.MinDepositAmt);
                    parameters.Add("MinDepositPercent", string.IsNullOrEmpty(line.MinDepositPercent.ToString()) ? null : line.MinDepositPercent);
                    parameters.Add("DeliveryType", line.DeliveryType);
                    parameters.Add("Posservice", line.Posservice);
                    parameters.Add("StoreAreaId", line.StoreAreaId);
                    parameters.Add("TimeFrameId", line.TimeFrameId);
                    parameters.Add("Duration", line.Duration);
                    parameters.Add("AppointmentDate", string.IsNullOrEmpty(line.AppointmentDate.ToString()) ? null : line.AppointmentDate);
                    parameters.Add("BomId", line.BomId);
                    parameters.Add("PromoPrice", string.IsNullOrEmpty(line.PromoPrice.ToString()) ? null : line.PromoPrice);
                    parameters.Add("PromoLineTotal", string.IsNullOrEmpty(line.PromoLineTotal.ToString()) ? null : line.PromoLineTotal);
                    parameters.Add("BaseLine", line.BaseLine);
                    parameters.Add("BaseTransId", line.BaseTransId);
                    parameters.Add("OpenQty", string.IsNullOrEmpty(line.OpenQty.ToString()) ? null : line.OpenQty);
                    parameters.Add("PromoDisAmt", line.PromoDisAmt);
                    parameters.Add("IsPromo", line.IsPromo);
                    parameters.Add("IsSerial", line.IsSerial);
                    parameters.Add("IsVoucher", line.IsVoucher);
                    parameters.Add("Description", line.Description);
                    parameters.Add("PrepaidCardNo", line.PrepaidCardNo);
                    parameters.Add("MemberDate", line.MemberDate);
                    parameters.Add("MemberValue", line.MemberValue);
                    parameters.Add("StartDate", line.StartDate);
                    parameters.Add("EndDate", line.EndDate);
                    parameters.Add("ItemType", line.ItemType);
                    parameters.Add("LineTotalBefDis", line.LineTotalBefDis);
                    parameters.Add("LineTotalDisIncludeHeader", line.LineTotalDisIncludeHeader);

                    string queryLine = $"usp_I_T_InvoiceLine N'{key}',N'{stt}',N'{model.CompanyCode}',N'{line.ItemCode}',N'{line.BarCode}',N'{line.UomCode}',N'{line.Quantity}',N'{line.Price}'" +
                        $",N'{line.LineTotal}',N'{line.DiscountType}',N'{line.DiscountAmt}',N'{line.DiscountRate}',N'{line.CreatedBy}',N'{line.PromoId}',N'{line.PromoType}',N'{line.Status}',N'{line.Remark}'" +
                        $",N'{line.PromoPercent}',N'{line.PromoBaseItem}',N'{ line.SalesMode}',N'{line.TaxRate}',N'{line.TaxAmt}',N'{line.TaxCode}',N'{line.SlocId}',N'{line.MinDepositAmt}',N'{line.MinDepositPercent}'" +
                        $",N'{line.DeliveryType}',N'{line.Posservice}',N'{line.StoreAreaId}',N'{line.TimeFrameId}',N'{line.AppointmentDate}',N'{line.BomId}',N'{line.PromoPrice}',N'{line.PromoLineTotal}',N'{line.BaseLine}',N'{line.BaseTransId}',N'{line.OpenQty}'";
                    //_invoiceHeaderRepository.GetConnection().Get("",);



                    db.Execute("usp_I_T_InvoiceLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                    int sttLine = 0;
                    if (line.SerialLines != null)
                    {
                        foreach (var serialLine in line.SerialLines)
                        {
                            sttLine++;
                            parameters = new DynamicParameters();
                            Guid newline = Guid.NewGuid();
                            parameters.Add("TransId", key, DbType.String);
                            parameters.Add("LineId", newline);
                            parameters.Add("CompanyCode", model.CompanyCode);
                            parameters.Add("ItemCode", serialLine.ItemCode);
                            parameters.Add("SerialNum", serialLine.SerialNum);
                            parameters.Add("SLocId", serialLine.SlocId);
                            parameters.Add("Quantity", serialLine.Quantity);
                            parameters.Add("Uomcode", serialLine.UomCode);
                            parameters.Add("CreatedBy", serialLine.CreatedBy);
                            parameters.Add("Status", serialLine.Status);
                            parameters.Add("OpenQty", serialLine.OpenQty);
                            parameters.Add("BaseLine", serialLine.BaseLine);
                            parameters.Add("BaseTransId", model.RefTransId);
                            parameters.Add("LineNum", sttLine);
                            parameters.Add("Description", serialLine.Description);

                            string q = $"USP_I_T_InvoiceLineSerial N'{key}',N'{newline}',N'{model.CompanyCode}',N'{serialLine.ItemCode}',N'{serialLine.SerialNum}',N'{serialLine.SlocId}'" +
                                $",N'{serialLine.Quantity}',N'{serialLine.UomCode}',N'{serialLine.CreatedBy}',N'{serialLine.Status}',N'{serialLine.OpenQty}',N'{serialLine.BaseLine}',N'{model.RefTransId}'" +
                                $",N'{sttLine}'";

                            db.Execute("USP_I_T_InvoiceLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                            //await _invoiceHeaderRepository.GetConnection().InsertAsync<string, TInvoiceLine>(line);
                        }
                    }

                    //db.Execute("usp_I_T_InvoiceLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                    //await _invoiceHeaderRepository.GetConnection().InsertAsync<string, TInvoiceLine>(line);
                }
                if (model.PromoLines != null && model.PromoLines.Count > 0)
                {
                    foreach (var line in model.PromoLines)
                    {
                        stt++;
                        parameters = new DynamicParameters();
                        parameters.Add("TransId", key, DbType.String);
                        //parameters.Add("LineId", stt);
                        parameters.Add("CompanyCode", model.CompanyCode);
                        parameters.Add("ItemCode", line.ItemCode);
                        parameters.Add("BarCode", line.BarCode);
                        parameters.Add("Uomcode", line.UomCode);
                        parameters.Add("RefTransId", line.RefTransId);
                        parameters.Add("ApplyType", line.ApplyType);
                        parameters.Add("ItemGroupId", line.ItemGroupId);
                        parameters.Add("Value", line.Value);
                        parameters.Add("PromoId", line.PromoId);
                        parameters.Add("PromoType", line.PromoType);
                        parameters.Add("PromoTypeLine", line.PromoTypeLine);
                        parameters.Add("Status", line.Status);
                        parameters.Add("CreatedBy", line.CreatedBy);
                        parameters.Add("PromoAmt", line.PromoAmt);
                        parameters.Add("PromoPercent", line.PromoPercent);
                        //USP_I_T_InvoicePromo
                        //USP_U_T_InvoiceLineSerial
                        db.Execute("USP_I_T_InvoicePromo", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                    }
                }
                stt = 0;
                if (model.Payments != null && model.Payments.Count > 0)
                {
                    foreach (var payment in model.Payments)
                    {
                        stt++;
                        if (string.IsNullOrEmpty(payment.Currency))
                        {
                            string CurrencyStr = $"select CurrencyCode from M_Store with (nolock) where StoreId =N'{model.StoreId}' and CompanyCode =N'{model.CompanyCode}' ";
                            string Currency = _saleHeaderRepository.GetScalar(CurrencyStr, null, commandType: CommandType.Text);
                            payment.Currency = Currency;
                        }
                        parameters = new DynamicParameters();
                        parameters.Add("PaymentCode", payment.PaymentCode, DbType.String);
                        parameters.Add("CompanyCode", model.CompanyCode);
                        parameters.Add("TransId", key);
                        parameters.Add("LineId", stt);
                        parameters.Add("LineId", stt);
                        parameters.Add("TotalAmt", payment.TotalAmt);
                        parameters.Add("FCAmount", payment.FCAmount);
                        parameters.Add("Rate", payment.Rate);
                        parameters.Add("ReceivedAmt", payment.ReceivedAmt);
                        parameters.Add("Currency", payment.Currency);
                        parameters.Add("PaidAmt", payment.PaidAmt);
                        if ((payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0) > 0 && (payment.ChangeAmt == 0 || payment.ChangeAmt == null))
                        {
                            payment.ChangeAmt = (payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0);
                        }
                        parameters.Add("ChangeAmt", payment.ChangeAmt);
                        parameters.Add("PaymentMode", payment.PaymentMode);
                        parameters.Add("CardType", payment.CardType);
                        parameters.Add("CardHolderName", payment.CardHolderName);
                        parameters.Add("CardNo", payment.CardNo);
                        parameters.Add("VoucherBarCode", payment.VoucherBarCode);
                        parameters.Add("VoucherSerial", payment.VoucherSerial);
                        parameters.Add("CreatedBy", payment.CreatedBy);
                        parameters.Add("ModifiedBy", null);
                        parameters.Add("ModifiedOn", null);
                        parameters.Add("Status", payment.Status);
                        parameters.Add("ChargableAmount", payment.ChargableAmount);
                        parameters.Add("PaymentDiscount", payment.PaymentDiscount);
                        parameters.Add("CollectedAmount", payment.CollectedAmount);
                        parameters.Add("RefNumber", payment.RefNumber);
                        parameters.Add("RefTransId", model.RefTransId);
                        parameters.Add("ShiftId", model.ShiftId);
                        parameters.Add("TerminalId", model.TerminalId);
                        db.Execute("USP_I_T_InvoicePayment", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                        //await _invoiceHeaderRepository.GetConnection().InsertAsync<string, TInvoiceLine>(line);
                    }
                }
                result.Success = true;
                result.Message = key;



            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //throw new NotImplementedException();
        }
        public async Task<GenericResult> ConfirmSO(SaleViewModel model)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(model.CompanyCode))
            {
                result.Success = false;
                result.Message = "Company Code not null.";
                return result;
            }
            if (model.Lines == null || model.Lines.Count() == 0)
            {
                result.Success = false;
                result.Message = "Doc line not null.";
                return result;
            }
            if (string.IsNullOrEmpty(model.StoreId))
            {
                result.Success = false;
                result.Message = " Store   not null.";
                return result;
            }
            model.Status = "O";
            //if (model.TotalAmount > model.TotalReceipt)
            //{
            //    result.Success = false;
            //    result.Message = "Please check your amount.";
            //    return result;
            //}
            try
            {
                //if (model.Payments.Count == 0 && model.SalesMode != "HOLD")
                //{
                //    result.Success = false;
                //    result.Message = "Payment list not null.";
                //    return result;
                //}
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var parameters = new DynamicParameters();
                                string key = "";
                                //if (!string.IsNullOrEmpty(model.TransId))
                                //{

                                //    parameters.Add("CompanyCode", model.CompanyCode);
                                //    parameters.Add("TransId", model.TransId);
                                //    parameters.Add("StoreId", model.StoreId);
                                //    var delAffectedRows = db.Execute("USP_D_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                //    key = model.TransId;
                                //}
                                //else
                                //{
                                //    key = _saleHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('SO',N'{model.CompanyCode}', N'{model.StoreId}')", null, commandType: CommandType.Text);
                                //    model.TransId = key;
                                //}
                                if (model.SalesMode != null && model.SalesMode == "Return")
                                {
                                    string checkResult = _saleHeaderRepository.GetScalar($"USP_Check_ReturnOrder N'{model.CompanyCode}', N'{model.StoreId}', N'{model.TransId}',N'{model.SalesType}',N'{model.SalesMode}'", null, commandType: CommandType.Text);
                                    if (checkResult == "0")
                                    {
                                        result.Success = false;
                                        result.Message = "Can't return order. Because the order date is not valid.";
                                        return result;
                                    }

                                }
                                string itemList = "";
                                foreach (var line in model.Lines)
                                {

                                    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                }

                                //Create and fill-up master table data


                                parameters.Add("TransId", model.TransId, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("ShiftId", model.ShiftId);
                                parameters.Add("ContractNo", model.ContractNo);
                                parameters.Add("CusId", model.CusId);
                                parameters.Add("CusIdentifier", model.CusIdentifier);

                                parameters.Add("TotalAmount", model.TotalAmount);
                                parameters.Add("TotalPayable", model.TotalPayable);
                                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                parameters.Add("TotalReceipt", model.TotalReceipt);
                                parameters.Add("AmountChange", model.AmountChange);
                                parameters.Add("PaymentDiscount", model.PaymentDiscount);

                                parameters.Add("TotalTax", model.TotalTax);
                                parameters.Add("DiscountType", model.DiscountType);
                                parameters.Add("DiscountAmount", model.DiscountAmount);
                                parameters.Add("DiscountRate", model.DiscountRate);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("Remarks", model.Remarks);
                                parameters.Add("SalesPerson", model.SalesPerson);
                                parameters.Add("SalesMode", model.SalesMode);
                                parameters.Add("RefTransId", model.RefTransId);
                                parameters.Add("ManualDiscount", model.ManualDiscount);
                                parameters.Add("SalesType", model.SalesType);
                                parameters.Add("DataSource", model.DataSource);
                                parameters.Add("POSType", model.POSType);
                                parameters.Add("TerminalId", model.TerminalId);

                                //_saleHeaderRepository.Insert("InsertSaleHeader", parameters, commandType: CommandType.StoredProcedure);

                                //Insert record in master table. Pass transaction parameter to Dapper.
                                //var affectedRows = db.Execute("USP_U_T_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //Get the Id newly created for master table record.
                                //If this is not an Identity, use different method here
                                //newId = Convert.ToInt64(connection.ExecuteScalar<object>("SELECT @@IDENTITY", null, transaction: transaction));

                                ////Create and fill-up detail table data
                                ////Use suitable loop as you want to insert multiple records.
                                ////for(......)
                                int stt = 0;
                                string defaultWhs = _saleHeaderRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);

                                foreach (var line in model.Lines)
                                {

                                    stt++;
                                    string getCapacity = $"select CustomField8 from M_Item  with (nolock)  where  ItemCode =N'{line.ItemCode}' ";
                                    var capaValue = _saleHeaderRepository.GetScalar(getCapacity, null, commandType: CommandType.Text);
                                    if (!string.IsNullOrEmpty(capaValue))
                                    {
                                        if (string.IsNullOrEmpty(line.TimeFrameId))
                                        {
                                            result.Success = false;
                                            result.Message = "Time Frame Id can't null";
                                            return result;
                                        }
                                        if (string.IsNullOrEmpty(line.AppointmentDate.ToString()))
                                        {
                                            result.Success = false;
                                            result.Message = "Appointment Date can't null";
                                            return result;
                                        }

                                        if (!string.IsNullOrEmpty(line.StoreAreaId))
                                        {
                                            string queryCheckStoreArea = $" [USP_CheckStoreAreaInStoreCapacity] N'{model.CompanyCode}', N'{model.StoreId}',N'{line.StoreAreaId}'";
                                            var AreaCount = _saleHeaderRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
                                            if (AreaCount == "0")
                                            {
                                                result.Success = false;
                                                result.Message = "Store Area Id does not match Store Capacity. Please check your data input";
                                                return result;
                                            }
                                        }
                                        if (string.IsNullOrEmpty(line.StoreAreaId))
                                        {
                                            string queryCheckStoreArea = $" [USP_S_StoreAreaIdByStore] N'{model.CompanyCode}', N'{model.StoreId}'";
                                            var AreaId = _saleHeaderRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
                                            if (string.IsNullOrEmpty(AreaId))
                                            {
                                                result.Success = false;
                                                result.Message = "Store Area Id can't null. Please check capacity setup";
                                                return result;
                                            }
                                            else
                                            {
                                                line.StoreAreaId = AreaId;
                                            }

                                        }
                                    }

                                    parameters = new DynamicParameters();
                                    parameters.Add("TransId", model.TransId, DbType.String);
                                    parameters.Add("LineId", stt);
                                    line.LineId = stt.ToString();
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Uomcode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("Price", line.Price);
                                    //parameters.Add("LineTotal", line.LineTotal);
                                    parameters.Add("DiscountType", string.IsNullOrEmpty(line.DiscountType) && string.IsNullOrEmpty(line.PromoType) ? line.PromoType : line.DiscountType);
                                    parameters.Add("DiscountAmt", !line.DiscountAmt.HasValue && line.PromoDisAmt.HasValue ? line.PromoDisAmt : line.DiscountAmt);
                                    parameters.Add("DiscountRate", !line.DiscountRate.HasValue && line.PromoPercent.HasValue ? line.PromoPercent : line.DiscountRate);
                                    parameters.Add("ModifiedBy", line.ModifiedBy);
                                    if (string.IsNullOrEmpty(line.SlocId))
                                    {
                                        line.SlocId = "SL001";
                                    }


                                    //parameters.Add("ModifiedBy", null);
                                    //parameters.Add("ModifiedOn", null);
                                    parameters.Add("PromoId", line.PromoId);
                                    parameters.Add("PromoType", line.PromoType);
                                    parameters.Add("Status", model.Status);
                                    parameters.Add("Remark", line.Remark);
                                    parameters.Add("PromoPercent", line.PromoPercent);
                                    parameters.Add("PromoBaseItem", line.PromoBaseItem);
                                    parameters.Add("SalesMode", line.SalesMode);
                                    parameters.Add("Remarks", line.Remark);
                                    parameters.Add("TaxRate", line.TaxRate);
                                    line.TaxAmt = line.Price * line.Quantity * line.DiscountRate * line.TaxRate;
                                    parameters.Add("TaxAmt", line.TaxAmt);
                                    parameters.Add("TaxCode", line.TaxCode);
                                    parameters.Add("SlocId", line.SlocId);
                                    parameters.Add("MinDepositAmt", line.MinDepositAmt);
                                    parameters.Add("MinDepositPercent", line.MinDepositPercent);
                                    parameters.Add("DeliveryType", line.DeliveryType);
                                    parameters.Add("Posservice", line.Posservice);
                                    parameters.Add("StoreAreaId", line.StoreAreaId);
                                    parameters.Add("TimeFrameId", line.TimeFrameId);
                                    parameters.Add("AppointmentDate", line.AppointmentDate);
                                    parameters.Add("BomId", line.BomId);
                                    parameters.Add("PromoPrice", line.PromoPrice);
                                    parameters.Add("PromoLineTotal", line.PromoLineTotal);
                                    //parameters.Add("BaseLine", line.BaseLine);
                                    //parameters.Add("BaseTransId", line.BaseTransId);
                                    //parameters.Add("OpenQty", line.OpenQty);
                                    //parameters.Add("PromoDisAmt", line.PromoDisAmt);
                                    //parameters.Add("IsPromo", line.IsPromo);
                                    //parameters.Add("IsSerial", line.IsSerial);
                                    //parameters.Add("Description", line.Description);
                                    //parameters.Add("PrepaidCardNo", line.PrepaidCardNo);
                                    //parameters.Add("MemberDate", line.MemberDate);
                                    //parameters.Add("MemberValue", line.MemberValue);
                                    //parameters.Add("StartDate", line.StartDate);
                                    //parameters.Add("EndDate", line.EndDate);
                                    parameters.Add("ItemType", line.ItemType);
                                    parameters.Add("StoreId", line.StoreId);

                                    db.Execute("usp_U_T_SalesLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    if (line.IsSerial == true && line.Quantity != line.SerialLines.Count())
                                    {
                                        result.Success = false;
                                        result.Message = line.ItemCode + " - " + line.ItemName;
                                        return result;
                                    }
                                    int sttLine = 0;
                                    foreach (var lineSerial in line.SerialLines)
                                    {
                                        sttLine++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("TransId", model.TransId, DbType.String);
                                        parameters.Add("LineId", Guid.NewGuid());
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", line.ItemCode);
                                        if (string.IsNullOrEmpty(lineSerial.SlocId))
                                        {
                                            lineSerial.SlocId = defaultWhs;
                                        }
                                        parameters.Add("SerialNum", lineSerial.SerialNum);
                                        parameters.Add("Uomcode", line.UomCode);
                                        parameters.Add("SLocId", lineSerial.SlocId);
                                        if (lineSerial.Quantity == null)
                                        {
                                            lineSerial.Quantity = 1;
                                        }
                                        if (lineSerial.OpenQty == null)
                                        {
                                            lineSerial.OpenQty = lineSerial.Quantity;
                                        }
                                        parameters.Add("Quantity", lineSerial.Quantity);
                                        parameters.Add("Status", "O");
                                        parameters.Add("CreatedBy", lineSerial.CreatedBy);
                                        parameters.Add("OpenQty", lineSerial.OpenQty);
                                        parameters.Add("BaseLine", lineSerial.BaseLine);
                                        parameters.Add("BaseTransId", lineSerial.BaseTransId);
                                        parameters.Add("LineNum", sttLine);
                                        parameters.Add("StoreId", model.StoreId);
                                        db.Execute("USP_U_T_SalesLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }

                                if (model.Payments != null && model.Payments.Count > 0)
                                {
                                    foreach (var payment in model.Payments)
                                    {
                                        stt++;

                                        parameters = new DynamicParameters();
                                        //parameters.Add("PaymentCode", payment.PaymentCode, DbType.String);
                                        //parameters.Add("CompanyCode", model.CompanyCode);
                                        //parameters.Add("TransId", model.TransId);
                                        //parameters.Add("LineId", payment.LineId);
                                        //parameters.Add("TotalAmt", payment.TotalAmt == null ? payment.ChargableAmount : payment.TotalAmt);
                                        //parameters.Add("ReceivedAmt", payment.ReceivedAmt);
                                        //parameters.Add("PaidAmt", payment.PaidAmt);
                                        //parameters.Add("PaidAmt", payment.PaidAmt);
                                        //parameters.Add("ChangeAmt", payment.ChangeAmt);
                                        //parameters.Add("PaymentMode", payment.PaymentMode);
                                        //parameters.Add("CardType", payment.CardType);
                                        //parameters.Add("CardHolderName", payment.CardHolderName);
                                        //parameters.Add("CardNo", payment.CardNo);
                                        //parameters.Add("VoucherBarCode", payment.VoucherBarCode);
                                        //parameters.Add("VoucherSerial", payment.VoucherSerial); 
                                        //parameters.Add("ModifiedBy", model.ModifiedBy); 
                                        //parameters.Add("Status", payment.Status);
                                        //parameters.Add("ChargableAmount", payment.ChargableAmount);
                                        //parameters.Add("PaymentDiscount", payment.PaymentDiscount);
                                        //parameters.Add("CollectedAmount", payment.CollectedAmount);
                                        //parameters.Add("RefNumber", payment.RefNumber);
                                        //parameters.Add("DataSource", model.DataSource);
                                        //parameters.Add("ShiftId", model.ShiftId);
                                        //parameters.Add("TerminalId", model.TerminalId);
                                        //parameters.Add("CardExpiryDate", payment.CardExpiryDate);
                                        //parameters.Add("AdjudicationCode", payment.AdjudicationCode);
                                        //parameters.Add("AuthorizationDateTime", payment.AuthorizationDateTime);

                                        parameters.Add("PaymentCode", payment.PaymentCode, DbType.String);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("TransId", key);
                                        parameters.Add("LineId", stt);
                                        parameters.Add("TotalAmt", payment.TotalAmt == null ? payment.ChargableAmount : payment.TotalAmt);
                                        parameters.Add("ReceivedAmt", payment.ReceivedAmt);

                                        parameters.Add("PaidAmt", payment.PaidAmt);
                                        if ((payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0) > 0 && (payment.ChangeAmt == 0 || payment.ChangeAmt == null))
                                        {
                                            payment.ChangeAmt = (payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0);
                                        }
                                        parameters.Add("ChangeAmt", payment.ChangeAmt);
                                        parameters.Add("PaymentMode", payment.PaymentMode);
                                        parameters.Add("CardType", payment.CardType);
                                        parameters.Add("CardHolderName", payment.CardHolderName);
                                        parameters.Add("CardNo", payment.CardNo);
                                        parameters.Add("VoucherBarCode", payment.VoucherBarCode);
                                        parameters.Add("VoucherSerial", payment.VoucherSerial);

                                        parameters.Add("ModifiedBy", model.ModifiedBy);

                                        parameters.Add("Status", payment.Status);
                                        parameters.Add("ChargableAmount", payment.ChargableAmount);
                                        parameters.Add("PaymentDiscount", payment.PaymentDiscount);
                                        parameters.Add("CollectedAmount", payment.CollectedAmount);
                                        parameters.Add("RefNumber", payment.RefNumber);
                                        parameters.Add("DataSource", payment.DataSource);
                                        parameters.Add("Currency", payment.Currency);
                                        parameters.Add("Rate", payment.Rate);
                                        parameters.Add("FCAmount", payment.FCAmount);
                                        parameters.Add("ShiftId", model.ShiftId);
                                        parameters.Add("CardExpiryDate", payment.CardExpiryDate);
                                        parameters.Add("AdjudicationCode", payment.AdjudicationCode);
                                        parameters.Add("AuthorizationDateTime", payment.AuthorizationDateTime);
                                        parameters.Add("StoreId", model.StoreId);
                                        db.Execute("USP_U_T_SalesPayment", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }
                                var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                if (settingData.Success)
                                {
                                    SettingList = settingData.Data as List<GeneralSettingStore>;
                                }

                                var setting1 = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "CheckInOut").FirstOrDefault();

                                if (setting1 != null && (setting1.SettingValue == "1" || setting1.SettingValue.ToLower() == "true"))
                                {
                                    db.Execute($"Update T_SalesHeader set TerminalId = N'{model.TerminalId}', ShiftId = N'{model.ShiftId}', Status='O' , CreatedOn = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}',  CollectedStatus = 'Open' where TransId=N'{model.TransId}' and CompanyCode= N'{model.CompanyCode}'", null, commandType: CommandType.Text, transaction: tran);

                                }
                                else
                                {
                                    db.Execute($"Update T_SalesHeader set TerminalId = N'{model.TerminalId}', ShiftId = N'{model.ShiftId}', Status='C' , CollectedStatus = 'Closed' where TransId=N'{model.TransId}' and CompanyCode= N'{model.CompanyCode}'", null, commandType: CommandType.Text, transaction: tran);

                                }
                                if (model.DataSource != "POS")
                                {
                                    InvoiceViewModel invoice = new InvoiceViewModel();
                                    invoice = _mapper.Map<InvoiceViewModel>(model);
                                    invoice.TransId = "";
                                    invoice.RefTransId = model.TransId;
                                    decimal? lineTotal = 0;

                                    List<TInvoiceLineViewModel> listLine = new List<TInvoiceLineViewModel>();
                                    bool hasMemberClass = false;
                                    foreach (var line in invoice.Lines)
                                    {
                                        if (setting1 != null && (setting1.SettingValue == "1" || setting1.SettingValue.ToLower() == "true"))
                                        {
                                            if (line.ItemType.ToLower() == "member" || line.ItemType.ToLower() == "class" || line.ItemType.ToLower() == "booklet")
                                            {
                                                if (!string.IsNullOrEmpty(line.LineId))
                                                {
                                                    line.BaseLine = int.Parse(line.LineId);
                                                    line.BaseTransId = model.TransId;
                                                }
                                                hasMemberClass = true;
                                                lineTotal += line.LineTotal;
                                                invoice.TotalAmount = lineTotal;
                                                invoice.TotalPayable = lineTotal;
                                                invoice.TotalAmount = lineTotal;
                                                //invoice.TotalAmount += line.LineTotal;
                                                //invoice.TotalPayable += line.LineTotal;
                                                //invoice.TotalReceipt += line.LineTotal;
                                                listLine.Add(line);
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(line.LineId))
                                            {
                                                line.BaseLine = int.Parse(line.LineId);
                                                line.BaseTransId = model.TransId;
                                            }
                                            lineTotal += line.LineTotal;
                                            invoice.TotalAmount = lineTotal;
                                            invoice.TotalPayable = lineTotal;
                                            invoice.TotalAmount = lineTotal;

                                            //invoice.TotalAmount += line.LineTotal;
                                            //invoice.TotalPayable += line.LineTotal;
                                            //invoice.TotalReceipt += line.LineTotal;
                                            listLine.Add(line);
                                        }
                                    }

                                    if (lineTotal == model.TotalPayable && hasMemberClass)
                                    {
                                        if (model.Payments != null && model.Payments.Count > 0)
                                        {
                                            db.Execute($"Update T_SalesHeader set TerminalId = N'{model.TerminalId}', ShiftId = N'{model.ShiftId}', Status='C' ,  CreatedOn = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}', CollectedStatus = 'Closed' where TransId=N'{model.TransId}' and CompanyCode= N'{model.CompanyCode}'", null, commandType: CommandType.Text, transaction: tran);

                                        }
                                    }
                                    if (listLine != null && listLine.Count > 0)
                                    {
                                        invoice.Lines = listLine;
                                        invoice.InvoiceType = "CheckIn";
                                        result = await CreateInvoice(invoice, db, tran);
                                        if (result.Success == false)
                                        {
                                            tran.Rollback();
                                            return result;
                                        }
                                    }

                                }
                                var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "AcceptOrder").FirstOrDefault();
                                if (setting != null && setting.SettingValue == "Grab")
                                {
                                    var resultCall = await GrabAcceptRejectOrder(model.OMSId, "Accepted");
                                    if (resultCall != null && !resultCall.success.Value)
                                    {
                                        tran.Rollback();
                                        result.Success = false;
                                        result.Message = resultCall.message;
                                        return result;
                                        //var data = JsonConvert.DeserializeObject<GrabResultModel>(resultCall.Data.ToString()); 
                                        ////var data = listData.FirstOrDefault();
                                        //if (data != null)
                                        //{

                                        //}


                                    }
                                }

                                result.Success = true;
                                result.Message = model.TransId;
                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //throw new NotImplementedException();
        }
        public class GrabResultModel
        {
            public string target { get; set; }
            public string reason { get; set; }
            public string message { get; set; }
        }


        public async Task<GenericResult> RejectSO(SaleViewModel model)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(model.CompanyCode))
            {
                result.Success = false;
                result.Message = "Company Code not null.";
                return result;
            }
            //if (model.Lines == null || model.Lines.Count() == 0)
            //{
            //    result.Success = false;
            //    result.Message = "Doc line not null.";
            //    return result;
            //}
            if (string.IsNullOrEmpty(model.StoreId))
            {
                result.Success = false;
                result.Message = " Store not null.";
                return result;
            }

            try
            {
                //if (model.Payments.Count == 0 && model.SalesMode != "HOLD")
                //{
                //    result.Success = false;
                //    result.Message = "Payment list not null.";
                //    return result;
                //}
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var parameters = new DynamicParameters();
                                //string key = "";
                                //if (!string.IsNullOrEmpty(model.TransId))
                                //{

                                //    parameters.Add("CompanyCode", model.CompanyCode);
                                //    parameters.Add("TransId", model.TransId);
                                //    parameters.Add("StoreId", model.StoreId);
                                //    var delAffectedRows = db.Execute("USP_D_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                //    key = model.TransId;
                                //}
                                //else
                                //{
                                //    key = _saleHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('SO',N'{model.CompanyCode}', N'{model.StoreId}')", null, commandType: CommandType.Text);
                                //    model.TransId = key;
                                //}
                                //if (model.SalesMode != null && model.SalesMode == "Return")
                                //{
                                //    string checkResult = _saleHeaderRepository.GetScalar($"USP_Check_ReturnOrder N'{model.CompanyCode}', N'{model.StoreId}', N'{model.TransId}',N'{model.SalesType}',N'{model.SalesMode}'", null, commandType: CommandType.Text);
                                //    if (checkResult == "0")
                                //    {
                                //        result.Success = false;
                                //        result.Message = "Can't return order. Because the order date is not valid.";
                                //        return result;
                                //    }

                                //}
                                //string itemList = "";
                                //foreach (var line in model.Lines)
                                //{

                                //    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                //}

                                ////Create and fill-up master table data


                                //parameters.Add("TransId", model.TransId, DbType.String);
                                //parameters.Add("CompanyCode", model.CompanyCode);
                                //parameters.Add("StoreId", model.StoreId);
                                //parameters.Add("StoreName", model.StoreName);
                                //parameters.Add("ShiftId", model.ShiftId);
                                //parameters.Add("ContractNo", model.ContractNo);
                                //parameters.Add("CusId", model.CusId);
                                //parameters.Add("CusIdentifier", model.CusIdentifier);

                                //parameters.Add("TotalAmount", model.TotalAmount);
                                //parameters.Add("TotalPayable", model.TotalPayable);
                                //parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                //parameters.Add("TotalReceipt", model.TotalReceipt);
                                //parameters.Add("AmountChange", model.AmountChange);
                                //parameters.Add("PaymentDiscount", model.PaymentDiscount);

                                //parameters.Add("TotalTax", model.TotalTax);
                                //parameters.Add("DiscountType", model.DiscountType);
                                //parameters.Add("DiscountAmount", model.DiscountAmount);
                                //parameters.Add("DiscountRate", model.DiscountRate);
                                //parameters.Add("CreatedBy", model.CreatedBy);
                                //parameters.Add("Status", model.Status);
                                //parameters.Add("IsCanceled", model.IsCanceled);
                                //parameters.Add("Remarks", model.Remarks);
                                //parameters.Add("SalesPerson", model.SalesPerson);
                                //parameters.Add("SalesMode", model.SalesMode);
                                //parameters.Add("RefTransId", model.RefTransId);
                                //parameters.Add("ManualDiscount", model.ManualDiscount);
                                //parameters.Add("SalesType", model.SalesType);
                                //parameters.Add("DataSource", model.DataSource);
                                //parameters.Add("POSType", model.POSType);


                                ////_saleHeaderRepository.Insert("InsertSaleHeader", parameters, commandType: CommandType.StoredProcedure);

                                ////Insert record in master table. Pass transaction parameter to Dapper.
                                //var affectedRows = db.Execute("USP_I_T_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                ////Get the Id newly created for master table record.
                                ////If this is not an Identity, use different method here
                                ////newId = Convert.ToInt64(connection.ExecuteScalar<object>("SELECT @@IDENTITY", null, transaction: transaction));

                                ////Create and fill-up detail table data
                                ////Use suitable loop as you want to insert multiple records.
                                ////for(......)
                                //int stt = 0;
                                //string defaultWhs = _saleHeaderRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);

                                //foreach (var line in model.Lines)
                                //{

                                //    stt++;


                                //    if (line.IsSerial == true && line.Quantity != line.SerialLines.Count())
                                //    {
                                //        result.Success = false;
                                //        result.Message = line.ItemCode + " - " + line.ItemName;
                                //        return result;
                                //    }
                                //    int sttLine = 0;
                                //    foreach (var lineSerial in line.SerialLines)
                                //    {
                                //        sttLine++;
                                //        parameters = new DynamicParameters();
                                //        parameters.Add("TransId", model.TransId, DbType.String);
                                //        parameters.Add("LineId", Guid.NewGuid());
                                //        parameters.Add("CompanyCode", model.CompanyCode);
                                //        parameters.Add("ItemCode", line.ItemCode);
                                //        if (string.IsNullOrEmpty(lineSerial.SlocId))
                                //        {
                                //            lineSerial.SlocId = defaultWhs;
                                //        }
                                //        parameters.Add("SerialNum", lineSerial.SerialNum);
                                //        parameters.Add("Uomcode", line.UomCode);
                                //        parameters.Add("SLocId", lineSerial.SlocId);
                                //        if (lineSerial.Quantity == null)
                                //        {
                                //            lineSerial.Quantity = 1;
                                //        }
                                //        if (lineSerial.OpenQty == null)
                                //        {
                                //            lineSerial.OpenQty = lineSerial.Quantity;
                                //        }
                                //        parameters.Add("Quantity", lineSerial.Quantity);
                                //        parameters.Add("Status", "O");
                                //        parameters.Add("CreatedBy", lineSerial.CreatedBy);
                                //        parameters.Add("OpenQty", lineSerial.OpenQty);
                                //        parameters.Add("BaseLine", lineSerial.BaseLine);
                                //        parameters.Add("BaseTransId", lineSerial.BaseTransId);
                                //        parameters.Add("LineNum", sttLine);

                                //        db.Execute("USP_I_T_SalesLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //    }
                                //}

                                var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                if (settingData.Success)
                                {
                                    SettingList = settingData.Data as List<GeneralSettingStore>;
                                }
                                var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "RejectOrder").FirstOrDefault();
                                if (setting != null && setting.SettingValue == "Grab")
                                {
                                    var resultCall = await GrabAcceptRejectOrder(model.OMSId, "Rejected");
                                    if (resultCall != null && !resultCall.success.Value)
                                    {
                                        var data = JsonConvert.DeserializeObject<GrabResultModel>(resultCall.Data.ToString());
                                        //var data = listData.FirstOrDefault();
                                        if (data != null)
                                        {
                                            tran.Rollback();
                                            result.Success = false;
                                            result.Message = data.message;
                                            return result;
                                        }


                                    }
                                    //if (!resultCall.success.Value)
                                    //{
                                    //    tran.Rollback();
                                    //    result.Success = false;
                                    //    result.Message = resultCall.message;

                                    //}
                                }

                                db.Execute($"Update T_SalesHeader set Status='R', CollectedStatus = 'Rejected' , Reason = N'{model.Reason}' where TransId=N'{model.TransId}' and CompanyCode= N'{model.CompanyCode}'", null, commandType: CommandType.Text, transaction: tran);
                                result.Success = true;
                                result.Message = model.TransId;

                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //throw new NotImplementedException();
        }

        public async Task<GenericResult> CancelSO(SaleViewModel model)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(model.CompanyCode))
            {
                result.Success = false;
                result.Message = "Company Code not null.";
                return result;
            }

            if (string.IsNullOrEmpty(model.StoreId))
            {
                result.Success = false;
                result.Message = " Store not null.";
                return result;
            }

            try
            {

                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var parameters = new DynamicParameters();

                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("TransId", model.TransId);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("Reason", model.Reason);

                                var resultX = db.Execute($"USP_CancelSO", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                result.Success = true;
                                result.Message = model.TransId;
                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //throw new NotImplementedException();
        }
        public async Task<GenericResult> UpdateStatusSO(string CompanyCode, string TransId, string Status, string Reason)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(TransId))
            {
                result.Success = false;
                result.Message = "TransId not null.";
                return result;
            }

            if (string.IsNullOrEmpty(Status))
            {
                result.Success = false;
                result.Message = "Status not null.";
                return result;
            }

            try
            {

                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var Ecomparameters = new DynamicParameters();
                                Ecomparameters.Add("CompanyCode", CompanyCode);
                                Ecomparameters.Add("EcomId", TransId);

                                var OMSSO = await _saleHeaderRepository.GetAllAsync("USP_S_T_SalesEcom", Ecomparameters, commandType: CommandType.StoredProcedure);
                                if (OMSSO != null && OMSSO.Count > 0)
                                {

                                    var firstSO = OMSSO.FirstOrDefault();

                                    if (Status.ToLower() == "canceled" || Status.ToLower() == "cancelled" || Status.ToLower() == "failed")
                                    {
                                        if (firstSO.Status.ToLower() == "h" || firstSO.Status.ToLower() == "hold")
                                        {
                                            //db.Execute($"Update T_SalesHeader set ", null, commandType: CommandType.Text, transaction: tran);
                                            db.Execute($"Update T_SalesHeader set IsCanceled = 'Y', Status = 'C', CollectedStatus = 'Canceled' where TransId=N'{firstSO.TransId}' and CompanyCode=N'{firstSO.CompanyCode}' and StoreId=N'{firstSO.StoreId}'", null, commandType: CommandType.Text, transaction: tran);

                                        }
                                        else
                                        {
                                            var SOData = await GetOrderById(firstSO.TransId, firstSO.CompanyCode, firstSO.StoreId);
                                            var SO = SOData.Data as SaleViewModel;
                                            SO.Reason = Reason;
                                            SO.ModifiedBy = "MWI.API";
                                            SO.RefTransId = SO.TransId;
                                            SO.TransId = "";
                                            SO.IsCanceled = "C";

                                            SO.TotalAmount = -SO.TotalAmount;
                                            SO.TotalDiscountAmt = -SO.TotalDiscountAmt;
                                            SO.TotalPayable = -SO.TotalPayable;
                                            SO.TotalReceipt = -SO.TotalReceipt;
                                            SO.AmountChange = -SO.AmountChange;
                                            foreach (var line in SO.Lines)
                                            {
                                                line.BaseLine = int.Parse(line.LineId);
                                                line.BaseTransId = line.TransId;
                                                line.Quantity = -line.Quantity;
                                                line.LineTotal = -line.LineTotal;
                                            }
                                            foreach (var payment in SO.Payments)
                                            {
                                                payment.TotalAmt = -payment.TotalAmt;
                                                payment.ChargableAmount = -payment.ChargableAmount;
                                                payment.CollectedAmount = -payment.CollectedAmount;
                                            }
                                            foreach (var line in SO.Lines)
                                            {
                                                var BomLine = line.Lines;
                                                //SO.Lines = new List<TSalesLineViewModel>();
                                                if (BomLine != null && BomLine.Count > 0)
                                                {
                                                    foreach (var lineBOM in BomLine)
                                                    {
                                                        SO.Lines.Add(lineBOM);
                                                    }

                                                }
                                            }

                                            result = await CreateSaleOrder(SO);
                                            if (!result.Success)
                                            {
                                                return result;
                                            }
                                        }

                                    }

                                    var parameters = new DynamicParameters();
                                    parameters.Add("TransId", TransId);
                                    parameters.Add("Status", Status);
                                    parameters.Add("Reason", Reason);

                                    var resultX = db.Execute($"USP_UpdateStatusSO", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    result.Success = true;
                                    result.Message = "Update order id: " + TransId + " to " + Status + " successfully completed.";
                                    tran.Commit();
                                }
                                else
                                {
                                    result.Success = false;
                                    result.Message = "Can't found order id: " + TransId;
                                    tran.Commit();
                                }


                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }

        public async Task<GenericResult> AddPayment(SaleViewModel model)
        {
            GenericResult result = new GenericResult();

            if (string.IsNullOrEmpty(model.CompanyCode))
            {
                result.Success = false;
                result.Message = "Company Code not null.";
                return result;
            }
            if (model.Payments == null || model.Payments.Count() == 0)
            {
                result.Success = false;
                result.Message = "Payments not null.";
                return result;
            }
            else
            {
                var payment = model.Payments.Where(x => x.CollectedAmount <= 0).ToList();
                if (payment != null && payment.Count > 0)
                {
                    result.Success = false;
                    result.Message = "Please complete progress payment. Can't payment with value 0";
                    return result;
                }
            }

            //if (model.StoreId == null)
            //{
            //    result.Success = false;
            //    result.Message = " Store   not null.";
            //    return result;
            //}
            //if (model.TotalAmount > model.TotalReceipt)
            //{
            //    result.Success = false;
            //    result.Message = "Please check your amount.";
            //    return result;
            //}
            try
            {
                //if (model.Payments.Count == 0 && model.SalesMode != "HOLD")
                //{
                //    result.Success = false;
                //    result.Message = "Payment list not null.";
                //    return result;
                //}
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                if (settingData.Success)
                                {
                                    SettingList = settingData.Data as List<GeneralSettingStore>;
                                }

                                var setting1 = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "CheckInOut").FirstOrDefault();

                                var parameters = new DynamicParameters();
                                string numofPayment = _saleHeaderRepository.GetScalar($"select isnull( MAX(Cast(LineId as Int)), 0) from T_SalesPayment where TransId =N'{model.TransId}' and CompanyCode=N'{model.CompanyCode}'", null, commandType: CommandType.Text);
                                int stt = int.Parse(numofPayment);
                                if (model.Payments != null && model.Payments.Count > 0)
                                {
                                    foreach (var payment in model.Payments)
                                    {
                                        stt++;
                                        if (string.IsNullOrEmpty(payment.Currency))
                                        {
                                            string CurrencyStr = $"select CurrencyCode from M_Store with (nolock) where StoreId =N'{model.StoreId}' and CompanyCode =N'{model.CompanyCode}' ";
                                            string Currency = _saleHeaderRepository.GetScalar(CurrencyStr, null, commandType: CommandType.Text);
                                            payment.Currency = Currency;
                                        }
                                        var getPayment = await _paymentService.GetByCode(model.CompanyCode, model.StoreId, payment.PaymentCode);
                                        if (getPayment.Success)
                                        {
                                            var paymentCheck = getPayment.Data as MPaymentMethod;
                                            if (paymentCheck != null && paymentCheck.RequireTerminal.HasValue && paymentCheck.RequireTerminal.Value)
                                            {
                                                if (string.IsNullOrEmpty(payment.CustomF4))
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = "Payment method " + payment.PaymentCode + ": Bank terminal required.";
                                                    return result;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tran.Rollback();
                                            return getPayment;
                                        }
                                        //tran.Rollback();
                                        parameters = new DynamicParameters();
                                        parameters.Add("PaymentCode", payment.PaymentCode, DbType.String);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("TransId", model.TransId);
                                        parameters.Add("LineId", stt);
                                        parameters.Add("TotalAmt", payment.TotalAmt == null ? payment.ChargableAmount : payment.TotalAmt);
                                        parameters.Add("ReceivedAmt", payment.ReceivedAmt);
                                        parameters.Add("PaidAmt", payment.PaidAmt);
                                        parameters.Add("Currency", payment.Currency);
                                        if ((payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0) > 0 && (payment.ChangeAmt == 0 || payment.ChangeAmt == null))
                                        {
                                            payment.ChangeAmt = (payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0);
                                        }
                                        parameters.Add("ChangeAmt", payment.ChangeAmt);
                                        parameters.Add("Forfeit", payment.Forfeit);
                                        parameters.Add("ForfeitCode", payment.ForfeitCode);
                                        parameters.Add("PaymentMode", payment.PaymentMode);
                                        parameters.Add("CardType", payment.CardType);
                                        parameters.Add("CardHolderName", payment.CardHolderName);
                                        parameters.Add("CardNo", payment.CardNo);
                                        parameters.Add("VoucherBarCode", payment.VoucherBarCode);
                                        parameters.Add("VoucherSerial", payment.VoucherSerial);
                                        parameters.Add("CreatedBy", model.CreatedBy);
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                        parameters.Add("Status", payment.Status);
                                        parameters.Add("ChargableAmount", payment.ChargableAmount);
                                        parameters.Add("PaymentDiscount", payment.PaymentDiscount);
                                        parameters.Add("CollectedAmount", payment.CollectedAmount);
                                        parameters.Add("RefNumber", payment.RefNumber);
                                        parameters.Add("DataSource", payment.DataSource);
                                        parameters.Add("Currency", payment.Currency);
                                        parameters.Add("Rate", payment.Rate);
                                        parameters.Add("FCAmount", payment.FCAmount);
                                        parameters.Add("ShiftId", model.ShiftId);
                                        parameters.Add("CardExpiryDate", payment.CardExpiryDate);
                                        parameters.Add("AdjudicationCode", payment.AdjudicationCode);
                                        parameters.Add("AuthorizationDateTime", payment.AuthorizationDateTime);
                                        parameters.Add("TerminalId", model.TerminalId);
                                        parameters.Add("RoundingOff", payment.RoundingOff);
                                        parameters.Add("FCRoundingOff", payment.FCRoundingOff);
                                        parameters.Add("CustomF1", payment.CustomF1);
                                        parameters.Add("CustomF2", payment.CustomF2);
                                        parameters.Add("CustomF3", payment.CustomF3);
                                        parameters.Add("CustomF4", payment.CustomF4);
                                        parameters.Add("CustomF5", payment.CustomF5);
                                        parameters.Add("StoreId", model.StoreId);
                                        if (!string.IsNullOrEmpty(payment.CardNo))
                                        {
                                            var prepaidCarDảta = await GetPrepaidCard(model.CompanyCode, payment.CardNo);
                                            var prepaidCar = prepaidCarDảta.Data as MPrepaidCard;
                                            decimal main = prepaidCar.MainBalance == null ? 0 : prepaidCar.MainBalance.Value;
                                            decimal sub = prepaidCar.SubBalance == null ? 0 : prepaidCar.SubBalance.Value;
                                            if (main + sub <= 0 || payment.CollectedAmount > main + sub)
                                            {
                                                tran.Rollback();
                                                result.Success = false;
                                                result.Message = "Balance of Card No " + payment.CardNo + " not available.";
                                                return result;
                                            }

                                        }

                                        if (model.IsCanceled == "C" && payment.CustomF2 == "E")
                                        {

                                            if (payment.CustomF3 == "EWallet")
                                            {
                                                decimal? value = payment.CollectedAmount.Value * 100;

                                                var CancelResult = await EpayVoidPaymentOrder(TerminalID, MerchantID, value, model.StoreId, "", payment.RefNumber, model.OrderId.ToString() + "-" + stt.ToString());
                                                if (CancelResult.Success)
                                                {
                                                    //var resultData = CancelResult.Data as EpayModel;
                                                    payment.CustomF1 = CancelResult.Message.ToString();
                                                }
                                                else
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = "Cancel Sarawak failed. Message " + CancelResult.Message;
                                                    return result;
                                                }
                                            }
                                            if (payment.CustomF3 == "Sarawak")
                                            {
                                                var CancelResult = await ServaySarawakRefund("M100004203", model.OMSId, payment.CustomF1, "https://abeoinc.com", payment.CollectedAmount);
                                                if (CancelResult != null)
                                                {
                                                    if (CancelResult.success.HasValue && !CancelResult.success.Value)
                                                    {
                                                        tran.Rollback();
                                                        result.Success = false;
                                                        result.Message = "Cancel Sarawak failed. Message " + CancelResult.message;
                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        payment.CustomF1 = CancelResult.Data.ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = "Cancel Sarawak failed.";
                                                    return result;
                                                }
                                            }


                                        }
                                        db.Execute("USP_I_T_SalesPayment", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                        //parameters = new DynamicParameters();
                                        //parameters.Add("PaymentCode", payment.PaymentCode, DbType.String);
                                        //parameters.Add("CompanyCode", model.CompanyCode);
                                        //parameters.Add("TransId", model.TransId);
                                        //parameters.Add("LineId", stt);
                                        //parameters.Add("TotalAmt", payment.TotalAmt == null ? payment.ChargableAmount : payment.TotalAmt);
                                        //parameters.Add("ReceivedAmt", payment.ReceivedAmt);
                                        //parameters.Add("Currency", payment.Currency);
                                        //parameters.Add("PaidAmt", payment.PaidAmt);
                                        //if ((payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0) > 0 && (payment.ChangeAmt == 0 || payment.ChangeAmt == null))
                                        //{
                                        //    payment.ChangeAmt = (payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0);
                                        //}
                                        //parameters.Add("ChangeAmt", payment.ChangeAmt);
                                        //parameters.Add("PaymentMode", payment.PaymentMode);
                                        //parameters.Add("CardType", payment.CardType);
                                        //parameters.Add("CardHolderName", payment.CardHolderName);
                                        //parameters.Add("CardNo", payment.CardNo);
                                        //parameters.Add("VoucherBarCode", payment.VoucherBarCode);
                                        //parameters.Add("VoucherSerial", payment.VoucherSerial);
                                        //parameters.Add("CreatedBy", model.CreatedBy);
                                        //parameters.Add("ModifiedBy", null);
                                        //parameters.Add("ModifiedOn", null);
                                        //parameters.Add("Status", payment.Status);
                                        //parameters.Add("ChargableAmount", payment.ChargableAmount);
                                        //parameters.Add("PaymentDiscount", payment.PaymentDiscount);
                                        //parameters.Add("CollectedAmount", payment.CollectedAmount);
                                        //parameters.Add("RefNumber", payment.RefNumber);
                                        //parameters.Add("DataSource", model.DataSource);
                                        //parameters.Add("ShiftId", model.ShiftId);
                                        //parameters.Add("TerminalId", model.TerminalId);
                                        //parameters.Add("CardExpiryDate", payment.CardExpiryDate);
                                        //parameters.Add("AdjudicationCode", payment.AdjudicationCode);
                                        //parameters.Add("AuthorizationDateTime", payment.AuthorizationDateTime);
                                        //parameters.Add("StoreId", model.StoreId);
                                        //db.Execute("USP_I_T_SalesPayment", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }

                                bool hasMemberClass = false;
                                foreach (var line in model.Lines)
                                {
                                    if (setting1 != null && (setting1.SettingValue == "1" || setting1.SettingValue.ToLower() == "true"))
                                    {
                                        if (line.ItemType.ToLower() == "member" || line.ItemType.ToLower() == "class" || line.ItemType.ToLower() == "booklet")
                                        {
                                            hasMemberClass = true;
                                        }
                                    }

                                }

                                if (hasMemberClass)
                                {
                                    db.Execute($"Update T_SalesHeader set TerminalId = N'{model.TerminalId}', ShiftId = N'{model.ShiftId}', Status='C' , CollectedStatus = 'Closed' where TransId=N'{model.TransId}' and CompanyCode= N'{model.CompanyCode}'", null, commandType: CommandType.Text, transaction: tran);

                                }

                                result.Success = true;
                                //result.Message = key;
                                tran.Commit();

                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //throw new NotImplementedException();
        }

        public Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }
        public async Task<GenericResult> GetCheckoutOpentList(string companycode, string storeId, string Type, string fromdate, string todate, string ViewBy)
        {
            GenericResult result = new GenericResult();
            try
            {
                string query = $"select * from T_SalesHeader with (nolock) where 1=1 ";
                if (!string.IsNullOrEmpty(companycode) && companycode != "null")
                {
                    query += $" and companycode = N'{companycode}' ";
                }
                if (!string.IsNullOrEmpty(storeId) && storeId != "null")
                {
                    query += $" and storeId = N'{storeId}' ";
                }
                if (!string.IsNullOrEmpty(Type) && Type != "null")
                {
                    query += $" and SalesMode = N'{Type}' ";
                }
                if (!string.IsNullOrEmpty(fromdate) && fromdate != "null")
                {
                    query += $" and createdOn >= N'{fromdate}'";
                }
                if (!string.IsNullOrEmpty(todate) && todate != "null")
                {
                    query += $" and createdOn <= N'{todate}'";
                }

                query += $" and isnull(DataSource,'') <> 'POS' and Status ='O' and PosType ='E' order by CreatedOn desc";
                var lst = await _saleHeaderRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                result.Success = true;
                result.Data = lst;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

            //throw new NotImplementedException();
        }
        public async Task<GenericResult> GetByType(string companycode, string storeId, string Type, string fromdate, string todate, string dataSource, string TransId, string Status, string SalesMan, string Keyword, string ViewBy, bool? includeDetail)
        {
            GenericResult result = new GenericResult();
            try
            {
                string queryX = $"USP_S_T_SalesHeaderByType N'{companycode}' ,N'{storeId}', N'{TransId}',N'{SalesMan}',N'{Type}',N'{fromdate}',N'{todate}', N'{dataSource}',  N'{Status}',N'{Keyword}', N'{ViewBy}'";
                var lstX = await _saleHeaderRepository.GetAllAsync(queryX, null, commandType: CommandType.Text);
                if (includeDetail.HasValue)
                {
                    List<SaleViewModel> sales = new List<SaleViewModel>();
                    foreach (var order in lstX)
                    {
                        var orderGet = await GetOrderById(order.TransId, order.CompanyCode, order.StoreId);
                        if (orderGet.Success)
                        {
                            SaleViewModel saleViewModel = new SaleViewModel();
                            saleViewModel = orderGet.Data as SaleViewModel;
                            sales.Add(saleViewModel);
                        }
                        else
                        {
                            return orderGet;
                        }
                    }
                    result.Success = true;
                    result.Data = sales;
                }
                else
                {
                    result.Success = true;
                    result.Data = lstX;
                }

                //return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //throw new NotImplementedException();
        }
        public async Task<List<TSalesHeader>> GetAll()
        {
            var lst = await _saleHeaderRepository.GetAllAsync("select * from T_SalesHeader with (nolock) order by CreatedOn desc", null, commandType: CommandType.Text);
            return lst;
            //throw new NotImplementedException();
        }

        public Task<TSalesHeader> GetById(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetSummaryPayment(string TransId, string EventId, string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                SaleViewModel order = new SaleViewModel();

                TSalesHeader header = await _saleHeaderRepository.GetAsync($"USP_S_T_SalesHeader N'{CompanyCode}', N'{TransId}', N'{StoreId}'", null, commandType: CommandType.Text);
                if (header == null)
                    return null;
                string queryLine = $" USP_S_T_SalesLine N'{CompanyCode}', N'{StoreId}', N'{EventId}'";
                string queryLineSerial = $"USP_S_T_SalesLineSerial N'{CompanyCode}',N'{TransId}',N'{StoreId}'";
                string queryPromo = $"USP_S_T_SalesPromo N'{CompanyCode}', N'{StoreId}', N'{TransId}'";
                string queryPayment = $"USP_S_T_InvoicePayment N'{CompanyCode}' , N'{StoreId}', N'{EventId}'";

                //List<TSalesLine> lines = await _saleLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                List<TSalesPayment> payments = await _salepaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId = N'{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<SaleViewModel>(header);
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TSalesLineViewModel>(queryLine, null, commandType: CommandType.Text);
                        var serialLines = db.Query<TSalesLineSerialViewModel>(queryLineSerial, null, commandType: CommandType.Text);
                        //decimal total = 0;
                        foreach (var line in lines)
                        {
                            line.LineTotal = line.CheckedQty * line.Price;
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                            //total += line.LineTotal.Value;
                        }

                        var promoLines = db.Query<TSalesPromoViewModel>(queryPromo, null, commandType: CommandType.Text);
                        order = _mapper.Map<SaleViewModel>(header);
                        order.Lines = lines.ToList();
                        order.SerialLines = serialLines.ToList();
                        order.PromoLines = promoLines.ToList();
                        order.Payments = payments;
                        //order.TotalAmount = total;
                        order.Customer = customer;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                result.Success = true;
                result.Data = order;
                //return order;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<List<TSalesHeader>> GetCheckOutList(string EventId, string CompanyCode, string StoreId, string ViewBy)
        {
            try
            {
                //SaleViewModel order = new SaleViewModel();

                var header = await _saleHeaderRepository.GetAllAsync($"USP_S_CheckOutByDate N'{CompanyCode}', N'{EventId}'", null, commandType: CommandType.Text);
                return header;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<GenericResult> GetSummaryPaymentByDate(string TransId, string EventId, string CompanyCode, string StoreId, DateTime Date)
        {
            GenericResult result = new GenericResult();
            try
            {
                SaleViewModel order = new SaleViewModel();

                TSalesHeader header = await _saleHeaderRepository.GetAsync($"USP_S_T_SalesHeader N'{CompanyCode}', N'{TransId}', N'{StoreId}'", null, commandType: CommandType.Text);
                if (header == null)
                    return null;
                string queryLine = $" USP_S_T_SalesLine N'{CompanyCode}', N'{StoreId}', N'{EventId}'";
                string queryLineSerial = $"USP_S_T_SalesLineSerial N'{CompanyCode}',N'{TransId}',N'{StoreId}'";
                string queryPromo = $"USP_S_T_SalesPromo N'{CompanyCode}', N'{StoreId}', N'{TransId}'";
                string queryPayment = $"USP_S_T_InvoicePayment N'{CompanyCode}' , N'{StoreId}', N'{EventId}'";


                List<TSalesPayment> payments = await _salepaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId =N'{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<SaleViewModel>(header);
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TSalesLineViewModel>(queryLine, null, commandType: CommandType.Text);
                        lines = lines.Where(x => x.CreatedOn.Value.ToString("yyyy/MM/dd") == Date.ToString("yyyy/MM/dd")).ToList();
                        var serialLines = db.Query<TSalesLineSerialViewModel>(queryLineSerial, null, commandType: CommandType.Text);
                        foreach (var line in lines)
                        {
                            line.LineTotal = line.CheckedQty * line.Price;
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                        }

                        var promoLines = db.Query<TSalesPromoViewModel>(queryPromo, null, commandType: CommandType.Text);
                        order = _mapper.Map<SaleViewModel>(header);
                        order.Lines = lines.ToList();
                        order.SerialLines = serialLines.ToList();
                        order.PromoLines = promoLines.ToList();
                        payments = payments.Where(x => x.CreatedOn.Value.ToString("yyyy/MM/dd") == Date.ToString("yyyy/MM/dd")).ToList();
                        order.Payments = payments;
                        order.Customer = customer;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                result.Success = true;
                result.Data = order;
                //return order;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetOrderById(string Id, string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var settingData = await _settingService.GetGeneralSettingByStore(CompanyCode, StoreId);
                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                if (settingData.Success)
                {

                    SettingList = settingData.Data as List<GeneralSettingStore>;


                }
                SaleViewModel order = new SaleViewModel();
                //string queryX = $"USP_S_T_SalesHeaderByType N'{companycode}' ,N'{storeId}', N'{TransId}',N'{SalesMan}',N'{Type}',N'{fromdate}',N'{todate}', N'{dataSource}',  N'{Status}',N'{Keyword}'";
                TSalesHeader header = await _saleHeaderRepository.GetAsync($"USP_S_T_SalesHeader_ByTransId N'{CompanyCode}',N'{StoreId}' ,N'{Id}'", null, commandType: CommandType.Text);
                if (header == null)
                {
                    result.Success = false;
                    result.Message = "Order doesn't existed";
                    return result;
                }

                string queryLine = $"USP_S_T_SaleLine N'{CompanyCode}' ,N'{Id}'";
                string queryLineSerial = $"select t1.* , t2.ItemName , t3.UOMName from T_SalesLineSerial t1 with(nolock) left join M_Item t2 with(nolock)  on t1.ItemCode = t2.ItemCode AND T1.CompanyCode = t2.CompanyCode left join M_UOM t3 with(nolock)  on t1.UOMCode = t3.UOMCode where t1.TransId = N'{Id}' and t1.CompanyCode = N'{CompanyCode}'";
                string queryPromo = $"select t1.* , t2.ItemName , t3.UOMName from T_SalesPromo t1 with(nolock)  left join M_Item t2 with(nolock)  on t1.ItemCode = t2.ItemCode AND T1.CompanyCode=t2.CompanyCode left join M_UOM t3 with(nolock)  on t1.UOMCode = t3.UOMCode where t1.TransId = N'{Id}' and t1.CompanyCode = N'{CompanyCode}'";
                
                string invoiceQuery = $"USP_S_T_SalesInvoice N'{CompanyCode}',N'{StoreId}' ,N'{Id}'";
                string queryPayment = $"USP_S_T_SalesPaymentByTransId";
                string queryDelivery = $"select * from T_Sales_Delivery with (nolock) where TransId=N'{Id}' and CompanyCode= N'{CompanyCode}'";
            
                var queryContractPayment = $"USP_S_T_InvoicePayment N'{CompanyCode}' , N'{StoreId}', N'{header.ContractNo}'";
                
               //List<TSalesLine> lines = await _saleLineRepository.GetAllAsync(, null, commandType: CommandType.Text);
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode, DbType.String);
                parameters.Add("StoreId", StoreId, DbType.String);
                parameters.Add("TransId", Id, DbType.String);

                List<TSalesPayment> payments = await _salepaymentLineRepository.GetAllAsync(queryPayment, parameters, commandType: CommandType.StoredProcedure);

                var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId =N'{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<SaleViewModel>(header);
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TSalesLineViewModel>(queryLine, null, commandType: CommandType.Text);
                       
                       
                        var serialLines = db.Query<TSalesLineSerialViewModel>(queryLineSerial, null, commandType: CommandType.Text);
                        var linesView = new List<TSalesLineViewModel>();
                        var NoBom = lines.Where(x => x.BomId == null || x.BomId.ToString() == "").ToList();
                        foreach (var line in NoBom)
                        {
                            //if(!string.IsNullOrEmpty(line.StoreAreaId) && !string.IsNullOrEmpty(line.TimeFrameId))
                            //{
                            //    if()
                            //    line.Lines.Add(line);
                            //}    
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                            linesView.Add(line);
                        }

                        var bomHeader = new List<TSalesLineViewModel>();
                        var bomlines = lines.Where(x => x.BomId != null && x.BomId.ToString() != "").ToList();
                        decimal? TotalTax = 0;
                        foreach (var line in linesView)
                        {
                            var bomlineX = bomlines.Where(x => x.BomId == line.ItemCode).ToList();
                            line.Lines = bomlineX;
                            TotalTax += line.TaxAmt;
                        }
                      
                        //foreach(var line in bomlines)
                        //{
                        //    TSalesLineViewModel BOMheader = new TSalesLineViewModel();

                        //    if(bomHeader.Where(x=>x.BarCode!= BOMheader.BarCode).SingleOrDefault()!=null)
                        //    {
                        //        bomHeader.Add(BOMheader);
                        //    }

                        //}    
                        //if(bomHeader.Count > 0)
                        //{
                        //    foreach (var line in bomHeader)
                        //    {
                        //        var bomLine = bomlines.Where(x => x.BomId != line.BarCode).ToList();
                        //        line.Lines = bomLine;
                        //        linesView.Add(line);
                        //    }
                        //}     
                        var promoLines = db.Query<TSalesPromoViewModel>(queryPromo, null, commandType: CommandType.Text);
                        var deliveryLines = db.Query<TSalesDelivery>(queryDelivery, null, commandType: CommandType.Text);
                         //string loyaltySystem = $"select SettingValue from S_GeneralSetting with (nolock) where SettingId ='Loyalty' and CompanyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}' ";
                        //loyaltySystem = _saleHeaderRepository.GetScalar(loyaltySystem, null, commandType: CommandType.Text);
                       


                        order = _mapper.Map<SaleViewModel>(header);

                        //var linesBooklet = linesView.Where(x => string.IsNullOrEmpty(x.BookletNo)).ToList();
                        foreach (var item in linesView.Where(x => !string.IsNullOrEmpty(x.BookletNo)).ToList())
                        {
                            if(serialLines.Where(x => x.CustomF1 == item.BookletNo).FirstOrDefault()!= null)
                            {
                                item.EndDate = serialLines.Where(x => x.CustomF1 == item.BookletNo).FirstOrDefault().ExpDate;
                            }    
                           
                        }
                        order.Lines = linesView;// lines.ToList();

                        order.TotalTax = TotalTax;
                        order.SerialLines = serialLines.ToList();
                        order.PromoLines = promoLines.ToList();
                        order.Payments = payments;
                        order.Customer = customer; 
                        var invoice = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "Invoice").FirstOrDefault();

                        if (invoice != null && (invoice.SettingValue == "true" || invoice.SettingValue == "1"))
                        {
                            var invoiceLines = db.QueryFirstOrDefault<TSalesInvoice>(invoiceQuery, null, commandType: CommandType.Text);
                            order.Invoice = invoiceLines;
                        }
                        if (!string.IsNullOrEmpty(header.ContractNo))
                        {
                            var ContractSalesPayment = db.Query<TSalesPayment>(queryContractPayment, null, commandType: CommandType.Text);
                            order.ContractPayments = ContractSalesPayment.ToList();
                        }
                         

                        //if (queryContractPayment!= null &&  queryContractPayment.Success && queryContractPayment.Data!=null)
                        //{
                        //    order.ContractPayments = queryContractPayment.Data as List<TSalesPayment>;
                        //}    
                        order.Deliveries = deliveryLines.ToList();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                //return order;
                result.Success = true;
                result.Data = order;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetCheckInById(string Id, string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                SaleViewModel order = new SaleViewModel();

                TSalesHeader header = await _saleHeaderRepository.GetAsync($"USP_S_GetCheckInById N'{CompanyCode}',N'{StoreId}',N'{Id}'", null, commandType: CommandType.Text);
                if (header == null)
                    return null;
                string queryLine = $"USP_S_T_SaleLine_CheckIn N'{CompanyCode}' ,N'{Id}'";
                string queryLineSerial = $"select t1.* , t2.ItemName , t3.UOMName from T_SalesLineSerial t1 with(nolock) left join M_Item t2 with(nolock)  on t1.ItemCode = t2.ItemCode AND T1.CompanyCode = t2.CompanyCode left join M_UOM t3 with(nolock)  on t1.UOMCode = t3.UOMCode where t1.TransId = N'{Id}' and t1.CompanyCode = N'{CompanyCode}'";
                string queryPromo = $"select t1.* , t2.ItemName , t3.UOMName from T_SalesPromo t1 with(nolock)  left join M_Item t2 with(nolock)  on t1.ItemCode = t2.ItemCode AND T1.CompanyCode=t2.CompanyCode left join M_UOM t3 with(nolock)  on t1.UOMCode = t3.UOMCode where t1.TransId = N'{Id}' and t1.CompanyCode = N'{CompanyCode}'";
                string queryPayment = $"select * from T_SalesPayment with (nolock) where TransId=N'{Id}' and CompanyCode= N'{CompanyCode}'";
                string queryDelivery = $"select * from T_Sales_Delivery with (nolock) where TransId=N'{Id}' and CompanyCode= N'{CompanyCode}'";

                //List<TSalesLine> lines = await _saleLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                List<TSalesPayment> payments = await _salepaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId =N'{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<SaleViewModel>(header);
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TSalesLineViewModel>(queryLine, null, commandType: CommandType.Text);
                        var serialLines = db.Query<TSalesLineSerialViewModel>(queryLineSerial, null, commandType: CommandType.Text);
                        var linesView = new List<TSalesLineViewModel>();
                        var NoBom = lines.Where(x => x.BomId == null || x.BomId.ToString() == "").ToList();
                        foreach (var line in NoBom)
                        {
                            //if(!string.IsNullOrEmpty(line.StoreAreaId) && !string.IsNullOrEmpty(line.TimeFrameId))
                            //{
                            //    if()
                            //    line.Lines.Add(line);
                            //}    
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                            linesView.Add(line);
                        }

                        var bomHeader = new List<TSalesLineViewModel>();
                        var bomlines = lines.Where(x => x.BomId != null && x.BomId.ToString() != "").ToList();
                        foreach (var line in linesView)
                        {
                            var bomlineX = bomlines.Where(x => x.BomId == line.ItemCode).ToList();
                            line.Lines = bomlineX;
                        }
                        //foreach(var line in bomlines)
                        //{
                        //    TSalesLineViewModel BOMheader = new TSalesLineViewModel();

                        //    if(bomHeader.Where(x=>x.BarCode!= BOMheader.BarCode).SingleOrDefault()!=null)
                        //    {
                        //        bomHeader.Add(BOMheader);
                        //    }

                        //}    
                        //if(bomHeader.Count > 0)
                        //{
                        //    foreach (var line in bomHeader)
                        //    {
                        //        var bomLine = bomlines.Where(x => x.BomId != line.BarCode).ToList();
                        //        line.Lines = bomLine;
                        //        linesView.Add(line);
                        //    }
                        //}     
                        var promoLines = db.Query<TSalesPromoViewModel>(queryPromo, null, commandType: CommandType.Text);
                        var deliveryLines = db.Query<TSalesDelivery>(queryDelivery, null, commandType: CommandType.Text);
                        order = _mapper.Map<SaleViewModel>(header);
                        order.Lines = linesView;// lines.ToList();
                        order.SerialLines = serialLines.ToList();
                        order.PromoLines = promoLines.ToList();
                        order.Payments = payments;
                        order.Customer = customer;
                        order.Deliveries = deliveryLines.ToList();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                result.Success = true;
                result.Data = order;
                //return order;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }

        public async Task<GenericResult> GetCheckOutById(string Id, string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                SaleViewModel order = new SaleViewModel();

                TSalesHeader header = await _saleHeaderRepository.GetAsync($"USP_S_GetCheckOutById N'{CompanyCode}',N'{StoreId}',N'{Id}'", null, commandType: CommandType.Text);
                if (header == null)
                    return null;
                string queryLine = $"[USP_S_T_SaleLine_CheckOut] N'{CompanyCode}' ,N'{Id}'";
                string queryLineSerial = $"select t1.* , t2.ItemName , t3.UOMName from T_SalesLineSerial t1 with(nolock) left join M_Item t2 with(nolock)  on t1.ItemCode = t2.ItemCode AND T1.CompanyCode = t2.CompanyCode left join M_UOM t3 with(nolock)  on t1.UOMCode = t3.UOMCode where t1.TransId = N'{Id}' and t1.CompanyCode = N'{CompanyCode}'";
                string queryPromo = $"select t1.* , t2.ItemName , t3.UOMName from T_SalesPromo t1 with(nolock)  left join M_Item t2 with(nolock)  on t1.ItemCode = t2.ItemCode AND T1.CompanyCode=t2.CompanyCode left join M_UOM t3 with(nolock)  on t1.UOMCode = t3.UOMCode where t1.TransId = N'{Id}' and t1.CompanyCode = N'{CompanyCode}'";
                string queryPayment = $"select * from T_SalesPayment with (nolock) where TransId=N'{Id}' and CompanyCode= N'{CompanyCode}'";
                string queryDelivery = $"select * from T_Sales_Delivery with (nolock) where TransId=N'{Id}' and CompanyCode= N'{CompanyCode}'";

                //List<TSalesLine> lines = await _saleLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                List<TSalesPayment> payments = await _salepaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId =N'{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<SaleViewModel>(header);
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TSalesLineViewModel>(queryLine, null, commandType: CommandType.Text);
                        var serialLines = db.Query<TSalesLineSerialViewModel>(queryLineSerial, null, commandType: CommandType.Text);
                        var linesView = new List<TSalesLineViewModel>();
                        var NoBom = lines.Where(x => x.BomId == null || x.BomId.ToString() == "").ToList();
                        foreach (var line in NoBom)
                        {
                            //if(!string.IsNullOrEmpty(line.StoreAreaId) && !string.IsNullOrEmpty(line.TimeFrameId))
                            //{
                            //    if()
                            //    line.Lines.Add(line);
                            //}    
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                            linesView.Add(line);
                        }

                        var bomHeader = new List<TSalesLineViewModel>();
                        var bomlines = lines.Where(x => x.BomId != null && x.BomId.ToString() != "").ToList();
                        foreach (var line in linesView)
                        {
                            var bomlineX = bomlines.Where(x => x.BomId == line.ItemCode).ToList();
                            line.Lines = bomlineX;
                        }
                        //foreach(var line in bomlines)
                        //{
                        //    TSalesLineViewModel BOMheader = new TSalesLineViewModel();

                        //    if(bomHeader.Where(x=>x.BarCode!= BOMheader.BarCode).SingleOrDefault()!=null)
                        //    {
                        //        bomHeader.Add(BOMheader);
                        //    }

                        //}    
                        //if(bomHeader.Count > 0)
                        //{
                        //    foreach (var line in bomHeader)
                        //    {
                        //        var bomLine = bomlines.Where(x => x.BomId != line.BarCode).ToList();
                        //        line.Lines = bomLine;
                        //        linesView.Add(line);
                        //    }
                        //}     
                        var promoLines = db.Query<TSalesPromoViewModel>(queryPromo, null, commandType: CommandType.Text);
                        var deliveryLines = db.Query<TSalesDelivery>(queryDelivery, null, commandType: CommandType.Text);
                        order = _mapper.Map<SaleViewModel>(header);
                        order.Lines = linesView;// lines.ToList();
                        order.SerialLines = serialLines.ToList();
                        order.PromoLines = promoLines.ToList();
                        order.Payments = payments;
                        order.Customer = customer;
                        order.Deliveries = deliveryLines.ToList();

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                result.Success = true;
                result.Data = order;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }
        public Task<TSalesHeader> GetByUser(string User)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TSalesLine>> GetLinesById(string Id)
        {
            var data = await _saleLineRepository.GetAllAsync($"select * from T_SalesLine with (nolock) where TransId = N'%{Id}%'", null, commandType: CommandType.Text);
            return data;
        }

        public async Task<string> GetNewOrderCode(string CompanyCode, string StoreId)
        {
            string qu = $"select dbo.[fnc_AutoGenDocumentCode]('{PrefixSO}', N'{CompanyCode}', N'{StoreId}')";
            string key = _saleHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixSO}', N'{CompanyCode}',  N'{StoreId}')", null, commandType: CommandType.Text);
            return key;
        }

        public async Task<PagedList<TSalesHeader>> GetPagedList(UserParams userParams)
        {
            try
            {
                string query = $"select * from T_SalesHeader with (nolock) " +
                    $"where ( Remarks like N'%{userParams.keyword}%' or TransId like N'%{userParams.keyword}%' or StoreId like N'%{userParams.keyword}%' or CusId like N'%{userParams.keyword}%'  or SalesPerson like N'%{userParams.keyword}%' )";
                if (!string.IsNullOrEmpty(userParams.status))
                {
                    query += $" and Status=N'{userParams.status}'";
                }
                var data = await _saleHeaderRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.CusId);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.TransId);
                }
                return await PagedList<TSalesHeader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GenericResult> UpdateTimeFrame(List<TimeFrameLine> models)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _saleHeaderRepository.GetConnection())
            {
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();

                        string upSql = "";
                        foreach (TimeFrameLine line in models)
                        {
                            string sql = " update T_SalesLine set TimeFrame = '" + line.TimeFrame + "' where TransId=" + line.TransId + "' and ItemCode = N'"
                                + line.ItemCode + "' and BaseLine = '" + line.EcomLineId + "' and BaseTransId= '" + line.EcomId + "' ";
                            upSql += sql;
                        }
                        await db.ExecuteAsync(upSql, null, transaction: tran, commandType: CommandType.Text);
                        result.Success = true;
                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        result.Message = ex.Message;
                        result.Success = false;
                        result.Code = 504;
                        tran.Rollback();

                    }
                }
            }
            return result;
        }

        public async Task<GenericResult> Update(SaleViewModel model)
        {
            GenericResult result = new GenericResult();



            if (model.CompanyCode == null || string.IsNullOrEmpty(model.CompanyCode))
            {
                result.Success = false;
                result.Message = "Company Code not null.";
                return result;
            }
            if (model.Lines == null || model.Lines.Count() == 0)
            {
                result.Success = false;
                result.Message = "Doc line not null.";
                return result;
            }
            if (model.StoreId == null || string.IsNullOrEmpty(model.StoreId))
            {
                result.Success = false;
                result.Message = " Store not null.";
                return result;
            }
            if (string.IsNullOrEmpty(model.CusId))
            {
                result.Success = false;
                result.Message = " Customer not null.";
                return result;
            }
            if (model.Payments != null && model.Payments.Count > 0 && model.SalesMode.ToLower() != "return")
            {
                var payment = model.Payments.Where(x => x.CollectedAmount <= 0).ToList();
                if (payment != null && payment.Count > 0)
                {
                    result.Success = false;
                    result.Message = "Please complete progress payment. Can't payment with value 0";
                    return result;
                }
            }
            if (model.SalesMode.ToLower() == "return" || ((model.SalesMode.ToLower() == "ex" || model.SalesMode.ToLower() == "exchange") && model.TotalPayable < 0))
            {
                decimal? numOfPayment = 0;
                foreach (var line in model.Payments)
                {
                    numOfPayment += line.CollectedAmount;
                }
                if (Math.Abs((decimal)numOfPayment) != Math.Abs((decimal)model.TotalPayable))
                {
                    result.Success = false;
                    result.Message = "Please check return amount. Return amount can't different collected amount.";
                    return result;
                }

            }
            //if (model.DataSource.ToLower() != "pos" && model.POSType != "E")
            //{
            //    result.Success = false;
            //    result.Message = "Please check bill payments.";
            //    return result;
            //}
            if (model.DataSource.ToLower() == "pos" && model.Payments != null && model.Payments.Count > 0)
            {
                decimal? numOfPayment = 0;
                decimal? numOfLine = 0;
                foreach (var line in model.Payments)
                {
                    numOfPayment += line.CollectedAmount;
                }
                foreach (var line in model.Lines.Where(x => string.IsNullOrEmpty(x.BomId)))
                {
                    decimal discountNum = line.DiscountAmt == null ? 0 : (decimal)line.DiscountAmt;
                    numOfLine += (line.LineTotal - discountNum);
                }
                decimal discountTotal = model.DiscountAmount == null ? 0 : (decimal)model.DiscountAmount;
                decimal totalPayable = Math.Abs((decimal)numOfLine - discountTotal);
                if (Math.Abs((decimal)numOfPayment) < totalPayable)
                {
                    result.Success = false;
                    result.Message = "Please check bill and amount.";
                    return result;
                }
            }

            try
            {

                if (model.DataSource.ToLower() == "pos" && string.IsNullOrEmpty(model.ContractNo))
                {
                    if (model.TotalAmount - model.TotalDiscountAmt > model.TotalPayable && model.SalesMode.ToLower() == "sales" && model.Status != "H")
                    {
                        result.Success = false;
                        result.Message = "101: Please check your receipt amount. TotalAmount: " + model.TotalAmount.Value.ToString("C2") + " TotalDiscountAmt: " + model.TotalDiscountAmt.Value.ToString("C2") + " TotalPayable: " + model.TotalPayable.Value.ToString("C2") + " TotalReceipt " + model.TotalReceipt.Value.ToString("C2");
                        return result;
                    }
                    //if (model.TotalReceipt > model.TotalPayable  && model.SalesMode.ToLower() == "sales" && model.Status != "H")
                    //{
                    //    result.Success = false;
                    //    result.Message = "102: Please check your receipt amount. TotalAmount: " + model.TotalAmount.Value.ToString("C2") + " TotalDiscountAmt: " + model.TotalDiscountAmt.Value.ToString("C2") + "  TotalPayable: " + model.TotalPayable.Value.ToString("C2") + " TotalReceipt " + model.TotalReceipt.Value.ToString("C2");
                    //    return result;
                    //}
                    if (model.TotalAmount - model.TotalDiscountAmt > model.TotalPayable && model.SalesMode.ToLower() != "sales" && model.Status == "H")
                    {
                        result.Success = false;
                        result.Message = "103: Please check your receipt amount. TotalAmount: " + model.TotalAmount.Value.ToString("C2") + " TotalDiscountAmt: " + model.TotalDiscountAmt.Value.ToString("C2") + "  TotalPayable: " + model.TotalPayable.Value.ToString("C2") + " TotalReceipt " + model.TotalReceipt.Value.ToString("C2");
                        return result;
                    }
                    //if (model.Payments == null || (model.SalesMode.ToLower() == "sales" && model.Status != "H" && model.Payments.Count() == 0) || (model.Payments.Count == 0 && model.SalesMode.ToLower() != "sales" && model.Status != "H" && model.SalesMode != "EX"))
                    //{
                    //    result.Success = false;
                    //    result.Message = "Payment list not null.";
                    //    return result;
                    //}
                }
                List<string> holdList = new List<string>();
                using (IDbConnection db = _saleHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            string key = "";
                            try
                            {
                                var parameters = new DynamicParameters();
                                if (model.DataSource.ToLower() != "pos" && model.POSType.ToLower() == "e")
                                {
                                    foreach (var line in model.Lines)
                                    {
                                        string getItemType = $"select CustomField1 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                        string itemType = _saleHeaderRepository.GetScalar(getItemType, null, commandType: CommandType.Text);
                                        if (string.IsNullOrEmpty(itemType))
                                        {
                                            result.Success = false;
                                            result.Message = "Please check master data (Item Type ) with your admin. Item " + line.ItemCode;
                                            return result;
                                        }
                                        else
                                        {
                                            if (itemType.ToLower() == "class" || itemType.ToLower() == "member" || itemType.ToLower() == "voucher" || itemType.ToLower() == "card")
                                            {
                                                result.Success = false;
                                                result.Message = "Event can't order item " + line.ItemCode + " b/c item in " + itemType.ToLower() + " group";
                                                return result;
                                            }
                                        }
                                    }
                                    //var ItemBan = model.Lines.Where(x=>x.ItemType)
                                }
                                if (model.DataSource.ToLower() != "pos" && model.Lines.Count > 0)
                                {
                                    foreach (var line in model.Lines)
                                    {
                                        string getItemType = $"select CustomField1 from M_Item  with(nolock) where  ItemCode =N'{line.ItemCode}' ";
                                        string itemType = _saleHeaderRepository.GetScalar(getItemType, null, commandType: CommandType.Text);
                                        if (string.IsNullOrEmpty(itemType))
                                        {
                                            result.Success = false;
                                            result.Message = "Please check master data (Item Type ) with your admin. Item " + line.ItemCode;
                                            return result;
                                        }
                                        else
                                        {
                                            if (itemType.ToLower() == "class" || itemType.ToLower() == "member")
                                            {
                                                if (line.StartDate == null)
                                                {
                                                    result.Success = false;
                                                    result.Message = itemType.ToLower() + " Start Date not null. Please input value to Start Date";
                                                    return result;
                                                }
                                                if (line.EndDate == null)
                                                {
                                                    result.Success = false;
                                                    result.Message = itemType.ToLower() + " End Date not null. Please input value to End Date";
                                                    return result;
                                                }

                                            }

                                        }
                                    }
                                    //var ItemBan = model.Lines.Where(x=>x.ItemType)
                                }
                                if (model.IsCanceled == "C")
                                {
                                    string querycheck = $"select isnull(count(*),0) from T_SalesHeader with (nolock) where RefTransId = N'{model.RefTransId}' and CompanyCode = N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'";
                                    string num = _saleHeaderRepository.GetScalar(querycheck, null, commandType: CommandType.Text);
                                    if (int.Parse(num) > 0)
                                    {
                                        result.Success = false;
                                        result.Message = "Can't cancel order. Because the order have return/exchange.";
                                        return result;
                                    }
                                }

                                if (!string.IsNullOrEmpty(model.TransId))
                                {

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.TransId;
                                }
                                else
                                {
                                    key = _saleHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixSO}',N'{model.CompanyCode}', N'{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.TransId = key;
                                }
                                if (model.SalesMode != null && model.SalesMode == "Return")
                                {
                                    string checkResult = _saleHeaderRepository.GetScalar($"USP_Check_ReturnOrder N'{model.CompanyCode}', N'{model.StoreId}', N'{model.TransId}',N'{model.SalesType}',N'{model.SalesMode}'", null, commandType: CommandType.Text);
                                    if (checkResult == "0")
                                    {
                                        result.Success = false;
                                        result.Message = "Can't return order. Because the order date is not valid.";
                                        return result;
                                    }
                                }
                                string itemList = "";
                                string defaultWhs = _saleHeaderRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);

                                foreach (var line in model.Lines)
                                {
                                    if (string.IsNullOrEmpty(line.SlocId))
                                    {
                                        line.SlocId = defaultWhs;
                                    }
                                    //if(string.IsNullOrEmpty(line.BomId))
                                    //{
                                    if (string.IsNullOrEmpty(line.TimeFrameId) && line.Quantity > 0)
                                    {
                                        itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                    }
                                    //}     
                                }
                                if (model.SalesMode.ToLower() != "return")
                                {
                                    DynamicParameters newParameters = new DynamicParameters();
                                    newParameters.Add("CompanyCode", model.CompanyCode);
                                    newParameters.Add("ListLine", itemList);
                                    var resultCheck = db.Query<ResultModel>($"USP_I_T_SalesLine_CheckNegative", newParameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    if (resultCheck != null && resultCheck.ToList().Count > 0)
                                    {
                                        var line = resultCheck.FirstOrDefault();
                                        if (line != null && line.ID != 0)
                                        {
                                            result.Success = false;
                                            result.Message = line.Message;
                                            return result;
                                        }
                                    }
                                }

                                //Create and fill-up master table data


                                parameters.Add("TransId", model.TransId, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("ShiftId", model.ShiftId);
                                parameters.Add("ContractNo", model.ContractNo);
                                parameters.Add("CusId", model.CusId);
                                parameters.Add("CusIdentifier", model.CusIdentifier);

                                parameters.Add("TotalAmount", model.TotalAmount);
                                parameters.Add("TotalPayable", model.TotalPayable);
                                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                parameters.Add("TotalReceipt", model.TotalReceipt);
                                parameters.Add("AmountChange", model.AmountChange);
                                parameters.Add("PaymentDiscount", model.PaymentDiscount);

                                parameters.Add("TotalTax", model.TotalTax);
                                parameters.Add("DiscountType", model.DiscountType);
                                parameters.Add("DiscountAmount", model.DiscountAmount);
                                parameters.Add("DiscountRate", model.DiscountRate);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "C" : model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("Remarks", model.Remarks);
                                parameters.Add("SalesPerson", model.SalesPerson);
                                parameters.Add("SalesPersonName", model.SalesPersonName);
                                parameters.Add("SalesMode", model.SalesMode);
                                parameters.Add("RefTransId", model.RefTransId);
                                parameters.Add("ManualDiscount", model.ManualDiscount);
                                parameters.Add("SalesType", model.SalesType);
                                parameters.Add("DataSource", model.DataSource);
                                parameters.Add("POSType", model.POSType);
                                parameters.Add("Phone", model.Phone);
                                parameters.Add("CusName", model.CusName);
                                parameters.Add("CusAddress", model.CusAddress);
                                parameters.Add("Reason", model.Reason);
                                parameters.Add("TerminalId", model.TerminalId);
                                parameters.Add("RewardPoints", model.RewardPoints);
                                parameters.Add("ExpiryDate", model.ExpiryDate == null ? DateTime.Now : model.ExpiryDate);
                                parameters.Add("DocDate", model.DocDate == null ? DateTime.Now : model.DocDate);
                                parameters.Add("CustomF1", model.CustomF1);
                                parameters.Add("CustomF2", model.CustomF2);
                                parameters.Add("CustomF3", model.CustomF3);
                                parameters.Add("CustomF4", model.CustomF4);
                                parameters.Add("CustomF5", model.CustomF5);

                                if (model.SalesMode.ToLower() == "return" || model.SalesMode.ToLower() == "ex" || model.SalesMode.ToLower() == "exchange")
                                {
                                    parameters.Add("CollectedStatus", "Closed");
                                }
                                else
                                {
                                    if (model.DataSource == "POS")
                                    {
                                        parameters.Add("CollectedStatus", "Completed");
                                    }
                                    else
                                    {
                                        parameters.Add("CollectedStatus", "Hold");
                                    }

                                }
                                //_saleHeaderRepository.Insert("InsertSaleHeader", parameters, commandType: CommandType.StoredProcedure);

                                //Insert record in master table. Pass transaction parameter to Dapper.

                                string getLastInsert = _saleHeaderRepository.GetScalar($"SELECT top 1  CreatedOn from T_SalesHeader with (nolock) where CompanyCode = '{model.CompanyCode}' and  CreatedBy = '{model.CreatedBy}' and TerminalId = '{model.TerminalId}' order by CreatedOn desc", null, commandType: CommandType.Text);
                                if (!string.IsNullOrEmpty(getLastInsert))
                                {
                                    DateTime last = DateTime.Parse(getLastInsert);
                                    var diffInSeconds = (DateTime.Now - last).TotalSeconds;
                                    if (diffInSeconds < 1)
                                    {
                                        result.Success = false;
                                        result.Message = "Can't insert multi bill one time.";
                                        return result;
                                    }
                                }
                                if (!string.IsNullOrEmpty(model.LuckyNo))
                                {
                                    parameters.Add("LuckyNo", model.LuckyNo);
                                }

                                var affectedRows = db.Execute("USP_I_T_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //Get the Id newly created for master table record.
                                //If this is not an Identity, use different method here
                                //newId = Convert.ToInt64(connection.ExecuteScalar<object>("SELECT @@IDENTITY", null, transaction: transaction));

                                //Create and fill-up detail table data
                                //Use suitable loop as you want to insert multiple records.
                                //for(......)
                                int stt = 0;

                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    if (line.Quantity.HasValue && Math.Abs(line.Quantity.Value) > 0)
                                    {
                                        parameters = new DynamicParameters();
                                        parameters.Add("TransId", key, DbType.String);
                                        parameters.Add("LineId", stt);
                                        line.LineId = stt.ToString();
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", line.ItemCode);
                                        parameters.Add("BarCode", line.BarCode);
                                        parameters.Add("Uomcode", line.UomCode);
                                        parameters.Add("Quantity", line.Quantity);
                                        parameters.Add("Price", line.Price);
                                        parameters.Add("LineTotal", line.LineTotal);
                                        parameters.Add("DiscountType", string.IsNullOrEmpty(line.DiscountType) && string.IsNullOrEmpty(line.PromoType) ? line.PromoType : line.DiscountType);
                                        parameters.Add("DiscountAmt", !line.DiscountAmt.HasValue && line.PromoDisAmt.HasValue ? line.PromoDisAmt : line.DiscountAmt);
                                        parameters.Add("DiscountRate", !line.DiscountRate.HasValue && line.PromoPercent.HasValue ? line.PromoPercent : line.DiscountRate);
                                        parameters.Add("CreatedBy", line.CreatedBy);
                                        if (string.IsNullOrEmpty(line.SlocId))
                                        {

                                            line.SlocId = defaultWhs;
                                        }
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                        parameters.Add("PromoId", line.PromoId);
                                        parameters.Add("PromoType", line.PromoType);
                                        parameters.Add("Status", line.Status);
                                        parameters.Add("Remark", line.Remark);
                                        parameters.Add("PromoPercent", line.PromoPercent);
                                        parameters.Add("PromoBaseItem", line.PromoBaseItem);
                                        parameters.Add("SalesMode", line.SalesMode);
                                        parameters.Add("Remarks", line.Remark);
                                        parameters.Add("TaxRate", line.TaxRate);
                                        line.TaxAmt = ((line.Price * line.Quantity) + line.DiscountAmt == null ? 1 : line.DiscountAmt) * line.TaxRate / 100;
                                        parameters.Add("TaxAmt", line.TaxAmt);
                                        parameters.Add("TaxCode", line.TaxCode);
                                        parameters.Add("SlocId", line.SlocId);
                                        parameters.Add("MinDepositAmt", line.MinDepositAmt);
                                        parameters.Add("MinDepositPercent", line.MinDepositPercent);
                                        parameters.Add("DeliveryType", line.DeliveryType);
                                        parameters.Add("Posservice", line.Posservice);
                                        parameters.Add("StoreAreaId", line.StoreAreaId);
                                        parameters.Add("TimeFrameId", line.TimeFrameId);
                                        parameters.Add("Duration", line.Duration);
                                        parameters.Add("AppointmentDate", line.AppointmentDate);
                                        parameters.Add("BomId", line.BomId);
                                        parameters.Add("PromoPrice", line.PromoPrice);
                                        parameters.Add("PromoLineTotal", line.PromoLineTotal);
                                        parameters.Add("BaseLine", line.BaseLine);
                                        parameters.Add("BaseTransId", line.BaseTransId);
                                        parameters.Add("OpenQty", line.OpenQty);
                                        parameters.Add("PromoDisAmt", line.PromoDisAmt);
                                        parameters.Add("IsPromo", line.IsPromo);
                                        parameters.Add("IsSerial", line.IsSerial);
                                        parameters.Add("IsVoucher", line.IsVoucher);
                                        parameters.Add("Description", line.Description);
                                        parameters.Add("PrepaidCardNo", line.PrepaidCardNo);
                                        parameters.Add("MemberDate", line.MemberDate);
                                        parameters.Add("MemberValue", line.MemberValue);
                                        parameters.Add("StartDate", line.StartDate);
                                        parameters.Add("EndDate", line.EndDate);
                                        parameters.Add("ItemType", line.ItemType);
                                        parameters.Add("WeightScaleBarcode", line.WeightScaleBarcode);
                                        parameters.Add("StoreId", model.StoreId);
                                        parameters.Add("BookletNo", line.BookletNo);
                                        db.Execute("usp_I_T_SalesLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    }

                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                }
                                int sttLine = 0;
                                if (model.SerialLines != null && model.SerialLines.Count > 0)
                                {
                                    foreach (var line in model.SerialLines)
                                    {
                                        sttLine++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("TransId", key, DbType.String);
                                        parameters.Add("LineId", Guid.NewGuid());
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", line.ItemCode);
                                        if (string.IsNullOrEmpty(line.SlocId))
                                        {
                                            line.SlocId = defaultWhs;
                                        }
                                        parameters.Add("SerialNum", line.SerialNum);
                                        parameters.Add("Uomcode", line.UomCode);
                                        parameters.Add("SLocId", line.SlocId);
                                        parameters.Add("Quantity", line.Quantity);
                                        parameters.Add("Status", line.Status);
                                        parameters.Add("CreatedBy", line.CreatedBy);
                                        parameters.Add("OpenQty", line.OpenQty);
                                        parameters.Add("BaseLine", line.BaseLine);
                                        parameters.Add("BaseTransId", line.BaseTransId);
                                        parameters.Add("LineNum", sttLine);
                                        parameters.Add("Prefix", line.Prefix);
                                        parameters.Add("Phone", line.Phone);
                                        parameters.Add("Name", line.Name);
                                        parameters.Add("CustomF1", line.CustomF1);
                                        parameters.Add("CustomF2", line.CustomF2);
                                        parameters.Add("ExpDate", line.ExpDate);
                                        parameters.Add("StoreId", model.StoreId);
                                        db.Execute("USP_I_T_SalesLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }
                                if (model.PromoLines != null && model.PromoLines.Count > 0)
                                {
                                    foreach (var line in model.PromoLines)
                                    {
                                        if (!string.IsNullOrEmpty(line.PromoId))
                                        {
                                            stt++;
                                            string[] splt = line.PromoId.Split(",");
                                            //foreach(var voucherCheck in model.VoucherApply)
                                            //{
                                            //    if(voucherCheck.discount_code===splt.)
                                            //}    
                                            //var voucher = model.VoucherApply.Where(x => splt.Any(y=>y.Contains(x.discount_code))).FirstOrDefault();
                                            var voucher = model.VoucherApply.Where(x => splt.Any(y => y == x.discount_code)).FirstOrDefault();
                                            parameters = new DynamicParameters();
                                            parameters.Add("TransId", key, DbType.String);
                                            //parameters.Add("LineId", stt);
                                            parameters.Add("CompanyCode", model.CompanyCode);
                                            parameters.Add("ItemCode", line.ItemCode);
                                            parameters.Add("BarCode", line.BarCode);
                                            parameters.Add("Uomcode", line.UomCode);

                                            if (voucher != null)
                                            {
                                                parameters.Add("RefTransId", voucher.voucher_code);
                                                parameters.Add("ApplyType", "Ecom");
                                            }
                                            else
                                            {
                                                parameters.Add("RefTransId", line.RefTransId);
                                                parameters.Add("ApplyType", line.ApplyType);
                                            }
                                            parameters.Add("ItemGroupId", line.ItemGroupId);
                                            parameters.Add("Value", line.Value);
                                            parameters.Add("PromoId", line.PromoId);
                                            parameters.Add("PromoType", line.PromoType);
                                            parameters.Add("PromoTypeLine", line.PromoTypeLine);
                                            parameters.Add("Status", line.Status);
                                            parameters.Add("CreatedBy", line.CreatedBy);
                                            parameters.Add("PromoAmt", line.PromoAmt);
                                            parameters.Add("PromoPercent", line.PromoPercent);
                                            parameters.Add("StoreId", model.StoreId);
                                            //USP_I_T_SalesPromo
                                            //USP_U_T_SalesLineSerial
                                            db.Execute("USP_I_T_SalesPromo", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                        }
                                    }
                                }
                                stt = 0;
                                var document = MapSOtoDocument(model);
                                document.UDiscountAmount = (double)(model.DiscountAmount ?? 0);
                                if (model.Payments != null && model.Payments.Count > 0)
                                {
                                    foreach (var payment in model.Payments)
                                    {
                                        stt++;
                                        if (string.IsNullOrEmpty(payment.Currency))
                                        {
                                            string CurrencyStr = $"select CurrencyCode from M_Store with (nolock) where StoreId =N'{model.StoreId}' and CompanyCode =N'{model.CompanyCode}' ";
                                            string Currency = _saleHeaderRepository.GetScalar(CurrencyStr, null, commandType: CommandType.Text);
                                            payment.Currency = Currency;
                                        }
                                        parameters = new DynamicParameters();
                                        parameters.Add("PaymentCode", payment.PaymentCode, DbType.String);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("TransId", key);
                                        parameters.Add("LineId", stt);
                                        parameters.Add("TotalAmt", payment.TotalAmt == null ? payment.ChargableAmount : payment.TotalAmt);
                                        parameters.Add("ReceivedAmt", payment.ReceivedAmt);
                                        parameters.Add("PaidAmt", payment.PaidAmt);

                                        if ((payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0) > 0 && (payment.ChangeAmt == 0 || payment.ChangeAmt == null))
                                        {
                                            payment.ChangeAmt = (payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0);
                                        }
                                        parameters.Add("ChangeAmt", payment.ChangeAmt);
                                        parameters.Add("PaymentMode", payment.PaymentMode);
                                        parameters.Add("CardType", payment.CardType);
                                        parameters.Add("CardHolderName", payment.CardHolderName);
                                        parameters.Add("CardNo", payment.CardNo);
                                        parameters.Add("VoucherBarCode", payment.VoucherBarCode);
                                        parameters.Add("VoucherSerial", payment.VoucherSerial);
                                        parameters.Add("CreatedBy", payment.CreatedBy);
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                        parameters.Add("Status", payment.Status);
                                        parameters.Add("ChargableAmount", payment.ChargableAmount);
                                        parameters.Add("PaymentDiscount", payment.PaymentDiscount);
                                        parameters.Add("CollectedAmount", payment.CollectedAmount);
                                        parameters.Add("RefNumber", payment.RefNumber);
                                        parameters.Add("DataSource", payment.DataSource);
                                        parameters.Add("Currency", payment.Currency);
                                        parameters.Add("Rate", payment.Rate);
                                        parameters.Add("FCAmount", payment.FCAmount);
                                        parameters.Add("ShiftId", model.ShiftId);
                                        parameters.Add("TerminalId", model.TerminalId);
                                        parameters.Add("CardExpiryDate", payment.CardExpiryDate);
                                        parameters.Add("AdjudicationCode", payment.AdjudicationCode);
                                        parameters.Add("AuthorizationDateTime", payment.AuthorizationDateTime);
                                        parameters.Add("StoreId", model.StoreId);
                                        if (!string.IsNullOrEmpty(payment.CardNo))
                                        {
                                            //var prepaidCar = await GetPrepaidCard(model.CompanyCode, payment.CardNo);
                                            var prepaidCarDảta = await GetPrepaidCard(model.CompanyCode, payment.CardNo);
                                            var prepaidCar = prepaidCarDảta.Data as MPrepaidCard;
                                            decimal main = prepaidCar.MainBalance == null ? 0 : prepaidCar.MainBalance.Value;
                                            decimal sub = prepaidCar.SubBalance == null ? 0 : prepaidCar.SubBalance.Value;
                                            if (main + sub <= 0 || payment.CollectedAmount > main + sub)
                                            {
                                                tran.Rollback();
                                                result.Success = false;
                                                result.Message = "Balance of Card No " + payment.CardNo + " not available.";
                                                return result;
                                            }

                                        }

                                        db.Execute("USP_I_T_SalesPayment", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                        //if(payment.PaymentCode == "TapTap")
                                        //{
                                        //    var resultHold = await HoldVoucher(payment.RefNumber, model.CusId, model.StoreId, key);
                                        //    if (resultHold.Status != 0)
                                        //    {
                                        //        //if (holdList != null && holdList.Count > 0)
                                        //        //{
                                        //        //    foreach (var voucher in holdList)
                                        //        //    {
                                        //        //        await UnholdVoucher(voucher, model.CusId, model.StoreId, key);
                                        //        //    }
                                        //        //}
                                        //        //tran.Rollback();
                                        //        //result.Success = false;
                                        //        //result.Message = resultHold.Msg;
                                        //        //return result; 
                                        //        throw new Exception(resultHold.Msg);
                                        //    }    
                                        //    holdList.Add(payment.RefNumber);
                                        //}    
                                        if (payment.PaymentCode == "Point")
                                        {
                                            document.UDiscountAmount += payment.CollectedAmount == null ? 0 : (double)payment.CollectedAmount;
                                            //db.Execute($"USP_UpdateLoyaltyPoint N'{model.CompanyCode}' ,N'{model.CusId}' , N'{payment.RefNumber}'", parameters, commandType: CommandType.Text, transaction: tran);
                                            _loyaltyService.InsertLoyaltyLog(true, document, 0, double.Parse(payment.RefNumber), double.Parse(payment.CollectedAmount.ToString()), out string _);
                                        }
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }
                                //foreach(var prepaidTrans in model.PrepaidLines)
                                //{
                                //    parameters = new DynamicParameters();
                                //    parameters.Add("TransId", key, DbType.String);
                                //    parameters.Add("LineId", stt);
                                //    parameters.Add("CompanyCode", model.CompanyCode);
                                //    parameters.Add("TransId", prepaidTrans.TransId);
                                //    parameters.Add("PepaidCardNo", prepaidTrans.PepaidCardNo);
                                //    parameters.Add("TransType", prepaidTrans.TransType);
                                //    parameters.Add("MainBalance", prepaidTrans.MainBalance);
                                //    parameters.Add("SubBlance", prepaidTrans.SubBlance);
                                //    parameters.Add("LineTotal", line.LineTotal);
                                //    parameters.Add("DiscountType", line.DiscountType);
                                //    db.Execute("usp_I_T_SalesLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //}    
                                //if (model.Vouchers != null && model.Vouchers.Count > 0)
                                //{
                                //    int sttVoucher = 0;
                                //    foreach (var prepaidTrans in model.Vouchers)
                                //    {
                                //        sttVoucher++;
                                //        parameters = new DynamicParameters();
                                //        parameters.Add("Id", Guid.NewGuid(), DbType.String);
                                //        parameters.Add("TransId", key, DbType.String);
                                //        parameters.Add("LineId", sttVoucher);
                                //        parameters.Add("CompanyCode", model.CompanyCode);
                                //        parameters.Add("TransId", prepaidTrans.TransId);
                                //        parameters.Add("PepaidCardNo", prepaidTrans.PepaidCardNo);
                                //        parameters.Add("TransType", prepaidTrans.TransType);
                                //        parameters.Add("MainBalance", prepaidTrans.MainBalance);
                                //        parameters.Add("SubBlance", prepaidTrans.SubBlance);
                                //        parameters.Add("LineTotal", line.LineTotal);
                                //        parameters.Add("DiscountType", line.DiscountType);
                                //        db.Execute("usp_I_T_SalesLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //    }
                                //}

                                if (model.Invoice != null)
                                {
                                    parameters = new DynamicParameters();

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId, DbType.String);
                                    parameters.Add("StoreId", model.StoreId);
                                    parameters.Add("StoreName", model.StoreName);
                                    parameters.Add("CustomerName", model.Invoice.CustomerName);
                                    parameters.Add("Name", model.Invoice.Name);
                                    parameters.Add("TaxCode", model.Invoice.TaxCode);
                                    parameters.Add("Email", model.Invoice.Email);
                                    parameters.Add("Address", model.Invoice.Address);
                                    parameters.Add("Phone", model.Invoice.Phone);
                                    parameters.Add("Remark", model.Invoice.Remark);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    db.Execute("USP_I_T_SalesInvoice", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                }

                                if (model.DataSource == "POS" && model.Status != "H")
                                {
                                    //Tạo Delivery
                                    TSalesDelivery delivery = new TSalesDelivery();
                                    delivery.TransId = key;
                                    delivery.CompanyCode = model.CompanyCode;
                                    delivery.DeliveryFee = 0;
                                    delivery.DeliveryMethod = "Giao tai cua hang";
                                    delivery.DeliveryType = "NONE";
                                    parameters = new DynamicParameters();

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId, DbType.String);
                                    parameters.Add("DeliveryType", delivery.DeliveryType);
                                    parameters.Add("DeliveryMethod", delivery.DeliveryMethod);
                                    parameters.Add("DeliveryFee", delivery.DeliveryFee);

                                    db.Execute("USP_I_T_Sales_Delivery", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    //Tạo Invoice

                                    InvoiceViewModel invoice = new InvoiceViewModel();
                                    invoice = _mapper.Map<InvoiceViewModel>(model);
                                    invoice.TransId = "";
                                    invoice.RefTransId = model.TransId;
                                    foreach (var line in invoice.Lines)
                                    {
                                        if (!string.IsNullOrEmpty(line.LineId))
                                        {
                                            line.BaseLine = int.Parse(line.LineId);
                                            line.BaseTransId = model.TransId;
                                        }
                                    }
                                    foreach (var line in invoice.Payments)
                                    {
                                        line.RefTransId = model.TransId;
                                    }
                                    result = await CreateInvoice(invoice, db, tran);
                                    if (result.Success == false)
                                    {
                                        tran.Rollback();
                                        return result;
                                    }

                                }
                                else
                                {
                                    if (model.Deliveries != null && model.Deliveries.Count > 0)
                                    {
                                        foreach (var delivery in model.Deliveries)
                                        {
                                            //TSalesDelivery delivery = new TSalesDelivery();
                                            //delivery.TransId = key;
                                            //delivery.CompanyCode = model.CompanyCode;
                                            //delivery.DeliveryFee = 0;
                                            //delivery.DeliveryMethod = "Giao tai cua hang";
                                            //delivery.DeliveryType = "NONE";
                                            parameters = new DynamicParameters();

                                            parameters.Add("CompanyCode", model.CompanyCode);
                                            parameters.Add("TransId", model.TransId, DbType.String);
                                            parameters.Add("DeliveryType", delivery.DeliveryType);
                                            parameters.Add("DeliveryMethod", delivery.DeliveryMethod);
                                            parameters.Add("DeliveryFee", delivery.DeliveryFee);
                                            db.Execute("USP_I_T_Sales_Delivery", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        }
                                    }
                                }

                                if (model.IsCanceled == "C")
                                {
                                    db.Execute($"Update T_SalesHeader set IsCanceled = 'Y' where TransId=N'{model.RefTransId}' and CompanyCode=N'{model.CompanyCode}' and StoreId=N'{model.StoreId}'", null, commandType: CommandType.Text, transaction: tran);
                                }
                                if (model.VoucherApply != null && model.VoucherApply.Count > 0)
                                {
                                    foreach (TapTapVoucherDetails voucher in model.VoucherApply)
                                    {
                                        if (voucher.source == "MWI")
                                        {

                                        }
                                        else
                                        {
                                            var resultHold = await HoldVoucher(voucher.voucher_code, model.CusId, model.StoreId, key);
                                            if (resultHold.Status != 0)
                                            {
                                                throw new Exception(resultHold.Msg);
                                            }
                                            holdList.Add(voucher.voucher_code);
                                        }

                                    }
                                }
                                //if (model.VoucherApply != null && model.VoucherApply.Count > 0)
                                //{
                                //    foreach (var voucher in model.Vouchers)
                                //    {
                                //        var resultRedeem = await Red(voucher.VoucherCode, model.CusId, model.StoreId, key);
                                //        redeemList.Add(voucher.VoucherCode);
                                //    }
                                //}

                                tran.Commit();
                                string loyaltySystem = $"select SettingValue from S_GeneralSetting with (nolock) where SettingId ='Loyalty' and CompanyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}' ";
                                loyaltySystem = _saleHeaderRepository.GetScalar(loyaltySystem, null, commandType: CommandType.Text);
                                if (!string.IsNullOrEmpty(loyaltySystem) && loyaltySystem == "true")
                                {
                                    var point = _loyaltyService.ApplyLoyalty(document, out string _);
                                    model.RewardPoints = point;
                                }

                                result.Success = true;
                                result.Message = key;
                                model.TransId = key;
                                result.Data = model;

                            }
                            catch (Exception ex)
                            {
                                if (holdList != null && holdList.Count > 0)
                                {
                                    foreach (var voucher in holdList)
                                    {
                                        var resultRedeem = await UnholdVoucher(voucher, model.CusId, model.StoreId, key);

                                    }
                                }
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //throw new NotImplementedException();
        }

        public async Task<GenericResult> CloseOMSEvent(CloseEventViewModel model)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _saleHeaderRepository.GetConnection())
            {
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();

                        string upSql = $"UPDATE [T_SalesHeader] SET [Status] = 'C', [CollectedStatus] = 'Closed', ModifiedOn = GETDATE(), ModifiedBy = 'MWI.API' WHERE DataSource = 'ECOM' AND POSType = 'E' AND Status = N'O' AND CompanyCode = N'{model.CompanyCode}' AND ContractNo = N'{model.ContractNo}' ";

                        await db.ExecuteAsync(upSql, null, transaction: tran, commandType: CommandType.Text);
                        result.Success = true;
                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        result.Message = ex.Message;
                        result.Success = false;
                        result.Code = 504;
                        tran.Rollback();
                    }
                }
            }
            return result;
        }

        public async Task<GenericResult> PrintReceiptAsync(string companyCode, string transId, string storeId, string printStatus,
            string size, string printName)
        {
            GenericResult result = new GenericResult();
            //result.Success = true;
            try
            {
                SaleViewModel model = null;

                GenericResult SOData = await GetOrderById(transId, companyCode, storeId);
                if (SOData != null && SOData.Data != null)
                {
                    model = (SaleViewModel)SOData.Data;
                }

                if (model != null)
                {
                    if (size == "57")
                    {
                        RPFO.Application.PrintLayout.PrintReceipt_55 receipt = new PrintLayout.PrintReceipt_55();
                        receipt.SetModel(model, printStatus);
                        receipt.CreateDocument();
                        receipt.Print();


                    }
                    else
                    {
                        RPFO.Application.PrintLayout.PrintReceipt receipt = new PrintLayout.PrintReceipt();
                        receipt.SetModel(model, printStatus);
                        receipt.CreateDocument();
                        receipt.Print();

                    }

                    var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                    List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                    if (settingData.Success)
                    {

                        SettingList = settingData.Data as List<GeneralSettingStore>;


                    }
                    byte[] paperCut = PaperCut;
                    paperCut.Print(printName);

                    string openDrawer = "false";
                    switch (printStatus.ToLower())
                    {
                        case "receipt re-print":
                            // code block
                            var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "OpenDrawerOnReprint").FirstOrDefault();
                            if (setting != null)
                            {
                                openDrawer = setting.SettingValue;
                            }
                            break;
                        case "hold":
                            // code block
                            var settingHold = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "OpenDrawerOnHold").FirstOrDefault();
                            if (settingHold != null)
                            {
                                openDrawer = settingHold.SettingValue;
                            }
                            break;
                        default:
                            // code block
                            var settingRc = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "OpenDrawerOnReceipt").FirstOrDefault();
                            if (settingRc != null)
                            {
                                openDrawer = settingRc.SettingValue;
                            }
                            break;
                    }
                    if (openDrawer == "true")
                    {

                        if (string.IsNullOrEmpty(model.SalesMode))
                        {
                            model.SalesMode = "";
                        }
                        if (string.IsNullOrEmpty(printName))
                        {
                            printName = "";
                        }
                        if ((model.SalesMode.ToLower() == "sales" || model.SalesMode.ToLower() == "retail") && printName != "")
                        {

                            byte[] openCash = OpenDrawer;
                            openCash.Print(printName);

                        }
                    }

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Code = -1;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Exception: " + ex.Message;
            }

            return result;
        }
    }

}
