
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using NUglify.JavaScript;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class GRPOService : IGRPOService
    {
        private readonly IGenericRepository<TGoodsReceiptPoheader> _GRPOHeaderRepository;
        private readonly IGenericRepository<TGoodsReceiptPoline> _GRPOLineRepository;
        private readonly IGenericRepository<TGoodsReceiptPolineSerial> _GRPOLineSerialRepository;
        //private readonly IGenericRepository<TPurchaseOrderPayment> _GRPOpaymentLineRepository;
        private readonly IGenericRepository<TPurchaseOrderHeader> _PurchaseHeaderRepository;
        private readonly IGenericRepository<MCustomer> _customerRepository;
        private readonly IGeneralSettingService _settingService;
        IMapper _mapper;
        private IResponseCacheService cacheService;
        private string PrefixCacheActionGR = "QAGR-{0}-{1}";
        private string PrefixGRPO = "";
        private TimeSpan timeQuickAction = TimeSpan.FromSeconds(15);
        public GRPOService(IGenericRepository<TGoodsReceiptPoheader> invoiceHeaderRepository, IConfiguration config, IGenericRepository<TGoodsReceiptPolineSerial> invoiceLineSerialRepository,
            IGenericRepository<TGoodsReceiptPoline> invoiceLineRepository, IGeneralSettingService settingService, IGenericRepository<MCustomer> customerRepository, IGenericRepository<TPurchaseOrderHeader> purchaseHeaderRepository, IMapper mapper, IResponseCacheService responseCacheService /*, IHubContext<RequestHub> hubContext IGenericRepository<TPurchaseOrderPayment> invoicepaymentLineRepository,*/
)//: base(hubContext)
        {
            _GRPOHeaderRepository = invoiceHeaderRepository;
            _GRPOLineRepository = invoiceLineRepository;
            _GRPOLineSerialRepository = invoiceLineSerialRepository;
            _settingService = settingService;
            //_GRPOpaymentLineRepository = invoicepaymentLineRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _PurchaseHeaderRepository = purchaseHeaderRepository;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            this.cacheService = responseCacheService;
            string timeCache = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("TimeCacheAction"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (!string.IsNullOrEmpty(timeCache) && double.TryParse(timeCache, out double timeAction))
            {
                timeQuickAction = TimeSpan.FromSeconds(timeAction);
            }
            PrefixGRPO = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixGRPO"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            if (string.IsNullOrEmpty(PrefixGRPO))
            {
                PrefixGRPO = "GP";
            }
            initService();
        }
        public GenericResult initService()
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _GRPOHeaderRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string queryCheckAndCreate = "IF (OBJECT_ID('USP_GetGRPO') IS NULL)  begin declare @string nvarchar(MAX) = '';" +
                        " set @string = 'create PROCEDURE [dbo].[USP_GetGRPO] @CompanyCode nvarchar(50), @storeId	nvarchar(50), @Status		nvarchar(5), @FrDate  datetime, @ToDate  datetime, @Keyword nvarchar(250), @ViewBy nvarchar(250) =null  AS " +
                        " begin select  [PurchaseId] ,[CompanyCode]  ,[StoreId] ,[StoreName] ,[DocStatus] ,[DocDate] ,[DocDueDate] ,[CardCode] ,[CardName] ,[InvoiceAddress] ,[TaxCode] ,[VATPercent] ,[VATTotal] ,[DocTotal]  ,[Comment] ,[CreatedBy] ,[CreatedOn] ,[ModifiedBy] ,[ModifiedOn] ,[Status] ,[RefTransId] ,[ShiftId], CASE IsCanceled WHEN  ''C'' THEN ''Y''   ELSE  IsCanceled end as   ''IsCanceled'' " +
                        " from T_GoodsReceiptPOHeader with (nolock)" +
                        " where (ISNULL(@CompanyCode, '''') = '''' OR CompanyCode = @CompanyCode ) AND (StoreId = @storeId  or ISNULL(@storeId, '''')='''')   and ( ISNULL(@Status, '''') = ''''  or ( CASE WHEN @Status = ''N'' THEN IsCanceled END = ''Y'' and Status = ''C'' " +
                        " or	( Status = @Status)  )  ) and (CONVERT(date,  CreatedOn) >= CONVERT(date,@FrDate)  or ISNULL(@FrDate, '''') = '''')   and (CONVERT(date,  CreatedOn) <= CONVERT(date,@ToDate ) or ISNULL(@ToDate, '''') = '''')  and (PurchaseId like N''%''+@Keyword+''%'' or Comment like N''%''+@Keyword+''%'' or ISNULL(@Keyword, '''') = '''')   order by CreatedOn desc end '; " +
                        " EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreate);
  
                    db.Close();
                    result.Success = true;
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

        public class ResultModel
        {
            public int ID { get; set; }
            public string Message { get; set; }
        }
        public async Task<GenericResult> UpdateStatus(GRPOViewModel model)
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
                //if(model.Payments.Count == 0)
                //{
                //    result.Success = false;
                //    result.Message = "Payment list not null.";
                //    return result;
                //}    
                using (IDbConnection db = _GRPOHeaderRepository.GetConnection())
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

                                string query = $"update T_GoodsReceiptPOHeader set Status = 'C',  DocStatus = 'C' where CompanyCode='{model.CompanyCode}' and PurchaseId = '{model.PurchaseId}'  and StoreId = '{model.StoreId}'";
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
        public GenericResult GetSourceDocument(string CompanyCode, string StoreId, string DocNum, string ConnectionStr)
        {
            GenericResult result = new GenericResult();
            try
            {
                IDbConnection dbConnect = _GRPOHeaderRepository.GetConnection();
                if (!string.IsNullOrEmpty(ConnectionStr))
                {
                    _GRPOHeaderRepository.GetConnectionCustom(ConnectionStr);
                }    
                using (IDbConnection db = dbConnect)
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open(); 
                        string query = $" select * from T_PurchaseOrderHeader with(nolock) where PurchaseId= N'{DocNum}' and CompanyCode = N'{CompanyCode}' and StoreId = N'{StoreId}'";

                        var data = db.QueryFirstOrDefault(query, null);
                        PurchaseOrderViewModel po = new PurchaseOrderViewModel();
                        po = _mapper.Map<PurchaseOrderViewModel>(data);
                        result.Success = true;
                        result.Data = po;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
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
        public async Task<GenericResult> Create(GRPOViewModel model)
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
            else
            {
                var receiptLine = model.Lines.Where(x => x.OpenQty <= 0).ToList();
                if(receiptLine!=null && receiptLine.Count > 0)
                {
                    result.Success = false;
                    result.Message = "Quantity of line can't equal 0.";
                    return result;
                }    
            }
            if (model.StoreId == null)
            {
                result.Success = false;
                result.Message = "From Store / To Store not null.";
                return result;
            }

            var Purchase = _PurchaseHeaderRepository.GetAll($"select * from T_PurchaseOrderHeader where PurchaseId= '{model.RefTransId}' and Status='C'", null, commandType: CommandType.Text);
            if(Purchase !=null && Purchase.Count>0)
            {
                result.Success = false;
                result.Message = "PO is Close By "+Purchase.FirstOrDefault().CreatedBy;
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
                using (IDbConnection db = _GRPOHeaderRepository.GetConnection())
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

                                bool checkSourceDocument = false;
                                var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                if (settingData.Success)
                                { 
                                    SettingList = settingData.Data as List<GeneralSettingStore>;  
                                } 
                                //Lấy data kiểm tra đơn gốc
                                var settingDataCheckSource = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "CheckSourceDocument").FirstOrDefault();
                                if (settingDataCheckSource != null && (settingDataCheckSource.SettingValue == "true" || settingDataCheckSource.SettingValue == "1"))
                                {
                                    checkSourceDocument = true;

                                }
                                else
                                {
                                    checkSourceDocument = false;
                                }
                                if (checkSourceDocument == true)
                                {
                                    string connStr = settingDataCheckSource.CustomField1;
                                    if (!string.IsNullOrEmpty(connStr))
                                    {
                                        connStr = Utilities.Helpers.Encryptor.DecryptString(connStr, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                        var getSourceDocumentRs = GetSourceDocument(model.CompanyCode, model.StoreId, model.RefTransId, connStr);
                                        if (getSourceDocumentRs.Success)
                                        {
                                            var sourceData = getSourceDocumentRs.Data as TInventoryHeader;
                                            if(string.IsNullOrEmpty(sourceData.Source))
                                            {
                                                sourceData.Source = "";
                                            }
                                            if (string.IsNullOrEmpty(PrefixGRPO))
                                            {
                                                PrefixGRPO = "";
                                            }
                                            if (PrefixGRPO == sourceData.Source)
                                            {
                                              
                                            }
                                            else
                                            {
                                                var dataCurrentRs = GetSourceDocument(model.CompanyCode, model.StoreId, model.RefTransId, "");
                                                if (dataCurrentRs.Success)
                                                { 
                                                    var currentData = dataCurrentRs.Data as TInventoryHeader;
                                                    if (sourceData.Status != currentData.Status)
                                                    {
                                                        result.Success = false;
                                                        result.Message = $"The status is not the same as the original document";
                                                        return result;
                                                    }

                                                }
                                                else
                                                {
                                                    return dataCurrentRs;
                                                }
                                            }
                                            
                                        }
                                        else
                                        {
                                            return getSourceDocumentRs;
                                        }
                                    }
                                    else
                                    {
                                        result.Success = false;
                                        result.Message = $"Can't found source document connection string (" + model.Source + ")";
                                        return result;
                                    }
                                }

                                if (!string.IsNullOrEmpty(model.PurchaseId))
                                {
                                   
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("PurchaseId", model.PurchaseId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_GRPO", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.PurchaseId;
                                    isNew = false;
                                }
                                else
                                {
                                    key = _GRPOHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixGRPO}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.PurchaseId = key;
                                    isNew = true;
                                }    
                                string itemList = "";
                                //foreach (var line in model.Lines)
                                //{

                                //    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                //}
                                //string querycheck = $"USP_I_T_GRPOLine_CheckNegative '{model.CompanyCode}', '{itemList}'";
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

                                string keyCache = string.Format(PrefixCacheActionGR, model.StoreId,model.RefTransId, model.TerminalId);
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
                                //Create and fill-up master table data


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
                                parameters.Add("VATPercent", model.VatPercent);
                                parameters.Add("VATTotal", model.VatTotal);
                                parameters.Add("DocTotal", model.DocTotal);
                                parameters.Add("Comment", model.Comment); 
                                parameters.Add("CreatedBy", model.CreatedBy); 
                                parameters.Add("Status", model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("RefTransId", model.RefTransId);
                                parameters.Add("ShiftId", model.ShiftId);
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

                                //Insert record in master table. Pass transaction parameter to Dapper.
                                var affectedRows = db.Execute("USP_I_T_GoodsReceiptPOHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

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
                                    parameters.Add("PurchaseId", key, DbType.String);
                                     
                                 
                                    //parameters.Add("BaseEntry",  line.BaseEntry == null ? null : line.BaseEntry);
                                   

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("LineId", stt);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("SlocId", line.SlocId);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Description", line.Description);
                                    parameters.Add("Uomcode", line.UomCode);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Description", line.Description);
                                    parameters.Add("Uomcode", line.UomCode); 
                           
                                    parameters.Add("Price", line.Price == null ? null : line.Price);

                                    //parameters.Add("BaseSAPId", line.BaseSAPId == null ? null : line.BaseSAPId);
                                    //parameters.Add("BaseSAPLine", line.BaseSAPLine == null ? null : line.BaseSAPLine);
                                    parameters.Add("BaseTrans", line.BaseTrans == null ? null : line.BaseTrans);
                                    parameters.Add("BaseType", line.BaseType);
                                    parameters.Add("BaseEntry", line.BaseEntry == null ? null : line.BaseEntry);
                                    parameters.Add("Status", line.Status);
                                    parameters.Add("LineStatus", line.LineStatus == null ? null : line.LineStatus);
                                    parameters.Add("DiscPercent", line.DiscPercent);
                                    parameters.Add("VATPercent", line.VatPercent);
                                    parameters.Add("LineTotal", line.LineTotal == null ? null : line.LineTotal);
                                    parameters.Add("Comment", line.Comment);
                                    parameters.Add("CreatedBy", line.CreatedBy);
                                    parameters.Add("BaseLine", line.BaseLine);
                                    parameters.Add("BaseTransId", line.BaseTransId);
                                    parameters.Add("Description", line.Description);
                                    

                                    if (isNew)
                                    {
                                        parameters.Add("Quantity", line.OpenQty == null ? null : line.OpenQty);
                                        parameters.Add("OpenQty", line.Quantity - line.OpenQty == null ? null : line.Quantity - line.OpenQty);
                                        parameters.Add("CreatedOn", DateTime.Now );
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                    }
                                    else
                                    {
                                        parameters.Add("Quantity", line.Quantity == null ? null : line.Quantity);
                                        parameters.Add("OpenQty",  line.OpenQty == null ? null :  line.OpenQty);
                                        parameters.Add("CreatedOn", model.CreatedOn);
                                        parameters.Add("CreatedBy", model.CreatedBy);
                                        parameters.Add("ModifiedBy", model.ModifiedBy);
                                        parameters.Add("ModifiedOn", DateTime.Now );
                                    }
                                    //string queryLine = $"usp_I_T_GRPOLine '{key}','{stt}','{model.CompanyCode}','{line.ItemCode}','{line.BarCode}','{line.UomCode}','{line.Quantity}','{line.Price}'" +
                                    //    $",'{line.LineTotal}','{line.DiscountType}','{line.DiscountAmt}','{line.DiscountRate}','{line.CreatedBy}','{line.PromoId}','{line.PromoType}','{line.Status}','{line.Remark}'" +
                                    //    $",'{line.PromoPercent}','{line.PromoBaseItem}','{ line.SalesMode}','{line.TaxRate}','{line.TaxAmt}','{line.TaxCode}','{line.SlocId}','{line.MinDepositAmt}','{line.MinDepositPercent}'" +
                                    //    $",'{line.DeliveryType}','{line.Posservice}','{line.StoreAreaId}','{line.TimeFrameId}','{line.AppointmentDate}','{line.BomId}','{line.PromoPrice}','{line.PromoLineTotal}','{line.BaseLine}','{line.BaseTransId}','{line.OpenQty}'";
                                    ////_GRPOHeaderRepository.GetConnection().Get("",);



                                    db.Execute("USP_I_T_GoodsReceiptPOLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    //db.Execute("usp_I_T_GRPOLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _GRPOHeaderRepository.GetConnection().InsertAsync<string, TPurchaseOrderLine>(line);
                                    int sttSerial = 0;
                                    foreach (var serialLine in line.SerialLines)
                                    {
                                        sttSerial++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("PurchaseId", key, DbType.String);
                                        parameters.Add("LineId", sttSerial);
                                        parameters.Add("CompanyCode", serialLine.CompanyCode);
                                        parameters.Add("ItemCode", serialLine.ItemCode);
                                        parameters.Add("SerialNum", serialLine.SerialNum);
                                        parameters.Add("SLocId", serialLine.SlocId);
                                        parameters.Add("Uomcode", serialLine.UomCode);
                                        parameters.Add("Quantity", serialLine.Quantity);
                                        //parameters.Add("Status", line.Status);
                                        parameters.Add("CreatedBy", serialLine.CreatedBy);
                                        parameters.Add("BaseLine", serialLine.BaseLine);
                                        parameters.Add("BaseTransId", serialLine.BaseTransId);
                                        parameters.Add("Description", serialLine.Description);
                                        //parameters.Add("LineNum", line.LineNum);
                                        if (isNew)
                                        {
                                            parameters.Add("CreatedOn", DateTime.Now );
                                            parameters.Add("ModifiedBy", null);
                                            parameters.Add("ModifiedOn", null);
                                        }
                                        else
                                        {
                                            parameters.Add("CreatedOn", model.CreatedOn);
                                            parameters.Add("ModifiedBy", model.ModifiedBy);
                                            parameters.Add("ModifiedOn", DateTime.Now );
                                        }
                                        db.Execute("USP_I_T_GoodsReceiptPOLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _GRPOHeaderRepository.GetConnection().InsertAsync<string, TPurchaseOrderLine>(line);
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

        public Task<GenericResult> Delete(string companycode, string storeId, string Code)
        {
            throw new NotImplementedException();
        }
        public async Task<GenericResult> GetByType(string companycode, string storeId, string fromdate, string todate, string key, string status)
        {
            GenericResult result = new GenericResult();
            try
            {

                //string query = $"select * from T_GoodsReceiptPOHeader with (nolock) where 1=1 ";
                //if (!string.IsNullOrEmpty(companycode) && companycode != "null")
                //{
                //    query += $" and companycode = '{companycode}' ";
                //}
                //if (!string.IsNullOrEmpty(storeId) && storeId != "null")
                //{
                //    query += $" and storeId = '{storeId}' ";
                //}
                //if (!string.IsNullOrEmpty(fromdate) && fromdate!="null")
                //{
                //    //query += $" and createdOn >= '{fromdate}'";
                //    query += $" and (CONVERT(date,  CreatedOn) >= '{ fromdate}')";  
                //}
                //if (!string.IsNullOrEmpty(todate) && todate != "null")
                //{
                //    //query += $" and createdOn <= '{todate}'";
                //    query += $" and (CONVERT(date,  CreatedOn) <= '{todate}')";
                //}
                //if (!string.IsNullOrEmpty(key) && key != "null")
                //{
                //    query += $" and PurchaseId = '{key}'";
                //}
                //if (!string.IsNullOrEmpty(status) && status != "null")
                //{
                //    query += $" and Status = '{status}'";
                //}

                //query += $" order by CreatedOn desc";

                string query = "USP_GetGRPO";
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companycode);
                parameters.Add("storeId", string.IsNullOrEmpty(storeId) ? "" : storeId);
                parameters.Add("Status", string.IsNullOrEmpty(status) ? "" : status);
                parameters.Add("FrDate", string.IsNullOrEmpty(fromdate) ? "" : fromdate);
                parameters.Add("ToDate", string.IsNullOrEmpty(todate) ? "" : todate);
                parameters.Add("Keyword", string.IsNullOrEmpty(key) ? "" : key);

                var lst = await _GRPOHeaderRepository.GetAllAsync(query, parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = lst;
                //return lst;
            }
            catch (Exception ex)
            {
                //return null;
                result.Success = false;
                result.Data = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _GRPOHeaderRepository.GetAllAsync($"select * from T_GoodsReceiptPOHeader with (nolock) where CompanyCode= '{CompanyCode}' order by CreatedOn desc", null, commandType: CommandType.Text);
                
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

        public Task<TGoodsReceiptPoheader> GetById(string companycode, string storeId, string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetOrderById(string Id, string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                GRPOViewModel order = new GRPOViewModel();

                string sql= $"USP_S_T_GoodsReceiptPOHeader '{CompanyCode}', '{StoreId}', '{Id}'";
                //string sql= $"select * from T_GoodsReceiptPOHeader with (nolock) where PurchaseId='{Id}' and CompanyCode= '{CompanyCode}' and StoreId= '{StoreId}'";
                TGoodsReceiptPoheader header = await _GRPOHeaderRepository.GetAsync(sql, null, commandType: CommandType.Text);
                if(header!=null)
                {
                    //string queryLine = $"select t1.*  from T_GoodsReceiptPOLine t1 with(nolock)  where t1.PurchaseId = '{Id}' and t1.CompanyCode = '{CompanyCode}'";
                    //string queryLineSerial = $"select t1.*   from T_GoodsReceiptPOLineSerial t1 with(nolock)   where t1.PurchaseId = '{Id}' and t1.CompanyCode = '{CompanyCode}'";
                    
                    string queryLine = $"USP_S_T_GoodsReceiptPOLine '{CompanyCode}', '{StoreId}', '{Id}'";
                    string queryLineSerial = $"USP_S_T_GoodsReceiptPOLineSerial '{CompanyCode}', '{StoreId}', '{Id}'";

                    //List<TPurchaseOrderLine> lines = await _GRPOLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                    //List<TPurchaseOrderPayment> payments = await _GRPOpaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                    //var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId ='{header.CusId}'", null, commandType: CommandType.Text);

                    var head = _mapper.Map<GRPOViewModel>(header);
                    using (IDbConnection db = _GRPOHeaderRepository.GetConnection())
                    {
                        try
                        {
                            if (db.State == ConnectionState.Closed)
                                db.Open();
                            var lines = db.Query<TGoodsReceiptPoline>(queryLine, null, commandType: CommandType.Text);
                            var serialLines = db.Query<TGoodsReceiptPolineSerial>(queryLineSerial, null, commandType: CommandType.Text);
                            foreach (var line in lines)
                            {
                                line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                            }
                            order = _mapper.Map<GRPOViewModel>(header);
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
                    result.Success = true;
                    result.Data = order;
                    return result;
                }    
                else
                {
                    result.Success = false;
                    result.Message = "TransId Not found";
                    return result;
                }    
                
                 
            }   
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
            
        }


       

        public async Task<List<TGoodsReceiptPoline>> GetLinesById(string companycode, string storeId, string Id)
        {
            var data = await _GRPOLineRepository.GetAllAsync($"USP_S_T_GoodsReceiptPOLine '{companycode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
            return data;
        }

        public async Task<string> GetNewOrderCode(string companyCode, string storeId)
        {
           string key = _GRPOHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixGRPO}','{companyCode}','{storeId}')", null, commandType: CommandType.Text);
           return key;
        }

        public async Task<PagedList<TGoodsReceiptPoheader>> GetPagedList(UserParams userParams)
        {
            try
            {
                string query = $"select * from T_GoodsReceiptPOHeader with (nolock) " +
                    $"where ( Remarks like N'%{userParams.keyword}%' or PurchaseId like N'%{userParams.keyword}%' or StoreId like N'%{userParams.keyword}%' or CusId like N'%{userParams.keyword}%'  or InvoicePerson like N'%{userParams.keyword}%' )";
                if(!string.IsNullOrEmpty(userParams.status))
                {
                    query += $" and Status='{userParams.status}'";
                }    
                var data = await _GRPOHeaderRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.CusId);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.TransId);
                //}
                return await PagedList<TGoodsReceiptPoheader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       

        public Task<GenericResult> Update(TGoodsReceiptPoheader model)
        {
            throw new NotImplementedException();
        }
    }

}
