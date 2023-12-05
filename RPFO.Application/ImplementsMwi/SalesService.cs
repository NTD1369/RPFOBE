using AutoMapper;
using Dapper;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.Entities;
using RPFO.Data.EntitiesMWI;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static RPFO.Application.Implements.CommonService;

namespace RPFO.Application.ImplementsMwi
{
    public class SalesService : ISalesService
    {
        private readonly IGenericRepository<Data.EntitiesMWI.TSalesHeader> _salesHeaderRepository;
        private readonly IGenericRepository<Data.EntitiesMWI.TSalesLine> _salesLinesRepository;
        private readonly IMapper _mapper;

        public SalesService(IMapper mapper, IGenericRepository<Data.EntitiesMWI.TSalesHeader> salesHeaderRepository,
            IGenericRepository<Data.EntitiesMWI.TSalesLine> salesLinesRepository)
        {
            //_salesHeaderRepository = salesHeaderRepository;
            _mapper = mapper;
            _salesHeaderRepository = salesHeaderRepository;
            _salesLinesRepository = salesLinesRepository;
        }

        //public List<TSalesViewModel> GetSales(string companyCode, string transId)
        //{
        //    List<TSalesViewModel> items = new List<TSalesViewModel>();
        //    try
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("CompanyCode", companyCode);
        //        parameters.Add("TransId", transId);
        //        var headers = _salesHeaderRepository.GetAll("USP_S_T_SalesHeader", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
        //        if (headers.Count > 0)
        //        {
        //            foreach (TSalesHeader head in headers)
        //            {
        //                TSalesViewModel viewModel = _mapper.Map<TSalesViewModel>(head);
        //                parameters = new DynamicParameters();
        //                parameters.Add("TransId", viewModel.PostransId);
        //                var lines = _salesLinesRepository.GetAll("USP_S_T_SalesLines", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
        //                viewModel.SalesLines = lines;
        //                items.Add(viewModel);
        //            }
        //        }
        //    }
        //    catch { }

        //    return items;
        //}

        public string CreateSales(TSalesViewModel sales, out string msg)
        {
            msg = "";
            string transId = "";
            using (IDbConnection db = _salesHeaderRepository.GetConnection(GConnection.MwiConnection))
            {
                try
                {
                    //  get trans id
                    var objId = db.ExecuteScalar($"SELECT dbo.fnc_AutoGenDocumentCode('SO', '{sales.CompanyCode}', '{sales.StoreId}')", commandType: CommandType.Text);
                    if (objId != null)
                    {
                        transId = objId.ToString();
                    }
                    else
                    {
                        msg += "Could not initialize the ID for the document, please try again! ";
                        return "";
                    }

                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            string discPr = sales.DiscPrcnt == null ? "null" : sales.DiscPrcnt.ToString();
                            string DiscSum = sales.DiscSum == null ? "null" : sales.DiscSum.ToString();
                            string VatSum = sales.VatSum == null ? "null" : sales.VatSum.ToString();
                            string DocTotal = sales.DocTotal == null ? "null" : sales.DocTotal.ToString();
                            string DocRate = sales.DocRate == null ? "null" : sales.DocRate.ToString();

                            var parameters = new DynamicParameters();
                            parameters.Add("POSTransId", transId);
                            parameters.Add("CompanyCode", sales.CompanyCode);
                            parameters.Add("StoreId", sales.StoreId);
                            parameters.Add("ContractNo", sales.ContractNo);
                            parameters.Add("DocEntry", sales.DocEntry);
                            parameters.Add("DocStatus", sales.DocStatus);
                            parameters.Add("CardCode", sales.CardCode);
                            parameters.Add("CardName", sales.CardName);
                            parameters.Add("DocCur", sales.DocCur);
                            parameters.Add("DocRate", 0);
                            parameters.Add("DiscPrcnt", 0);
                            parameters.Add("DiscSum", 0);
                            parameters.Add("VatSum", 0);
                            parameters.Add("DocTotal", 0);
                            parameters.Add("DocType", sales.DocType);
                            string query = $"USP_I_T_SalesHeader '{transId}','{sales.CompanyCode}','{sales.StoreId}','{sales.ContractNo}','{sales.DocEntry}','{sales.DocStatus}'," +
                                $"'{sales.CardCode}','{sales.CardName}','{sales.DocCur}',{DocRate},{discPr},{DiscSum},{VatSum},{DocTotal},'{sales.DocType}'";
                            var affected = db.Execute(query, null, transaction: tran, commandType: CommandType.Text);

                            //var affectedRows = db.Execute("USP_I_T_SalesHeader", parameters, transaction: tran, commandType: CommandType.StoredProcedure);
                            //if (affectedRows > 0)
                            //{
                            if (sales.SalesLines != null && sales.SalesLines.Count > 0)
                            {
                                foreach (Data.EntitiesMWI.TSalesLine line in sales.SalesLines)
                                {

                                    parameters = new DynamicParameters();
                                    parameters.Add("POSTransId", transId);
                                    parameters.Add("LineId", line.LineId);
                                    parameters.Add("LineStatus", line.LineStatus);
                                    parameters.Add("ItemCode", line.ItemCode);
                                    parameters.Add("UOMCode", line.UomCode);
                                    parameters.Add("BarCode", line.BarCode);
                                    parameters.Add("Quantity", line.Quantity);
                                    parameters.Add("Price", line.Price);
                                    parameters.Add("WhsCode", line.WhsCode);
                                    parameters.Add("DiscPrcnt", line.DiscPrcnt);
                                    parameters.Add("DiscSum", line.DiscSum);
                                    parameters.Add("TaxCode", line.TaxCode);
                                    parameters.Add("TaxRate", line.TaxRate);
                                    parameters.Add("TaxAmt", line.TaxAmt);
                                    parameters.Add("Remark", line.Remark);
                                    parameters.Add("PromoId", line.PromoId);
                                    parameters.Add("PromoType", line.PromoType);
                                    parameters.Add("LineTotal", line.LineTotal);

                                    var affectLine = db.Execute("USP_I_T_SalesLines", parameters, transaction: tran, commandType: CommandType.StoredProcedure);
                                    //if (affectLine <= 0)
                                    //{
                                    //    msg += $"Insert line data error. LineId: '{line.LineId}'; ItemCode: '{line.ItemCode}'. ";
                                    //    tran.Rollback();
                                    //    return "";
                                    //}
                                }
                            }
                            //}
                            //else
                            //{
                            //    tran.Rollback();
                            //    msg += "Could not add header data, please try again! ";
                            //    return "";
                            //}
                            msg = transId;
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
                    msg += "Exception: " + ex.Message;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }

            return msg;
        }

        public bool UpdateDocStatus(string companyCode, string transId, string status)
        {
            bool result = false;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("TransId", transId);
                parameters.Add("Status", status);
                var res = _salesHeaderRepository.Execute("USP_U_T_SalesOrder", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
                result = res >= 0;
            }
            catch { }
            return result;
        }

        public bool CancelSalesOrder(string companyCode, string transId, string remark)
        {
            bool result = false;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("TransId", transId);
                parameters.Add("Remark", remark);
                var res = _salesHeaderRepository.Execute("USP_U_T_CancelSalesOrder", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
                result = res >= 0;
            }
            catch { }
            return result;
        }

        public bool RpfoWriteLog(SaleViewModel orderModel, string type, string voucherType, string status, out string message)
        {
            message = string.Empty;
            bool result = false;
            try
            {
                orderModel.Logs = new List<OrderLogModel>();

                string transId = Guid.NewGuid().ToString();

                TSalesRedeemVoucher voucher = orderModel.Vouchers[0];

                OrderLogModel RowRequest = new OrderLogModel
                {
                    Type = type,
                    Action = "Request",
                    Result = "",
                    Value = voucher.VoucherCode,
                    CustomF1 = "",
                    CustomF2 = "",
                    CustomF3 = "",
                    CustomF4 = "",
                    CustomF5 = "",
                    CustomF6 = "",
                    CustomF7 = "",
                    CustomF8 = "",
                    CustomF9 = "",
                    CustomF10 = "",
                    CreatedBy = orderModel.CreatedBy,
                    Time = DateTime.Now,
                    StoreId = orderModel.StoreId,
                    CompanyCode = orderModel.CompanyCode,
                    TerminalId = orderModel.TerminalId,
                    TransId = transId
                };


                orderModel.Logs.Add(RowRequest);

                OrderLogModel RowRedeem = new OrderLogModel
                {
                    Type = type,
                    Action = voucherType,
                    Result = status,
                    Value = voucher.VoucherCode,
                    CustomF1 = voucher.Name,
                    CustomF2 = orderModel.CusId,
                    CustomF3 = orderModel.StoreId,
                    CustomF4 = "",
                    CustomF5 = orderModel.Phone,
                    CustomF6 = orderModel.CusName,
                    CustomF7 = "",
                    CustomF8 = "",
                    CustomF9 = "",
                    CustomF10 = "",
                    CreatedBy = orderModel.CreatedBy,
                    Time = DateTime.Now,
                    StoreId = orderModel.StoreId,
                    CompanyCode = orderModel.CompanyCode,
                    TransId = transId,
                    TerminalId = orderModel.TerminalId
                };

                orderModel.Logs.Add(RowRedeem);

                using (IDbConnection db = _salesHeaderRepository.GetConnection(GConnection.CentralConnection))
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {
                                result = WriteLog(orderModel, out string msg);
                                if (!result)
                                {
                                    message = msg;
                                }
                            }
                            catch (Exception ex)
                            {
                                result = false;
                                message = ex.Message;
                                tran.Rollback();
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        message = ex.Message;
                        throw ex;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                }
            }
            catch(Exception ex )
            {
                result = false;
                message = ex.Message;
            }

            return result;
        }

        public bool WriteLog(SaleViewModel model, out string message)
        {
            message = string.Empty;
            bool result;
            try
            {
                int lineNum = 0;
                var logs = model.Logs;
                string LogsTbl = "S_Log";

                //var Logs = _commonService.CreaDataTable(LogsTbl);
                DataTable Logs = null;
                using (IDbConnection db = _salesHeaderRepository.GetConnection(GConnection.CentralConnection))
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();

                        string getTable = $"select * from dbo.[fn_GetTableInfor]('{LogsTbl}' , '1')";
                        var items = db.Query<ObjectTableInfor>(getTable, null, commandType: CommandType.Text, commandTimeout: 3600);

                        var GILines = new DataTable(LogsTbl + "TableType");
                        foreach (var item in items)
                        {
                            Type type = typeof(string);
                            if (item.ColumnType.ToLower() == "string") type = typeof(string);
                            if (item.ColumnType.ToLower() == "long") type = typeof(long);
                            if (item.ColumnType.ToLower() == "byte[]") type = typeof(byte[]);
                            if (item.ColumnType.ToLower() == "bool") type = typeof(bool);
                            if (item.ColumnType.ToLower() == "datetime") type = typeof(DateTime);
                            if (item.ColumnType.ToLower() == "datetimeoffset") type = typeof(DateTimeOffset);
                            if (item.ColumnType.ToLower() == "decimal") type = typeof(decimal);
                            if (item.ColumnType.ToLower() == "double") type = typeof(double);
                            if (item.ColumnType.ToLower() == "int") type = typeof(int);
                            if (item.ColumnType.ToLower() == "float") type = typeof(float);
                            if (item.ColumnType.ToLower() == "short") type = typeof(short);
                            if (item.ColumnType.ToLower() == "timespan") type = typeof(TimeSpan);
                            if (item.ColumnType.ToLower() == "byte") type = typeof(byte);
                            if (item.ColumnType.ToLower() == "guid") type = typeof(Guid);

                            GILines.Columns.Add(item.ColumnName, type);
                        }
                        Logs = GILines;
                    }
                    catch (Exception ex)
                    {
                        Logs = null;
                        message = "Create Datatable Exception: " + ex.Message;
                        result = false;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                }

                if (logs != null && logs.Count > 0)
                {
                    foreach (var line in logs)
                    {
                        line.LineNum = lineNum++;
                        line.Id = Guid.NewGuid();
                        line.TerminalId = model.TerminalId;
                        line.CompanyCode = model.CompanyCode;
                        line.StoreId = model.StoreId;
                        line.TransId = model.TransId;

                    }
                    Logs = ExtensionsNew.ConvertListToDataTable(logs, Logs);
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", model.CompanyCode);
                    parameters.Add("StoreId", model.StoreId);
                    parameters.Add("TransId", model.TransId);
                    parameters.Add("TerminalId", model.TerminalId);
                    parameters.Add("CreatedBy", model.CreatedBy);
                    parameters.Add("Lines", Logs.AsTableValuedParameter(LogsTbl + "TableType"));

                    using (IDbConnection db = _salesHeaderRepository.GetConnection(GConnection.CentralConnection))
                    {
                        db.Execute("USP_I_S_LogByTableType", parameters, commandType: CommandType.StoredProcedure);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                message = "WriteLog Exception: " + ex.Message;
                result = false;
            }

            return result;
        }
    }
}
