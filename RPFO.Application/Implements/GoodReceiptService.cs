
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
    public class GoodReceiptService : IGoodReceiptService
    {
        private readonly IGenericRepository<TGoodsReceiptHeader> _headerRepository;
        private readonly IGenericRepository<TGoodsReceiptLine> _lineRepository;
        private readonly IGenericRepository<TGoodsReceiptLineSerial> _lineSerialRepository;
        private IWarehouseService _warehouseService;
        private readonly IMapper _mapper;
        private IResponseCacheService cacheService;
        private string PrefixCacheActionGR = "QAGR-{0}-{1}";
        private string PrefixGR = "GR";
        string ServiceName = "T_GoodsReceipt";
        List<string> TableNameList = new List<string>();
        private readonly ICommonService _commonService;
        private TimeSpan timeQuickAction = TimeSpan.FromSeconds(15);
        public GoodReceiptService(IGenericRepository<TGoodsReceiptHeader> goodreceiptRepository, ICommonService commonService, IGenericRepository<TGoodsReceiptLine> goodreceiptLineRepository,
             IGenericRepository<TGoodsReceiptLineSerial> lineSerialRepository, IWarehouseService warehouseService, IMapper mapper, IResponseCacheService responseCacheService, IConfiguration config/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _headerRepository = goodreceiptRepository;
            _lineRepository = goodreceiptLineRepository;
            _lineSerialRepository = lineSerialRepository;
            _warehouseService = warehouseService;
            _commonService = commonService;
            _mapper = mapper;
            this.cacheService = responseCacheService;
            string timeCache = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("TimeCacheAction"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (!string.IsNullOrEmpty(timeCache) && double.TryParse(timeCache, out double timeAction))
            {
                timeQuickAction = TimeSpan.FromSeconds(timeAction);
            }
            PrefixGR = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixGR"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            if (string.IsNullOrEmpty(PrefixGR))
            {
                PrefixGR = "GR";
            }
            TableNameList.Add(ServiceName + "Line");
            TableNameList.Add(ServiceName + "LineSerial");
            _commonService.InitService(ServiceName, TableNameList);
        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<GoodReceiptLineResultViewModel> resultlist = new List<GoodReceiptLineResultViewModel>();
            try
            {
                //TShiftHeader shiftHeader = new TShiftHeader();
                //shiftHeader.StoreId = model.StoreId;
                //shiftHeader.CompanyCode = model.CompanyCode;
                //shiftHeader.Status = "O";
                //GenericResult shiftResult = await _shiftService.Create(shiftHeader);
                //if (shiftResult.Success)
                //{
                //}
                //else
                //{
                //    return shiftResult;
                //}

                var List = model.GoodsReceiptImport.GroupBy(g => new { g.StoreId })
                   .Select(s => s.ToList())
                   .ToList();
                foreach (var group in List)
                {
                    var first = group.FirstOrDefault();
                    GoodReceiptViewModel goodReceipt = new GoodReceiptViewModel(); 
                    goodReceipt.CreatedBy = model.CreatedBy;
                    goodReceipt.CompanyCode = model.CompanyCode;
                    goodReceipt.IsCanceled = "N";
                    goodReceipt.StoreId = first.StoreId;
                    goodReceipt.MovementType = first.MovementType;
                    goodReceipt.Lines = _mapper.Map<List<GoodReceiptLineViewModel>>(group.ToList());
                    goodReceipt.Status = "C";
                    goodReceipt.TotalReceipt = goodReceipt.Lines.Sum(x => x.Quantity);
                    goodReceipt.TotalPayable = goodReceipt.Lines.Sum(x => x.LineTotal==null ? 0 : x.LineTotal);
                    var itemResult = await Create(goodReceipt);
                    GoodReceiptLineResultViewModel itemRs = new GoodReceiptLineResultViewModel();
                    //itemRs = itemResult.Success
                    itemRs.Success = itemResult.Success;
                    itemRs.Message = itemResult.Message;
                    resultlist.Add(itemRs);

                }
               
                //foreach (var item in model.GoodsReceipt)
                //{
                //    GoodReceiptViewModel goodReceipt = new GoodReceiptViewModel();

                //    goodReceipt.CreatedBy = model.CreatedBy;
                //    goodReceipt.CompanyCode = model.CompanyCode;
                //    goodReceipt.IsCanceled = "N";

                //    //goodReceipt.StoreId = item.S;
                //    goodReceipt.CompanyCode = model.CompanyCode;
                //    //item.ShiftId = shiftResult.Message;
                //    //item.TransId = "";

                //    var itemResult = await Create(goodReceipt);

                //    GoodReceiptLineResultViewModel itemRs = new GoodReceiptLineResultViewModel();
                //    itemRs = _mapper.Map<GoodReceiptLineResultViewModel>(item);
                //    itemRs.Success = itemResult.Success;
                //    itemRs.Message = itemResult.Message;
                //    resultlist.Add(itemRs);
                //}
                //await _shiftService.EndShift(shiftHeader);
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
        private static string ListCheck<T>(IEnumerable<T> l1, IEnumerable<T> l2)
        {
            //// TODO: Null parm checks
            //if (l1.Intersect(l2).Any())
            //{
            //    //Console.WriteLine("matched");
            //    return true;
            //}
            //else
            //{
            //    //Console.WriteLine("not matched");
            //    return false;
            //}
            string resultStr = "";
            foreach (var itemSloc in l2)
            {
                if (!l1.Contains(itemSloc))
                {
                    resultStr += itemSloc + ",";

                }
            }
            if (resultStr.Length > 0)
            {
                resultStr = resultStr.Substring(0, resultStr.Length - 1);
            }
            return resultStr;
        }
        public async Task<GenericResult> Create(GoodReceiptViewModel model)
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
                                //string itemList = "";
                                //foreach (var line in model.Lines)
                                //{
                                //    itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                //}
                                //string queryGetMoveType = $"select * from M_MovementType with (nolock)  where Code= N'{model.MovementType}'";

                                //var lstX = await db.QueryAsync<MMovementType>("USP_RPT_InvoiceTransactionDetail", null, commandType: CommandType.Text);
                                //if (lstX == null)
                                //{
                                //    result.Success = true;
                                //    result.Message = " Movement Type wrong";
                                //    tran.Rollback();
                                //    return result;
                                //}    
                                //Create and fill-up master table data

                                var listSlocData = await _warehouseService.GetByStore(model.CompanyCode, model.StoreId);
                                if (listSlocData.Success && listSlocData.Data != null)
                                {
                                    var lst = listSlocData.Data as List<MWarehouse>;
                                    if (lst.Count > 0)
                                    {

                                        var SlocInStore = lst.Select(x => x.WhsCode).Distinct();
                                        var SlocInData = model.Lines.Select(x => x.SlocId).Distinct();
                                        string checkStr = ListCheck(SlocInStore, SlocInData);
                                        string keyCache = string.Format(PrefixCacheActionGR, model.StoreId, model.TerminalId);
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
                                        if (string.IsNullOrEmpty(checkStr))
                                        {
                                            string key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixGR}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
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
                                            parameters.Add("IsCanceled", model.IsCanceled);
                                            parameters.Add("Remark", model.Remark);
                                            parameters.Add("StoreName", model.StoreName);
                                            parameters.Add("RefId", model.RefId);
                                            parameters.Add("MovementType", model.MovementType);
                                            parameters.Add("ShiftId", model.ShiftId);
                                            if (model.IsCanceled == "Y")
                                            {
                                                model.IsCanceled = "C";
                                                string updateQry = $"update T_GoodsReceiptHeader set IsCanceled = 'Y', Status='C' where INVTId = '{model.RefId}'and CompanyCode='{model.CompanyCode}'";
                                                db.Execute(updateQry, null, commandType: CommandType.Text, transaction: tran);
                                            }
                                            parameters.Add("IsCanceled", model.IsCanceled);
                                            var affectedRows = db.Execute("USP_I_T_GoodsReceiptHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                            int stt = 0;
                                            foreach (var line in model.Lines)
                                            {
                                                stt++;
                                                parameters = new DynamicParameters();
                                                parameters.Add("INVTId", key, DbType.String);
                                                parameters.Add("LineId", stt);
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
                                                parameters.Add("CreatedBy", model.CreatedBy);
                                                parameters.Add("Status", line.Status);
                                                parameters.Add("Description", line.Description);

                                                db.Execute("usp_I_T_GoodsReceiptLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
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
                                                    parameters.Add("SLocId", serial.SlocId);
                                                    parameters.Add("Quantity", serial.Quantity);
                                                    parameters.Add("UOMCode", serial.UomCode);
                                                    parameters.Add("CreatedBy", model.CreatedBy);
                                                    parameters.Add("Status", serial.Status);
                                                    parameters.Add("Description", serial.Description);

                                                    db.Execute("USP_I_T_GoodsReceiptLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                                }
                                            }


                                            result.Success = true;
                                            result.Message = key;
                                            tran.Commit();
                                        }
                                        else
                                        {
                                            result.Success = false;
                                            result.Message = "Sloc:" + checkStr  + " not in existed in Store: " + model.StoreId;
                                        }
                                    }
                                    else
                                    {
                                        result.Success = false;
                                        result.Message = "Please check Setup Sloc for Store: " + model.StoreId;
                                    }

                                }
                                else
                                {
                                    result.Success = false;
                                    result.Message = "Sloc Store: " + model.StoreId + " is null. Please check Setup Sloc for Store: " + model.StoreId;
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


        public async Task<GenericResult> CreateByTableType(GoodReceiptViewModel model)
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

                                string GRLineTbl = "T_GoodsReceiptLine";
                                var GRLines = _commonService.CreaDataTable(GRLineTbl);
                                string GRSerialTbl = "T_GoodsReceiptLineSerial";

                                var GRLineSerial = _commonService.CreaDataTable(GRSerialTbl);

                                if (GRLines == null || GRLineSerial == null)
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
                                //string queryGetMoveType = $"select * from M_MovementType with (nolock)  where Code= N'{model.MovementType}'";

                                //var lstX = await db.QueryAsync<MMovementType>("USP_RPT_InvoiceTransactionDetail", null, commandType: CommandType.Text);
                                //if (lstX == null)
                                //{
                                //    result.Success = true;
                                //    result.Message = " Movement Type wrong";
                                //    tran.Rollback();
                                //    return result;
                                //}    
                                //Create and fill-up master table data

                                var listSlocData = await _warehouseService.GetByStore(model.CompanyCode, model.StoreId);
                                if (listSlocData.Success && listSlocData.Data != null)
                                {
                                    var lst = listSlocData.Data as List<MWarehouse>;
                                    if (lst.Count > 0)
                                    {

                                        var SlocInStore = lst.Select(x => x.WhsCode).Distinct();
                                        var SlocInData = model.Lines.Select(x => x.SlocId).Distinct();
                                        string checkStr = ListCheck(SlocInStore, SlocInData);
                                        string keyCache = string.Format(PrefixCacheActionGR, model.StoreId, model.TerminalId);
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
                                        if (string.IsNullOrEmpty(checkStr))
                                        {
                                            string key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('{PrefixGR}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
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
                                            parameters.Add("IsCanceled", model.IsCanceled);
                                            parameters.Add("Remark", model.Remark);
                                            parameters.Add("StoreName", model.StoreName);
                                            parameters.Add("RefId", model.RefId);
                                            parameters.Add("MovementType", model.MovementType);
                                            parameters.Add("ShiftId", model.ShiftId);
                                            parameters.Add("PrefixGR", PrefixGR);
                                          
                                            parameters.Add("IsCanceled", model.IsCanceled);
                                            //var affectedRows = db.Execute("USP_I_T_GoodsReceiptHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                            int stt = 0;
                                            List<TGoodsReceiptLineSerial> lineSerials = new List<TGoodsReceiptLineSerial>();
                                            foreach (var line in model.Lines)
                                            {
                                                stt++;
                                                line.Invtid = key;
                                                line.LineId = stt.ToString();
                                                line.CompanyCode = model.CompanyCode;
                                                line.CreatedBy = model.CreatedBy;


                                                //parameters = new DynamicParameters();
                                                //parameters.Add("INVTId", key, DbType.String);
                                                //parameters.Add("LineId", stt);
                                                //parameters.Add("CompanyCode", model.CompanyCode);
                                                //parameters.Add("ItemCode", line.ItemCode);
                                                //parameters.Add("SLocId", line.SlocId);
                                                //parameters.Add("BarCode", line.BarCode);
                                                //parameters.Add("Uomcode", line.UomCode);
                                                //parameters.Add("Quantity", line.Quantity);
                                                //parameters.Add("Price", line.Price);
                                                //parameters.Add("LineTotal", line.LineTotal);
                                                //parameters.Add("CurrencyCode", line.CurrencyCode);
                                                //parameters.Add("CurrencyRate", line.CurrencyRate);
                                                //parameters.Add("TaxCode", line.TaxCode);
                                                //parameters.Add("TaxRate", line.TaxRate);
                                                //parameters.Add("TaxAmt", line.TaxAmt);
                                                //parameters.Add("Remark", line.Remark);
                                                //parameters.Add("CreatedBy", model.CreatedBy);
                                                //parameters.Add("Status", line.Status);
                                                //parameters.Add("Description", line.Description);

                                                //db.Execute("usp_I_T_GoodsReceiptLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                      

                                                int sttSerial = 0;
                                                foreach (var serial in line.Lines)
                                                {
                                                    sttSerial++;
                                                    serial.Invtid = key;
                                                    serial.LineId = stt.ToString();
                                                    serial.CompanyCode = model.CompanyCode;
                                                    lineSerials.Add(serial);
                                                    //parameters = new DynamicParameters();
                                                    //parameters.Add("INVTId", key);
                                                    //parameters.Add("LineId", stt);
                                                    //parameters.Add("CompanyCode", model.CompanyCode);
                                                    //parameters.Add("ItemCode", serial.ItemCode);
                                                    //parameters.Add("SerialNum", serial.SerialNum);
                                                    //parameters.Add("SLocId", serial.SlocId);
                                                    //parameters.Add("Quantity", serial.Quantity);
                                                    //parameters.Add("UOMCode", serial.UomCode);
                                                    //parameters.Add("CreatedBy", model.CreatedBy);
                                                    //parameters.Add("Status", serial.Status);
                                                    //parameters.Add("Description", serial.Description);

                                                    //db.Execute("USP_I_T_GoodsReceiptLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                                }
                                            }

                                            GRLines = ExtensionsNew.ConvertListToDataTable(model.Lines, GRLines);
                                            GRLineSerial = ExtensionsNew.ConvertListToDataTable(lineSerials, GRLineSerial);
                                            string tblLineType = GRLineTbl + "TableType";
                                            string tblGISerialTbl = GRSerialTbl + "TableType";
                                            parameters.Add("@Lines", GRLines.AsTableValuedParameter(GRLineTbl + "TableType"));
                                            parameters.Add("@LineSerials", GRLineSerial.AsTableValuedParameter(GRSerialTbl + "TableType"));
                                            key = db.ExecuteScalar("USP_I_T_GoodsReceipt", parameters, commandType: CommandType.StoredProcedure, transaction: tran).ToString();
                                            if (model.IsCanceled == "Y")
                                            {
                                                model.IsCanceled = "C";
                                                string updateQry = $"update T_GoodsReceiptHeader set IsCanceled = 'Y', Status='C' where INVTId = '{model.RefId}'and CompanyCode='{model.CompanyCode}'";
                                                db.Execute(updateQry, null, commandType: CommandType.Text, transaction: tran);
                                            }
                                            result.Success = true;
                                            result.Message = key;
                                            tran.Commit();
                                        }
                                        else
                                        {
                                            result.Success = false;
                                            result.Message = "Sloc:" + checkStr + " not in existed in Store: " + model.StoreId;
                                        }
                                    }
                                    else
                                    {
                                        result.Success = false;
                                        result.Message = "Please check Setup Sloc for Store: " + model.StoreId;
                                    }

                                }
                                else
                                {
                                    result.Success = false;
                                    result.Message = "Sloc Store: " + model.StoreId + " is null. Please check Setup Sloc for Store: " + model.StoreId;
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


        public Task<GenericResult> Delete(string companyCode, string storeId, string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string companyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _headerRepository.GetAllAsync($"USP_S_T_GoodsReceiptHeader '{companyCode}', '', ''", null, commandType: CommandType.Text);
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
                var data = await _headerRepository.GetAllAsync($"USP_S_T_GoodsReceiptHeader '{companyCode}', '{storeId}', ''", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetGoodsReceiptList(string CompanyCode, string StoreId, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy)
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
                var data = await _headerRepository.GetAllAsync($"USP_GetGoodsReceipt", parameters, commandType: CommandType.StoredProcedure);
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
                var data = await _headerRepository.GetAsync($"USP_S_T_GoodsReceiptHeader '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);

                if (data == null)
                    return null;
                GoodReceiptViewModel goodReceipt = new GoodReceiptViewModel();
                goodReceipt = _mapper.Map<GoodReceiptViewModel>(data);
                var lines = await _lineRepository.GetAllAsync($"USP_S_T_GoodsReceiptLine '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
              
                var lineData = _mapper.Map<List<GoodReceiptLineViewModel>>(lines);
                goodReceipt.Lines = new List<GoodReceiptLineViewModel>();
                goodReceipt.Lines = lineData;
                foreach (var line in goodReceipt.Lines)
                {
                    var serials = await _lineSerialRepository.GetAllAsync($"USP_S_T_GoodsReceiptLineSerial '{companyCode}', '{storeId}', '{Id}', '{line.LineId}'", null, commandType: CommandType.Text);
                    line.Lines = serials; 
                }
                result.Success = true;
                result.Data = goodReceipt;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }

    

        public async Task<PagedList<TGoodsReceiptHeader>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _headerRepository.GetAllAsync($"select * from T_GoodsReceiptHeader with (nolock) where INVTId like N'%{userParams.keyword}%' ", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.EmployeeId);
                //}
                return await PagedList<TGoodsReceiptHeader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(GoodReceiptViewModel model)
        {
            GenericResult result = new GenericResult();
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
                                parameters.Add("Remark", model.Remark);
                                parameters.Add("RefId", model.RefId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("MovementType", model.MovementType);
                                parameters.Add("ShiftId", model.ShiftId);
                                var affectedRows = db.Execute("USP_U_T_GoodsReceiptHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

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

                                    db.Execute("usp_U_T_GoodsReceiptLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);

                                     
                                    foreach (var serial in line.Lines)
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

                                        db.Execute("USP_U_T_GoodsReceiptLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                } 
                                result.Success = true; result.Message = model.Invtid;
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
