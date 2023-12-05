
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RPFO.Utilities.Extensions;

namespace RPFO.Application.Implements
{
   
    public class GoodIssueService : IGoodIssueService
    {
        private readonly IGenericRepository<TGoodsIssueHeader> _headerRepository;
        private readonly IGenericRepository<TGoodsIssueLine> _lineRepository;
        private readonly IGenericRepository<TGoodsIssueLineSerial> _lineSerialRepository;
        private readonly IBOMService _bomeService;
        private readonly IGeneralSettingService _settingService;
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;
        private IResponseCacheService cacheService;
        string  ServiceName = "T_GoodsIssue";
        List<string> TableNameList = new List<string>();
        private string PrefixCacheActionGI = "QAGI-{0}-{1}";
        private string PrefixGI = "";
        private TimeSpan timeQuickAction = TimeSpan.FromSeconds(15);
      
        public GoodIssueService(IGenericRepository<TGoodsIssueHeader> goodreceiptRepository, IBOMService bomeService, IGenericRepository<TGoodsIssueLine> goodreceiptLineRepository, ICommonService commonService,
             IGenericRepository<TGoodsIssueLineSerial> lineSerialRepository, IGeneralSettingService settingService, IMapper mapper, IResponseCacheService responseCacheService, IConfiguration config/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _headerRepository = goodreceiptRepository;
            _lineRepository = goodreceiptLineRepository;
            _lineSerialRepository = lineSerialRepository;
            _settingService = settingService;
            _commonService = commonService;
            _bomeService = bomeService;
            _mapper = mapper;
            this.cacheService = responseCacheService;
            string timeCache = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("TimeCacheAction"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (!string.IsNullOrEmpty(timeCache) && double.TryParse(timeCache, out double timeAction))
            {
                timeQuickAction = TimeSpan.FromSeconds(timeAction);
            }
            PrefixGI = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixGI"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            if (string.IsNullOrEmpty(PrefixGI))
            {
                PrefixGI = "GI";
            }
            TableNameList.Add("T_GoodsIssueLine");
            TableNameList.Add("T_GoodsIssueLineSerial");
            _commonService.InitService(ServiceName, TableNameList);
            
        }

     
        public class ResultModel
        {
            public int ID { get; set; }
            public string Message { get; set; }
        }
        public async Task<GenericResult> Create(GoodsIssueViewModel model)
        {

            //return await CreateByTableType(model);

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
                result.Message = "From Store / To Store not null.";
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
                                var excludeBOMLines = model.Lines.Where(x => string.IsNullOrEmpty(x.BOMId));
                                foreach (var line in excludeBOMLines)
                                {
                                   var bom = _bomeService.GetByItemCode(model.CompanyCode, line.ItemCode).Result;
                                    if(bom.Success)
                                    {
                                       var bomModel =  bom.Data as BOMViewModel;
                                       if(bomModel!=null && bomModel.Lines!=null && bomModel.Lines.Count > 0)
                                       {
                                            var chekItemBOMLine = model.Lines.Where(x=>x.BOMId == bomModel.ItemCode);
                                            if(chekItemBOMLine==null || chekItemBOMLine.Count() == 0)
                                            {
                                                foreach(var bomline in bomModel.Lines)
                                                {
                                                    var coppyLine = new GoodIssueLineViewModel();
                                                    coppyLine = model.Lines[0];
                                                    coppyLine.BOMId = bomModel.ItemCode;
                                                    coppyLine.BOMValue = bomline.Quantity;
                                                    coppyLine.ItemCode = bomline.ItemCode;
                                                    coppyLine.UomCode = bomline.UomCode;
                                                    var itemLine = model.Lines.Where(x => x.ItemCode == bomModel.ItemCode).FirstOrDefault();
                                                    coppyLine.Quantity = itemLine.Quantity * bomline.Quantity;
                                                    coppyLine.Price = 0;
                                                    coppyLine.BarCode = "";
                                                    coppyLine.Description = bomline.ItemName;
                                                    coppyLine.LineTotal = itemLine.Quantity * bomline.Quantity;
                                                    coppyLine.LineId = (bomModel.Lines.Count() + 1).ToString();
                                                    model.Lines.Add(coppyLine);
                                                }    
                                                

                                            }    
                                       }    
                                    }    
                                }

                                string itemList = "";
                                var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                if (settingData.Success)
                                {
                                    SettingList = settingData.Data as List<GeneralSettingStore>;
                                }

                                var checkShiftsetting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "INVInShift").FirstOrDefault();
                                if (checkShiftsetting != null && (checkShiftsetting.SettingValue == "true" || checkShiftsetting.SettingValue == "1"))
                                {
                                    if (string.IsNullOrEmpty(model.ShiftId))
                                    {
                                        result.Success = false;
                                        result.Message = "Not in shift please Create / Load your shift Or Try Login again. thanks";
                                        return result;
                                    }
                                }
                                var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ManageStock").FirstOrDefault();

                                List<ItemCheckModel> listItemCheck = new List<ItemCheckModel>();
                                if (setting != null && (setting.SettingValue == "true" || setting.SettingValue == "1"))
                                {
                                    foreach (var line in model.Lines)
                                    {
                                        if (line.Quantity > 0)
                                        {
                                            //itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                            ItemCheckModel itemCheck = new ItemCheckModel();
                                            itemCheck.ItemCode = line.ItemCode;
                                            itemCheck.SlocId = line.SlocId;
                                            itemCheck.UomCode = line.UomCode;
                                            itemCheck.Quantity = (double)line.Quantity;
                                            if (listItemCheck == null || listItemCheck.Count == 0)
                                            {
                                                listItemCheck.Add(itemCheck);
                                            }
                                            else
                                            {
                                                var checkInList = listItemCheck.Where(x => x.ItemCode == line.ItemCode && x.SlocId == line.SlocId && x.UomCode == line.UomCode).FirstOrDefault();
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
                                string keyCache = string.Format(PrefixCacheActionGI, model.StoreId, model.TerminalId);
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
                                string key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixGI}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
                                model.Invtid = key;
                                var parameters = new DynamicParameters();
                                parameters.Add("Invtid", model.Invtid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("TotalPayable", model.TotalPayable);
                                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                parameters.Add("TotalReceipt", model.TotalReceipt);
                                parameters.Add("TotalTax", model.TotalTax);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);

                                parameters.Add("Remark", model.Remark);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("RefId", model.RefId);
                                parameters.Add("MovementType", model.MovementType);
                                parameters.Add("ShiftId", model.ShiftId);
                                if (model.IsCanceled == "Y")
                                {
                                    model.IsCanceled = "C";
                                    string updateQry = $"update T_GoodsIssueHeader set IsCanceled = 'Y', Status='C' where INVTId = '{model.RefId}'and CompanyCode='{model.CompanyCode}'";
                                    db.Execute(updateQry, null, commandType: CommandType.Text, transaction: tran);
                                }
                                parameters.Add("IsCanceled", model.IsCanceled);
                                var affectedRows = db.Execute("USP_I_T_GoodsIssueHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    parameters = new DynamicParameters();
                                    parameters.Add("INVTId", key, DbType.String);
                                    parameters.Add("LineId", stt);
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("Description", line.Description);
                                    parameters.Add("SLocId", line.SlocId);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Uomcode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("Price", line.Price);
                                    parameters.Add("LineTotal", line.LineTotal);
                                    parameters.Add("CurrencyCode", line.CurrencyCode);
                                    parameters.Add("CurrencyRate", line.CurrencyRate);
                                    parameters.Add("TaxCode", line.TaxCode);
                                    parameters.Add("TaxRate", line.TaxRate);
                                    parameters.Add("TaxAmt", line.TaxAmt);
                                    parameters.Add("Remark", line.Remark);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    parameters.Add("Status", line.Status);
                                    parameters.Add("BOMId", line.BOMId);
                                    parameters.Add("BOMValue", line.BOMValue);

                                    db.Execute("usp_I_T_GoodsIssueLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);

                                    int sttSerial = 0;
                                    foreach (var serial in line.SerialLines)
                                    {
                                        sttSerial++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("INVTId", key);
                                        parameters.Add("LineId", stt);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", serial.ItemCode);
                                        parameters.Add("SerialNum", serial.SerialNum);
                                        parameters.Add("SLocId", serial.SlocId);
                                        parameters.Add("Quantity", serial.Quantity);
                                        parameters.Add("UOMCode", serial.UomCode);
                                        parameters.Add("CreatedBy", model.CreatedBy);
                                        parameters.Add("Status", serial.Status);
                                        parameters.Add("Description", serial.Description);

                                        db.Execute("USP_I_T_GoodsIssueLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
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
        }
        //public class MyObject
        //{
        //    public int Sno { get; set; }
        //    public string Name { get; set; }
        //    public DateTime Dat { get; set; }
        //}

   
        public async Task<GenericResult> CreateByTableType(GoodsIssueViewModel model)
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
                result.Message = "From Store / To Store not null.";
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
                                string GILineTbl = "T_GoodsIssueLine"; 
                                var GILines = _commonService.CreaDataTable(GILineTbl);
                                string GISerialTbl = "T_GoodsIssueLineSerial"; 

                                var GILineSerial = _commonService.CreaDataTable(GISerialTbl);

                                if(GILines == null || GILineSerial == null)
                                {
                                    result.Success = false;
                                    result.Message = "Table Type Object can't init";
                                    return result;
                                }    
                                string itemList = "";
                                var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                                if (settingData.Success)
                                {
                                    SettingList = settingData.Data as List<GeneralSettingStore>;
                                }

                                var checkShiftsetting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "INVInShift").FirstOrDefault();
                                if (checkShiftsetting != null && (checkShiftsetting.SettingValue == "true" || checkShiftsetting.SettingValue == "1"))
                                {
                                    if (string.IsNullOrEmpty(model.ShiftId))
                                    {
                                        result.Success = false;
                                        result.Message = "Not in shift please Create / Load your shift Or Try Login again. thanks";
                                        return result;
                                    }
                                }
                                var excludeBOMLines = model.Lines.Where(x => string.IsNullOrEmpty(x.BOMId));
                                foreach (var line in excludeBOMLines)
                                {
                                    var bom = _bomeService.GetByItemCode(model.CompanyCode, line.ItemCode).Result;
                                    if (bom.Success)
                                    {
                                        var bomModel = bom.Data as BOMViewModel;
                                        if (bomModel != null && bomModel.Lines != null && bomModel.Lines.Count > 0)
                                        {
                                            var chekItemBOMLine = model.Lines.Where(x => x.BOMId == bomModel.ItemCode);
                                            if (chekItemBOMLine == null || chekItemBOMLine.Count() == 0)
                                            {
                                                foreach (var bomline in bomModel.Lines)
                                                {
                                                    var coppyLine = new GoodIssueLineViewModel();
                                                    coppyLine = model.Lines[0];
                                                    coppyLine.BOMId = bomModel.ItemCode;
                                                    coppyLine.BOMValue = bomline.Quantity;
                                                    coppyLine.ItemCode = bomline.ItemCode;
                                                    coppyLine.UomCode = bomline.UomCode;
                                                    var itemLine = model.Lines.Where(x => x.ItemCode == bomModel.ItemCode).FirstOrDefault();
                                                    coppyLine.Quantity = itemLine.Quantity * bomline.Quantity;
                                                    coppyLine.Price = 0;
                                                    coppyLine.BarCode = "";
                                                    coppyLine.Description = bomline.ItemName;
                                                    coppyLine.LineTotal = itemLine.Quantity * bomline.Quantity;
                                                    coppyLine.LineId = (bomModel.Lines.Count() + 1).ToString();
                                                    model.Lines.Add(coppyLine);
                                                }


                                            }
                                        }
                                    }
                                }


                                var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ManageStock").FirstOrDefault();

                                List<ItemCheckModel> listItemCheck = new List<ItemCheckModel>();
                                if (setting != null && (setting.SettingValue == "true" || setting.SettingValue == "1"))
                                {
                                    foreach (var line in model.Lines)
                                    {
                                        if (line.Quantity > 0)
                                        {
                                            //itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                            ItemCheckModel itemCheck = new ItemCheckModel();
                                            itemCheck.ItemCode = line.ItemCode;
                                            itemCheck.SlocId = line.SlocId;
                                            itemCheck.UomCode = line.UomCode;
                                            itemCheck.Quantity = (double)line.Quantity;
                                            if (listItemCheck == null || listItemCheck.Count == 0)
                                            {
                                                listItemCheck.Add(itemCheck);
                                            }
                                            else
                                            {
                                                var checkInList = listItemCheck.Where(x => x.ItemCode == line.ItemCode && x.SlocId == line.SlocId && x.UomCode == line.UomCode).FirstOrDefault();
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
                                string keyCache = string.Format(PrefixCacheActionGI, model.StoreId, model.TerminalId);
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
                                string key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixGI}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
                                model.Invtid = key;
                                var parameters = new DynamicParameters();
                                parameters.Add("Invtid", model.Invtid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("TotalPayable", model.TotalPayable);
                                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                parameters.Add("TotalReceipt", model.TotalReceipt);
                                parameters.Add("TotalTax", model.TotalTax);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);

                                parameters.Add("Remark", model.Remark);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("RefId", model.RefId);
                                parameters.Add("MovementType", model.MovementType);
                                parameters.Add("ShiftId", model.ShiftId);
                                parameters.Add("PrefixGI", PrefixGI);
                                if (model.IsCanceled == "Y")
                                {
                                    model.IsCanceled = "C";
                                    string updateQry = $"update T_GoodsIssueHeader set IsCanceled = 'Y', Status='C' where INVTId = '{model.RefId}'and CompanyCode='{model.CompanyCode}'";
                                    db.Execute(updateQry, null, commandType: CommandType.Text, transaction: tran);
                                }
                                parameters.Add("IsCanceled", model.IsCanceled);
                               
                                int stt = 0;
                                List<TGoodsIssueLineSerial> lineSerials = new List<TGoodsIssueLineSerial>();
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    
                                    line.Invtid = key;
                                    line.LineId = stt.ToString();
                                    line.Status = model.Status;
                                    line.CreatedBy = model.CreatedBy;
                                    line.CompanyCode = model.CompanyCode; 
                                    int sttSerial = 0;
                                    foreach (var serial in line.SerialLines)
                                    {
                                        sttSerial++;
                                        serial.Invtid = key;
                                        serial.LineId = sttSerial.ToString();
                                        serial.CompanyCode = model.CompanyCode;
                                        lineSerials.Add(serial); 
                                    }
                                }

                                GILines = ExtensionsNew.ConvertListToDataTable(model.Lines, GILines);
                                GILineSerial = ExtensionsNew.ConvertListToDataTable(lineSerials, GILineSerial);
                                string tblLineType = GILineTbl + "TableType";
                                string tblGISerialTbl = GISerialTbl + "TableType";
                                parameters.Add("@Lines", GILines.AsTableValuedParameter( GILineTbl + "TableType"));
                                parameters.Add("@LineSerials", GILineSerial.AsTableValuedParameter( GISerialTbl + "TableType"));

                                key = db.ExecuteScalar("USP_I_T_GoodsIssue", parameters, commandType: CommandType.StoredProcedure, transaction: tran).ToString(); 
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
                var data = await _headerRepository.GetAllAsync($"USP_S_T_GoodsIssueHeader '{CompanyCode}', '', ''", null, commandType: CommandType.Text);
                //var data = await _headerRepository.GetAllAsync($"select * from T_GoodsIssueHeader with (nolock)", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetByStore(string companyCode, string storeId)
        {
            GenericResult result = new GenericResult();

            try
            {
                var data = await _headerRepository.GetAllAsync($"USP_S_T_GoodsIssueHeader '{companyCode}', '{storeId}', ''", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetGoodsIssueList(string CompanyCode, string StoreId, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("Status", Status);
                parameters.Add("FrDate", FrDate);
                parameters.Add("ToDate", ToDate);
                parameters.Add("Keyword", Keyword);
                parameters.Add("ViewBy", ViewBy);
                var data = await _headerRepository.GetAllAsync($"USP_GetGoodsIssue", parameters, commandType: CommandType.StoredProcedure);
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
                var data = await _headerRepository.GetAsync($"USP_S_T_GoodsIssueHeader '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
                GoodsIssueViewModel goodIssue = new GoodsIssueViewModel();
                goodIssue = _mapper.Map<GoodsIssueViewModel>(data);
                var lines = await _lineRepository.GetAllAsync($"USP_S_T_GoodsIssueLine '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
              
                var lineData = _mapper.Map<List<GoodIssueLineViewModel>>(lines);
                var linesView = new List<GoodIssueLineViewModel>();
                var NoBom = lineData.Where(x => x.BOMId == null || x.BOMId.ToString() == "").ToList();
                foreach (var line in NoBom)
                {

                    //line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                    linesView.Add(line);
                }

                var bomHeader = new List<GoodIssueLineViewModel>();
                var bomlines = lineData.Where(x => x.BOMId != null && x.BOMId.ToString() != "").ToList();
                foreach (var line in linesView)
                {
                    var bomlineX = bomlines.Where(x => x.BOMId == line.ItemCode).ToList();
                    line.Lines = bomlineX;
                }

                goodIssue.Lines = linesView;

                foreach (var line in goodIssue.Lines)
                {
                    //with(nolock) where INVTId = '{Id}' and LineId = '{line.LineId}' and CompanyCode = '{companyCode}'
                    var serials = await _lineSerialRepository.GetAllAsync($"USP_S_T_GoodsIssueLineSerial '{companyCode}', '{storeId}', '{Id}', '{line.LineId}' ", null, commandType: CommandType.Text);
                    line.SerialLines = serials; 
                }
                result.Success = true;
                result.Data = goodIssue;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }

    

        public async Task<PagedList<TGoodsIssueHeader>> GetPagedList(UserParams userParams)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _headerRepository.GetAllAsync($"select * from T_GoodsIssueHeader with (nolock) where INVTId like N'%{userParams.keyword}%' ", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.EmployeeId);
                //}
                return await PagedList<TGoodsIssueHeader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(GoodsIssueViewModel model)
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
                result.Message = "From Store / To Store not null.";
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
                                string itemList = "";
                                foreach (var line in model.Lines)
                                {
                                    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                }

                                //Create and fill-up master table data
                                //string key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('GP',''{model.CompanyCode}'','{model.StoreId}')", null, commandType: CommandType.Text);
                                //model.Invtid = key;
                                var parameters = new DynamicParameters();
                                 
	  
                                parameters.Add("Invtid", model.Invtid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("TotalPayable", model.TotalPayable);
                                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                parameters.Add("TotalReceipt", model.TotalReceipt);
                                parameters.Add("TotalTax", model.TotalTax);
                                parameters.Add("ModifiedBy", model.ModifiedBy);
                                parameters.Add("Status", model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("Remark", model.Remark);
                                parameters.Add("RefId", model.RefId);
                                parameters.Add("MovementType", model.MovementType);
                                parameters.Add("ShiftId", model.ShiftId);
                                var affectedRows = db.Execute("USP_U_T_GoodsIssueHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    //stt++;
                                    parameters = new DynamicParameters();
                                    parameters.Add("INVTId", model.Invtid, DbType.String);
                                    parameters.Add("LineId", line.LineId);
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("SLocId", line.SlocId);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Uomcode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("Price", line.Price);
                                    parameters.Add("LineTotal", line.LineTotal);
                                    parameters.Add("CurrencyCode", line.CurrencyCode);
                                    parameters.Add("CurrencyRate", line.CurrencyRate);
                                    parameters.Add("TaxCode", line.TaxCode);
                                    parameters.Add("TaxRate", line.TaxRate);
                                    parameters.Add("TaxAmt", line.TaxAmt);
                                    parameters.Add("Remark", line.Remark);
                                    parameters.Add("ModifiedBy", line.ModifiedBy);
                                    parameters.Add("Status", line.Status);

                                    db.Execute("usp_U_T_GoodsIssueLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);

                                     
                                    foreach (var serial in line.SerialLines)
                                    {
                                        
                                        parameters = new DynamicParameters();
                                        parameters.Add("INVTId", model.Invtid);
                                        parameters.Add("LineId", serial.LineId);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", serial.ItemCode);
                                        parameters.Add("SerialNum", serial.SerialNum);
                                        parameters.Add("SLocId", serial.SlocId);
                                        parameters.Add("Quantity", serial.Quantity);
                                        parameters.Add("UOMCode", serial.UomCode);
                                        parameters.Add("ModifiedBy", serial.ModifiedBy);
                                        parameters.Add("Status", serial.Status);

                                        db.Execute("USP_U_T_GoodsIssueLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                } 
                                result.Success = true;
                                result.Message = model.Invtid;
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
