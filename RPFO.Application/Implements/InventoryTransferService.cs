
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
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
    public class InventoryTransferService : IInventoryTransferService
    {
        private readonly IGenericRepository<TInventoryTransferHeader> _headerRepository;
        private readonly IGenericRepository<TInventoryTransferLine> _lineRepository;
        private readonly IGenericRepository<TInventoryTransferLineSerial> _lineSerialRepository;
        private readonly IMapper _mapper;
        public InventoryTransferService(IGenericRepository<TInventoryTransferHeader> goodreceiptRepository, IGenericRepository<TInventoryTransferLine> goodreceiptLineRepository,
             IGenericRepository<TInventoryTransferLineSerial> lineSerialRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _headerRepository = goodreceiptRepository;
            _lineRepository = goodreceiptLineRepository;
            _lineSerialRepository = lineSerialRepository;

            _mapper = mapper;

        }
        public class ResultModel
        {
            public int ID { get; set; }
            public string Message { get; set; }
        }
        public async Task<GenericResult> Create(InventoryTransferViewModel model)
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

            if (model.FromSloc == null || model.ToSloc == null)
            {
                result.Success = false;
                result.Message = "From Sloc / To Sloc not null.";
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
                                string VirtualWhs = $"select   dbo.[fnc_GetVirtualWhs]( '{model.CompanyCode}','{model.StoreId}')";
                                string geWhs = _headerRepository.GetScalar(VirtualWhs, null, commandType: CommandType.Text);
                                if (string.IsNullOrEmpty(geWhs))
                                {
                                    result.Success = false;
                                    result.Message = $"Can't found virtual warehouse ({model.FromSlocName})";
                                    return result;
                                }
                                string itemList = "";
                                List<ItemCheckModel> listItemCheck = new List<ItemCheckModel>();
                                if (model.DocType == "S")
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
                                if (model.DocType == "R")
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
                                //Create and fill-up master table data
                                string key = _headerRepository.GetScalar($" select   dbo.fnc_AutoGenDocumentCode('IVT','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
                                model.InvtTransid = key;
                                var parameters = new DynamicParameters();
                                parameters.Add("InvtTransid", model.InvtTransid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("DocType", model.DocType);
                                parameters.Add("RefINVTId", model.RefInvtid);
                                parameters.Add("DocDate", model.DocDate);
                                parameters.Add("DocDueDate", model.DocDueDate);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("FromSloc", model.FromSloc);
                                parameters.Add("FromSlocName", model.FromSlocName);
                                parameters.Add("ToSloc", model.ToSloc);
                                parameters.Add("ToSlocName", model.ToSlocName);
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
                                    string updateQry = $"update T_InventoryTransferHeader set IsCanceled = 'Y', Status='C' where INVTTransId = '{model.RefId}'and CompanyCode='{model.CompanyCode}'";
                                    db.Execute(updateQry, null, commandType: CommandType.Text, transaction: tran);
                                }
                                parameters.Add("IsCanceled", model.IsCanceled);
                                var affectedRows = db.Execute("USP_I_T_InventoryTransferHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    string Whs = model.TransitWhs;
                                    if (model.DocType == "S")
                                    {
                                        line.OpenQty = line.Quantity;
                                        Whs = line.FrSlocId;
                                    }

                                    parameters = new DynamicParameters();
                                    parameters.Add("InvtTransid", model.InvtTransid, DbType.String);
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
                                    checkparameters.Add("StoreId", model.StoreId);
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
                                    db.Execute("usp_I_T_InventoryTransferLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);

                                    int sttSerial = 0;
                                    foreach (var serial in line.Lines)
                                    {
                                        sttSerial++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("InvtTransid", key);
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
                                        checkparameters.Add("StoreId", model.StoreId);
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
                                        db.Execute("USP_I_T_InventoryTransferLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                    }
                                }
                                if (model.DocType == "R")
                                {
                                    string updateHeader = $"USP_Update_Status_SalesTransHeader '{model.RefInvtid}', '{model.CompanyCode}'";
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
                var data = await _headerRepository.GetAllAsync($"select * from T_InventoryTransferHeader with (nolock) where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetInventoryList(string CompanyCode, string StoreId, string FromSloc, string ToSloc, string DocType, string Status, DateTime? FrDate, DateTime? ToDate, string Keyword, string ViewBy)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("FromSloc", FromSloc);
                parameters.Add("ToSloc", ToSloc);
                parameters.Add("DocType", DocType);
                parameters.Add("Status", Status);
                parameters.Add("FrDate", FrDate);
                parameters.Add("ToDate", ToDate);
                parameters.Add("Keyword", Keyword);
                parameters.Add("ViewBy", ViewBy);

                var data = await _headerRepository.GetAllAsync($"USP_GetInventoryTransferList", parameters, commandType: CommandType.StoredProcedure);
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
                var data = await _headerRepository.GetAsync($"USP_S_T_InventoryTransferHeader '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
                InventoryTransferViewModel transfer = new InventoryTransferViewModel();
                transfer = _mapper.Map<InventoryTransferViewModel>(data);
                var lines = await _lineRepository.GetAllAsync($"USP_S_T_InventoryTransferLine '{companyCode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);

                var lineData = _mapper.Map<List<InventoryTransferLineViewModel>>(lines);
                transfer.Lines = lineData;
                foreach (var line in transfer.Lines)
                {
                    var serials = await _lineSerialRepository.GetAllAsync($"USP_S_T_InventoryTransferLineSerial '{companyCode}', '{storeId}', '{Id}', '{line.LineId}'", null, commandType: CommandType.Text);
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



        public async Task<PagedList<TInventoryTransferHeader>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _headerRepository.GetAllAsync($"select * from T_InventoryTransferHeader with (nolock) where INVTTransId like N'%{userParams.keyword}%' ", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.EmployeeId);
                //}
                return await PagedList<TInventoryTransferHeader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(InventoryTransferViewModel model)
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
            if (model.FromSloc == null || model.ToSloc.Count() == 0)
            {
                result.Success = false;
                result.Message = "From Sloc / To Sloc not null.";
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
                              
                                string VirtualWhs = $"select   dbo.[fnc_GetVirtualWhs]( '{model.CompanyCode}','{model.StoreId}')";
                                string geWhs = _headerRepository.GetScalar(VirtualWhs, null, commandType: CommandType.Text);
                                if (string.IsNullOrEmpty(geWhs))
                                {
                                    result.Success = false;
                                    result.Message = $"Can't found virtual warehouse ({model.FromSlocName})";
                                    return result;
                                }
                                
                                var parameters = new DynamicParameters();
                                parameters.Add("InvtTransid", model.InvtTransid, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("DocType", model.DocType);
                                parameters.Add("RefINVTId", model.RefInvtid);
                                parameters.Add("ModifiedBy", model.ModifiedBy);
                                parameters.Add("Status", model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("DocDate", model.DocDate == null ? null : model.DocDate.Value.ToString("yyyy-MM-dd"));
                                parameters.Add("DocDueDate", model.DocDueDate == null ? null : model.DocDueDate.Value.ToString("yyyy-MM-dd"));
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("FromSloc", model.FromSloc);
                                parameters.Add("FromSlocName", model.FromSlocName);
                                parameters.Add("ToSloc", model.ToSloc);
                                parameters.Add("ToSlocName", model.ToSlocName);
                                parameters.Add("Name", model.Name);
                                parameters.Add("Remark", model.Remark);
                                parameters.Add("RefId", model.RefId);
                                parameters.Add("TransitWhs", model.TransitWhs);
                                parameters.Add("FromWhs", model.FromWhs);
                                parameters.Add("ToWhs", model.ToWhs); 
                                parameters.Add("ShiftId", model.ShiftId);
                                string DocDate = model.DocDate == null ? "null" : model.DocDate.Value.ToString("yyyy-MM-dd");
                                string DocDueDate = model.DocDueDate == null ? "null" : model.DocDueDate.Value.ToString("yyyy-MM-dd");
                               
                                var affectedRows = db.Execute("USP_U_T_InventoryTransferHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                var deletParameters = new DynamicParameters();
                                deletParameters.Add("InvtTransid", model.InvtTransid);
                                deletParameters.Add("CompanyCode", model.CompanyCode);

                                var removeLine = db.Execute("USP_D_T_InventoryTransferLineAndSerialLine", deletParameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                foreach (var line in model.Lines)
                                {
                                    //stt++;
                                    string ShipDate = line.ShipDate == null ? null : line.ShipDate.Value.ToString("yyyy-MM-dd");
                                    string Quantity = line.Quantity == null ? null : line.Quantity.Value.ToString();
                                    string OpenQty = line.OpenQty == null ? null : line.OpenQty.Value.ToString();
                                    string Price = line.Price == null ? null : line.Price.Value.ToString();
                                    decimal LineTotal = decimal.Parse(Quantity) * decimal.Parse(Price);

                                    parameters = new DynamicParameters();
                                    parameters.Add("INVTTransId", model.InvtTransid, DbType.String);
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("LineId", line.LineId);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("FrSlocId", line.FrSlocId);
                                    parameters.Add("ToSlocId", line.ToSlocId);
                                    parameters.Add("DocType", line.DocType);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Description", line.Description);
                                    parameters.Add("UOMCode", line.UomCode);
                                    parameters.Add("Quantity", Quantity);
                                    parameters.Add("ShipDate", ShipDate);
                                    parameters.Add("OpenQty", OpenQty);
                                    parameters.Add("Price", Price);
                                    parameters.Add("LineTotal", LineTotal);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    parameters.Add("Status", line.Status);
                                    parameters.Add("BaseTransId", line.BaseTransId);
                                    parameters.Add("BaseLine", line.BaseLine);


                                    string queryLine = $"usp_U_T_InventoryTransferLine '{model.InvtTransid}','{model.CompanyCode}','{line.LineId}','{line.ItemCode}'" +
                                        $",'{line.FrSlocId}','{line.ToSlocId}','{line.DocType}','{line.BarCode}','{line.Description}','{line.UomCode}'" +
                                        $",'{Quantity}','{ShipDate}','{OpenQty}','{Price}','{LineTotal}','{line.ModifiedBy}','{line.Status}'";


                                    var affectedRowsLine = db.Execute("usp_I_T_InventoryTransferLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    //var affectedRowsLineA = db.Execute(queryLine, null, commandType: CommandType.Text, transaction: tran);

                                    //await _saleHeaderRepository.GetConnection().InsertAsync<string, TSalesLine>(line);


                                    foreach (var serial in line.Lines)
                                    {

                                        parameters = new DynamicParameters();
                                        parameters.Add("INVTTransId", model.InvtTransid);
                                        parameters.Add("LineId", serial.LineId);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", serial.ItemCode);
                                        parameters.Add("SerialNum", serial.SerialNum);
                                        parameters.Add("FrSlocId", line.FrSlocId);
                                        parameters.Add("ToSlocId", line.ToSlocId);
                                        parameters.Add("Quantity", serial.Quantity);
                                        parameters.Add("UOMCode", serial.UomCode);
                                        //parameters.Add("ModifiedBy", serial.ModifiedBy);
                                        parameters.Add("CreatedBy", model.CreatedBy);
                                        parameters.Add("Status", serial.Status);
                                        parameters.Add("Description", line.Description);
                                        db.Execute("USP_I_T_InventoryTransferLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
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
