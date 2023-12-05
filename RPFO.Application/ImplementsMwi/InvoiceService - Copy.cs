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
    public class InvoiceService : IInvoiceService
    {
        private readonly IGenericRepository<TInvoiceHeader> _invoiceHeaderRepository;
        private readonly IGenericRepository<TInvoiceLine> _invoiceLinesRepository;
        private readonly IMapper _mapper;

        public InvoiceService(IMapper mapper, IGenericRepository<TInvoiceHeader> invoiceHeaderRepository, IGenericRepository<TInvoiceLine> invoiceLinesRepository)
        {
            this._mapper = mapper;
            this._invoiceHeaderRepository = invoiceHeaderRepository;
            this._invoiceLinesRepository = invoiceLinesRepository;
        }

        public List<TInvoiceViewModel> GetInvoices(string companyCode, string transId)
        {
            List<TInvoiceViewModel> items = new List<TInvoiceViewModel>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("TransId", transId);
                var headers = _invoiceHeaderRepository.GetAll("USP_S_T_InvoiceHeader", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
                if (headers.Count > 0)
                {
                    foreach (TInvoiceHeader head in headers)
                    {
                        TInvoiceViewModel viewModel = _mapper.Map<TInvoiceViewModel>(head);
                        parameters = new DynamicParameters();
                        parameters.Add("TransId", viewModel.PostransId);
                        var lines = _invoiceLinesRepository.GetAll("USP_S_T_InvoiceLines", parameters, CommandType.StoredProcedure, GConnection.MwiConnection);
                        viewModel.InvoiceLines = lines;
                        items.Add(viewModel);
                    }
                }
            }
            catch { }

            return items;
        }

        public string CreateInvoice(TInvoiceViewModel invoice, out string msg)
        {
            msg = "";
            string transId = "";
            using (IDbConnection db = _invoiceHeaderRepository.GetConnection(GConnection.MwiConnection))
            {
                try
                {
                    //  get trans id
                    var objId = db.ExecuteScalar($"SELECT dbo.fnc_AutoGenDocumentCode('IN', '{invoice.CompanyCode}', '{invoice.StoreId}')", commandType: CommandType.Text);
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
                            parameters.Add("CompanyCode", invoice.CompanyCode);
                            parameters.Add("StoreId", invoice.StoreId);
                            parameters.Add("DocEntry", invoice.DocEntry);
                            parameters.Add("ContractNo", invoice.ContractNo);
                            parameters.Add("DocStatus", invoice.DocStatus);
                            parameters.Add("CardCode", invoice.CardCode);
                            parameters.Add("CardName", invoice.CardName);
                            parameters.Add("DocCur", invoice.DocCur);
                            parameters.Add("DocRate", invoice.DocRate);
                            parameters.Add("DiscPrcnt", invoice.DiscPrcnt);
                            parameters.Add("DiscSum", invoice.DiscSum);
                            parameters.Add("VatSum", invoice.VatSum);
                            parameters.Add("DocTotal", invoice.DocTotal);

                            var affectedRows = db.Execute("USP_I_T_InvoiceHeader", parameters, transaction: tran, commandType: CommandType.StoredProcedure);
                            //if (affectedRows > 0)
                            //{
                            if (invoice.InvoiceLines != null && invoice.InvoiceLines.Count > 0)
                            {
                                foreach (TInvoiceLine line in invoice.InvoiceLines)
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

                                    var affectLine = db.Execute("USP_I_T_InvoiceLines", parameters, transaction: tran, commandType: CommandType.StoredProcedure);
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
    }
}
