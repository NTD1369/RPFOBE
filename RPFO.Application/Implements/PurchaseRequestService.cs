using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
   public class PurchaseRequestService: IPurchaseRequestService
    {
        private readonly IGenericRepository<TPurchaseRequestHeader> _poHeaderRepository;
        private readonly IGenericRepository<TPurchaseRequestLine> _poLineRepository;
        private readonly IGenericRepository<TPurchaseRequestLineSerial> _poLineSerialRepository;
        //private readonly IGenericRepository<TPurchaseRequestPayment> _popaymentLineRepository;
        private readonly IGenericRepository<MCustomer> _customerRepository;
        IMapper _mapper;
        private IResponseCacheService cacheService;
        private string PrefixCacheActionPU = "QAPU-{0}-{1}";
        private TimeSpan timeQuickAction = TimeSpan.FromSeconds(15);
        private string PrefixPR = "";
        string ServiceName = "T_PurchaseRequest";
        List<string> TableNameList = new List<string>();
        private readonly ICommonService _commonService;
        public PurchaseRequestService(IGenericRepository<TPurchaseRequestHeader> poHeaderRepository, IConfiguration config, ICommonService commonService,
        IGenericRepository<TPurchaseRequestLine> poLineRepository, IGenericRepository<TPurchaseRequestLineSerial>
            poLineSerialRepository, IGenericRepository<MCustomer> customerRepository, IMapper mapper, IResponseCacheService responseCacheService)
        {
            _poHeaderRepository = poHeaderRepository;
            _poLineRepository = poLineRepository;
            _poLineSerialRepository = poLineSerialRepository;
            _customerRepository = customerRepository;
            _commonService = commonService;
            PrefixPR = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixPR"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            _mapper = mapper;
            this.cacheService = responseCacheService;
            string timeCache = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("TimeCacheAction"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (!string.IsNullOrEmpty(timeCache) && double.TryParse(timeCache, out double timeAction))
            {
                timeQuickAction = TimeSpan.FromSeconds(timeAction);
            }
            if (string.IsNullOrEmpty(PrefixPR))
            {
                PrefixPR = "PU";
            }
            TableNameList.Add(ServiceName + "Line");
            TableNameList.Add(ServiceName + "LineSerial");
            _commonService.InitService(ServiceName, TableNameList);
        }

        public class ResultModel
        {
            public int ID { get; set; }
            public string Message { get; set; }
        }
        public async Task<GenericResult> UpdateStatus(PurchaseRequestViewModel model)
        {
            GenericResult result = new GenericResult();
            if (model.DocDate == null)
            {
                result.Success = false;
                result.Message = "Doc date not null.";
                return result;
            }
            if (model.Lines == null || model.Lines.Count() == 0)
            {
                result.Success = false;
                result.Message = "Doc line not null.";
                return result;
            }
            //else
            //{
            //    var receiptLine = model.Lines.Where(x => x.Quantity <= 0).ToList();
            //    if (receiptLine != null)
            //    {
            //        result.Success = false;
            //        result.Message = "Quantity of line can't equal 0.";
            //        return result;
            //    }
            //}
            if (model.StoreId == null)
            {
                result.Success = false;
                result.Message = "From Store / To Store not null.";
                return result;
            }

            try
            {

                using (IDbConnection db = _poHeaderRepository.GetConnection())
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

                                string query = $"update T_PurchaseRequestHeader set Status = 'C',  DocStatus = 'C' where CompanyCode='{model.CompanyCode}' and PurchaseId = '{model.PurchaseId}'  and StoreId = '{model.StoreId}'";
                                var delAffectedRows = db.Execute(query, parameters, commandType: CommandType.Text, transaction: tran);
                                result.Success = true;
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
        public async Task<GenericResult> SavePO(PurchaseRequestViewModel model)
        {
            //return await SavePOByTableType(model);

            GenericResult result = new GenericResult();
            if (model.DocDate == null)
            {
                result.Success = false;
                result.Message = "Doc date not null.";
                return result;
            }
            if (model.DocDueDate == null)
            {
                result.Success = false;
                result.Message = "Doc due date not null.";
                return result;
            }

            if (model.Lines == null || model.Lines.Count() == 0)
            {
                result.Success = false;
                result.Message = "Doc line not null.";
                return result;
            }
            if (model.StoreId == null)
            {
                result.Success = false;
                result.Message = " Store   not null.";
                return result;
            }
            if (model.CompanyCode == null)
            {
                result.Success = false;
                result.Message = "CompanyCode not null.";
                return result;
            }
            try
            {
                //if(model.Payments.Count == 0)
                //{
                //    result.Success = false;
                //    result.Message = "Payment list not null.";
                //    return result;
                //}    
                using (IDbConnection db = _poHeaderRepository.GetConnection())
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
                                bool isNew = false;
                                if (!string.IsNullOrEmpty(model.PurchaseId))
                                {

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("PurchaseId", model.PurchaseId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_PurchaseRequestHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.PurchaseId;
                                    isNew = false;
                                }
                                else
                                {
                                   
                                    key = _poHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixPR}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.PurchaseId = key;
                                    isNew = true;
                                }
                                string itemList = "";
                                string keyCache = string.Format(PrefixCacheActionPU, model.StoreId, model.TerminalId);
                                string storeCache = cacheService.GetCachedData<string>(keyCache);
                                if (string.IsNullOrEmpty(storeCache))
                                {
                                    cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
                                }
                                else
                                {
                                    result.Success = false;
                                    result.Message = "Your actions are too fast and too dangerous. Please wait for your PO to be completed.";
                                    return result;
                                }

                                parameters.Add("PurchaseId", model.PurchaseId, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("DocStatus", model.DocStatus);
                                parameters.Add("DocDate", model.DocDate);
                                parameters.Add("DocDueDate", model.DocDueDate);
                                parameters.Add("CardCode", model.CardCode);

                                parameters.Add("CardName", model.CardName);
                                parameters.Add("InvoiceAddress", model.InvoiceAddress);
                                parameters.Add("TaxCode", model.TaxCode);
                                parameters.Add("VatTotal", model.Vattotal);
                                parameters.Add("DocTotal", model.DocTotal);
                                parameters.Add("Comment", model.Comment);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("ModifiedBy", model.ModifiedBy);
                                parameters.Add("ModifiedOn", model.ModifiedOn);
                                parameters.Add("Status", model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("DataSource", model.DataSource);
                                if (isNew)
                                {
                                    parameters.Add("CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    parameters.Add("ModifiedBy", null);
                                    parameters.Add("ModifiedOn", null);
                                }
                                else
                                {
                                    parameters.Add("CreatedOn", model.CreatedOn);
                                    parameters.Add("ModifiedBy", model.ModifiedBy);
                                    parameters.Add("ModifiedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                }

                                var affectedRows = db.Execute("USP_I_T_PurchaseRequestHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);


                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    parameters = new DynamicParameters();
                                    parameters.Add("PurchaseId", key, DbType.String);
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("LineId", stt);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("SlocId", line.SlocId);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Description", line.Description);
                                    parameters.Add("Uomcode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity == null ? null : line.Quantity);
                                    parameters.Add("OpenQty", line.OpenQty == null ? null : line.OpenQty);
                                    parameters.Add("Price", line.Price == null ? null : line.Price);
                                    parameters.Add("BaseSAPId", line.BaseSAPId == null ? null : line.BaseSAPId);
                                    parameters.Add("BaseSAPLine", line.BaseSAPLine == null ? null : line.BaseSAPLine);
                                    //parameters.Add("BaseEntry",  line.BaseEntry == null ? null : line.BaseEntry);
                                    parameters.Add("LineStatus", line.LineStatus == null ? null : line.LineStatus);
                                    parameters.Add("DiscPercent", line.DiscPercent);
                                    parameters.Add("VATPercent", line.Vatpercent);
                                    parameters.Add("LineTotal", line.LineTotal == null ? null : line.LineTotal);
                                    parameters.Add("Comment", line.Comment);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    //parameters.Add("ModifiedBy", string.IsNullOrEmpty(line.ModifiedBy.ToString()) ? null : line.ModifiedBy);
                                    //parameters.Add("ModifiedOn", string.IsNullOrEmpty(line.ModifiedOn.ToString()) ? null : line.ModifiedOn);
                                    parameters.Add("Status", line.Status);
                                    parameters.Add("FromDate", line.FromDate);
                                    parameters.Add("ToDate", line.ToDate);
                                    parameters.Add("NumOfDate", line.NumOfDate);
                                    parameters.Add("SalesDay", line.SalesDay);
                                    parameters.Add("SalesWeek", line.SalesWeek);
                                    parameters.Add("SalesMonth", line.SalesMonth);
                                    parameters.Add("Turns", line.Turns);
                                    parameters.Add("LastQtyOrder", line.LastQtyOrder);
                                    parameters.Add("CustomField1", line.CustomField1);
                                    parameters.Add("CustomField2", line.CustomField2);
                                    parameters.Add("CustomField3", line.CustomField3);
                                    parameters.Add("CustomField4", line.CustomField4);
                                    parameters.Add("CustomField5", line.CustomField5);
                                    parameters.Add("CustomField6", line.CustomField6);
                                    parameters.Add("CustomField7", line.CustomField7);
                                    parameters.Add("CustomField8", line.CustomField8);
                                    parameters.Add("CustomField9", line.CustomField9);
                                    parameters.Add("CustomField10", line.CustomField10);

                                    if (isNew)
                                    {
                                        parameters.Add("CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                    }
                                    else
                                    {
                                        parameters.Add("CreatedOn", model.CreatedOn);
                                        parameters.Add("ModifiedBy", model.ModifiedBy);
                                        parameters.Add("ModifiedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    //string queryLine = $"usp_I_T_poLine '{key}','{stt}','{model.CompanyCode}','{line.ItemCode}','{line.BarCode}','{line.UomCode}','{line.Quantity}','{line.Price}'" +
                                    //    $",'{line.LineTotal}','{line.DiscountType}','{line.DiscountAmt}','{line.DiscountRate}','{line.CreatedBy}','{line.PromoId}','{line.PromoType}','{line.Status}','{line.Remark}'" +
                                    //    $",'{line.PromoPercent}','{line.PromoBaseItem}','{ line.SalesMode}','{line.TaxRate}','{line.TaxAmt}','{line.TaxCode}','{line.SlocId}','{line.MinDepositAmt}','{line.MinDepositPercent}'" +
                                    //    $",'{line.DeliveryType}','{line.Posservice}','{line.StoreAreaId}','{line.TimeFrameId}','{line.AppointmentDate}','{line.BomId}','{line.PromoPrice}','{line.PromoLineTotal}','{line.BaseLine}','{line.BaseTransId}','{line.OpenQty}'";
                                    ////_poHeaderRepository.GetConnection().Get("",);



                                    db.Execute("USP_I_T_PurchaseRequestLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    //db.Execute("usp_I_T_poLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _poHeaderRepository.GetConnection().InsertAsync<string, TPurchaseRequestLine>(line);
                                }
                                int sttSer = 0;
                                foreach (var line in model.SerialLines)
                                {
                                    sttSer++;
                                    parameters = new DynamicParameters();
                                    parameters.Add("PurchaseId", key, DbType.String);
                                    parameters.Add("LineId", sttSer);
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("SerialNum", line.SerialNum);
                                    parameters.Add("SLocId", line.SlocId);
                                    parameters.Add("Uomcode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    parameters.Add("Description", line.Description);
                                    if (isNew)
                                    {
                                        parameters.Add("CreatedOn", DateTime.Now.ToString("yyyy-MM-dd"));
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                    }
                                    else
                                    {
                                        parameters.Add("CreatedOn", model.CreatedOn);
                                        parameters.Add("ModifiedBy", model.ModifiedBy);
                                        parameters.Add("ModifiedOn", DateTime.Now.ToString("yyyy-MM-dd"));
                                    }
                                    parameters.Add("OpenQty", line.OpenQty);

                                    db.Execute("USP_I_T_PurchaseRequestLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _poHeaderRepository.GetConnection().InsertAsync<string, TPurchaseRequestLine>(line);
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


        public async Task<GenericResult> SavePOByTableType(PurchaseRequestViewModel model)
        {
            GenericResult result = new GenericResult();
            if (model.Lines == null || model.Lines.Count() == 0)
            {
                result.Success = false;
                result.Message = "Doc line not null.";
                return result;
            }
            if (model.StoreId == null)
            {
                result.Success = false;
                result.Message = " Store   not null.";
                return result;
            }
            if (model.CompanyCode == null)
            {
                result.Success = false;
                result.Message = "CompanyCode not null.";
                return result;
            }
            try
            {
                //if(model.Payments.Count == 0)
                //{
                //    result.Success = false;
                //    result.Message = "Payment list not null.";
                //    return result;
                //}    
                using (IDbConnection db = _poHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {

                                string PRLineTbl = ServiceName + "Line";
                                var PRLines = _commonService.CreaDataTable(PRLineTbl);
                                string PRSerialTbl = ServiceName + "LineSerial";

                                var PRLineSerial = _commonService.CreaDataTable(PRSerialTbl);

                                if (PRLines == null || PRLineSerial == null)
                                {
                                    result.Success = false;
                                    result.Message = "Table Type Object can't init";
                                    return result;
                                }

                                var parameters = new DynamicParameters();
                                string key = "";
                                bool isNew = false;
                                if (!string.IsNullOrEmpty(model.PurchaseId))
                                {

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("PurchaseId", model.PurchaseId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_PurchaseRequestHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.PurchaseId;
                                    isNew = false;
                                }
                                else
                                {

                                    key = _poHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixPR}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.PurchaseId = key;
                                    isNew = true;
                                }
                                string itemList = "";
                                string keyCache = string.Format(PrefixCacheActionPU, model.StoreId, model.TerminalId);
                                string storeCache = cacheService.GetCachedData<string>(keyCache);
                                if (string.IsNullOrEmpty(storeCache))
                                {
                                    cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
                                }
                                else
                                {
                                    result.Success = false;
                                    result.Message = "Your actions are too fast and too dangerous. Please wait for your PO to be completed.";
                                    return result;
                                }

                                parameters.Add("PurchaseId", model.PurchaseId, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("DocStatus", model.DocStatus);
                                parameters.Add("DocDate", model.DocDate);
                                parameters.Add("DocDueDate", model.DocDueDate);
                                parameters.Add("CardCode", model.CardCode);

                                parameters.Add("CardName", model.CardName);
                                parameters.Add("InvoiceAddress", model.InvoiceAddress);
                                parameters.Add("TaxCode", model.TaxCode);
                                parameters.Add("VatTotal", model.Vattotal);
                                parameters.Add("DocTotal", model.DocTotal);
                                parameters.Add("Comment", model.Comment);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("ModifiedBy", model.ModifiedBy);
                                parameters.Add("ModifiedOn", model.ModifiedOn);
                                parameters.Add("Status", model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("DataSource", model.DataSource);
                                parameters.Add("PrefixPR", PrefixPR);
                                if (isNew)
                                {
                                    
                                    parameters.Add("CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    parameters.Add("ModifiedBy", null);
                                    parameters.Add("ModifiedOn", null);
                                }
                                else
                                {
                                    parameters.Add("CreatedOn", model.CreatedOn);
                                    parameters.Add("ModifiedBy", model.ModifiedBy);
                                    parameters.Add("ModifiedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                }

                                //var affectedRows = db.Execute("USP_I_T_PurchaseRequestHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);


                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    line.PurchaseId = key;
                                    line.LineId = stt.ToString();
                                    line.CreatedBy = model.CreatedBy;
                                    line.CompanyCode = model.CompanyCode;

                                    //parameters = new DynamicParameters();
                                    //parameters.Add("PurchaseId", key, DbType.String);
                                    //parameters.Add("CompanyCode", model.CompanyCode);
                                    //parameters.Add("LineId", stt);
                                    //parameters.Add("ItemCode", line.ItemCode);
                                    //parameters.Add("SlocId", line.SlocId);
                                    //parameters.Add("BarCode", line.BarCode);
                                    //parameters.Add("Description", line.Description);
                                    //parameters.Add("Uomcode", line.UomCode);
                                    //parameters.Add("Quantity", line.Quantity == null ? null : line.Quantity);
                                    //parameters.Add("OpenQty", line.OpenQty == null ? null : line.OpenQty);
                                    //parameters.Add("Price", line.Price == null ? null : line.Price);
                                    //parameters.Add("BaseSAPId", line.BaseSAPId == null ? null : line.BaseSAPId);
                                    //parameters.Add("BaseSAPLine", line.BaseSAPLine == null ? null : line.BaseSAPLine);
                                   
                                    //parameters.Add("LineStatus", line.LineStatus == null ? null : line.LineStatus);
                                    //parameters.Add("DiscPercent", line.DiscPercent);
                                    //parameters.Add("VATPercent", line.Vatpercent);
                                    //parameters.Add("LineTotal", line.LineTotal == null ? null : line.LineTotal);
                                    //parameters.Add("Comment", line.Comment);
                                    //parameters.Add("CreatedBy", model.CreatedBy);
                                  
                                    //parameters.Add("Status", line.Status);

                                    if (isNew)
                                    {
                                        line.CreatedOn = DateTime.Now;
                                        line.ModifiedBy = null;
                                        line.ModifiedOn = null;
                                        //parameters.Add("CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        //parameters.Add("ModifiedBy", null);
                                        //parameters.Add("ModifiedOn", null);
                                    }
                                    else
                                    {

                                        line.CreatedOn = model.CreatedOn;
                                        line.ModifiedBy = model.ModifiedBy;
                                        line.ModifiedOn = DateTime.Now;

                                        //parameters.Add("CreatedOn", model.CreatedOn);
                                        //parameters.Add("ModifiedBy", model.ModifiedBy);
                                        //parameters.Add("ModifiedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                   

                                    //db.Execute("USP_I_T_PurchaseRequestLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                  
                                }
                                int sttSer = 0;
                                List<TPurchaseRequestLineSerial> lineSerials = new List<TPurchaseRequestLineSerial>();
                                foreach (var line in model.SerialLines)
                                {
                                    sttSer++;
                                    line.PurchaseId = key;
                                    line.LineId = sttSer.ToString(); 
                                    line.CreatedBy = model.CreatedBy;
                                    line.CompanyCode = model.CompanyCode; 
                                    line.Description = line.Description;

                                    //parameters = new DynamicParameters();
                                    //parameters.Add("PurchaseId", key, DbType.String);
                                    //parameters.Add("LineId", sttSer);
                                    //parameters.Add("CompanyCode", model.CompanyCode);
                                    //parameters.Add("ItemCode", line.ItemCode);
                                    //parameters.Add("SerialNum", line.SerialNum);
                                    //parameters.Add("SLocId", line.SlocId);
                                    //parameters.Add("Uomcode", line.UomCode);
                                    //parameters.Add("Quantity", line.Quantity);
                                    //parameters.Add("CreatedBy", model.CreatedBy);
                                    //parameters.Add("Description", line.Description);
                                    if (isNew)
                                    {
                                        line.CreatedOn = DateTime.Now;
                                        line.ModifiedBy = null;
                                        line.ModifiedOn = null;
                                        //parameters.Add("CreatedOn", DateTime.Now.ToString("yyyy-MM-dd"));
                                        //parameters.Add("ModifiedBy", null);
                                        //parameters.Add("ModifiedOn", null);
                                    }
                                    else
                                    {
                                        line.CreatedOn = model.CreatedOn;
                                        line.ModifiedBy = model.ModifiedBy;
                                        line.ModifiedOn = DateTime.Now;
                                        //parameters.Add("CreatedOn", model.CreatedOn);
                                        //parameters.Add("ModifiedBy", model.ModifiedBy);
                                        //parameters.Add("ModifiedOn", DateTime.Now.ToString("yyyy-MM-dd"));
                                    }
                                    lineSerials.Add(line);
                                    //parameters.Add("OpenQty", line.OpenQty);

                                    //db.Execute("USP_I_T_PurchaseRequestLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran); 
                                }
                                PRLines = ExtensionsNew.ConvertListToDataTable(model.Lines, PRLines);
                                PRLineSerial = ExtensionsNew.ConvertListToDataTable(lineSerials, PRLineSerial);
                                string tblLineType = PRLineTbl + "TableType";
                                string tblGISerialTbl = PRSerialTbl + "TableType";

                                parameters.Add("@Lines", PRLines.AsTableValuedParameter(PRLineTbl + "TableType"));
                                parameters.Add("@LineSerials", PRLineSerial.AsTableValuedParameter(PRSerialTbl + "TableType"));

                                key = db.ExecuteScalar("USP_I_T_PurchaseRequest", parameters, commandType: CommandType.StoredProcedure, transaction: tran).ToString();


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

        public Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }
        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {
            //try
            //{
            //    string query = $"select * from T_PurchaseRequestHeader with (nolock) where 1=1";
            //    if (!string.IsNullOrEmpty(companycode) && companycode != "null")
            //    {
            //        query += $" and companycode = '{companycode}' ";
            //    }
            //    if (!string.IsNullOrEmpty(storeId) && storeId != "null")
            //    {
            //        query += $" and storeId = '{storeId}' ";
            //    }
            //    //if (!string.IsNullOrEmpty(Type) && Type != "null")
            //    //{
            //    //    query += $" and InvoiceMode = '{Type}' ";
            //    //}
            //    if (!string.IsNullOrEmpty(fromdate) && fromdate != "null")
            //    {
            //        query += $" and createdOn >= '{fromdate}'";
            //    }
            //    if (!string.IsNullOrEmpty(todate) && todate != "null")
            //    {
            //        query += $" and createdOn <= '{todate}'";
            //    }

            //    if (!string.IsNullOrEmpty(key) && key != "null")
            //    {
            //        query += $" and PurchaseId = '{key}'";
            //    }

            //    if (!string.IsNullOrEmpty(status) && status != "null")
            //    {
            //        query += $" and Status = '{status}'";
            //    }

            //    query += $" order by CreatedOn desc";
            //    var lst = await _poHeaderRepository.GetAllAsync(query, null, commandType: CommandType.Text);


            //    return lst;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companycode);
                parameters.Add("storeId", storeId);
                parameters.Add("Status", status);
                parameters.Add("FrDate", fromdate);
                parameters.Add("ToDate", todate);
                parameters.Add("Keyword", key);
                //parameters.Add("ViewBy", ViewBy); 

                var data = await _poHeaderRepository.GetAllAsync($"USP_GetPurchaseRequest", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //throw new NotImplementedException();
        }
        public async Task<List<TPurchaseRequestHeader>> GetAll()
        {
            var lst = await _poHeaderRepository.GetAllAsync("select * from T_poHeader with (nolock) order by CreatedOn desc", null, commandType: CommandType.Text);
            return lst;
            //throw new NotImplementedException();
        }

        public Task<TPurchaseRequestHeader> GetById(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<PurchaseRequestViewModel> GetOrderById(string Id, string CompanyCode, string StoreId)
        {
            try
            {
                PurchaseRequestViewModel order = new PurchaseRequestViewModel();

                TPurchaseRequestHeader header = await _poHeaderRepository.GetAsync($"select T1.*,T2.PRNum from T_PurchaseRequestHeader T1 LEFT JOIN T_PurchaseOrderHeader T2 on T1.PurchaseId =T2.PRNum" +
                    $" where T1.PurchaseId='{Id}' and T1.CompanyCode= '{CompanyCode}' and T1.StoreId= '{StoreId}'", null, commandType: CommandType.Text);

                string queryLine = $"select t1.*  from T_PurchaseRequestLine t1 with(nolock)  where t1.PurchaseId = '{Id}' and t1.CompanyCode = '{CompanyCode}'";
                string queryLineSerial = $"select t1.*   from T_PurchaseRequestLineSerial t1 with(nolock)   where t1.PurchaseId = '{Id}' and t1.CompanyCode = '{CompanyCode}'";

                //List<TPurchaseRequestLine> lines = await _poLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                //List<TPurchaseRequestPayment> payments = await _popaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                //var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId ='{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<PurchaseRequestViewModel>(header);
                using (IDbConnection db = _poHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TPurchaseRequestLine>(queryLine, null, commandType: CommandType.Text);
                        var serialLines = db.Query<TPurchaseRequestLineSerial>(queryLineSerial, null, commandType: CommandType.Text);
                        foreach (var line in lines)
                        {
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                        }
                        order = _mapper.Map<PurchaseRequestViewModel>(header);
                        order.Lines = lines.ToList();
                        order.SerialLines = serialLines.ToList();
                        //order.PromoLines = promoLines.ToList();
                        //order.Payments = payments;
                        //order.Customer = customer;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return order;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public Task<TPurchaseRequestHeader> GetByUser(string User)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TPurchaseRequestLine>> GetLinesById(string Id)
        {
            var data = await _poLineRepository.GetAllAsync($"select * from T_poLine with (nolock) where PurchaseId = N'%{Id}%'", null, commandType: CommandType.Text);
            return data;
        }

        public async Task<string> GetNewOrderCode(string companyCode, string storeId)
        {
            string key = _poHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('AR','{companyCode}','{storeId}')", null, commandType: CommandType.Text);
            return key;
        }
        public async Task<string> GetLastPricePO(string companyCode, string storeId, string ItemCode, string UomCode, string Barcode)
        {
            string query = $"select top 1 Price from T_PurchaseRequestLine where ItemCode = '{ItemCode}' and UOMCode ='{UomCode}' and CompanyCode= '{companyCode}' order by CreatedOn desc";
            string key = _poHeaderRepository.GetScalar(query, null, commandType: CommandType.Text);
            return key;
        }

        public async Task<PagedList<TPurchaseRequestHeader>> GetPagedList(UserParams userParams)
        {
            try
            {
                string query = $"select * from T_poHeader with (nolock) " +
                    $"where ( Remarks like N'%{userParams.keyword}%' or PurchaseId like N'%{userParams.keyword}%' or StoreId like N'%{userParams.keyword}%' or CusId like N'%{userParams.keyword}%'  or InvoicePerson like N'%{userParams.keyword}%' )";
                if (!string.IsNullOrEmpty(userParams.status))
                {
                    query += $" and Status='{userParams.status}'";
                }
                var data = await _poHeaderRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.CusId);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.TransId);
                //}
                return await PagedList<TPurchaseRequestHeader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<GenericResult> UpdateCancel(PurchaseRequestViewModel model)
        {
            GenericResult result = new GenericResult();
            if (model.DocDate == null)
            {
                result.Success = false;
                result.Message = "Doc date not null.";
                return result;
            }
            if (model.Lines == null || model.Lines.Count() == 0)
            {
                result.Success = false;
                result.Message = "Doc line not null.";
                return result;
            }
            //else
            //{
            //    var receiptLine = model.Lines.Where(x => x.Quantity <= 0).ToList();
            //    if (receiptLine != null)
            //    {
            //        result.Success = false;
            //        result.Message = "Quantity of line can't equal 0.";
            //        return result;
            //    }
            //}
            if (model.StoreId == null)
            {
                result.Success = false;
                result.Message = "From Store / To Store not null.";
                return result;
            }

            try
            {

                using (IDbConnection db = _poHeaderRepository.GetConnection())
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

                                string query = $"update T_PurchaseRequestHeader set Status = 'C',  DocStatus = 'C',IsCanceled ='Y' where CompanyCode='{model.CompanyCode}' and PurchaseId = '{model.PurchaseId}'  and StoreId = '{model.StoreId}'";
                                var delAffectedRows = db.Execute(query, parameters, commandType: CommandType.Text, transaction: tran);
                                result.Success = true;
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

        public async Task<GenericResult> GetSalesPeriod(AverageNumberSaleModel model)
        {
            GenericResult result = new GenericResult();
            try
            {
                using (IDbConnection db = _poHeaderRepository.GetConnection())
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    using (var tran = db.BeginTransaction())
                    {

                        var parameters = new DynamicParameters();
                        parameters.Add("@ItemCode", model.ItemCode);
                        parameters.Add("@UomCode", model.UomCode);
                        parameters.Add("@SlocId", model.SlocId);
                        parameters.Add("@FromDate", model.FromDate);
                        parameters.Add("@ToDate", model.ToDate);

                        var items = await db.QueryMultipleAsync("USP_GetSalesPeriod", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
 
                        var getSalesPeriod = items.Read<AverageNumberSaleModel>().ToList();
                        var getLastOrderQuantity = items.Read<AverageNumberSaleModel>().ToList();

                        AverageNumberSaleListModel rst = new AverageNumberSaleListModel();
                        rst.AverageNumberSaleModel = getSalesPeriod;
                        rst.QtyPOModel = getLastOrderQuantity;

                        if (items != null)
                        {
                            result.Data = rst;
                            result.Success = true;
                        }
                        else
                        {
                            result.Success = false;
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

    }
}
