
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
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
    public class PurchaseService : IPurchaseService
    {
        private readonly IGenericRepository<TPurchaseOrderHeader> _poHeaderRepository;
        private readonly IGenericRepository<TPurchaseOrderLine> _poLineRepository;
        private readonly IGenericRepository<TPurchaseOrderLineSerial> _poLineSerialRepository;
        //private readonly IGenericRepository<TPurchaseOrderPayment> _popaymentLineRepository;
        private readonly IGenericRepository<MCustomer> _customerRepository;

        string ServiceName = "T_PurchaseOrder";
        List<string> TableNameList = new List<string>();
        private readonly ICommonService _commonService;

        private string PrefixPO = "";
        IMapper _mapper;
        public PurchaseService(IGenericRepository<TPurchaseOrderHeader> invoiceHeaderRepository, ICommonService commonService, IConfiguration config, IGenericRepository<TPurchaseOrderLineSerial> invoiceLineSerialRepository,
            IGenericRepository<TPurchaseOrderLine> invoiceLineRepository, IGenericRepository<MCustomer> customerRepository, IMapper mapper /*, IHubContext<RequestHub> hubContext IGenericRepository<TPurchaseOrderPayment> invoicepaymentLineRepository,*/
         )//: base(hubContext)
        {
            _poHeaderRepository = invoiceHeaderRepository;
            _poLineRepository = invoiceLineRepository;
            _poLineSerialRepository = invoiceLineSerialRepository;
            _commonService = commonService;
            //_popaymentLineRepository = invoicepaymentLineRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            PrefixPO = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixPO"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            if (string.IsNullOrEmpty(PrefixPO))
            {
                PrefixPO = "PO";
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
        public async Task<GenericResult> UpdateStatus(PurchaseOrderViewModel model)
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

                                string query = $"update T_PurchaseOrderHeader set Status = 'C',  DocStatus = 'C' where CompanyCode='{model.CompanyCode}' and PurchaseId = '{model.PurchaseId}'  and StoreId = '{model.StoreId}'";
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
        public async Task<GenericResult> SavePO(PurchaseOrderViewModel model)
        {
            //return await SavePOByTableType(model);
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
                                var parameters = new DynamicParameters();
                                string key = "";
                                bool isNew = false;
                                if (!string.IsNullOrEmpty(model.PurchaseId))
                                {

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("PurchaseId", model.PurchaseId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_PurchaseOrderHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.PurchaseId;
                                    isNew = false;
                                }
                                else
                                {
                                    key = _poHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixPO}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.PurchaseId = key;
                                    isNew = true;
                                }
                                string itemList = "";


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

                                var affectedRows = db.Execute("USP_I_T_PurchaseOrderHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);


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



                                    db.Execute("usp_I_T_PurchaseOrderLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    //db.Execute("usp_I_T_poLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _poHeaderRepository.GetConnection().InsertAsync<string, TPurchaseOrderLine>(line);
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
                                        line.CreatedOn = DateTime.Now;
                                        line.ModifiedBy = null;
                                        line.ModifiedOn = null;
                                        parameters.Add("CreatedOn", DateTime.Now.ToString("yyyy-MM-dd"));
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                    }
                                    else
                                    {
                                        line.CreatedOn = model.CreatedOn;
                                        line.ModifiedBy = model.ModifiedBy;
                                        line.ModifiedOn = DateTime.Now;
                                        parameters.Add("CreatedOn", model.CreatedOn);
                                        parameters.Add("ModifiedBy", model.ModifiedBy);
                                        parameters.Add("ModifiedOn", DateTime.Now.ToString("yyyy-MM-dd"));
                                    }
                                    parameters.Add("OpenQty", line.OpenQty);

                                    db.Execute("USP_I_T_PurchaseOrderLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //await _poHeaderRepository.GetConnection().InsertAsync<string, TPurchaseOrderLine>(line);
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
        public async Task<GenericResult> SavePOByTableType(PurchaseOrderViewModel model)
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
                                var parameters = new DynamicParameters();
                                string key = "";
                                bool isNew = false;


                                string POLineTbl = ServiceName + "Line";
                                var POLines = _commonService.CreaDataTable(POLineTbl);
                                string POSerialTbl = ServiceName + "LineSerial";
                                var POLineSerial = _commonService.CreaDataTable(POSerialTbl);

                                if (POLines == null || POLineSerial == null)
                                {
                                    result.Success = false;
                                    result.Message = "Table Type Object can't init";
                                    return result;
                                }

                                if (!string.IsNullOrEmpty(model.PurchaseId))
                                {

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("PurchaseId", model.PurchaseId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_PurchaseOrderHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.PurchaseId;
                                    isNew = false;
                                }
                                else
                                {
                                    key = _poHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixPO}','{model.CompanyCode}','{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.PurchaseId = key;
                                    isNew = true;
                                }
                                string itemList = "";


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
                                parameters.Add("PrefixPO", PrefixPO);
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

                                //var affectedRows = db.Execute("USP_I_T_PurchaseOrderHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);


                                int stt = 0;
                                foreach (var line in model.Lines)
                                {
                                    stt++;

                                    line.PurchaseId = key;
                                    line.LineId = stt.ToString();
                                    line.CreatedBy = model.CreatedBy;
                                    line.CompanyCode = model.CompanyCode;


                                    if (isNew)
                                    {
                                        line.CreatedOn = DateTime.Now;
                                        line.ModifiedBy = null;
                                        line.ModifiedOn = null;

                                    }
                                    else
                                    {
                                        line.CreatedOn = model.CreatedOn;
                                        line.ModifiedBy = model.ModifiedBy;
                                        line.ModifiedOn = DateTime.Now;

                                    }

                                }
                                int sttSer = 0;
                                List<TPurchaseOrderLineSerial> lineSerials = new List<TPurchaseOrderLineSerial>();
                                foreach (var line in model.SerialLines)
                                {
                                    sttSer++;
                                    line.PurchaseId = key;
                                    line.LineId = sttSer.ToString();

                                    line.CreatedBy = model.CreatedBy;
                                    line.CompanyCode = model.CompanyCode;

                                    line.Description = line.Description;


                                    if (isNew)
                                    {
                                        line.CreatedOn = DateTime.Now;
                                        line.ModifiedBy = null;
                                        line.ModifiedOn = null;

                                    }
                                    else
                                    {
                                        line.CreatedOn = model.CreatedOn;
                                        line.ModifiedBy = model.ModifiedBy;
                                        line.ModifiedOn = DateTime.Now;
                                    }

                                    lineSerials.Add(line);

                                }

                                POLines = ExtensionsNew.ConvertListToDataTable(model.Lines, POLines);
                                POLineSerial = ExtensionsNew.ConvertListToDataTable(lineSerials, POLineSerial);
                                string tblLineType = POLineTbl + "TableType";
                                string tblGISerialTbl = POSerialTbl + "TableType";

                                parameters.Add("@Lines", POLines.AsTableValuedParameter(POLineTbl + "TableType"));
                                parameters.Add("@LineSerials", POLineSerial.AsTableValuedParameter(POSerialTbl + "TableType"));

                                key = db.ExecuteScalar("USP_I_T_PurchaseOrder", parameters, commandType: CommandType.StoredProcedure, transaction: tran).ToString();


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
            //    string query = $"select * from T_PurchaseOrderHeader with (nolock) where 1=1 ";
            //    if (!string.IsNullOrEmpty(companycode) && companycode != "null")
            //    {
            //        query += $" and companycode = N'{companycode}' ";
            //    }

            //    if (!string.IsNullOrEmpty(storeId) && storeId != "null")
            //    {
            //        query += $" and storeId = N'{storeId}' ";
            //    }

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

                var data = await _poHeaderRepository.GetAllAsync($"USP_GetPurchaseOder", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<List<TPurchaseOrderHeader>> GetAll()
        {
            var lst = await _poHeaderRepository.GetAllAsync("select * from T_poHeader with (nolock) order by CreatedOn desc", null, commandType: CommandType.Text);
            return lst;
            //throw new NotImplementedException();
        }

        public Task<TPurchaseOrderHeader> GetById(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<PurchaseOrderViewModel> GetOrderById(string Id, string CompanyCode, string StoreId)
        {
            try
            {
                PurchaseOrderViewModel order = new PurchaseOrderViewModel();

                TPurchaseOrderHeader header = await _poHeaderRepository.GetAsync($"select * from T_PurchaseOrderHeader with (nolock) where PurchaseId='{Id}' and CompanyCode= '{CompanyCode}' and StoreId= '{StoreId}'", null, commandType: CommandType.Text);

                string queryLine = $"select t1.*  from T_PurchaseOrderLine t1 with(nolock)  where t1.PurchaseId = '{Id}' and t1.CompanyCode = '{CompanyCode}'";
                string queryLineSerial = $"select t1.*   from T_PurchaseOrderLineSerial t1 with(nolock)   where t1.PurchaseId = '{Id}' and t1.CompanyCode = '{CompanyCode}'";

                //List<TPurchaseOrderLine> lines = await _poLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                //List<TPurchaseOrderPayment> payments = await _popaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                //var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId ='{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<PurchaseOrderViewModel>(header);
                using (IDbConnection db = _poHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TPurchaseOrderLine>(queryLine, null, commandType: CommandType.Text);
                        var serialLines = db.Query<TPurchaseOrderLineSerial>(queryLineSerial, null, commandType: CommandType.Text);
                        foreach (var line in lines)
                        {
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                        }
                        order = _mapper.Map<PurchaseOrderViewModel>(header);
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


        public Task<TPurchaseOrderHeader> GetByUser(string User)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TPurchaseOrderLine>> GetLinesById(string Id)
        {
            var data = await _poLineRepository.GetAllAsync($"select * from T_poLine with (nolock) where PurchaseId = N'%{Id}%'", null, commandType: CommandType.Text);
            return data;
        }

        public async Task<string> GetNewOrderCode(string companyCode, string storeId)
        {
            string key = _poHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixPO}','{companyCode}','{storeId}')", null, commandType: CommandType.Text);
            return key;
        }
        public async Task<string> GetLastPricePO(string companyCode, string storeId, string ItemCode, string UomCode, string Barcode)
        {
            string query = $"select top 1 Price from T_PurchaseOrderLine where ItemCode = '{ItemCode}' and UOMCode ='{UomCode}' and CompanyCode= '{companyCode}' order by CreatedOn desc";
            string key = _poHeaderRepository.GetScalar(query, null, commandType: CommandType.Text);
            return key;
        }

        public async Task<PagedList<TPurchaseOrderHeader>> GetPagedList(UserParams userParams)
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
                return await PagedList<TPurchaseOrderHeader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



    }

}
