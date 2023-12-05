
using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using DevExpress.Printing.Utils.DocumentStoring;
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
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace RPFO.Application.Implements
{
    public class InventoryService : IInventoryService
    {
        private readonly IGenericRepository<TInventoryHeader> _headerRepository;
        private readonly IGenericRepository<TInventoryLine> _lineRepository;
        private readonly IGenericRepository<TInventoryLineSerial> _lineSerialRepository;
        private readonly IGeneralSettingService _settingService;
        private readonly IMapper _mapper;
        private IResponseCacheService cacheService;
        private string PrefixCacheActionIN = "QAIN-{0}-{1}";
        private string PrefixTS = "TS";
        private string PrefixTR = "TR";
        private string PrefixTF = "IV";
        private string PrefixTQ = "TQ";
        private readonly IGenericRepository<MItem> _itemRepository;
        string ServiceName = "T_Inventory";
        List<string> TableNameList = new List<string>();
        private readonly ICommonService _commonService;
        private TimeSpan timeQuickAction = TimeSpan.FromSeconds(15);
        public InventoryService(IGenericRepository<TInventoryHeader> goodreceiptRepository, ICommonService commonService, IGenericRepository<TInventoryLine> goodreceiptLineRepository, IConfiguration config,
             IGenericRepository<TInventoryLineSerial> lineSerialRepository, IGeneralSettingService settingService, IMapper mapper, IResponseCacheService responseCacheService/*, IHubContext<RequestHub> hubContext*/
, IGenericRepository<MItem> itemRepository)//: base(hubContext)
        {
            _headerRepository = goodreceiptRepository;
            _lineRepository = goodreceiptLineRepository;
            _lineSerialRepository = lineSerialRepository;
            _settingService = settingService;
            _commonService = commonService;
            _mapper = mapper;
            this.cacheService = responseCacheService;
            string timeCache = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("TimeCacheAction"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (!string.IsNullOrEmpty(timeCache) && double.TryParse(timeCache, out double timeAction))
            {
                timeQuickAction = TimeSpan.FromSeconds(timeAction);
            }

            PrefixTS = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixTS"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (string.IsNullOrEmpty(PrefixTS))
            {
                PrefixTS = "TS";
            }

            PrefixTR = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixTR"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (string.IsNullOrEmpty(PrefixTR))
            {
                PrefixTR = "TR";
            }

            PrefixTF = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixTF"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (string.IsNullOrEmpty(PrefixTF))
            {
                PrefixTF = "IV";
            }
            PrefixTQ = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixTQ"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (string.IsNullOrEmpty(PrefixTQ))
            {
                PrefixTQ = "TQ";
            }

            TableNameList.Add(ServiceName + "Line");
            TableNameList.Add(ServiceName + "LineSerial");
            _commonService.InitService(ServiceName, TableNameList);
            _itemRepository = itemRepository;
        }
        public class ResultModel
        {
            public int ID { get; set; }
            public string Message { get; set; }
        }
        public async Task<GenericResult> Create(InventoryViewModel model)
        {
            return await CreateByTableType(model);
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
           
            if (model.FromStore == null ||   model.ToStore == null)
            {
                result.Success = false;
                result.Message = "From Store / To Store not null.";
                return result;
            }
            if (string.IsNullOrEmpty(model.DocType))
            {
                result.Success = false;
                result.Message = "Doctype null. Please check doctype of document.";
                return result;
            }
            foreach (var line in model.Lines)
            {
                if (line.FrSlocId == line.ToSlocId)
                {
                    result.Success = false;
                    result.Message = "Can't transfer the same of Storage. " + line.ItemCode;
                    return result;
                }
            }
            if (model.DocType == "R" && string.IsNullOrEmpty(model.RefInvtid))
            {
                result.Success = false;
                result.Message = "please check Inventory receipt. Ref Id not null";
                return result;
            }
           
            try
            {

                using (IDbConnection db = _headerRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                //string itemList = "";
                                //foreach (var line in model.Lines)
                                //{
                                //    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                //}
                                string VirtualWhs = $"select   dbo.[fnc_GetVirtualWhs]( '{model.CompanyCode}','{model.FromStore}')";
                                string geWhs = _headerRepository.GetScalar(VirtualWhs, null, commandType: CommandType.Text);
                                if (string.IsNullOrEmpty(geWhs))
                                {
                                    result.Success = false;
                                    result.Message = $"Can't found virtual warehouse ({model.FromStoreName})";
                                    return result;
                                }
                                string itemList = "";
                                string key = "";
                                List<ItemCheckModel> listItemCheck = new List<ItemCheckModel>();
                                if (model.DocType == "R")
                                {
                                    var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.FromStore);
                                    List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                    if (settingData.Success)
                                    {
                                        SettingList = settingData.Data as List<GeneralSettingStore>;
                                    }
                                    var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ManageStock").FirstOrDefault();
                                    if (setting != null && (setting.SettingValue == "true" || setting.SettingValue == "1"))
                                    {
                                        foreach (var line in model.Lines)
                                        {
                                            if (line.Quantity > 0)
                                            {
                                                //itemList += line.ItemCode + "-" + line.FrSlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                                ItemCheckModel itemCheck = new ItemCheckModel();
                                                itemCheck.ItemCode = line.ItemCode;
                                                itemCheck.SlocId = model.TransitWhs;// line.FrSlocId;
                                                itemCheck.UomCode = line.UomCode;
                                                itemCheck.Quantity = (double)line.Quantity;
                                                if (listItemCheck == null || listItemCheck.Count == 0)
                                                {
                                                    listItemCheck.Add(itemCheck);
                                                }
                                                else
                                                {
                                                    var checkInList = listItemCheck.Where(x => x.ItemCode == line.ItemCode && x.SlocId == model.TransitWhs && x.UomCode == line.UomCode).FirstOrDefault();
                                                    if (checkInList != null)
                                                    {
                                                        checkInList.Quantity += (double)line.Quantity;
                                                    }
                                                    else
                                                    {
                                                        listItemCheck.Add(itemCheck);
                                                    }
                                                }
                                            }    
                                        }
                                        if (listItemCheck != null && listItemCheck.Count > 0)
                                        {
                                            foreach (var line in listItemCheck)
                                            {
                                                itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                            }

                                        }
                                        if (model.IsCanceled.ToLower() == "n")
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
                                    }
                                       
                                    key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixTR}','{model.CompanyCode}','{model.FromStore}')", null, commandType: CommandType.Text);
                                }
                                   
                                if (model.DocType == "T")
                                {
                                    var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.FromStore);
                                    List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                    if (settingData.Success)
                                    { 
                                        SettingList = settingData.Data as List<GeneralSettingStore>; 
                                    }
                                    var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ManageStock").FirstOrDefault();
                                    if (setting != null && (setting.SettingValue == "true" || setting.SettingValue == "1"))
                                    {
                                        //foreach (var line in model.Lines)
                                        //{
                                        //    if (line.Quantity > 0)
                                        //    {
                                        //        itemList += line.ItemCode + "-" + model.TransitWhs + "-" + line.UomCode + "-" + line.Quantity + ";";
                                        //    }
                                        //}
                                        foreach (var line in model.Lines)
                                        {
                                            if (line.Quantity > 0)
                                            {
                                                //itemList += line.ItemCode + "-" + line.FrSlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                                ItemCheckModel itemCheck = new ItemCheckModel();
                                                itemCheck.ItemCode = line.ItemCode;
                                                itemCheck.SlocId = model.TransitWhs;
                                                itemCheck.UomCode = line.UomCode;
                                                itemCheck.Quantity = (double)line.Quantity;
                                                if (listItemCheck == null || listItemCheck.Count == 0)
                                                {
                                                    listItemCheck.Add(itemCheck);
                                                }
                                                else
                                                {
                                                    var checkInList = listItemCheck.Where(x => x.ItemCode == line.ItemCode && x.SlocId == model.TransitWhs && x.UomCode == line.UomCode).FirstOrDefault();
                                                    if (checkInList != null)
                                                    {
                                                        checkInList.Quantity += (double)line.Quantity;
                                                    }
                                                    else
                                                    {
                                                        listItemCheck.Add(itemCheck);
                                                    }
                                                }
                                            }
                                        }
                                        if (listItemCheck != null && listItemCheck.Count > 0)
                                        {
                                            foreach (var line in listItemCheck)
                                            {
                                                itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                            }

                                        }
                                        if (model.IsCanceled.ToLower() == "n")
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

                                    }
                                    key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixTF}','{model.CompanyCode}','{model.FromStore}')", null, commandType: CommandType.Text);
                                }

                                if (model.DocType == "S")
                                {
                                    var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.FromStore);
                                    List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                    if (settingData.Success)
                                    {
                                        SettingList = settingData.Data as List<GeneralSettingStore>;
                                    }
                                    var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ManageStock").FirstOrDefault();
                                    if (setting != null && (setting.SettingValue == "true" || setting.SettingValue == "1"))
                                    {
                                        //foreach (var line in model.Lines)
                                        //{
                                        //    if (line.Quantity > 0)
                                        //        itemList += line.ItemCode + "-" + line.FrSlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                        //}

                                        foreach (var line in model.Lines)
                                        {
                                            if (line.Quantity > 0)
                                            {
                                                //itemList += line.ItemCode + "-" + line.FrSlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                                ItemCheckModel itemCheck = new ItemCheckModel();
                                                itemCheck.ItemCode = line.ItemCode;
                                                itemCheck.SlocId = line.FrSlocId;
                                                itemCheck.UomCode = line.UomCode;
                                                itemCheck.Quantity = (double)line.Quantity;
                                                if (listItemCheck == null || listItemCheck.Count == 0)
                                                {
                                                    listItemCheck.Add(itemCheck);
                                                }
                                                else
                                                {
                                                    var checkInList = listItemCheck.Where(x => x.ItemCode == line.ItemCode && x.SlocId == line.FrSlocId && x.UomCode == line.UomCode).FirstOrDefault();
                                                    if (checkInList != null)
                                                    {
                                                        checkInList.Quantity += (double)line.Quantity;
                                                    }
                                                    else
                                                    {
                                                        listItemCheck.Add(itemCheck);
                                                    }
                                                }
                                            }
                                        }
                                        if (listItemCheck != null && listItemCheck.Count > 0)
                                        {
                                            foreach (var line in listItemCheck)
                                            {
                                                itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                            }

                                        }
                                        if (model.IsCanceled.ToLower() == "n")
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
                                    }
                                    
                                     key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixTS}','{model.CompanyCode}','{model.FromStore}')", null, commandType: CommandType.Text);
                                }

                                if (model.DocType == "Q")
                                {
                                    key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixTQ}','{model.CompanyCode}','{model.FromStore}')", null, commandType: CommandType.Text);
                                }
                                string keyCache = string.Format(PrefixCacheActionIN, model.FromStore,model.ToStore, model.TerminalId);
                                string storeCache = cacheService.GetCachedData<string>(keyCache);
                                if (string.IsNullOrEmpty(storeCache))
                                {
                                    cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
                                }
                                else
                                {
                                    result.Success = false;
                                    result.Message = "Your actions are too fast and too dangerous. Please wait for your Transaction to be completed.";
                                    return result;
                                }
                                //Create and fill-up master table data

                                model.Invtid = key;
                                var parameters = new DynamicParameters();
                                parameters.Add("Invtid", model.Invtid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("DocType", model.DocType);
                                parameters.Add("RefINVTId", model.RefInvtid);
                                parameters.Add("DocDate", model.DocDate);
                                parameters.Add("DocDueDate", model.DocDueDate);
                                parameters.Add("FromStore", model.FromStore);
                                parameters.Add("FromStoreName", model.FromStoreName);
                                parameters.Add("ToStore", model.ToStore);
                                parameters.Add("ToStoreName", model.ToStoreName);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("Name", model.Name);
                                parameters.Add("Remark", model.Remark);
                                parameters.Add("RefId", model.RefId);
                                parameters.Add("TransitWhs", model.TransitWhs);
                                parameters.Add("FromWhs", model.FromWhs);
                                parameters.Add("ToWhs", model.ToWhs);
                                parameters.Add("ShiftId", model.ShiftId);
                                if (model.IsCanceled == "Y")
                                {
                                    model.IsCanceled = "C";
                                    string updateQry = $"update T_InventoryHeader set IsCanceled = 'Y', Status='C' where INVTId = '{model.RefId}'and CompanyCode='{model.CompanyCode}'";
                                    db.Execute(updateQry, null, commandType: CommandType.Text, transaction: tran);
                                }
                                parameters.Add("IsCanceled", model.IsCanceled);
                                var affectedRows = db.Execute("USP_I_T_InventoryHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
 
                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    string Whs = model.TransitWhs;
                                    if(model.DocType=="S" || model.DocType =="T")
                                    {
                                        line.OpenQty = line.Quantity;
                                        Whs = line.FrSlocId;
                                    }
                                    
                                    parameters = new DynamicParameters();
                                    parameters.Add("INVTId", model.Invtid, DbType.String);
                                    parameters.Add("LineId", line.LineId);
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("FrSLocId", line.FrSlocId);
                                    parameters.Add("ToSlocId", line.ToSlocId);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Uomcode", line.UomCode);
                                    
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("Price", line.Price);
                                    parameters.Add("LineTotal", line.Price * line.Quantity);
                                    parameters.Add("DocType", model.DocType);
                                    parameters.Add("Description", line.Description);
                                    parameters.Add("ShipDate", line.ShipDate);
                                    parameters.Add("OpenQty", line.OpenQty);
                                    
                                    parameters.Add("CreatedBy", line.CreatedBy);  
                                    parameters.Add("Status", line.Status);
                                    parameters.Add("BaseTransId", line.BaseTransId);
                                    parameters.Add("BaseLine", line.BaseLine);

                                    var checkparameters = new DynamicParameters();
                                    checkparameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                                    checkparameters.Add("StoreId", model.FromStore);
                                    checkparameters.Add("SlocId", Whs);
                                    checkparameters.Add("ItemCode", line.ItemCode);
                                    checkparameters.Add("UomCode", line.UomCode);
                                    checkparameters.Add("BarCode", line.BarCode);
                                    checkparameters.Add("SerialNum", "");

                                    var checkresult = db.Query<ItemStockViewModel>("USP_GetItemStock", checkparameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //if (checkresult.Count() == 0 || checkresult.ToList()[0].Quantity < line.Quantity)
                                    //{
                                    //    //
                                    //    tran.Rollback();
                                    //    result.Success = false;
                                    //    result.Message = line.ItemCode + line.UomCode + " not enough inventory";
                                    //    return result;
                                    //}
                                    db.Execute("usp_I_T_InventoryLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);

                                    int sttSerial = 0;
                                    foreach (var serial in line.Lines)
                                    {
                                        sttSerial++;
                                        parameters = new DynamicParameters(); 
                                        parameters.Add("INVTId", key);
                                        parameters.Add("LineId", stt);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", serial.ItemCode);
                                        parameters.Add("SerialNum", serial.SerialNum);
                                        parameters.Add("FrSlocId", line.FrSlocId);
                                        parameters.Add("ToSlocId", line.ToSlocId);
                                        parameters.Add("Quantity", serial.Quantity);
                                        parameters.Add("UOMCode", serial.UomCode);
                                        parameters.Add("CreatedBy", serial.CreatedBy); 
                                        parameters.Add("Status", serial.Status);
                                        parameters.Add("Description", line.Description);

                                        checkparameters = new DynamicParameters();
                                        checkparameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                                        checkparameters.Add("StoreId", model.FromStore);
                                        checkparameters.Add("SlocId", Whs);
                                        checkparameters.Add("ItemCode", serial.ItemCode);
                                        checkparameters.Add("UomCode", serial.UomCode);
                                        checkparameters.Add("BarCode", "");
                                        checkparameters.Add("SerialNum", serial.SerialNum);

                                        var checkresultSerial = db.Query<ItemStockViewModel>("USP_GetItemStock", checkparameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //if (checkresultSerial.Count() == 0 || checkresultSerial.ToList()[0].Quantity < line.Quantity)
                                        //{
                                        //    //
                                        //    tran.Rollback();
                                        //    result.Success = false;
                                        //    result.Message = line.ItemCode + line.UomCode + ",Serial: " + serial.SerialNum + " not enough inventory";
                                        //    return result;
                                        //}
                                        db.Execute("USP_I_T_InventoryLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }
                                if (model.DocType == "R")
                                {
                                    string updateHeader = $"USP_Update_Status_SalesHeader '{model.RefInvtid}', '{model.CompanyCode}'";
                                    db.Execute(updateHeader, null, commandType: CommandType.Text, transaction: tran);
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
        }

        public GenericResult GetSourceDocument(string CompanyCode, string StoreId, string DocNum, string ConnectionStr)
        {
            GenericResult result = new GenericResult();
            try
            {
                using (IDbConnection db = _headerRepository.GetConnectionCustom(ConnectionStr))
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();

                        string query = $"USP_S_T_InventoryHeader N'{CompanyCode}', N'{StoreId}', N'{DocNum}'";

                        var data = db.QueryFirstOrDefault(query, null);
                        InventoryViewModel transfer = new InventoryViewModel();
                        transfer = _mapper.Map<InventoryViewModel>(data);
                        result.Success = true;
                        result.Data = transfer;
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
        public async Task<GenericResult> CreateByTableType(InventoryViewModel model)
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

            if (model.FromStore == null || model.ToStore == null)
            {
                result.Success = false;
                result.Message = "From Store / To Store not null.";
                return result;
            }
            if (string.IsNullOrEmpty(model.DocType))
            {
                result.Success = false;
                result.Message = "Doctype null. Please check doctype of document.";
                return result;
            }
            foreach (var line in model.Lines)
            {
                if (line.FrSlocId == line.ToSlocId)
                {
                    result.Success = false;
                    result.Message = "Can't transfer the same of Storage. " + line.ItemCode;
                    return result;
                }
            }
            if (model.DocType == "R" && string.IsNullOrEmpty(model.RefInvtid))
            {
                result.Success = false;
                result.Message = "please check Inventory receipt. Ref Id not null";
                return result;
            }
            
            try
            {

                using (IDbConnection db = _headerRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {

                                string IVLineTbl = "T_InventoryLine";
                                var IVLines = _commonService.CreaDataTable(IVLineTbl);
                                string IVSerialTbl = "T_InventoryLineSerial";

                                var IVLineSerial = _commonService.CreaDataTable(IVSerialTbl);

                                if (IVLines == null || IVLineSerial == null)
                                {
                                    result.Success = false;
                                    result.Message = "Table Type Object can't init";
                                    return result;
                                }

                                //string itemList = "";
                                //foreach (var line in model.Lines)
                                //{
                                //    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                //}
                                string VirtualWhs = $"select   dbo.[fnc_GetVirtualWhs]( '{model.CompanyCode}','{model.FromStore}')";
                                string geWhs = _headerRepository.GetScalar(VirtualWhs, null, commandType: CommandType.Text);
                                if (string.IsNullOrEmpty(geWhs))
                                {
                                    result.Success = false;
                                    result.Message = $"Can't found virtual warehouse ({model.FromStoreName})";
                                    return result;
                                }

                                //Lấy thông tin setting của Store

                                //var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.FromStore);
                                //List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                //if (settingData.Success)
                                //{
                                //    SettingList = settingData.Data as List<GeneralSettingStore>; 
                                //}
                               

                                string itemList = "";
                                string key = "";
                                List<ItemCheckModel> listItemCheck = new List<ItemCheckModel>();
                                bool checkSourceDocument = false;
                              
                                //Mã cho phiếu Receipt
                                if (model.DocType == "R")
                                {
                                    var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.FromStore);
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
                                    if(checkSourceDocument==true)
                                    {
                                        string connStr = settingDataCheckSource.CustomField1;
                                        if(!string.IsNullOrEmpty(connStr))
                                        {
                                            connStr = Utilities.Helpers.Encryptor.DecryptString(connStr, RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                            var getSourceDocumentRs = GetSourceDocument(model.CompanyCode, model.FromStore, model.RefId, connStr);
                                            if(getSourceDocumentRs.Success)
                                            {
                                                var sourceData = getSourceDocumentRs.Data as TInventoryHeader;
                                                if (string.IsNullOrEmpty(sourceData.Source))
                                                {
                                                    sourceData.Source = "";
                                                }
                                                if (string.IsNullOrEmpty(PrefixTR))
                                                {
                                                    PrefixTR = "";
                                                }
                                                if (PrefixTR == sourceData.Source)
                                                {

                                                }
                                                else
                                                {
                                                    var dataCurrentRs = GetByIdNotAsync(model.CompanyCode, model.FromStore, model.RefId);
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
                                            result.Message = $"Can't found source document connection string ("+model.Source+")";
                                            return result;
                                        }    
                                    }   
                                    //else
                                    //{

                                    //}    
                                    var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ManageStock").FirstOrDefault();
                                    if (setting != null && (setting.SettingValue == "true" || setting.SettingValue == "1"))
                                    {
                                        foreach (var line in model.Lines)
                                        {
                                            if (line.Quantity > 0)
                                            {
                                                //itemList += line.ItemCode + "-" + line.FrSlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                                ItemCheckModel itemCheck = new ItemCheckModel();
                                                itemCheck.ItemCode = line.ItemCode;
                                                itemCheck.SlocId = model.TransitWhs;
                                                itemCheck.UomCode = line.UomCode;
                                                itemCheck.Quantity = (double)line.Quantity;
                                                if (listItemCheck == null || listItemCheck.Count == 0)
                                                {
                                                    listItemCheck.Add(itemCheck);
                                                }
                                                else
                                                {
                                                    var checkInList = listItemCheck.Where(x => x.ItemCode == line.ItemCode && x.SlocId == model.TransitWhs && x.UomCode == line.UomCode).FirstOrDefault();
                                                    if (checkInList != null)
                                                    {
                                                        checkInList.Quantity += (double)line.Quantity;
                                                    }
                                                    else
                                                    {
                                                        listItemCheck.Add(itemCheck);
                                                    }
                                                }
                                            }
                                        }
                                        if (listItemCheck != null && listItemCheck.Count > 0)
                                        {
                                            foreach (var line in listItemCheck)
                                            {
                                                itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                            }

                                        }
                                        if (model.IsCanceled.ToLower() == "n")
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
                                    }

                                    key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixTR}','{model.CompanyCode}','{model.FromStore}')", null, commandType: CommandType.Text);
                                    
                                

                                }
                                //Mã cho phiếu Transfer

                                if (model.DocType == "T")
                                {
                                    var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.FromStore);
                                    List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                    if (settingData.Success)
                                    {
                                        SettingList = settingData.Data as List<GeneralSettingStore>;
                                    }
                                    var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ManageStock").FirstOrDefault();
                                    if (setting != null && (setting.SettingValue == "true" || setting.SettingValue == "1"))
                                    {
                                        //foreach (var line in model.Lines)
                                        //{
                                        //    if (line.Quantity > 0)
                                        //    {
                                        //        itemList += line.ItemCode + "-" + model.TransitWhs + "-" + line.UomCode + "-" + line.Quantity + ";";
                                        //    }
                                        //}
                                        foreach (var line in model.Lines)
                                        {
                                            if (line.Quantity > 0)
                                            {
                                                //itemList += line.ItemCode + "-" + line.FrSlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                                ItemCheckModel itemCheck = new ItemCheckModel();
                                                itemCheck.ItemCode = line.ItemCode;
                                                itemCheck.SlocId = model.TransitWhs;
                                                itemCheck.UomCode = line.UomCode;
                                                itemCheck.Quantity = (double)line.Quantity;
                                                if (listItemCheck == null || listItemCheck.Count == 0)
                                                {
                                                    listItemCheck.Add(itemCheck);
                                                }
                                                else
                                                {
                                                    var checkInList = listItemCheck.Where(x => x.ItemCode == line.ItemCode && x.SlocId == model.TransitWhs && x.UomCode == line.UomCode).FirstOrDefault();
                                                    if (checkInList != null)
                                                    {
                                                        checkInList.Quantity += (double)line.Quantity;
                                                    }
                                                    else
                                                    {
                                                        listItemCheck.Add(itemCheck);
                                                    }
                                                }
                                            }
                                        }
                                        if (listItemCheck != null && listItemCheck.Count > 0)
                                        {
                                            foreach (var line in listItemCheck)
                                            {
                                                itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                            }

                                        }
                                        if (model.IsCanceled.ToLower() == "n")
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

                                    }
                                    key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixTF}','{model.CompanyCode}','{model.FromStore}')", null, commandType: CommandType.Text);
                                }
                                //Mã cho phiếu Shipment 
                                if (model.DocType == "S")
                                {
                                    var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.FromStore);
                                    List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                    if (settingData.Success)
                                    {
                                        SettingList = settingData.Data as List<GeneralSettingStore>;
                                    }
                                    var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ManageStock").FirstOrDefault();
                                    if (setting != null && (setting.SettingValue == "true" || setting.SettingValue == "1"))
                                    {
                                        //foreach (var line in model.Lines)
                                        //{
                                        //    if (line.Quantity > 0)
                                        //        itemList += line.ItemCode + "-" + line.FrSlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                        //}

                                        foreach (var line in model.Lines)
                                        {
                                            if (line.Quantity > 0)
                                            {
                                                //itemList += line.ItemCode + "-" + line.FrSlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                                ItemCheckModel itemCheck = new ItemCheckModel();
                                                itemCheck.ItemCode = line.ItemCode;
                                                itemCheck.SlocId = line.FrSlocId;
                                                itemCheck.UomCode = line.UomCode;
                                                itemCheck.Quantity = (double)line.Quantity;
                                                if (listItemCheck == null || listItemCheck.Count == 0)
                                                {
                                                    listItemCheck.Add(itemCheck);
                                                }
                                                else
                                                {
                                                    var checkInList = listItemCheck.Where(x => x.ItemCode == line.ItemCode && x.SlocId == line.FrSlocId && x.UomCode == line.UomCode).FirstOrDefault();
                                                    if (checkInList != null)
                                                    {
                                                        checkInList.Quantity += (double)line.Quantity;
                                                    }
                                                    else
                                                    {
                                                        listItemCheck.Add(itemCheck);
                                                    }
                                                }
                                            }
                                        }
                                        if (listItemCheck != null && listItemCheck.Count > 0)
                                        {
                                            foreach (var line in listItemCheck)
                                            {
                                                itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                            }

                                        }
                                        if (model.IsCanceled.ToLower() == "n")
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
                                    }

                                    key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixTS}','{model.CompanyCode}','{model.FromStore}')", null, commandType: CommandType.Text);
                                }

                                //Mã cho phiếu Request  
                                if (model.DocType == "Q")
                                {
                                    key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixTQ}','{model.CompanyCode}','{model.ToStore}')", null, commandType: CommandType.Text);
                                }
                                string keyCache = string.Format(PrefixCacheActionIN, model.FromStore, model.ToStore, model.TerminalId);
                                string storeCache = cacheService.GetCachedData<string>(keyCache);
                                if (string.IsNullOrEmpty(storeCache))
                                {
                                    cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
                                }
                                else
                                {
                                    result.Success = false;
                                    result.Message = "Your actions are too fast and too dangerous. Please wait for your Transaction to be completed.";
                                    return result;
                                }
                                //Create and fill-up master table data

                                model.Invtid = key;
                                var parameters = new DynamicParameters();
                                parameters.Add("Invtid", model.Invtid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("DocType", model.DocType);
                                parameters.Add("RefINVTId", model.RefInvtid);
                                parameters.Add("DocDate", model.DocDate);
                                parameters.Add("DocDueDate", model.DocDueDate);
                                parameters.Add("FromStore", model.FromStore);
                                parameters.Add("FromStoreName", model.FromStoreName);
                                parameters.Add("ToStore", model.ToStore);
                                parameters.Add("ToStoreName", model.ToStoreName);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("Name", model.Name);
                                parameters.Add("Remark", model.Remark);
                                parameters.Add("RefId", model.RefId);
                                parameters.Add("TransitWhs", model.TransitWhs);
                                parameters.Add("FromWhs", model.FromWhs);
                                parameters.Add("ToWhs", model.ToWhs);
                                parameters.Add("ShiftId", model.ShiftId);
                                if (model.IsCanceled == "Y")
                                {
                                    model.IsCanceled = "C";
                                 
                                }
                                string Prefix = "";
                                if (model.DocType == "T")
                                {
                                    Prefix = PrefixTF;
                                }
                                if (model.DocType == "S")
                                {
                                    Prefix =PrefixTS;
                                }
                                if (model.DocType == "R")
                                {
                                    Prefix = PrefixTR;
                                }
                                if (model.DocType == "Q")
                                {
                                    Prefix = PrefixTQ;
                                }
                                parameters.Add("PrefixIV", Prefix);

                               
                                parameters.Add("IsCanceled", model.IsCanceled);
                               
                                int stt = 0;
                                List<TInventoryLineSerial> lineSerials = new List<TInventoryLineSerial>();
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    string Whs = model.TransitWhs;
                                    if (model.DocType == "S" || model.DocType == "T")
                                    {
                                        line.OpenQty = line.Quantity;
                                        Whs = line.FrSlocId;
                                    }
                                    line.Invtid = key;
                                    line.CreatedBy = model.CreatedBy;
                                    line.CompanyCode = model.CompanyCode;
                                    line.LineTotal = line.Price * line.Quantity;
                                    line.DocType = model.DocType;
                                    parameters.Add("DocType", model.DocType);


                                    var checkparameters = new DynamicParameters();
                                    checkparameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                                    checkparameters.Add("StoreId", model.FromStore);
                                    checkparameters.Add("SlocId", Whs);
                                    checkparameters.Add("ItemCode", line.ItemCode);
                                    checkparameters.Add("UomCode", line.UomCode);
                                    checkparameters.Add("BarCode", line.BarCode);
                                    checkparameters.Add("SerialNum", "");

                               
                                    int sttSerial = 0;
                                 
                                    foreach (var serial in line.Lines)
                                    {
                                        sttSerial++;
                                        serial.Invtid = key;
                                        serial.LineId = stt.ToString();
                                        serial.CreatedBy = model.CreatedBy;
                                        serial.CompanyCode = model.CompanyCode;
                                        serial.FrSlocId = line.FrSlocId;
                                        serial.ToSlocId = line.ToSlocId;
                                        serial.Description = line.Description;
                                        lineSerials.Add(serial);
                                      

                                        checkparameters = new DynamicParameters();
                                        checkparameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                                        checkparameters.Add("StoreId", model.FromStore);
                                        checkparameters.Add("SlocId", Whs);
                                        checkparameters.Add("ItemCode", serial.ItemCode);
                                        checkparameters.Add("UomCode", serial.UomCode);
                                        checkparameters.Add("BarCode", "");
                                        checkparameters.Add("SerialNum", serial.SerialNum);
 
                                    }
                                }

                                IVLines = ExtensionsNew.ConvertListToDataTable(model.Lines, IVLines);
                                IVLineSerial = ExtensionsNew.ConvertListToDataTable(lineSerials, IVLineSerial);
                                string tblLineType = IVLineTbl + "TableType";
                                string tblGISerialTbl = IVSerialTbl + "TableType";

                                parameters.Add("@Lines", IVLines.AsTableValuedParameter(IVLineTbl + "TableType"));
                                parameters.Add("@LineSerials", IVLineSerial.AsTableValuedParameter(IVSerialTbl + "TableType"));

                                key = db.ExecuteScalar("USP_I_T_Inventory", parameters, commandType: CommandType.StoredProcedure, transaction: tran).ToString();

                                if (model.IsCanceled == "Y" || model.IsCanceled == "C")
                                {
                                    //model.IsCanceled = "C";
                                    string updateQry = $"update T_InventoryHeader set IsCanceled = 'Y', Status='C' where INVTId = '{model.RefId}'and CompanyCode='{model.CompanyCode}'";
                                    db.Execute(updateQry, null, commandType: CommandType.Text, transaction: tran);
                                }

                                if (model.DocType == "R" || model.DocType == "S")
                                {
                                    string updateHeader = $"USP_Update_Status_SalesHeader '{model.RefInvtid}', '{model.CompanyCode}'";
                                    db.Execute(updateHeader, null, commandType: CommandType.Text, transaction: tran);
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
        }

        public Task<GenericResult> Delete(string companyCode, string storeId, string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _headerRepository.GetAllAsync($"select * from T_InventoryHeader with (nolock) where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetInventoryList(string CompanyCode, string FromStore, string ToStore, string DocType, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode );
                parameters.Add("FrStoreId", FromStore);
                parameters.Add("ToStoreId", ToStore );
                parameters.Add("DocType", DocType );
                parameters.Add("Status", Status );
                parameters.Add("FrDate", FrDate );
                parameters.Add("ToDate", ToDate );
                parameters.Add("Keyword", Keyword ); 
                //parameters.Add("ViewBy", ViewBy); 

                var data = await _headerRepository.GetAllAsync($"USP_GetInventoryTransfer", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetById(string companyCode, string storeId, string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _headerRepository.GetAsync($"USP_S_T_InventoryHeader '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
                InventoryViewModel transfer = new InventoryViewModel();
                transfer = _mapper.Map<InventoryViewModel>(data);
                var lines = await _lineRepository.GetAllAsync($"USP_S_T_InventoryLine '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
              
                var lineData = _mapper.Map<List<InventoryLineViewModel>>(lines);
                transfer.Lines = lineData;
                foreach (var line in transfer.Lines)
                {
                    var serials = await _lineSerialRepository.GetAllAsync($"USP_S_T_InventoryLineSerial '{companyCode}', '{storeId}', '{Id}', '{line.LineId}'", null, commandType: CommandType.Text);
                    line.Lines = serials; 
                }
                result.Success = true;
                result.Data = transfer;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public GenericResult GetByIdNotAsync(string companyCode, string storeId, string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = _headerRepository.Get($"USP_S_T_InventoryHeader '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
                InventoryViewModel transfer = new InventoryViewModel();
                transfer = _mapper.Map<InventoryViewModel>(data);
                var lines =  _lineRepository.GetAll($"USP_S_T_InventoryLine '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);

                var lineData = _mapper.Map<List<InventoryLineViewModel>>(lines);
                transfer.Lines = lineData;
                foreach (var line in transfer.Lines)
                {
                    var serials =  _lineSerialRepository.GetAll($"USP_S_T_InventoryLineSerial '{companyCode}', '{storeId}', '{Id}', '{line.LineId}'", null, commandType: CommandType.Text);
                    line.Lines = serials;
                }
                result.Success = true;
                result.Data = transfer;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }


        public async Task<PagedList<TInventoryHeader>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _headerRepository.GetAllAsync($"select * from T_InventoryHeader with (nolock) where INVTId like N'%{userParams.keyword}%' ", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.EmployeeId);
                //}
                return await PagedList<TInventoryHeader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(InventoryViewModel model)
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
            if (model.FromStore == null || model.ToStore.Count() == 0)
            {
                result.Success = false;
                result.Message = "From Store / To Store not null.";
                return result;
            }
            if (string.IsNullOrEmpty(model.DocType))
            {
                result.Success = false;
                result.Message = "Doctype null. Please check doctype of document.";
                return result;
            }
            
            try
            {
                using (IDbConnection db = _headerRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                //string itemList = "";
                                //foreach (var line in model.Lines)
                                //{
                                //    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                //}
                                string VirtualWhs = "";
                                if (model.IsCanceled == "Y")
                                {
                                    VirtualWhs = $"select   dbo.[fnc_GetVirtualWhs]( '{model.CompanyCode}','{model.ToStore}')";
                                }
                                else
                                {
                                    VirtualWhs = $"select   dbo.[fnc_GetVirtualWhs]( '{model.CompanyCode}','{model.FromStore}')";
                                }

                                
                                string geWhs = _headerRepository.GetScalar(VirtualWhs, null, commandType: CommandType.Text);
                                if (string.IsNullOrEmpty(geWhs))
                                {
                                    result.Success = false;
                                    result.Message = $"Can't found virtual warehouse ({model.FromStoreName})";
                                    return result;
                                }
                                //Create and fill-up master table data
                                //string key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('GP',''{model.CompanyCode}'','{model.StoreId}')", null, commandType: CommandType.Text);
                                //model.Invtid = key;
                                if(model.DocType =="Q")
                                {
                                    model.RefInvtid = model.Invtid;
                                }    
                                var parameters = new DynamicParameters();
                                parameters.Add("Invtid", model.Invtid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("DocType", model.DocType);
                                parameters.Add("RefINVTId", model.RefInvtid);
                                parameters.Add("ModifiedBy", model.ModifiedBy);
                                parameters.Add("Status", model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("DocDate", model.DocDate == null ? null : model.DocDate.Value.ToString("yyyy-MM-dd"));
                                parameters.Add("DocDueDate", model.DocDueDate == null ? null : model.DocDueDate.Value.ToString("yyyy-MM-dd"));
                                
                                if(model.IsCanceled=="Y")
                                { 
                                    parameters.Add("FromStore", model.ToStore);
                                    parameters.Add("FromStoreName", model.ToStoreName);
                                    parameters.Add("ToStore", model.FromStore);
                                    parameters.Add("ToStoreName", model.FromStoreName);
                                }   
                                else
                                { 
                                    parameters.Add("FromStore", model.FromStore);
                                    parameters.Add("FromStoreName", model.FromStoreName);
                                    parameters.Add("ToStore", model.ToStore);
                                    parameters.Add("ToStoreName", model.ToStoreName);
                                }    
                               
                                parameters.Add("Name", model.Name);
                                parameters.Add("Remark", model.Remark);
                                parameters.Add("RefId", model.RefId);
                                parameters.Add("TransitWhs", model.TransitWhs);
                                parameters.Add("FromWhs", model.FromWhs);
                                parameters.Add("ToWhs", model.ToWhs);
                                parameters.Add("ShiftId", model.ShiftId);
                                string DocDate = model.DocDate == null ? "null" : model.DocDate.Value.ToString("yyyy-MM-dd");
                                string DocDueDate = model.DocDueDate == null ? "null" : model.DocDueDate.Value.ToString("yyyy-MM-dd");
                                //string query = $"[USP_U_T_InventoryHeader] '{model.Invtid}','{model.CompanyCode}','{model.DocType}','{model.RefInvtid}','{model.ModifiedBy}'" +
                                //    $",'{model.Status}',{model.IsCanceled},{DocDate},{DocDueDate},'{model.FromStore}','{model.FromStoreName}','{model.ToStore}','{model.ToStoreName}','{model.Name}','{model.Remark}'";
                                //"USP_U_T_InventoryHeader"
                                var affectedRows = db.Execute("USP_U_T_InventoryHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                var deletParameters = new DynamicParameters();
                                deletParameters.Add("Invtid", model.Invtid);
                                deletParameters.Add("CompanyCode", model.CompanyCode);

                                var removeLine = db.Execute("USP_D_T_InventoryLineAndSerialLine", deletParameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                decimal LineTotal = 0;
                                foreach (var line in model.Lines)
                                {
                                    //stt++;
                                    string ShipDate = line.ShipDate == null ? null : line.ShipDate.Value.ToString("yyyy-MM-dd");
                                    string Quantity = line.Quantity == null ? null : line.Quantity.Value.ToString();
                                    string OpenQty = line.OpenQty == null ? null : line.OpenQty.Value.ToString();
                                    string Price = line.Price == null ? null : line.Price.Value.ToString();
                                    if (!string.IsNullOrEmpty(Price))
                                    {
                                        LineTotal = decimal.Parse( Quantity )* decimal.Parse(Price);
                                    }
                                   
                                    parameters = new DynamicParameters();
                                    parameters.Add("INVTId", model.Invtid, DbType.String);
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("LineId", line.LineId);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    if (model.IsCanceled == "Y")
                                    {
                                        parameters.Add("FrSlocId", line.ToSlocId);
                                        parameters.Add("ToSlocId", line.FrSlocId);
                                    }
                                    else
                                    {
                                        parameters.Add("FrSlocId", line.FrSlocId);
                                        parameters.Add("ToSlocId", line.ToSlocId);
                                    }
                                 
                                    parameters.Add("DocType", line.DocType);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Description", line.Description);
                                    parameters.Add("UOMCode", line.UomCode);
                                    parameters.Add("Quantity", Quantity);
                                    parameters.Add("ShipDate", ShipDate);
                                    parameters.Add("OpenQty", OpenQty ); 
                                    parameters.Add("Price", Price );
                                    parameters.Add("LineTotal", LineTotal);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    parameters.Add("Status", line.Status);
                                    parameters.Add("BaseTransId", line.BaseTransId);
                                    parameters.Add("BaseLine", line.BaseLine);
                                    parameters.Add("Approve", line.Approve);

                                    string queryLine = $"usp_U_T_InventoryLine '{model.Invtid}','{model.CompanyCode}','{line.LineId}','{line.ItemCode}'" +
                                        $",'{line.FrSlocId}','{line.ToSlocId}','{line.DocType}','{line.BarCode}','{line.Description}','{line.UomCode}'" +
                                        $",'{Quantity}','{ShipDate}','{OpenQty}','{Price}','{LineTotal}','{line.ModifiedBy}','{line.Status}'";


                                    var affectedRowsLine = db.Execute("usp_I_T_InventoryLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    //var affectedRowsLineA = db.Execute(queryLine, null, commandType: CommandType.Text, transaction: tran);

                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);


                                    foreach (var serial in line.Lines)
                                    {
                                        
                                        parameters = new DynamicParameters();
                                        parameters.Add("INVTId", model.Invtid);
                                        parameters.Add("LineId", serial.LineId);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", serial.ItemCode);
                                        parameters.Add("SerialNum", serial.SerialNum);
                                        if (model.IsCanceled == "Y")
                                        {
                                            parameters.Add("FrSlocId", line.ToSlocId);
                                            parameters.Add("ToSlocId", line.FrSlocId);
                                        }
                                        else
                                        {
                                            parameters.Add("FrSlocId", line.FrSlocId);
                                            parameters.Add("ToSlocId", line.ToSlocId);
                                        }

                                        //parameters.Add("FrSlocId", line.FrSlocId);
                                        //parameters.Add("ToSlocId", line.ToSlocId);
                                        parameters.Add("Quantity", serial.Quantity);
                                        parameters.Add("UOMCode", serial.UomCode);
                                        //parameters.Add("ModifiedBy", serial.ModifiedBy);
                                        parameters.Add("CreatedBy", model.CreatedBy);
                                        parameters.Add("Status", serial.Status);
                                        parameters.Add("Description", line.Description);
                                        db.Execute("USP_I_T_InventoryLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                } 
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
        }

        public async Task<GenericResult> GetTranferNotify(string CompanyCode,string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _headerRepository.GetAllAsync($"select * from T_InventoryHeader with (nolock) where CompanyCode='{CompanyCode}' and (FromStore= '{StoreId}' or ToStore = '{StoreId}') and Status = 'O'", null, commandType: CommandType.Text);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> CheckitemImport(List<ItemModel> models)
        {
            var result = new GenericResult();
                using (IDbConnection db = _itemRepository.GetConnection())
                {
                    try
                    {
                        List<ItemModel> items = new List<ItemModel>();

                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        if(models.Count==0)
                        {
                        result.Success = false;
                        result.Message = "No Data!";
                    }    
                        foreach (var model in models)
                        {
   
                            var parameters = new DynamicParameters();
                            parameters.Add("CompanyCode", string.IsNullOrEmpty(model.CompanyCode) ? "" : model.CompanyCode);
                            parameters.Add("StoreId", string.IsNullOrEmpty(model.StoreId) ? "" : model.StoreId);
                            parameters.Add("ItemCode", string.IsNullOrEmpty(model.ItemCode) ? "" : model.ItemCode);
                            parameters.Add("UomCode", string.IsNullOrEmpty(model.UomCode) ? "" : model.UomCode);
                            parameters.Add("BarCode", string.IsNullOrEmpty(model.BarCode) ? "" : model.BarCode);
                            parameters.Add("Keyword", "");
                            parameters.Add("Merchandise", "");
                            parameters.Add("Type", "");
                             if (model.Iscount==true) { parameters.Add("Iscount", model.Iscount); }
                            var dblist = await db.QueryAsync<ItemViewModel>($"USP_GetItemWithoutPrice", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                            dblist = dblist.Where(x => x.CustomField8 != "999999");
                            ItemModel item = new ItemModel();
                            if (dblist.Count()>0)
                            {
                            if(model.ItemCode !=dblist.ToList()[0].ItemCode || model.UomCode != dblist.ToList()[0].UomCode || model.BarCode != dblist.ToList()[0].BarCode)
                            {
                                item.ItemCode = model.ItemCode;
                                item.IsNotfound = true;
                                item.BarCode = model.BarCode;
                                item.CompanyCode = model.CompanyCode;
                                item.UomCode = model.UomCode;
                                items.Add(item);
                            }   
                            else
                            {
                                item.ItemCode = dblist.ToList()[0].ItemCode;
                                if (model.IsOnhand == true)
                                {
                                    var parameters1 = new DynamicParameters();
                                    parameters1.Add("CompanyCode", model.CompanyCode);
                                    parameters1.Add("StoreId", model.StoreId);
                                    parameters1.Add("SlocId", model.FrSlocId);
                                    parameters1.Add("ItemCode", model.ItemCode);
                                    parameters1.Add("UomCode", model.UomCode);
                                    parameters1.Add("BarCode", model.BarCode);
                                    parameters1.Add("SerialNum", "");

                                    var itemStocks = await db.QueryAsync<ItemStockViewModel>($"USP_GetItemStock", param: parameters1, commandType: CommandType.StoredProcedure, commandTimeout: 600);
                                    if (itemStocks.Count() > 0)
                                    {
                                        item.Stock = itemStocks.ToList()[0].Quantity;
                                    }
                                    else
                                    {
                                        item.Stock = 0;
                                    }
                                }

                                item.ItemName = dblist.ToList()[0].ItemName;
                                item.DefaultPrice = dblist.ToList()[0].DefaultPrice;
                                item.BarCode = dblist.ToList()[0].BarCode;
                                item.CompanyCode = dblist.ToList()[0].CompanyCode;
                                item.UomCode = dblist.ToList()[0].UomCode;
                                item.IsNotfound = false;
                                //item.ItemName = dblist.ToList()[0].ItemCode;

                                items.Add(item);
                            }    
                            
                            }
                            else
                            {
                            item.ItemCode = model.ItemCode;
                            item.IsNotfound = true;
                            item.BarCode =model.BarCode;
                            item.CompanyCode = model.CompanyCode;
                            item.UomCode = model.UomCode;
                            items.Add(item);
                            }    
                            result.Success = true;
                            result.Data = items;
                        }
                        //db.Close();

                        //return dblist.ToList(); 
                    }
                catch (Exception ex)
                {
                    db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
                finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                }
                return result;
        }

        public async Task<GenericResult> Cancel(InventoryViewModel model)
        {
            GenericResult result = new GenericResult();

            try
            {
               
                using (IDbConnection db = _headerRepository.GetConnection())
                {
                    var id = await _headerRepository.GetAllAsync($"select * from T_InventoryHeader with (nolock) where  RefINVTId ='{model.Invtid}' and DocType = 'S' order by CreatedOn desc", null, commandType: CommandType.Text);
                    if(id.Count>0)
                    {
                        result.Success = false;
                        result.Message = "Phiếu đã tạo Tranfer Shipment !";
                        return result;
                    }    
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                var query = $"Update T_InventoryHeader set Status ='C' , IsCanceled ='Y' where INVTId = '{model.Invtid}'";
                                db.Execute(query, null, commandType: CommandType.Text, transaction: tran);
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
        }
    } 

}
