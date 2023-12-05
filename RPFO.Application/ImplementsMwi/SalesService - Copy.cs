using AutoMapper;
using Dapper;
using RPFO.Application.InterfacesMwi;
using RPFO.Data.EntitiesMWI;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RPFO.Application.ImplementsMwi
{
    public class SalesService : ISalesService
    {
        private readonly IGenericRepository<TSalesHeader> _salesHeaderRepository;
        private readonly IGenericRepository<TSalesLine> _salesLinesRepository;
        private readonly IMapper _mapper;

        public SalesService(IMapper mapper, IGenericRepository<TSalesHeader> salesHeaderRepository, IGenericRepository<TSalesLine> salesLinesRepository)
        {
            this._mapper = mapper;
            this._salesHeaderRepository = salesHeaderRepository;
            this._salesLinesRepository = salesLinesRepository;
        }

        public List<TSalesViewModel> GetSales(string companyCode, string transId)
        {
            List<TSalesViewModel> items = new List<TSalesViewModel>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("TransId", transId);
                var headers = _salesHeaderRepository.GetAll("USP_S_T_SalesHeader", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
                if (headers.Count > 0)
                {
                    foreach (TSalesHeader head in headers)
                    {
                        TSalesViewModel viewModel = _mapper.Map<TSalesViewModel>(head);
                        parameters = new DynamicParameters();
                        parameters.Add("TransId", viewModel.PostransId);
                        var lines = _salesLinesRepository.GetAll("USP_S_T_SalesLines", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
                        viewModel.SalesLines = lines;
                        items.Add(viewModel);
                    }
                }
            }
            catch { }

            return items;
        }

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
                            parameters.Add("DocRate", sales.DocRate);
                            parameters.Add("DiscPrcnt", sales.DiscPrcnt);
                            parameters.Add("DiscSum", sales.DiscSum);
                            parameters.Add("VatSum", sales.VatSum);
                            parameters.Add("DocTotal", sales.DocTotal);

                            var affectedRows = db.Execute("USP_I_T_SalesHeader", parameters, transaction: tran, commandType: CommandType.StoredProcedure);
                            //if (affectedRows > 0)
                            //{
                            if (sales.SalesLines != null && sales.SalesLines.Count > 0)
                            {
                                foreach (TSalesLine line in sales.SalesLines)
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

            return transId;
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
    }
}
