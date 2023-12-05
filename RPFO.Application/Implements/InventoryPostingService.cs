
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
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
    public class InventoryPostingService : IInventoryPostingService
    {
        private readonly IGenericRepository<TInventoryPostingHeader> _headerRepository;
        private readonly IGenericRepository<TInventoryPostingLine> _lineRepository;
        private readonly IGenericRepository<TInventoryPostingLineSerial> _lineSerialRepository;
        private readonly IMapper _mapper;
        private IResponseCacheService cacheService;
        private string PrefixCacheActionPT = "QAPT-{0}-{1}";
        private string PrefixPosting = "IP";
        private TimeSpan timeQuickAction = TimeSpan.FromSeconds(15);

        string ServiceName = "T_InventoryPosting";
        List<string> TableNameList = new List<string>();
        private readonly ICommonService _commonService;

        public InventoryPostingService(IGenericRepository<TInventoryPostingHeader> goodreceiptRepository, ICommonService commonService, IGenericRepository<TInventoryPostingLine> goodreceiptLineRepository,
             IGenericRepository<TInventoryPostingLineSerial> lineSerialRepository, IMapper mapper, IResponseCacheService responseCacheService, IConfiguration config/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _headerRepository = goodreceiptRepository;
            _lineRepository = goodreceiptLineRepository;
            _lineSerialRepository = lineSerialRepository;
            _commonService = commonService;
            _mapper = mapper;
            this.cacheService = responseCacheService;
            string timeCache = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("TimeCacheAction"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (!string.IsNullOrEmpty(timeCache) && double.TryParse(timeCache, out double timeAction))
            {
                timeQuickAction = TimeSpan.FromSeconds(timeAction);
            }
            PrefixPosting = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixPosting"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);

            if (string.IsNullOrEmpty(PrefixPosting))
            {
                PrefixPosting = "IP";
            }
            TableNameList.Add(ServiceName + "Line");
            TableNameList.Add(ServiceName + "LineSerial");
            _commonService.InitService(ServiceName, TableNameList);
        }

        public async Task<GenericResult> Create(InventoryPostingViewModel model)
        {
            //return await CreateByTableType(model);
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
                                //string itemList = "";
                                //foreach (var line in model.Lines)
                                //{
                                //    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                //}
                                string keyCache = string.Format(PrefixCacheActionPT, model.StoreId, model.TerminalId);
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
                                string key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixPosting}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
                                model.Ipid = key;
                                var parameters = new DynamicParameters();
                                parameters.Add("IPId", model.Ipid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("DocStatus", model.DocStatus);
                                parameters.Add("DocDate", model.DocDate);
                                parameters.Add("DocDueDate", model.DocDueDate);
                                parameters.Add("DocTotal", model.DocTotal);
                                parameters.Add("Comment", model.Comment);
                                parameters.Add("Name", model.Name);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);
                                //parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("RefId", model.RefId);
                                if (model.IsCanceled == "Y")
                                {
                                    model.IsCanceled = "C";
                                    string updateQry = $"update T_InventoryPostingHeader set IsCanceled = 'Y', Status='C' where IPId = '{model.RefId}'and CompanyCode='{model.CompanyCode}'";
                                    db.Execute(updateQry, null, commandType: CommandType.Text, transaction: tran);
                                }
                                parameters.Add("IsCanceled", model.IsCanceled);
                                var affectedRows = db.Execute("USP_I_T_InventoryPostingHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    parameters = new DynamicParameters();
                                    parameters.Add("IPId", model.Ipid, DbType.String);
                                    parameters.Add("CompanyCode", line.CompanyCode);
                                    parameters.Add("LineId", line.LineId);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("SLocId", line.SlocId);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Description", line.Description);
                                    parameters.Add("UOMCode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("Price", line.Price);
                                    parameters.Add("BaseRef", line.BaseRef);
                                    parameters.Add("BaseType", line.BaseType);
                                    parameters.Add("BaseEntry", line.BaseEntry);
                                    parameters.Add("LineStatus", line.LineStatus);
                                    parameters.Add("LineTotal", line.LineTotal);
                                    parameters.Add("Comment", line.Comment);

                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    parameters.Add("Status", line.Status);

                                    db.Execute("usp_I_T_InventoryPostingLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);

                                    //int sttSerial = 0;
                                    foreach (var serial in line.Lines)
                                    {
                                        //sttSerial++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("IPId", key);
                                        parameters.Add("LineId", line.LineId);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", serial.ItemCode);
                                        parameters.Add("SerialNum", serial.SerialNum);
                                        parameters.Add("SLocId", serial.SlocId);
                                        parameters.Add("Quantity", serial.Quantity);
                                        parameters.Add("UOMCode", serial.UomCode);
                                        parameters.Add("CreatedBy", model.CreatedBy);
                                        //parameters.Add("Status", serial.Status);
                                        parameters.Add("Description", line.Description);
                                        db.Execute("USP_I_T_InventoryPostingLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
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

        public async Task<GenericResult> CreateByTableType(InventoryPostingViewModel model)
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

                                string IPLineTbl = ServiceName + "Line";
                                var IPLines = _commonService.CreaDataTable(IPLineTbl);
                                string IPSerialTbl = ServiceName + "LineSerial";

                                var IPLineSerial = _commonService.CreaDataTable(IPSerialTbl);

                                if (IPLines == null || IPLineSerial == null)
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
                                string keyCache = string.Format(PrefixCacheActionPT, model.StoreId, model.TerminalId);
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
                                string key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixPosting}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
                                model.Ipid = key;
                                var parameters = new DynamicParameters();
                                parameters.Add("IPId", model.Ipid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("DocStatus", model.DocStatus);
                                parameters.Add("DocDate", model.DocDate);
                                parameters.Add("DocDueDate", model.DocDueDate);
                                parameters.Add("DocTotal", model.DocTotal);
                                parameters.Add("Comment", model.Comment);
                                parameters.Add("Name", model.Name);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);
                                //parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("RefId", model.RefId);
                                parameters.Add("PrefixIP", PrefixPosting);


                                parameters.Add("IsCanceled", model.IsCanceled);
                                List<TInventoryPostingLineSerial> lineSerials = new List<TInventoryPostingLineSerial>();
                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    line.Ipid = model.Ipid;
                                    line.CreatedBy = model.CreatedBy;
                                    line.CompanyCode = model.CompanyCode;


                                    //parameters = new DynamicParameters();
                                    //parameters.Add("IPId", model.Ipid, DbType.String);
                                    //parameters.Add("CompanyCode", line.CompanyCode);
                                    //parameters.Add("LineId", line.LineId);
                                    //parameters.Add("ItemCode", line.ItemCode);
                                    //parameters.Add("SLocId", line.SlocId);
                                    //parameters.Add("BarCode", line.BarCode);
                                    //parameters.Add("Description", line.Description);
                                    //parameters.Add("UOMCode", line.UomCode);
                                    //parameters.Add("Quantity", line.Quantity);
                                    //parameters.Add("Price", line.Price);
                                    //parameters.Add("BaseRef", line.BaseRef);
                                    //parameters.Add("BaseType", line.BaseType);
                                    //parameters.Add("BaseEntry", line.BaseEntry);
                                    //parameters.Add("LineStatus", line.LineStatus);
                                    //parameters.Add("LineTotal", line.LineTotal);
                                    //parameters.Add("Comment", line.Comment); 

                                    //parameters.Add("CreatedBy", model.CreatedBy);  
                                    //parameters.Add("Status", line.Status);

                                    //db.Execute("usp_I_T_InventoryPostingLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran); 

                                    //int sttSerial = 0;
                                    foreach (var serial in line.Lines)
                                    {
                                        serial.Ipid = key;

                                        serial.CreatedBy = model.CreatedBy;
                                        serial.CompanyCode = model.CompanyCode;

                                        serial.Description = line.Description;
                                        lineSerials.Add(serial);

                                        //sttSerial++;
                                        //parameters = new DynamicParameters(); 
                                        //parameters.Add("IPId", key);
                                        //parameters.Add("LineId", line.LineId);
                                        //parameters.Add("CompanyCode", model.CompanyCode);
                                        //parameters.Add("ItemCode", serial.ItemCode);
                                        //parameters.Add("SerialNum", serial.SerialNum);
                                        //parameters.Add("SLocId", serial.SlocId); 
                                        //parameters.Add("Quantity", serial.Quantity); 
                                        //parameters.Add("UOMCode", serial.UomCode);
                                        //parameters.Add("CreatedBy", model.CreatedBy);
                                        ////parameters.Add("Status", serial.Status);
                                        //parameters.Add("Description", line.Description);
                                        //db.Execute("USP_I_T_InventoryPostingLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    }
                                }

                                IPLines = ExtensionsNew.ConvertListToDataTable(model.Lines, IPLines);
                                IPLineSerial = ExtensionsNew.ConvertListToDataTable(lineSerials, IPLineSerial);
                                string tblLineType = IPLineTbl + "TableType";
                                string tblGISerialTbl = IPSerialTbl + "TableType";

                                parameters.Add("@Lines", IPLines.AsTableValuedParameter(IPLineTbl + "TableType"));
                                parameters.Add("@LineSerials", IPLineSerial.AsTableValuedParameter(IPSerialTbl + "TableType"));

                                key = db.ExecuteScalar("USP_I_T_InventoryPosting", parameters, commandType: CommandType.StoredProcedure, transaction: tran).ToString();

                                if (model.IsCanceled == "Y")
                                {
                                    model.IsCanceled = "C";
                                    string updateQry = $"update T_InventoryPostingHeader set IsCanceled = 'Y', Status='C' where IPId = '{model.RefId}'and CompanyCode='{model.CompanyCode}'";
                                    db.Execute(updateQry, null, commandType: CommandType.Text, transaction: tran);
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
        public Task<GenericResult> Delete(string companyCode,   string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string companyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _headerRepository.GetAllAsync($"select * from T_InventoryPostingHeader with (nolock) where CompanyCode ='{companyCode}'", null, commandType: CommandType.Text);
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
                var data = await _headerRepository.GetAllAsync($"USP_S_T_InventoryPostingHeader '{companyCode}', '{storeId}', ''", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetInventoryList(string CompanyCode, string StoreId, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy)
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
                var data = await _headerRepository.GetAllAsync($"USP_GetInventoryPosting", parameters, commandType: CommandType.StoredProcedure);
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
                var data = await _headerRepository.GetAsync($"USP_S_T_InventoryPostingHeader '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
                InventoryPostingViewModel model = new InventoryPostingViewModel();
                model = _mapper.Map<InventoryPostingViewModel>(data);
                var lines = await _lineRepository.GetAllAsync($"USP_S_T_InventoryPostingLine '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
              
                var lineData = _mapper.Map<List<InventoryPostingLineViewModel>>(lines);
                model.Lines = lineData;
                foreach (var line in model.Lines)
                {
                    string query = $"USP_S_T_InventoryPostingLineSerial '{companyCode}', '{storeId}', '{Id}', '{line.LineId}'";
                    var serials = await _lineSerialRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                    line.Lines = serials; 
                }
                result.Success = true;
                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }

    

        public async Task<PagedList<TInventoryPostingHeader>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _headerRepository.GetAllAsync($"select * from T_InventoryPostingHeader with (nolock) where IPId like N'%{userParams.keyword}%' ", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.EmployeeId);
                //}
                return await PagedList<TInventoryPostingHeader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(InventoryPostingViewModel model)
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
                                //string itemList = "";
                                //foreach (var line in model.Lines)
                                //{
                                //    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                //}

                                //Create and fill-up master table data
                                //string key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('GP',''{model.CompanyCode}'','{model.StoreId}')", null, commandType: CommandType.Text);
                                //model.Invtid = key;
                                var parameters = new DynamicParameters();
                                parameters.Add("IPId", model.Ipid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("DocStatus", model.DocStatus);
                                parameters.Add("DocDate", model.DocDate);
                                parameters.Add("DocDueDate", model.DocDueDate);
                                parameters.Add("DocTotal", model.DocTotal);
                                parameters.Add("Comment", model.Comment);
                                parameters.Add("Name", model.Name);
                                parameters.Add("ModifiedBy", model.ModifiedBy);
                                parameters.Add("Status", model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("RefId", model.RefId);
                                string DocDate = model.DocDate == null ? "null" : model.DocDate.Value.ToString("yyyy-MM-dd");
                                string DocDueDate = model.DocDueDate == null ? "null" : model.DocDueDate.Value.ToString("yyyy-MM-dd");
                                //string query = $"[USP_U_T_InventoryHeader] '{model.Invtid}','{model.CompanyCode}','{model.DocType}','{model.RefInvtid}','{model.ModifiedBy}'" +
                                //    $",'{model.Status}',{model.IsCanceled},{DocDate},{DocDueDate},'{model.FromStore}','{model.FromStoreName}','{model.ToStore}','{model.ToStoreName}','{model.Name}','{model.Remark}'";
                                //"USP_U_T_InventoryHeader"
                                var affectedRows = db.Execute("USP_U_T_InventoryPostingHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //var deletParameters = new DynamicParameters();
                                //deletParameters.Add("Invtid", model.Ipid);
                                //deletParameters.Add("CompanyCode", model.CompanyCode);

                                //var removeLine = db.Execute("USP_D_T_InventoryPostingLineAndSerialLine", deletParameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                foreach (var line in model.Lines)
                                {
                                    //stt++;
                                    //string ShipDate = line.ShipDate == null ? null : line.ShipDate.Value.ToString("yyyy-MM-dd");
                                    //string Quantity = line.Quantity == null ? null : line.Quantity.Value.ToString();
                                    string Quantity = line.Quantity == null ? null : line.Quantity.Value.ToString();
                                    string Price = line.Price == null ? null : line.Price.Value.ToString();
                                    string LineTotal = line.LineTotal == null ? null : line.LineTotal.Value.ToString();

                                    parameters = new DynamicParameters();
                                    parameters.Add("IPId", model.Ipid, DbType.String);
                                    parameters.Add("CompanyCode", line.CompanyCode);
                                    parameters.Add("LineId", line.LineId);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("SLocId", line.SlocId);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Description", line.Description);
                                    parameters.Add("UOMCode", line.UomCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("Price", line.Price);
                                    parameters.Add("BaseRef", line.BaseRef);
                                    parameters.Add("BaseType", line.BaseType);
                                    parameters.Add("BaseEntry", line.BaseEntry);
                                    parameters.Add("LineStatus", line.LineStatus);
                                    parameters.Add("LineTotal", line.LineTotal);
                                    parameters.Add("Comment", line.Comment); 
                                    parameters.Add("ModifiedBy", model.ModifiedBy);
                                    parameters.Add("Status", line.Status);


                                    //string queryLine = $"usp_U_T_InventoryLine '{model.Invtid}','{model.CompanyCode}','{line.LineId}','{line.ItemCode}'" +
                                    //    $",'{line.FrSlocId}','{line.ToSlocId}','{line.DocType}','{line.BarCode}','{line.Description}','{line.UomCode}'" +
                                    //    $",'{Quantity}','{ShipDate}','{OpenQty}','{Price}','{LineTotal}','{line.ModifiedBy}','{line.Status}'";


                                    var affectedRowsLine = db.Execute("usp_U_T_InventoryPostingLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    //var affectedRowsLineA = db.Execute(queryLine, null, commandType: CommandType.Text, transaction: tran);

                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);


                                    foreach (var serial in line.Lines)
                                    {
                                        
                                        parameters = new DynamicParameters();
                                        parameters.Add("IPId", model.Ipid);
                                        parameters.Add("LineId", line.LineId);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", serial.ItemCode);
                                        parameters.Add("SerialNum", serial.SerialNum);
                                        parameters.Add("SLocId", serial.SlocId);
                                        parameters.Add("Quantity", serial.Quantity); 
                                        parameters.Add("UOMCode", serial.UomCode);
                                        parameters.Add("ModifiedBy", model.ModifiedBy);
                                        //parameters.Add("Status", serial.Status);
                                        parameters.Add("Description", line.Description);
                                        db.Execute("USP_U_T_InventoryPostingLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
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
    } 

}
