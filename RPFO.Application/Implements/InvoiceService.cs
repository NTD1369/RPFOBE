
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
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
    public class InvoiceService : IInvoiceService
    {
        private readonly IGenericRepository<TInvoiceHeader> _invoiceHeaderRepository;
        private readonly IGenericRepository<TInvoiceLine> _invoiceLineRepository;
        private readonly IGenericRepository<TInvoicePayment> _invoicepaymentLineRepository;
        private readonly IGenericRepository<MCustomer> _customerRepository;


        private readonly string PrefixSO = "";
        private readonly string PrefixAR = "";
        IMapper _mapper;
        public InvoiceService(IGenericRepository<TInvoiceHeader> invoiceHeaderRepository, IConfiguration config, IGenericRepository<TInvoicePayment> invoicepaymentLineRepository,
            IGenericRepository<TInvoiceLine> invoiceLineRepository, IGenericRepository<MCustomer> customerRepository, IMapper mapper /*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _invoiceHeaderRepository = invoiceHeaderRepository;
            _invoiceLineRepository = invoiceLineRepository;
            _invoicepaymentLineRepository = invoicepaymentLineRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            PrefixSO = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixSO"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            PrefixAR = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixAR"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);

            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public Task<GenericResult> Create(TInvoiceHeader model)
        {
            throw new NotImplementedException();
        }
        public class ResultModel
        {
            public int ID { get; set; }
            public string Message { get; set; }
        }
        public string StringToBinary(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        public async Task<GenericResult> SaveImage(InvoiceViewModel model)
        {
            GenericResult result = new GenericResult();

          
            try
            {

                //if (model.Payments.Count == 0)
                //{
                //    result.Success = false;
                //    result.Message = "Payment list not null.";
                //    return result;
                //}
                using (IDbConnection db = _invoiceHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {


 
                                if (!string.IsNullOrEmpty(model.Image))
                                {
                                    MImage image = new MImage();
                                    image.Id = Guid.NewGuid();
                                    image.CompanyCode = model.CompanyCode;
                                    image.Description = model.TransId;
                                    image.Num = model.TransId;
                                    image.Type = "CheckIn";
                                    image.Image = model.Image;//StringToBinary(
                                    image.CreateOn = DateTime.Now;
                                    image.CustomerPhone = model.Phone;
                                    image.CustomerName = model.CusName;

                                    var parametersImg = new DynamicParameters();

                                    parametersImg.Add("Id", image.Id);
                                    parametersImg.Add("CompanyCode", model.CompanyCode);
                                    parametersImg.Add("Description", image.Description);
                                    parametersImg.Add("Num", image.Num);
                                    parametersImg.Add("Type", image.Type);
                                    parametersImg.Add("Image", image.Image);
                                    parametersImg.Add("CreateOn", image.CreateOn);
                                    parametersImg.Add("CustomerPhone", image.CustomerPhone);
                                    parametersImg.Add("CustomerName", image.CustomerName);

                                    db.Execute("USP_I_M_Image", parametersImg, commandType: CommandType.StoredProcedure, transaction: tran);
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
            //throw new NotImplementedException();
        }
      
        public async Task<GenericResult> CreateInvoice(InvoiceViewModel model)
        {
            GenericResult result = new GenericResult();

            if (model.Lines == null || model.Lines.Count() == 0)
            {
                result.Success = false;
                result.Message = "Doc line not null.";
                return result;
            }
            if (string.IsNullOrEmpty(model.StoreId))
            {
                result.Success = false;
                result.Message = "From Store / To Store not null.";
                return result;
            }

            try
            {

                //if (model.Payments.Count == 0)
                //{
                //    result.Success = false;
                //    result.Message = "Payment list not null.";
                //    return result;
                //}
                using (IDbConnection db = _invoiceHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {

                                //var InvoiceHeader = new DataTable("T_InvoiceHeaderTableType");
                                //InvoiceHeader.Columns.Add("TransId", typeof(string));
                                //InvoiceHeader.Columns.Add("CompanyCode", typeof(string));
                                //InvoiceHeader.Columns.Add("StoreId", typeof(string));
                                //InvoiceHeader.Columns.Add("ContractNo", typeof(string));
                                //InvoiceHeader.Columns.Add("StoreName", typeof(string));
                                //InvoiceHeader.Columns.Add("ShiftId", typeof(string));
                                //InvoiceHeader.Columns.Add("CusId", typeof(string));
                                //InvoiceHeader.Columns.Add("CusIdentifier", typeof(string));
                                //InvoiceHeader.Columns.Add("TotalAmount", typeof(decimal));
                                //InvoiceHeader.Columns.Add("TotalPayable", typeof(decimal));
                                //InvoiceHeader.Columns.Add("TotalDiscountAmt", typeof(decimal));
                                //InvoiceHeader.Columns.Add("TotalReceipt", typeof(decimal));
                                //InvoiceHeader.Columns.Add("AmountChange", typeof(decimal));
                                //InvoiceHeader.Columns.Add("PaymentDiscount", typeof(decimal));
                                //InvoiceHeader.Columns.Add("TotalTax", typeof(decimal));
                                //InvoiceHeader.Columns.Add("DiscountType", typeof(string));
                                //InvoiceHeader.Columns.Add("DiscountAmount", typeof(decimal));
                                //InvoiceHeader.Columns.Add("DiscountRate", typeof(decimal));
                                //InvoiceHeader.Columns.Add("CreatedOn", typeof(DateTime));
                                //InvoiceHeader.Columns.Add("CreatedBy", typeof(string));
                                //InvoiceHeader.Columns.Add("ModifiedOn", typeof(DateTime));
                                //InvoiceHeader.Columns.Add("ModifiedBy", typeof(string));
                                //InvoiceHeader.Columns.Add("Status", typeof(string));
                                //InvoiceHeader.Columns.Add("IsCanceled", typeof(string));
                                //InvoiceHeader.Columns.Add("RefId", typeof(string));
                                //InvoiceHeader.Columns.Add("Remarks", typeof(string));
                                //InvoiceHeader.Columns.Add("SalesPerson", typeof(string));
                                //InvoiceHeader.Columns.Add("SalesPersonName", typeof(string));
                                //InvoiceHeader.Columns.Add("SalesMode", typeof(string));
                                //InvoiceHeader.Columns.Add("RefTransId", typeof(string));
                                //InvoiceHeader.Columns.Add("SalesType", typeof(string));
                                //InvoiceHeader.Columns.Add("ManualDiscount", typeof(string));
                                //InvoiceHeader.Columns.Add("DataSource", typeof(string));
                                //InvoiceHeader.Columns.Add("POSType", typeof(string));
                                //InvoiceHeader.Columns.Add("SyncMWIStatus", typeof(string));
                                //InvoiceHeader.Columns.Add("SyncMWIDate", typeof(DateTime));
                                //InvoiceHeader.Columns.Add("SyncMWIMsg", typeof(string));
                                //InvoiceHeader.Columns.Add("InvoiceType", typeof(string));
                                //InvoiceHeader.Columns.Add("Image", typeof(string));
                                //InvoiceHeader.Columns.Add("Phone", typeof(string));
                                //InvoiceHeader.Columns.Add("CusName", typeof(string));
                                //InvoiceHeader.Columns.Add("CusAddress", typeof(string));
                                //InvoiceHeader.Columns.Add("Reason", typeof(string));
                                //InvoiceHeader.Columns.Add("CollectedStatus", typeof(string));
                                //InvoiceHeader.Columns.Add("Chanel", typeof(string));
                                //InvoiceHeader.Columns.Add("TerminalId", typeof(string));



                                var InvoiceLineSerial = new DataTable("T_InvoiceLineSerialTableType");
                                InvoiceLineSerial.Columns.Add("TransId", typeof(string));
                                InvoiceLineSerial.Columns.Add("LineId", typeof(string));
                                InvoiceLineSerial.Columns.Add("CompanyCode", typeof(string));
                                InvoiceLineSerial.Columns.Add("ItemCode", typeof(string));
                                InvoiceLineSerial.Columns.Add("SerialNum", typeof(string));
                                InvoiceLineSerial.Columns.Add("SLocId", typeof(string));
                                InvoiceLineSerial.Columns.Add("Quantity", typeof(decimal));
                                InvoiceLineSerial.Columns.Add("UOMCode", typeof(string));
                                InvoiceLineSerial.Columns.Add("CreatedBy", typeof(string));
                                InvoiceLineSerial.Columns.Add("CreatedOn", typeof(DateTime));
                                InvoiceLineSerial.Columns.Add("ModifiedBy", typeof(string));
                                InvoiceLineSerial.Columns.Add("ModifiedOn", typeof(DateTime));
                                InvoiceLineSerial.Columns.Add("Status", typeof(string));
                                InvoiceLineSerial.Columns.Add("OpenQty", typeof(decimal));
                                InvoiceLineSerial.Columns.Add("BaseLine", typeof(int));
                                InvoiceLineSerial.Columns.Add("BaseTransId", typeof(string));
                                InvoiceLineSerial.Columns.Add("LineNum", typeof(int));
                                InvoiceLineSerial.Columns.Add("ItemName", typeof(string));
                                InvoiceLineSerial.Columns.Add("Description", typeof(string));
                                InvoiceLineSerial.Columns.Add("StoreId", typeof(string));

                                var InvoiceLine = new DataTable("T_InvoiceLineTableType");
                                InvoiceLine.Columns.Add("TransId", typeof(string));
                                InvoiceLine.Columns.Add("LineId", typeof(string));
                                InvoiceLine.Columns.Add("CompanyCode", typeof(string));
                                InvoiceLine.Columns.Add("ItemCode", typeof(string));
                                InvoiceLine.Columns.Add("SLocId", typeof(string));
                                InvoiceLine.Columns.Add("BarCode", typeof(string));
                                InvoiceLine.Columns.Add("UOMCode", typeof(string));
                                InvoiceLine.Columns.Add("Quantity", typeof(decimal));
                                InvoiceLine.Columns.Add("Price", typeof(decimal));
                                InvoiceLine.Columns.Add("LineTotal", typeof(decimal));
                                InvoiceLine.Columns.Add("DiscountType", typeof(string));
                                InvoiceLine.Columns.Add("DiscountAmt", typeof(decimal));
                                InvoiceLine.Columns.Add("DiscountRate", typeof(decimal));
                                InvoiceLine.Columns.Add("CreatedBy", typeof(string));
                                InvoiceLine.Columns.Add("CreatedOn", typeof(DateTime));
                                InvoiceLine.Columns.Add("ModifiedBy", typeof(string));
                                InvoiceLine.Columns.Add("ModifiedOn", typeof(DateTime));
                                InvoiceLine.Columns.Add("Status", typeof(string));
                                InvoiceLine.Columns.Add("Remark", typeof(string));
                                InvoiceLine.Columns.Add("PromoId", typeof(string));
                                InvoiceLine.Columns.Add("PromoType", typeof(string));
                                InvoiceLine.Columns.Add("PromoPercent", typeof(decimal));
                                InvoiceLine.Columns.Add("PromoBaseItem", typeof(string));
                                InvoiceLine.Columns.Add("SalesMode", typeof(string));
                                InvoiceLine.Columns.Add("TaxRate", typeof(decimal));
                                InvoiceLine.Columns.Add("TaxAmt", typeof(decimal));
                                InvoiceLine.Columns.Add("TaxCode", typeof(string));
                                InvoiceLine.Columns.Add("MinDepositAmt", typeof(decimal));
                                InvoiceLine.Columns.Add("MinDepositPercent", typeof(decimal));
                                InvoiceLine.Columns.Add("DeliveryType", typeof(string));
                                InvoiceLine.Columns.Add("POSService", typeof(string));
                                InvoiceLine.Columns.Add("StoreAreaId", typeof(string));
                                InvoiceLine.Columns.Add("TimeFrameId", typeof(string));
                                InvoiceLine.Columns.Add("Duration", typeof(int));
                                InvoiceLine.Columns.Add("AppointmentDate", typeof(DateTime));
                                InvoiceLine.Columns.Add("BomID", typeof(string));
                                InvoiceLine.Columns.Add("PromoPrice", typeof(decimal));
                                InvoiceLine.Columns.Add("PromoLineTotal", typeof(decimal));
                                InvoiceLine.Columns.Add("BaseLine", typeof(int));
                                InvoiceLine.Columns.Add("BaseTransId", typeof(string));
                                InvoiceLine.Columns.Add("OpenQty", typeof(decimal));
                                InvoiceLine.Columns.Add("PromoDisAmt", typeof(decimal));
                                InvoiceLine.Columns.Add("IsPromo", typeof(string));
                                InvoiceLine.Columns.Add("IsSerial", typeof(bool));
                                InvoiceLine.Columns.Add("IsVoucher", typeof(bool));
                                InvoiceLine.Columns.Add("Description", typeof(string));
                                InvoiceLine.Columns.Add("PrepaidCardNo", typeof(string));
                                InvoiceLine.Columns.Add("MemberDate", typeof(DateTime));
                                InvoiceLine.Columns.Add("MemberValue", typeof(int));
                                InvoiceLine.Columns.Add("StartDate", typeof(DateTime));
                                InvoiceLine.Columns.Add("EndDate", typeof(DateTime));
                                InvoiceLine.Columns.Add("ItemType", typeof(string));
                                InvoiceLine.Columns.Add("LineTotalBefDis", typeof(decimal));
                                InvoiceLine.Columns.Add("LineTotalDisIncludeHeader", typeof(decimal));
                                InvoiceLine.Columns.Add("StoreId", typeof(string));


                                var InvoicePayment = new DataTable("T_InvoicePaymentTableType");
                                InvoicePayment.Columns.Add("PaymentCode", typeof(string));
                                InvoicePayment.Columns.Add("CompanyCode", typeof(string));
                                InvoicePayment.Columns.Add("TransId", typeof(string));
                                InvoicePayment.Columns.Add("LineId", typeof(string));
                                InvoicePayment.Columns.Add("TotalAmt", typeof(decimal));
                                InvoicePayment.Columns.Add("ReceivedAmt", typeof(decimal));
                                InvoicePayment.Columns.Add("PaidAmt", typeof(decimal));
                                InvoicePayment.Columns.Add("ChangeAmt", typeof(decimal));
                                InvoicePayment.Columns.Add("PaymentMode", typeof(string));
                                InvoicePayment.Columns.Add("CardType", typeof(string));
                                InvoicePayment.Columns.Add("CardHolderName", typeof(string));
                                InvoicePayment.Columns.Add("CardNo", typeof(string));
                                InvoicePayment.Columns.Add("VoucherBarCode", typeof(string));
                                InvoicePayment.Columns.Add("VoucherSerial", typeof(string));
                                InvoicePayment.Columns.Add("CreatedBy", typeof(string));
                                InvoicePayment.Columns.Add("CreatedOn", typeof(DateTime));
                                InvoicePayment.Columns.Add("ModifiedBy", typeof(string));
                                InvoicePayment.Columns.Add("ModifiedOn", typeof(DateTime));
                                InvoicePayment.Columns.Add("Status", typeof(string));
                                InvoicePayment.Columns.Add("ChargableAmount", typeof(decimal));
                                InvoicePayment.Columns.Add("PaymentDiscount", typeof(decimal));
                                InvoicePayment.Columns.Add("CollectedAmount", typeof(decimal));
                                InvoicePayment.Columns.Add("RefNumber", typeof(string));
                                InvoicePayment.Columns.Add("RefTransId", typeof(string));
                                InvoicePayment.Columns.Add("ShiftId", typeof(string));
                                InvoicePayment.Columns.Add("TerminalId", typeof(string));
                                InvoicePayment.Columns.Add("Currency", typeof(string));
                                InvoicePayment.Columns.Add("FCAmount", typeof(decimal));
                                InvoicePayment.Columns.Add("Rate", typeof(decimal));
                                InvoicePayment.Columns.Add("StoreId", typeof(string));


                                var InvoicePromo = new DataTable("T_InvoicePromoTableType");
                                InvoicePromo.Columns.Add("Id", typeof(string));
                                InvoicePromo.Columns.Add("TransId", typeof(string));
                                InvoicePromo.Columns.Add("CompanyCode", typeof(string));
                                InvoicePromo.Columns.Add("ItemCode", typeof(string));
                                InvoicePromo.Columns.Add("BarCode", typeof(string));
                                InvoicePromo.Columns.Add("RefTransId", typeof(string));
                                InvoicePromo.Columns.Add("ApplyType", typeof(string));
                                InvoicePromo.Columns.Add("ItemGroupId", typeof(string));
                                InvoicePromo.Columns.Add("UOMCode", typeof(string));
                                InvoicePromo.Columns.Add("Value", typeof(decimal));
                                InvoicePromo.Columns.Add("PromoId", typeof(string));
                                InvoicePromo.Columns.Add("PromoType", typeof(string));
                                InvoicePromo.Columns.Add("PromoTypeLine", typeof(string));
                                InvoicePromo.Columns.Add("CreatedBy", typeof(string));
                                InvoicePromo.Columns.Add("CreatedOn", typeof(DateTime));
                                InvoicePromo.Columns.Add("ModifiedBy", typeof(string));
                                InvoicePromo.Columns.Add("ModifiedOn", typeof(DateTime));
                                InvoicePromo.Columns.Add("Status", typeof(string));
                                InvoicePromo.Columns.Add("PromoPercent", typeof(decimal));
                                InvoicePromo.Columns.Add("PromoAmt", typeof(decimal));
                                InvoicePromo.Columns.Add("StoreId", typeof(string));



                                var parameters = new DynamicParameters();
                                string key = "";




                                if (!string.IsNullOrEmpty(model.TransId))
                                {

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_InvoiceHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.TransId;
                                }
                                else
                                {
                                    key = _invoiceHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] (N'{PrefixAR}',N'{model.CompanyCode}',N'{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.TransId = key;
                                }

                                if (!string.IsNullOrEmpty(model.Image))
                                {
                                    MImage image = new MImage();
                                    image.Id = Guid.NewGuid();
                                    image.CompanyCode = model.CompanyCode;
                                    image.Description = model.TransId;
                                    image.Num = model.TransId;
                                    image.Type = "CheckIn";
                                    image.Image = model.Image;//StringToBinary(
                                    image.CreateOn = DateTime.Now;
                                    image.CustomerPhone = model.Phone;
                                    image.CustomerName = model.CusName;

                                    var parametersImg = new DynamicParameters();

                                    parametersImg.Add("Id", image.Id);
                                    parametersImg.Add("CompanyCode", model.CompanyCode);
                                    parametersImg.Add("Description", image.Description);
                                    parametersImg.Add("Num", image.Num);
                                    parametersImg.Add("Type", image.Type);
                                    parametersImg.Add("Image", image.Image);
                                    parametersImg.Add("CreateOn", image.CreateOn);
                                    parametersImg.Add("CustomerPhone", image.CustomerPhone);
                                    parametersImg.Add("CustomerName", image.CustomerName);

                                    db.Execute("USP_I_M_Image", parametersImg, commandType: CommandType.StoredProcedure, transaction: tran);
                                }

                                string defaultWhs = _invoiceHeaderRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);
                                if(string.IsNullOrEmpty(model.IsCanceled))
                                {
                                    model.IsCanceled = "N";
                                }
                                if (string.IsNullOrEmpty(model.InvoiceType))
                                {
                                    model.InvoiceType = "";
                                }
                                if (model.IsCanceled == "N" && (model.InvoiceType == "CheckIn" || model.InvoiceType == "CheckOut"))
                                {
                                    string itemList = "";

                                    List<ItemCheckModel> listItemCheck = new List<ItemCheckModel>();
                                    foreach (var line in model.Lines.Where(x=> x.Quantity != 0).ToList())
                                    {
                                        if (string.IsNullOrEmpty(line.SlocId))
                                        {
                                            line.SlocId = defaultWhs;
                                        }

                                        //itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                        //if (line.Quantity > 0)
                                        //{
                                           
                                        //}
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
                                   
                                    if (listItemCheck != null && listItemCheck.Count > 0)
                                    {
                                        foreach (var line in listItemCheck)
                                        {
                                            itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                        }

                                    }
                                    DynamicParameters newParameters = new DynamicParameters();
                                    newParameters.Add("CompanyCode", model.CompanyCode);
                                    newParameters.Add("ListLine", itemList);
                                    var resultCheck = db.Query<ResultModel>($"USP_I_T_SalesLine_CheckNegative", newParameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //string querycheck = $"USP_I_T_SalesLine_CheckNegative N'{model.CompanyCode}', N'{itemList}'";
                                    //var resultCheck = db.Query(querycheck, null, commandType: CommandType.Text, transaction: tran);
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
                                


                                //Create and fill-up master table data


                                parameters.Add("TransId", model.TransId, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("ContractNo", model.ContractNo);
                                parameters.Add("ShiftId", model.ShiftId);
                                parameters.Add("CusId", model.CusId);
                                parameters.Add("CusIdentifier", model.CusIdentifier);

                                parameters.Add("TotalAmount", model.TotalAmount);
                                parameters.Add("TotalPayable", model.TotalPayable);
                                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                parameters.Add("TotalReceipt", model.TotalReceipt);
                                parameters.Add("AmountChange", model.AmountChange);
                                parameters.Add("PaymentDiscount", model.PaymentDiscount);

                                parameters.Add("TotalTax", model.TotalTax);
                                parameters.Add("DiscountType", model.DiscountType);
                                parameters.Add("DiscountAmount", model.DiscountAmount);
                                parameters.Add("DiscountRate", model.DiscountRate);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "C" : model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("Remarks", model.Remarks);
                                parameters.Add("SalesPerson", model.SalesPerson);
                                parameters.Add("SalesPersonName", model.SalesPersonName);
                                parameters.Add("SalesMode", model.SalesMode);
                                parameters.Add("RefTransId", model.RefTransId);
                                parameters.Add("ManualDiscount", model.ManualDiscount);
                                parameters.Add("SalesType", model.SalesType);
                                parameters.Add("DataSource", model.DataSource);
                                parameters.Add("POSType", model.POSType);
                                parameters.Add("InvoiceType", model.InvoiceType);
                                parameters.Add("Phone", model.Phone);
                                parameters.Add("CusAddress", model.CusAddress);
                                parameters.Add("CusName", model.CusName);
                                parameters.Add("Reason", model.Reason);
                                parameters.Add("Chanel", model.Chanel);
                                parameters.Add("TerminalId", model.TerminalId);
                                //_invoiceHeaderRepository.Insert("InsertSaleHeader", parameters, commandType: CommandType.StoredProcedure);

                                //Insert record in master table. Pass transaction parameter to Dapper.
                                var affectedRows = db.Execute("USP_I_T_InvoiceHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                //string defaultWhs = _invoiceHeaderRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode = N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);

                                //Get the Id newly created for master table record.
                                //If this is not an Identity, use different method here
                                //newId = Convert.ToInt64(connection.ExecuteScalar<object>("SELECT @@IDENTITY", null, transaction: transaction));

                                //Create and fill-up detail table data
                                //Use suitable loop as you want to insert multiple records.
                                //for(......)
                                int stt = 0;
                                if (model.Lines != null && model.Lines.Count > 0)
                                {
                                    foreach (var line in model.Lines)
                                    {
                                        stt++;
                                        string getCapacity = $"select CustomField8 from M_Item with (nolock) where  ItemCode =N'{line.ItemCode}' ";
                                        var capaValue = _invoiceHeaderRepository.GetScalar(getCapacity, null, commandType: CommandType.Text);
                                        if (!string.IsNullOrEmpty(capaValue))
                                        {
                                            
                                            
                                            if (string.IsNullOrEmpty(line.TimeFrameId) && model.InvoiceType.ToLower()== "checkout")
                                            {
                                                result.Success = false;
                                                result.Message = "Time Frame Id can't null";
                                                return result;
                                            }
                                            if (string.IsNullOrEmpty(line.TimeFrameId) && model.InvoiceType.ToLower() == "checkin")
                                            {
                                                string TimeFrame = $"select TimeFrameId from M_TimeFrame with (nolock) where  CONVERT(time ,'{DateTime.Now.TimeOfDay}') between StartTime  and EndTime ";
                                                var TimeFrameId = _invoiceHeaderRepository.GetScalar(TimeFrame, null, commandType: CommandType.Text);
                                                line.TimeFrameId = TimeFrameId;
                                            }
                                            if (string.IsNullOrEmpty(line.TimeFrameId))
                                            {
                                                result.Success = false;
                                                result.Message = "Time Frame Id can't null";
                                                return result;
                                            }
                                          
                                            if (string.IsNullOrEmpty(line.AppointmentDate.ToString()))
                                            {
                                                line.AppointmentDate = DateTime.Now;
                                                //result.Success = false;
                                                //result.Message = "Appointment Date can't null";
                                                //return result;
                                            }

                                            if (!string.IsNullOrEmpty(line.StoreAreaId))
                                            {
                                                string queryCheckStoreArea = $" [USP_CheckStoreAreaInStoreCapacity] N'{model.CompanyCode}', N'{model.StoreId}',N'{line.StoreAreaId}'";
                                                var AreaCount = _invoiceHeaderRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
                                                if (AreaCount == "0")
                                                {
                                                    result.Success = false;
                                                    result.Message = "Store Area Id does not match Store Capacity. Please check your data input";
                                                    return result;
                                                }
                                            }
                                            if (string.IsNullOrEmpty(line.StoreAreaId))
                                            {
                                                string queryCheckStoreArea = $" [USP_S_StoreAreaIdByStore] N'{model.CompanyCode}', N'{model.StoreId}'";
                                                var AreaId = _invoiceHeaderRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
                                                if (string.IsNullOrEmpty(AreaId))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Store Area Id can't null. Please check capacity setup";
                                                    return result;
                                                }
                                                else
                                                {
                                                    line.StoreAreaId = AreaId;
                                                }

                                            }
                                        }

                                        parameters = new DynamicParameters();
                                        parameters.Add("TransId", key, DbType.String);
                                        parameters.Add("LineId", stt);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", line.ItemCode);
                                        parameters.Add("BarCode", line.BarCode);
                                        parameters.Add("Uomcode", line.UomCode);
                                        parameters.Add("Quantity", string.IsNullOrEmpty(line.Quantity.ToString()) ? null : line.Quantity);
                                        parameters.Add("Price", string.IsNullOrEmpty(line.Price.ToString()) ? null : line.Price);
                                        parameters.Add("LineTotal", string.IsNullOrEmpty(line.LineTotal.ToString()) ? null : line.LineTotal);
                                        parameters.Add("DiscountType", line.DiscountType);
                                        parameters.Add("DiscountAmt", string.IsNullOrEmpty(line.DiscountAmt.ToString()) ? null : line.DiscountAmt);
                                        parameters.Add("DiscountRate", string.IsNullOrEmpty(line.DiscountRate.ToString()) ? null : line.DiscountRate);
                                        parameters.Add("CreatedBy", line.CreatedBy);
                                        parameters.Add("Status", line.Status);
                                        parameters.Add("PromoId", line.PromoId);
                                        parameters.Add("PromoType", line.PromoType);
                                        parameters.Add("Remark", line.Remark);
                                        parameters.Add("PromoPercent", string.IsNullOrEmpty(line.PromoPercent.ToString()) ? null : line.PromoPercent);
                                        parameters.Add("PromoBaseItem", line.PromoBaseItem);
                                        parameters.Add("SalesMode", line.SalesMode);
                                        parameters.Add("TaxRate", string.IsNullOrEmpty(line.TaxRate.ToString()) ? null : line.TaxRate);
                                        parameters.Add("TaxAmt", string.IsNullOrEmpty(line.TaxAmt.ToString()) ? null : line.TaxAmt);
                                        parameters.Add("TaxCode", line.TaxCode);

                                        if (string.IsNullOrEmpty(line.SlocId))
                                        {

                                            line.SlocId = defaultWhs;
                                        }
                                        
                                        parameters.Add("SlocId", line.SlocId);
                                        parameters.Add("MinDepositAmt", string.IsNullOrEmpty(line.MinDepositAmt.ToString()) ? null : line.MinDepositAmt);
                                        parameters.Add("MinDepositPercent", string.IsNullOrEmpty(line.MinDepositPercent.ToString()) ? null : line.MinDepositPercent);
                                        parameters.Add("DeliveryType", line.DeliveryType);
                                        parameters.Add("Posservice", line.Posservice);
                                        parameters.Add("StoreAreaId", line.StoreAreaId);
                                        parameters.Add("TimeFrameId", line.TimeFrameId);
                                        parameters.Add("Duration", line.Duration);
                                        parameters.Add("AppointmentDate", string.IsNullOrEmpty(line.AppointmentDate.ToString()) ? null : line.AppointmentDate);
                                        parameters.Add("BomId", line.BomId);
                                        parameters.Add("PromoPrice", string.IsNullOrEmpty(line.PromoPrice.ToString()) ? null : line.PromoPrice);
                                        parameters.Add("PromoLineTotal", string.IsNullOrEmpty(line.PromoLineTotal.ToString()) ? null : line.PromoLineTotal);
                                        parameters.Add("BaseLine", line.BaseLine);
                                        parameters.Add("BaseTransId", line.BaseTransId);
                                        parameters.Add("OpenQty", string.IsNullOrEmpty(line.OpenQty.ToString()) ? null : line.OpenQty);
                                        parameters.Add("PromoDisAmt", line.PromoDisAmt);
                                        parameters.Add("IsPromo", line.IsPromo);
                                        parameters.Add("IsSerial", line.IsSerial);
                                        parameters.Add("IsVoucher", line.IsVoucher);
                                        parameters.Add("Description", line.Description);
                                        parameters.Add("PrepaidCardNo", line.PrepaidCardNo);
                                        parameters.Add("MemberDate", line.MemberDate);
                                        parameters.Add("MemberValue", line.MemberValue);
                                        parameters.Add("StartDate", line.StartDate);
                                        parameters.Add("EndDate", line.EndDate);
                                        parameters.Add("ItemType", line.ItemType);
                                        parameters.Add("LineTotalBefDis", line.LineTotalBefDis);
                                        parameters.Add("LineTotalDisIncludeHeader", line.LineTotalDisIncludeHeader);
                                        parameters.Add("StoreId", model.StoreId);

                                        string queryLine = $"usp_I_T_InvoiceLine '{key}','{stt}','{model.CompanyCode}','{line.ItemCode}','{line.BarCode}','{line.UomCode}','{line.Quantity}','{line.Price}'" +
                                            $",'{line.LineTotal}','{line.DiscountType}','{line.DiscountAmt}','{line.DiscountRate}','{line.CreatedBy}','{line.PromoId}','{line.PromoType}','{line.Status}','{line.Remark}'" +
                                            $",'{line.PromoPercent}','{line.PromoBaseItem}','{ line.SalesMode}','{line.TaxRate}','{line.TaxAmt}','{line.TaxCode}','{line.SlocId}','{line.MinDepositAmt}','{line.MinDepositPercent}'" +
                                            $",'{line.DeliveryType}','{line.Posservice}','{line.StoreAreaId}','{line.TimeFrameId}','{line.AppointmentDate}','{line.BomId}','{line.PromoPrice}','{line.PromoLineTotal}','{line.BaseLine}','{line.BaseTransId}','{line.OpenQty}'";
                                        //_invoiceHeaderRepository.GetConnection().Get("",);



                                        db.Execute("usp_I_T_InvoiceLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        int sttLine = 0;
                                        if (line.SerialLines != null)
                                        {
                                            foreach (var serialLine in line.SerialLines)
                                            {
                                                sttLine++;
                                                parameters = new DynamicParameters();
                                                Guid newline = Guid.NewGuid();
                                                parameters.Add("TransId", key, DbType.String);
                                                parameters.Add("LineId", newline);
                                                parameters.Add("CompanyCode", model.CompanyCode);
                                                parameters.Add("ItemCode", serialLine.ItemCode);
                                                parameters.Add("SerialNum", serialLine.SerialNum);
                                                if (string.IsNullOrEmpty(serialLine.SlocId))
                                                {
                                                    serialLine.SlocId = defaultWhs;
                                                }
                                                parameters.Add("SLocId", serialLine.SlocId);
                                                parameters.Add("Quantity", serialLine.Quantity);
                                                parameters.Add("Uomcode", serialLine.UomCode);
                                                parameters.Add("CreatedBy", serialLine.CreatedBy);
                                                parameters.Add("Status", serialLine.Status);
                                                parameters.Add("OpenQty", serialLine.OpenQty);
                                                parameters.Add("BaseLine", serialLine.BaseLine);
                                                parameters.Add("BaseTransId", model.RefTransId);
                                                parameters.Add("LineNum", sttLine);
                                                parameters.Add("Description", serialLine.Description);
                                                parameters.Add("StoreId", model.StoreId);

                                                //string q = $"USP_I_T_InvoiceLineSerial '{key}','{newline}','{model.CompanyCode}','{serialLine.ItemCode}','{serialLine.SerialNum}','{serialLine.SlocId}'" +
                                                //    $",'{serialLine.Quantity}','{serialLine.UomCode}','{serialLine.CreatedBy}','{serialLine.Status}','{serialLine.OpenQty}','{serialLine.BaseLine}','{model.RefTransId}'" +
                                                //    $",'{sttLine}'";

                                                db.Execute("USP_I_T_InvoiceLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                                //await _invoiceHeaderRepository.GetConnection().InsertAsync<string, TInvoiceLine>(line);
                                            }
                                        }

                                       
                                    }
                                }

                                if (model.PromoLines != null && model.PromoLines.Count > 0)
                                {
                                    foreach (var line in model.PromoLines)
                                    {
                                        stt++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("TransId", key, DbType.String);
                                        //parameters.Add("LineId", stt);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("ItemCode", line.ItemCode);
                                        parameters.Add("BarCode", line.BarCode);
                                        parameters.Add("Uomcode", line.UomCode);
                                        parameters.Add("RefTransId", line.RefTransId);
                                        parameters.Add("ApplyType", line.ApplyType);
                                        parameters.Add("ItemGroupId", line.ItemGroupId);
                                        parameters.Add("Value", line.Value);
                                        parameters.Add("PromoId", line.PromoId);
                                        parameters.Add("PromoType", line.PromoType);
                                        parameters.Add("PromoTypeLine", line.PromoTypeLine);
                                        parameters.Add("Status", line.Status);
                                        parameters.Add("CreatedBy", line.CreatedBy);
                                        parameters.Add("PromoAmt", line.PromoAmt);
                                        parameters.Add("PromoPercent", line.PromoPercent);
                                        //USP_I_T_InvoicePromo
                                        //USP_U_T_InvoiceLineSerial
                                        db.Execute("USP_I_T_InvoicePromo", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                    }
                                }
                                stt = 0;
                                if (model.Payments != null && model.Payments.Count > 0)
                                {
                                    foreach (var payment in model.Payments)
                                    {
                                        stt++;
                                        parameters = new DynamicParameters();
                                        parameters.Add("PaymentCode", payment.PaymentCode, DbType.String);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("Currency", payment.Currency);
                                        parameters.Add("FCAmount", payment.FCAmount);
                                        parameters.Add("Rate", payment.Rate);
                                        parameters.Add("TransId", key);
                                        parameters.Add("LineId", stt);
                                        parameters.Add("TotalAmt", payment.TotalAmt);
                                        parameters.Add("ReceivedAmt", payment.ReceivedAmt);
                                        parameters.Add("PaidAmt", payment.PaidAmt);
                                      
                                        parameters.Add("ChangeAmt", payment.ChangeAmt);
                                        parameters.Add("PaymentMode", payment.PaymentMode);
                                        parameters.Add("CardType", payment.CardType);
                                        parameters.Add("CardHolderName", payment.CardHolderName);
                                        parameters.Add("CardNo", payment.CardNo);
                                        parameters.Add("VoucherBarCode", payment.VoucherBarCode);
                                        parameters.Add("VoucherSerial", payment.VoucherSerial);
                                        parameters.Add("CreatedBy", payment.CreatedBy);
                                        parameters.Add("ModifiedBy", null);
                                        parameters.Add("ModifiedOn", null);
                                        parameters.Add("Status", payment.Status);
                                        parameters.Add("ChargableAmount", payment.ChargableAmount);
                                        parameters.Add("PaymentDiscount", payment.PaymentDiscount);
                                        parameters.Add("CollectedAmount", payment.CollectedAmount);
                                        parameters.Add("RefNumber", payment.RefNumber);
                                        parameters.Add("RefTransId", model.RefTransId);
                                        parameters.Add("ShiftId", model.ShiftId);
                                        parameters.Add("TerminalId", model.TerminalId);
                                        parameters.Add("StoreId", model.StoreId);
                                        db.Execute("USP_I_T_InvoicePayment", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        //await _invoiceHeaderRepository.GetConnection().InsertAsync<string, TInvoiceLine>(line);
                                    }
                                }
                                if (model.InvoiceType.ToLower() == "checkin" && model.IsCanceled != "C")
                                {
                                    db.Execute($"USP_UpdateCheckinOrder '{model.CompanyCode}','{model.StoreId}','{model.DataSource}','{model.POSType}','{model.RefTransId}'", parameters, commandType: CommandType.Text, transaction: tran);

                                }
                                if (model.IsCanceled == "C")
                                {
                                    db.Execute($"Update T_InvoiceHeader set IsCanceled = 'Y'  where TransId=N'{model.RefTransId}' and CompanyCode=N'{model.CompanyCode}' and StoreId=N'{model.StoreId}'", null, commandType: CommandType.Text, transaction: tran);
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

        public async Task<GenericResult> CreateInvoiceByTableType(InvoiceViewModel model)
        {
            GenericResult result = new GenericResult();

            if (model.Lines == null || model.Lines.Count() == 0)
            {
                result.Success = false;
                result.Message = "Doc line not null.";
                return result;
            }
            if (string.IsNullOrEmpty(model.StoreId))
            {
                result.Success = false;
                result.Message = "From Store / To Store not null.";
                return result;
            }

            try
            {
 
                using (IDbConnection db = _invoiceHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {
                            try
                            {

                                //var InvoiceHeader = new DataTable("T_InvoiceHeaderTableType");
                                //InvoiceHeader.Columns.Add("TransId", typeof(string));
                                //InvoiceHeader.Columns.Add("CompanyCode", typeof(string));
                                //InvoiceHeader.Columns.Add("StoreId", typeof(string));
                                //InvoiceHeader.Columns.Add("ContractNo", typeof(string));
                                //InvoiceHeader.Columns.Add("StoreName", typeof(string));
                                //InvoiceHeader.Columns.Add("ShiftId", typeof(string));
                                //InvoiceHeader.Columns.Add("CusId", typeof(string));
                                //InvoiceHeader.Columns.Add("CusIdentifier", typeof(string));
                                //InvoiceHeader.Columns.Add("TotalAmount", typeof(decimal));
                                //InvoiceHeader.Columns.Add("TotalPayable", typeof(decimal));
                                //InvoiceHeader.Columns.Add("TotalDiscountAmt", typeof(decimal));
                                //InvoiceHeader.Columns.Add("TotalReceipt", typeof(decimal));
                                //InvoiceHeader.Columns.Add("AmountChange", typeof(decimal));
                                //InvoiceHeader.Columns.Add("PaymentDiscount", typeof(decimal));
                                //InvoiceHeader.Columns.Add("TotalTax", typeof(decimal));
                                //InvoiceHeader.Columns.Add("DiscountType", typeof(string));
                                //InvoiceHeader.Columns.Add("DiscountAmount", typeof(decimal));
                                //InvoiceHeader.Columns.Add("DiscountRate", typeof(decimal));
                                //InvoiceHeader.Columns.Add("CreatedOn", typeof(DateTime));
                                //InvoiceHeader.Columns.Add("CreatedBy", typeof(string));
                                //InvoiceHeader.Columns.Add("ModifiedOn", typeof(DateTime));
                                //InvoiceHeader.Columns.Add("ModifiedBy", typeof(string));
                                //InvoiceHeader.Columns.Add("Status", typeof(string));
                                //InvoiceHeader.Columns.Add("IsCanceled", typeof(string));
                                //InvoiceHeader.Columns.Add("RefId", typeof(string));
                                //InvoiceHeader.Columns.Add("Remarks", typeof(string));
                                //InvoiceHeader.Columns.Add("SalesPerson", typeof(string));
                                //InvoiceHeader.Columns.Add("SalesPersonName", typeof(string));
                                //InvoiceHeader.Columns.Add("SalesMode", typeof(string));
                                //InvoiceHeader.Columns.Add("RefTransId", typeof(string));
                                //InvoiceHeader.Columns.Add("SalesType", typeof(string));
                                //InvoiceHeader.Columns.Add("ManualDiscount", typeof(string));
                                //InvoiceHeader.Columns.Add("DataSource", typeof(string));
                                //InvoiceHeader.Columns.Add("POSType", typeof(string));
                                //InvoiceHeader.Columns.Add("SyncMWIStatus", typeof(string));
                                //InvoiceHeader.Columns.Add("SyncMWIDate", typeof(DateTime));
                                //InvoiceHeader.Columns.Add("SyncMWIMsg", typeof(string));
                                //InvoiceHeader.Columns.Add("InvoiceType", typeof(string));
                                //InvoiceHeader.Columns.Add("Image", typeof(string));
                                //InvoiceHeader.Columns.Add("Phone", typeof(string));
                                //InvoiceHeader.Columns.Add("CusName", typeof(string));
                                //InvoiceHeader.Columns.Add("CusAddress", typeof(string));
                                //InvoiceHeader.Columns.Add("Reason", typeof(string));
                                //InvoiceHeader.Columns.Add("CollectedStatus", typeof(string));
                                //InvoiceHeader.Columns.Add("Chanel", typeof(string));
                                //InvoiceHeader.Columns.Add("TerminalId", typeof(string));



                                var InvoiceLineSerial = new DataTable("T_InvoiceLineSerialTableType");
                                InvoiceLineSerial.Columns.Add("TransId", typeof(string));
                                InvoiceLineSerial.Columns.Add("LineId", typeof(string));
                                InvoiceLineSerial.Columns.Add("CompanyCode", typeof(string));
                                InvoiceLineSerial.Columns.Add("ItemCode", typeof(string));
                                InvoiceLineSerial.Columns.Add("SerialNum", typeof(string));
                                InvoiceLineSerial.Columns.Add("SLocId", typeof(string));
                                InvoiceLineSerial.Columns.Add("Quantity", typeof(decimal));
                                InvoiceLineSerial.Columns.Add("UOMCode", typeof(string));
                                InvoiceLineSerial.Columns.Add("CreatedBy", typeof(string));
                                InvoiceLineSerial.Columns.Add("CreatedOn", typeof(DateTime));
                                InvoiceLineSerial.Columns.Add("ModifiedBy", typeof(string));
                                InvoiceLineSerial.Columns.Add("ModifiedOn", typeof(DateTime));
                                InvoiceLineSerial.Columns.Add("Status", typeof(string));
                                InvoiceLineSerial.Columns.Add("OpenQty", typeof(decimal));
                                InvoiceLineSerial.Columns.Add("BaseLine", typeof(int));
                                InvoiceLineSerial.Columns.Add("BaseTransId", typeof(string));
                                InvoiceLineSerial.Columns.Add("LineNum", typeof(int));
                                InvoiceLineSerial.Columns.Add("ItemName", typeof(string));
                                InvoiceLineSerial.Columns.Add("Description", typeof(string));
                                InvoiceLineSerial.Columns.Add("StoreId", typeof(string));

                                var InvoiceLine = new DataTable("T_InvoiceLineTableType");
                                InvoiceLine.Columns.Add("TransId", typeof(string));
                                InvoiceLine.Columns.Add("LineId", typeof(string));
                                InvoiceLine.Columns.Add("CompanyCode", typeof(string));
                                InvoiceLine.Columns.Add("ItemCode", typeof(string));
                                InvoiceLine.Columns.Add("SLocId", typeof(string));
                                InvoiceLine.Columns.Add("BarCode", typeof(string));
                                InvoiceLine.Columns.Add("UOMCode", typeof(string));
                                InvoiceLine.Columns.Add("Quantity", typeof(decimal));
                                InvoiceLine.Columns.Add("Price", typeof(decimal));
                                InvoiceLine.Columns.Add("LineTotal", typeof(decimal));
                                InvoiceLine.Columns.Add("DiscountType", typeof(string));
                                InvoiceLine.Columns.Add("DiscountAmt", typeof(decimal));
                                InvoiceLine.Columns.Add("DiscountRate", typeof(decimal));
                                InvoiceLine.Columns.Add("CreatedBy", typeof(string));
                                InvoiceLine.Columns.Add("CreatedOn", typeof(DateTime));
                                InvoiceLine.Columns.Add("ModifiedBy", typeof(string));
                                InvoiceLine.Columns.Add("ModifiedOn", typeof(DateTime));
                                InvoiceLine.Columns.Add("Status", typeof(string));
                                InvoiceLine.Columns.Add("Remark", typeof(string));
                                InvoiceLine.Columns.Add("PromoId", typeof(string));
                                InvoiceLine.Columns.Add("PromoType", typeof(string));
                                InvoiceLine.Columns.Add("PromoPercent", typeof(decimal));
                                InvoiceLine.Columns.Add("PromoBaseItem", typeof(string));
                                InvoiceLine.Columns.Add("SalesMode", typeof(string));
                                InvoiceLine.Columns.Add("TaxRate", typeof(decimal));
                                InvoiceLine.Columns.Add("TaxAmt", typeof(decimal));
                                InvoiceLine.Columns.Add("TaxCode", typeof(string));
                                InvoiceLine.Columns.Add("MinDepositAmt", typeof(decimal));
                                InvoiceLine.Columns.Add("MinDepositPercent", typeof(decimal));
                                InvoiceLine.Columns.Add("DeliveryType", typeof(string));
                                InvoiceLine.Columns.Add("POSService", typeof(string));
                                InvoiceLine.Columns.Add("StoreAreaId", typeof(string));
                                InvoiceLine.Columns.Add("TimeFrameId", typeof(string));
                                InvoiceLine.Columns.Add("Duration", typeof(int));
                                InvoiceLine.Columns.Add("AppointmentDate", typeof(DateTime));
                                InvoiceLine.Columns.Add("BomID", typeof(string));
                                InvoiceLine.Columns.Add("PromoPrice", typeof(decimal));
                                InvoiceLine.Columns.Add("PromoLineTotal", typeof(decimal));
                                InvoiceLine.Columns.Add("BaseLine", typeof(int));
                                InvoiceLine.Columns.Add("BaseTransId", typeof(string));
                                InvoiceLine.Columns.Add("OpenQty", typeof(decimal));
                                InvoiceLine.Columns.Add("PromoDisAmt", typeof(decimal));
                                InvoiceLine.Columns.Add("IsPromo", typeof(string));
                                InvoiceLine.Columns.Add("IsSerial", typeof(bool));
                                InvoiceLine.Columns.Add("IsVoucher", typeof(bool));
                                InvoiceLine.Columns.Add("Description", typeof(string));
                                InvoiceLine.Columns.Add("PrepaidCardNo", typeof(string));
                                InvoiceLine.Columns.Add("MemberDate", typeof(DateTime));
                                InvoiceLine.Columns.Add("MemberValue", typeof(int));
                                InvoiceLine.Columns.Add("StartDate", typeof(DateTime));
                                InvoiceLine.Columns.Add("EndDate", typeof(DateTime));
                                InvoiceLine.Columns.Add("ItemType", typeof(string));
                                InvoiceLine.Columns.Add("LineTotalBefDis", typeof(decimal));
                                InvoiceLine.Columns.Add("LineTotalDisIncludeHeader", typeof(decimal));
                                InvoiceLine.Columns.Add("StoreId", typeof(string));


                                var InvoicePayment = new DataTable("T_InvoicePaymentTableType");
                                InvoicePayment.Columns.Add("PaymentCode", typeof(string));
                                InvoicePayment.Columns.Add("CompanyCode", typeof(string));
                                InvoicePayment.Columns.Add("TransId", typeof(string));
                                InvoicePayment.Columns.Add("LineId", typeof(string));
                                InvoicePayment.Columns.Add("TotalAmt", typeof(decimal));
                                InvoicePayment.Columns.Add("ReceivedAmt", typeof(decimal));
                                InvoicePayment.Columns.Add("PaidAmt", typeof(decimal));
                                InvoicePayment.Columns.Add("ChangeAmt", typeof(decimal));
                                InvoicePayment.Columns.Add("PaymentMode", typeof(string));
                                InvoicePayment.Columns.Add("CardType", typeof(string));
                                InvoicePayment.Columns.Add("CardHolderName", typeof(string));
                                InvoicePayment.Columns.Add("CardNo", typeof(string));
                                InvoicePayment.Columns.Add("VoucherBarCode", typeof(string));
                                InvoicePayment.Columns.Add("VoucherSerial", typeof(string));
                                InvoicePayment.Columns.Add("CreatedBy", typeof(string));
                                InvoicePayment.Columns.Add("CreatedOn", typeof(DateTime));
                                InvoicePayment.Columns.Add("ModifiedBy", typeof(string));
                                InvoicePayment.Columns.Add("ModifiedOn", typeof(DateTime));
                                InvoicePayment.Columns.Add("Status", typeof(string));
                                InvoicePayment.Columns.Add("ChargableAmount", typeof(decimal));
                                InvoicePayment.Columns.Add("PaymentDiscount", typeof(decimal));
                                InvoicePayment.Columns.Add("CollectedAmount", typeof(decimal));
                                InvoicePayment.Columns.Add("RefNumber", typeof(string));
                                InvoicePayment.Columns.Add("RefTransId", typeof(string));
                                InvoicePayment.Columns.Add("ShiftId", typeof(string));
                                InvoicePayment.Columns.Add("TerminalId", typeof(string));
                                InvoicePayment.Columns.Add("Currency", typeof(string));
                                InvoicePayment.Columns.Add("FCAmount", typeof(decimal));
                                InvoicePayment.Columns.Add("Rate", typeof(decimal));
                                InvoicePayment.Columns.Add("StoreId", typeof(string));


                                var InvoicePromo = new DataTable("T_InvoicePromoTableType");
                                InvoicePromo.Columns.Add("Id", typeof(string));
                                InvoicePromo.Columns.Add("TransId", typeof(string));
                                InvoicePromo.Columns.Add("CompanyCode", typeof(string));
                                InvoicePromo.Columns.Add("ItemCode", typeof(string));
                                InvoicePromo.Columns.Add("BarCode", typeof(string));
                                InvoicePromo.Columns.Add("RefTransId", typeof(string));
                                InvoicePromo.Columns.Add("ApplyType", typeof(string));
                                InvoicePromo.Columns.Add("ItemGroupId", typeof(string));
                                InvoicePromo.Columns.Add("UOMCode", typeof(string));
                                InvoicePromo.Columns.Add("Value", typeof(decimal));
                                InvoicePromo.Columns.Add("PromoId", typeof(string));
                                InvoicePromo.Columns.Add("PromoType", typeof(string));
                                InvoicePromo.Columns.Add("PromoTypeLine", typeof(string));
                                InvoicePromo.Columns.Add("CreatedBy", typeof(string));
                                InvoicePromo.Columns.Add("CreatedOn", typeof(DateTime));
                                InvoicePromo.Columns.Add("ModifiedBy", typeof(string));
                                InvoicePromo.Columns.Add("ModifiedOn", typeof(DateTime));
                                InvoicePromo.Columns.Add("Status", typeof(string));
                                InvoicePromo.Columns.Add("PromoPercent", typeof(decimal));
                                InvoicePromo.Columns.Add("PromoAmt", typeof(decimal));
                                InvoicePromo.Columns.Add("StoreId", typeof(string));



                                var parameters = new DynamicParameters();
                                string key = "";




                                if (!string.IsNullOrEmpty(model.TransId))
                                {

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_InvoiceHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.TransId;
                                }
                                else
                                {
                                    key = _invoiceHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] (N'{PrefixAR}',N'{model.CompanyCode}',N'{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.TransId = key;
                                }

                                if (!string.IsNullOrEmpty(model.Image))
                                {
                                    MImage image = new MImage();
                                    image.Id = Guid.NewGuid();
                                    image.CompanyCode = model.CompanyCode;
                                    image.Description = model.TransId;
                                    image.Num = model.TransId;
                                    image.Type = "CheckIn";
                                    image.Image = model.Image;//StringToBinary(
                                    image.CreateOn = DateTime.Now;
                                    image.CustomerPhone = model.Phone;
                                    image.CustomerName = model.CusName;

                                    var parametersImg = new DynamicParameters();

                                    parametersImg.Add("Id", image.Id);
                                    parametersImg.Add("CompanyCode", model.CompanyCode);
                                    parametersImg.Add("Description", image.Description);
                                    parametersImg.Add("Num", image.Num);
                                    parametersImg.Add("Type", image.Type);
                                    parametersImg.Add("Image", image.Image);
                                    parametersImg.Add("CreateOn", image.CreateOn);
                                    parametersImg.Add("CustomerPhone", image.CustomerPhone);
                                    parametersImg.Add("CustomerName", image.CustomerName);

                                    db.Execute("USP_I_M_Image", parametersImg, commandType: CommandType.StoredProcedure, transaction: tran);
                                }

                                string defaultWhs = _invoiceHeaderRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);
                                if (string.IsNullOrEmpty(model.IsCanceled))
                                {
                                    model.IsCanceled = "N";
                                }
                                if (string.IsNullOrEmpty(model.InvoiceType))
                                {
                                    model.InvoiceType = "";
                                }
                                if (model.IsCanceled == "N" && (model.InvoiceType == "CheckIn" || model.InvoiceType == "CheckOut"))
                                {
                                    string itemList = "";

                                    List<ItemCheckModel> listItemCheck = new List<ItemCheckModel>();
                                    foreach (var line in model.Lines.Where(x => x.Quantity != 0).ToList())
                                    {
                                        if (string.IsNullOrEmpty(line.SlocId))
                                        {
                                            line.SlocId = defaultWhs;
                                        }

                                        //itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                        //if (line.Quantity > 0)
                                        //{

                                        //}
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

                                    if (listItemCheck != null && listItemCheck.Count > 0)
                                    {
                                        foreach (var line in listItemCheck)
                                        {
                                            itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                        }

                                    }
                                    DynamicParameters newParameters = new DynamicParameters();
                                    newParameters.Add("CompanyCode", model.CompanyCode);
                                    newParameters.Add("ListLine", itemList);
                                    var resultCheck = db.Query<ResultModel>($"USP_I_T_SalesLine_CheckNegative", newParameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    //string querycheck = $"USP_I_T_SalesLine_CheckNegative N'{model.CompanyCode}', N'{itemList}'";
                                    //var resultCheck = db.Query(querycheck, null, commandType: CommandType.Text, transaction: tran);
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



                                //Create and fill-up master table data


                                parameters.Add("TransId", model.TransId, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("ContractNo", model.ContractNo);
                                parameters.Add("ShiftId", model.ShiftId);
                                parameters.Add("CusId", model.CusId);
                                parameters.Add("CusIdentifier", model.CusIdentifier);

                                parameters.Add("TotalAmount", model.TotalAmount);
                                parameters.Add("TotalPayable", model.TotalPayable);
                                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                parameters.Add("TotalReceipt", model.TotalReceipt);
                                parameters.Add("AmountChange", model.AmountChange);
                                parameters.Add("PaymentDiscount", model.PaymentDiscount);

                                parameters.Add("TotalTax", model.TotalTax);
                                parameters.Add("DiscountType", model.DiscountType);
                                parameters.Add("DiscountAmount", model.DiscountAmount);
                                parameters.Add("DiscountRate", model.DiscountRate);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "C" : model.Status);
                                parameters.Add("IsCanceled", model.IsCanceled);
                                parameters.Add("Remarks", model.Remarks);
                                parameters.Add("SalesPerson", model.SalesPerson);
                                parameters.Add("SalesPersonName", model.SalesPersonName);
                                parameters.Add("SalesMode", model.SalesMode);
                                parameters.Add("RefTransId", model.RefTransId);
                                parameters.Add("ManualDiscount", model.ManualDiscount);
                                parameters.Add("SalesType", model.SalesType);
                                parameters.Add("DataSource", model.DataSource);
                                parameters.Add("POSType", model.POSType);
                                parameters.Add("InvoiceType", model.InvoiceType);
                                parameters.Add("Phone", model.Phone);
                                parameters.Add("CusAddress", model.CusAddress);
                                parameters.Add("CusName", model.CusName);
                                parameters.Add("Reason", model.Reason);
                                parameters.Add("Chanel", model.Chanel);
                                parameters.Add("TerminalId", model.TerminalId);
                                parameters.Add("PrefixAR", PrefixAR);

                                //_invoiceHeaderRepository.Insert("InsertSaleHeader", parameters, commandType: CommandType.StoredProcedure);

                                //Insert record in master table. Pass transaction parameter to Dapper.
                              
                                //string defaultWhs = _invoiceHeaderRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode = N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);

                                //Get the Id newly created for master table record.
                                //If this is not an Identity, use different method here
                                //newId = Convert.ToInt64(connection.ExecuteScalar<object>("SELECT @@IDENTITY", null, transaction: transaction));

                                //Create and fill-up detail table data
                                //Use suitable loop as you want to insert multiple records.
                                //for(......)
                                int stt = 0;
                                if (model.Lines != null && model.Lines.Count > 0)
                                {
                                    foreach (var line in model.Lines)
                                    {
                                        stt++;
                                        string getCapacity = $"select CustomField8 from M_Item with (nolock) where  ItemCode =N'{line.ItemCode}' ";
                                        var capaValue = _invoiceHeaderRepository.GetScalar(getCapacity, null, commandType: CommandType.Text);
                                        if (!string.IsNullOrEmpty(capaValue))
                                        {


                                            if (string.IsNullOrEmpty(line.TimeFrameId) && model.InvoiceType.ToLower() == "checkout")
                                            {
                                                result.Success = false;
                                                result.Message = "Time Frame Id can't null";
                                                return result;
                                            }
                                            if (string.IsNullOrEmpty(line.TimeFrameId) && model.InvoiceType.ToLower() == "checkin")
                                            {
                                                string TimeFrame = $"select TimeFrameId from M_TimeFrame with (nolock) where  CONVERT(time ,'{DateTime.Now.TimeOfDay}') between StartTime  and EndTime ";
                                                var TimeFrameId = _invoiceHeaderRepository.GetScalar(TimeFrame, null, commandType: CommandType.Text);
                                                line.TimeFrameId = TimeFrameId;
                                            }
                                            if (string.IsNullOrEmpty(line.TimeFrameId))
                                            {
                                                result.Success = false;
                                                result.Message = "Time Frame Id can't null";
                                                return result;
                                            }

                                            if (string.IsNullOrEmpty(line.AppointmentDate.ToString()))
                                            {
                                                line.AppointmentDate = DateTime.Now;
                                                //result.Success = false;
                                                //result.Message = "Appointment Date can't null";
                                                //return result;
                                            }

                                            if (!string.IsNullOrEmpty(line.StoreAreaId))
                                            {
                                                string queryCheckStoreArea = $" [USP_CheckStoreAreaInStoreCapacity] N'{model.CompanyCode}', N'{model.StoreId}',N'{line.StoreAreaId}'";
                                                var AreaCount = _invoiceHeaderRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
                                                if (AreaCount == "0")
                                                {
                                                    result.Success = false;
                                                    result.Message = "Store Area Id does not match Store Capacity. Please check your data input";
                                                    return result;
                                                }
                                            }
                                            if (string.IsNullOrEmpty(line.StoreAreaId))
                                            {
                                                string queryCheckStoreArea = $" [USP_S_StoreAreaIdByStore] N'{model.CompanyCode}', N'{model.StoreId}'";
                                                var AreaId = _invoiceHeaderRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
                                                if (string.IsNullOrEmpty(AreaId))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Store Area Id can't null. Please check capacity setup";
                                                    return result;
                                                }
                                                else
                                                {
                                                    line.StoreAreaId = AreaId;
                                                }

                                            }
                                        }

                                        //parameters = new DynamicParameters();
                                        //parameters.Add("TransId", key, DbType.String);
                                        //parameters.Add("LineId", stt);
                                        //parameters.Add("CompanyCode", model.CompanyCode);
                                        //parameters.Add("ItemCode", line.ItemCode);
                                        //parameters.Add("BarCode", line.BarCode);
                                        //parameters.Add("Uomcode", line.UomCode);
                                        //parameters.Add("Quantity", string.IsNullOrEmpty(line.Quantity.ToString()) ? null : line.Quantity);
                                        //parameters.Add("Price", string.IsNullOrEmpty(line.Price.ToString()) ? null : line.Price);
                                        //parameters.Add("LineTotal", string.IsNullOrEmpty(line.LineTotal.ToString()) ? null : line.LineTotal);
                                        //parameters.Add("DiscountType", line.DiscountType);
                                        //parameters.Add("DiscountAmt", string.IsNullOrEmpty(line.DiscountAmt.ToString()) ? null : line.DiscountAmt);
                                        //parameters.Add("DiscountRate", string.IsNullOrEmpty(line.DiscountRate.ToString()) ? null : line.DiscountRate);
                                        //parameters.Add("CreatedBy", line.CreatedBy);
                                        //parameters.Add("Status", line.Status);
                                        //parameters.Add("PromoId", line.PromoId);
                                        //parameters.Add("PromoType", line.PromoType);
                                        //parameters.Add("Remark", line.Remark);
                                        //parameters.Add("PromoPercent", string.IsNullOrEmpty(line.PromoPercent.ToString()) ? null : line.PromoPercent);
                                        //parameters.Add("PromoBaseItem", line.PromoBaseItem);
                                        //parameters.Add("SalesMode", line.SalesMode);
                                        //parameters.Add("TaxRate", string.IsNullOrEmpty(line.TaxRate.ToString()) ? null : line.TaxRate);
                                        //parameters.Add("TaxAmt", string.IsNullOrEmpty(line.TaxAmt.ToString()) ? null : line.TaxAmt);
                                        //parameters.Add("TaxCode", line.TaxCode);

                                        //if (string.IsNullOrEmpty(line.SlocId))
                                        //{

                                        //    line.SlocId = defaultWhs;
                                        //}

                                        //parameters.Add("SlocId", line.SlocId);
                                        //parameters.Add("MinDepositAmt", string.IsNullOrEmpty(line.MinDepositAmt.ToString()) ? null : line.MinDepositAmt);
                                        //parameters.Add("MinDepositPercent", string.IsNullOrEmpty(line.MinDepositPercent.ToString()) ? null : line.MinDepositPercent);
                                        //parameters.Add("DeliveryType", line.DeliveryType);
                                        //parameters.Add("Posservice", line.Posservice);
                                        //parameters.Add("StoreAreaId", line.StoreAreaId);
                                        //parameters.Add("TimeFrameId", line.TimeFrameId);
                                        //parameters.Add("Duration", line.Duration);
                                        //parameters.Add("AppointmentDate", string.IsNullOrEmpty(line.AppointmentDate.ToString()) ? null : line.AppointmentDate);
                                        //parameters.Add("BomId", line.BomId);
                                        //parameters.Add("PromoPrice", string.IsNullOrEmpty(line.PromoPrice.ToString()) ? null : line.PromoPrice);
                                        //parameters.Add("PromoLineTotal", string.IsNullOrEmpty(line.PromoLineTotal.ToString()) ? null : line.PromoLineTotal);
                                        //parameters.Add("BaseLine", line.BaseLine);
                                        //parameters.Add("BaseTransId", line.BaseTransId);
                                        //parameters.Add("OpenQty", string.IsNullOrEmpty(line.OpenQty.ToString()) ? null : line.OpenQty);
                                        //parameters.Add("PromoDisAmt", line.PromoDisAmt);
                                        //parameters.Add("IsPromo", line.IsPromo);
                                        //parameters.Add("IsSerial", line.IsSerial);
                                        //parameters.Add("IsVoucher", line.IsVoucher);
                                        //parameters.Add("Description", line.Description);
                                        //parameters.Add("PrepaidCardNo", line.PrepaidCardNo);
                                        //parameters.Add("MemberDate", line.MemberDate);
                                        //parameters.Add("MemberValue", line.MemberValue);
                                        //parameters.Add("StartDate", line.StartDate);
                                        //parameters.Add("EndDate", line.EndDate);
                                        //parameters.Add("ItemType", line.ItemType);
                                        //parameters.Add("LineTotalBefDis", line.LineTotalBefDis);
                                        //parameters.Add("LineTotalDisIncludeHeader", line.LineTotalDisIncludeHeader);
                                        //parameters.Add("StoreId", model.StoreId);

                                        //string queryLine = $"usp_I_T_InvoiceLine '{key}','{stt}','{model.CompanyCode}','{line.ItemCode}','{line.BarCode}','{line.UomCode}','{line.Quantity}','{line.Price}'" +
                                        //    $",'{line.LineTotal}','{line.DiscountType}','{line.DiscountAmt}','{line.DiscountRate}','{line.CreatedBy}','{line.PromoId}','{line.PromoType}','{line.Status}','{line.Remark}'" +
                                        //    $",'{line.PromoPercent}','{line.PromoBaseItem}','{ line.SalesMode}','{line.TaxRate}','{line.TaxAmt}','{line.TaxCode}','{line.SlocId}','{line.MinDepositAmt}','{line.MinDepositPercent}'" +
                                        //    $",'{line.DeliveryType}','{line.Posservice}','{line.StoreAreaId}','{line.TimeFrameId}','{line.AppointmentDate}','{line.BomId}','{line.PromoPrice}','{line.PromoLineTotal}','{line.BaseLine}','{line.BaseTransId}','{line.OpenQty}'";
                                        //_invoiceHeaderRepository.GetConnection().Get("",);

                                        //if(string.IsNullOrEmpty(model.InvoiceType) && model.InvoiceType.ToLower() == "checkin")
                                        //{
                                        //    if (string.IsNullOrEmpty(model.SalesType) && model.SalesType.ToLower() == "voucher")
                                        //    {

                                        //    }
                                        //}    


                                        //db.Execute("usp_I_T_InvoiceLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        InvoiceLine.Rows.Add(key, stt, model.CompanyCode, line.ItemCode, line.SlocId, line.BarCode, line.UomCode, string.IsNullOrEmpty(line.Quantity.ToString()) ? null : line.Quantity,
                                            string.IsNullOrEmpty(line.Price.ToString()) ? null : line.Price,
                                            string.IsNullOrEmpty(line.LineTotal.ToString()) ? null : line.LineTotal,
                                            line.DiscountType,
                                            string.IsNullOrEmpty(line.DiscountAmt.ToString()) ? null : line.DiscountAmt,
                                            string.IsNullOrEmpty(line.DiscountRate.ToString()) ? null : line.DiscountRate,
                                            model.CreatedBy, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), null, null,
                                            line.Status, line.Remark, line.PromoId, line.PromoType, string.IsNullOrEmpty(line.PromoPercent.ToString()) ? null : line.PromoPercent,
                                            line.PromoBaseItem, model.SalesMode, string.IsNullOrEmpty(line.TaxRate.ToString()) ? null : line.TaxRate,
                                            string.IsNullOrEmpty(line.TaxAmt.ToString()) ? null : line.TaxAmt, line.TaxCode, string.IsNullOrEmpty(line.MinDepositAmt.ToString()) ? null : line.MinDepositAmt,
                                             string.IsNullOrEmpty(line.MinDepositPercent.ToString()) ? null : line.MinDepositPercent, line.DeliveryType, line.Posservice, line.StoreAreaId, line.TimeFrameId,
                                             line.Duration, string.IsNullOrEmpty(line.AppointmentDate.ToString()) ? null : line.AppointmentDate, line.BomId, string.IsNullOrEmpty(line.PromoPrice.ToString()) ? null : line.PromoPrice,
                                             string.IsNullOrEmpty(line.PromoLineTotal.ToString()) ? null : line.PromoLineTotal, line.BaseLine, line.BaseTransId, string.IsNullOrEmpty(line.OpenQty.ToString()) ? null : line.OpenQty,
                                            line.PromoDisAmt, line.IsPromo, line.IsSerial, line.IsVoucher, line.Description, line.PrepaidCardNo, line.MemberDate, line.MemberValue, line.StartDate, line.EndDate,
                                            line.ItemType, line.LineTotalBefDis, line.LineTotalDisIncludeHeader, model.StoreId);
                                         
                                        

                                        int sttLine = 0;
                                        if (line.SerialLines != null)
                                        {
                                            foreach (var serialLine in line.SerialLines)
                                            {
                                                sttLine++;
                                                if (string.IsNullOrEmpty(serialLine.SlocId))
                                                {
                                                    serialLine.SlocId = defaultWhs;
                                                }
                                                Guid newline = Guid.NewGuid();
                                                //parameters = new DynamicParameters();
                                             
                                                //parameters.Add("TransId", key, DbType.String);
                                                //parameters.Add("LineId", newline);
                                                //parameters.Add("CompanyCode", model.CompanyCode);
                                                //parameters.Add("ItemCode", serialLine.ItemCode);
                                                //parameters.Add("SerialNum", serialLine.SerialNum);
                                               
                                                //parameters.Add("SLocId", serialLine.SlocId);
                                                //parameters.Add("Quantity", serialLine.Quantity);
                                                //parameters.Add("Uomcode", serialLine.UomCode);
                                                //parameters.Add("CreatedBy", serialLine.CreatedBy);
                                                //parameters.Add("Status", serialLine.Status);
                                                //parameters.Add("OpenQty", serialLine.OpenQty);
                                                //parameters.Add("BaseLine", serialLine.BaseLine);
                                                //parameters.Add("BaseTransId", model.RefTransId);
                                                //parameters.Add("LineNum", sttLine);
                                                //parameters.Add("Description", serialLine.Description);
                                                //parameters.Add("StoreId", model.StoreId);

                                                InvoiceLineSerial.Rows.Add(key, newline, model.CompanyCode, serialLine.ItemCode, serialLine.SerialNum, serialLine.SlocId, serialLine.Quantity,
                                                  serialLine.UomCode, model.CreatedBy, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), null, null, serialLine.Status, serialLine.OpenQty,
                                                  serialLine.BaseLine, model.RefTransId, sttLine, serialLine.Description, serialLine.Description, model.StoreId);

                                                
                                              
                                                //InvoiceLineSerial.Columns.Add("LineNum", typeof(int));
                                                //InvoiceLineSerial.Columns.Add("ItemName", typeof(string));
                                                //InvoiceLineSerial.Columns.Add("Description", typeof(string));
                                                //InvoiceLineSerial.Columns.Add("StoreId", typeof(string));

 

                                                //db.Execute("USP_I_T_InvoiceLineSerial", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                                 
                                            }
                                        }



                                    }
                                }
                               

                                if (model.PromoLines != null && model.PromoLines.Count > 0)
                                {
                                    foreach (var line in model.PromoLines)
                                    {
                                        stt++;
                                        //parameters = new DynamicParameters();
                                        //parameters.Add("TransId", key, DbType.String);
                                        ////parameters.Add("LineId", stt);
                                        //parameters.Add("CompanyCode", model.CompanyCode);
                                        //parameters.Add("ItemCode", line.ItemCode);
                                        //parameters.Add("BarCode", line.BarCode);
                                        //parameters.Add("Uomcode", line.UomCode);
                                        //parameters.Add("RefTransId", line.RefTransId);
                                        //parameters.Add("ApplyType", line.ApplyType);
                                        //parameters.Add("ItemGroupId", line.ItemGroupId);
                                        //parameters.Add("Value", line.Value);
                                        //parameters.Add("PromoId", line.PromoId);
                                        //parameters.Add("PromoType", line.PromoType);
                                        //parameters.Add("PromoTypeLine", line.PromoTypeLine);
                                        //parameters.Add("Status", line.Status);
                                        //parameters.Add("CreatedBy", line.CreatedBy);
                                        //parameters.Add("PromoAmt", line.PromoAmt);
                                        //parameters.Add("PromoPercent", line.PromoPercent);
                                        //USP_I_T_InvoicePromo
                                        //USP_U_T_InvoiceLineSerial
                                        //db.Execute("USP_I_T_InvoicePromo", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                        InvoicePromo.Rows.Add( Guid.NewGuid(), key, model.CompanyCode, line.ItemCode, line.BarCode, line.RefTransId, line.ApplyType, line.ItemGroupId, line.UomCode,
                                            line.Value, line.PromoId, line.PromoType, line.PromoTypeLine, model.CreatedBy, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), null,null, line.Status, line.PromoPercent, line.PromoAmt, model.StoreId);
                                         
                                    }
                                }
                                stt = 0;
                                if (model.Payments != null && model.Payments.Count > 0)
                                {
                                    foreach (var payment in model.Payments)
                                    {
                                        stt++;
                                        //parameters = new DynamicParameters();
                                        //parameters.Add("PaymentCode", payment.PaymentCode, DbType.String);
                                        //parameters.Add("CompanyCode", model.CompanyCode);
                                        //parameters.Add("Currency", payment.Currency);
                                        //parameters.Add("FCAmount", payment.FCAmount);
                                        //parameters.Add("Rate", payment.Rate);
                                        //parameters.Add("TransId", key);
                                        //parameters.Add("LineId", stt);
                                        //parameters.Add("TotalAmt", payment.TotalAmt);
                                        //parameters.Add("ReceivedAmt", payment.ReceivedAmt);
                                        //parameters.Add("PaidAmt", payment.PaidAmt);

                                        //parameters.Add("ChangeAmt", payment.ChangeAmt);
                                        //parameters.Add("PaymentMode", payment.PaymentMode);
                                        //parameters.Add("CardType", payment.CardType);
                                        //parameters.Add("CardHolderName", payment.CardHolderName);
                                        //parameters.Add("CardNo", payment.CardNo);
                                        //parameters.Add("VoucherBarCode", payment.VoucherBarCode);
                                        //parameters.Add("VoucherSerial", payment.VoucherSerial);
                                        //parameters.Add("CreatedBy", payment.CreatedBy);
                                        //parameters.Add("ModifiedBy", null);
                                        //parameters.Add("ModifiedOn", null);
                                        //parameters.Add("Status", payment.Status);
                                        //parameters.Add("ChargableAmount", payment.ChargableAmount);
                                        //parameters.Add("PaymentDiscount", payment.PaymentDiscount);
                                        //parameters.Add("CollectedAmount", payment.CollectedAmount);
                                        //parameters.Add("RefNumber", payment.RefNumber);
                                        //parameters.Add("RefTransId", model.RefTransId);
                                        //parameters.Add("ShiftId", model.ShiftId);
                                        //parameters.Add("TerminalId", model.TerminalId);
                                        //parameters.Add("StoreId", model.StoreId);
                                        //db.Execute("USP_I_T_InvoicePayment", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                        InvoicePayment.Rows.Add(payment.PaymentCode, model.CompanyCode, key, stt, payment.TotalAmt, payment.ReceivedAmt, payment.PaidAmt, payment.ChangeAmt,
                                            payment.PaymentMode, payment.CardType, payment.CardHolderName, payment.CardNo, payment.VoucherBarCode, payment.VoucherSerial, model.CreatedBy,
                                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), null, null, payment.Status, payment.ChargableAmount, payment.PaymentDiscount, payment.CollectedAmount,
                                            payment.RefNumber, model.RefTransId, model.ShiftId, model.TerminalId, payment.Currency, payment.FCAmount, payment.Rate, model.StoreId 
                                            );

                                    
                                    }
                                }

                                parameters.Add("@Lines", InvoiceLine.AsTableValuedParameter("[dbo].[T_InvoiceLineTableType]"));
                                parameters.Add("@LineSerials", InvoiceLineSerial.AsTableValuedParameter("[dbo].[T_InvoiceLineSerialTableType]")); 
                                parameters.Add("@LinePayments", InvoicePayment.AsTableValuedParameter("[dbo].[T_InvoicePaymentTableType]"));
                                parameters.Add("@LinePromos", InvoicePromo.AsTableValuedParameter("[dbo].[T_InvoicePromoTableType]"));
                                //if (model.IsCanceled == "C")
                                //{
                                //    db.Execute($"Update T_InvoiceHeader set IsCanceled = 'Y'  where TransId=N'{model.RefTransId}' and CompanyCode=N'{model.CompanyCode}' and StoreId=N'{model.StoreId}'", null, commandType: CommandType.Text, transaction: tran);
                                //}
                                //if (model.InvoiceType.ToLower() == "checkin" && model.IsCanceled != "C")
                                //{
                                //    db.Execute($"USP_UpdateCheckinOrder '{model.CompanyCode}','{model.StoreId}','{model.DataSource}','{model.POSType}','{model.RefTransId}'", parameters, commandType: CommandType.Text, transaction: tran);

                                //}
                                key = db.ExecuteScalar("USP_I_T_ARInvoice", parameters, commandType: CommandType.StoredProcedure, transaction: tran).ToString();
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
        public async Task<GenericResult> GetByType(string companycode, string storeId, string Type, string fromdate, string todate, string top)
        {
            GenericResult result = new GenericResult();
            try
            {

                //string query = $"select * from T_InvoiceHeader with (nolock) where 1=1 ";
                //if (!string.IsNullOrEmpty(companycode) && companycode != "null")
                //{
                //    query += $" and companycode = '{companycode}' ";
                //}
                //if (!string.IsNullOrEmpty(storeId) && storeId != "null")
                //{
                //    query += $" and storeId = '{storeId}' ";
                //}
                //if (!string.IsNullOrEmpty(Type) && Type != "null")
                //{
                //    query += $" and InvoiceType = '{Type}' ";
                //}
                //if (!string.IsNullOrEmpty(fromdate) && fromdate != "null")
                //{
                //    query += $" and createdOn >= '{fromdate}'";
                //}
                //if (!string.IsNullOrEmpty(todate) && todate != "null")
                //{
                //    query += $" and createdOn <= '{todate}'";
                //}
                //query += $" order by CreatedOn desc";

                string qStr = "USP_S_T_InvoiceHeaderByType";
                var parameters = new DynamicParameters(); 
                parameters.Add("TransId", "");
                parameters.Add("StoreId", storeId);
                parameters.Add("CompanyCode", companycode);
                parameters.Add("Type", Type);
                parameters.Add("FromDate", fromdate);
                parameters.Add("ToDate", todate);
                if(!string.IsNullOrEmpty(top))
                {
                    parameters.Add("Top", string.IsNullOrEmpty(top)? null: top );

                }    
                //parameters.Add("CompanyCode", model.CompanyCode);
                var lst = await _invoiceHeaderRepository.GetAllAsync(qStr, parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = lst;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            
            //throw new NotImplementedException();
        }

        
        public async Task<List<TInvoiceHeader>> GetAll()
        {
            var lst = await _invoiceHeaderRepository.GetAllAsync("select * from T_InvoiceHeader with (nolock) order by CreatedOn desc", null, commandType: CommandType.Text);
            return lst;
            //throw new NotImplementedException();
        }

        public Task<TInvoiceHeader> GetById(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetOrderById(string Id, string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {

                InvoiceViewModel order = new InvoiceViewModel();


                TInvoiceHeader header = await _invoiceHeaderRepository.GetAsync($"USP_S_T_InvoiceHeader '{CompanyCode}','{StoreId}','{Id}' ", null, commandType: CommandType.Text);
                if (header == null)
                {
                    result.Success = false;
                    result.Message = "Invoice doesn't existed";
                    return result;
                }
                string queryLine = $"USP_S_T_InvoiceLine '{CompanyCode}','{StoreId}','{Id}'";
                string queryLineSerial = $"select t1.* , t2.ItemName , t3.UOMName from T_InvoiceLineSerial t1 with(nolock) left join M_Item t2 with(nolock)  on t1.ItemCode = t2.ItemCode AND T1.CompanyCode = t2.CompanyCode left join M_UOM t3 with(nolock)  on t1.UOMCode = t3.UOMCode where t1.TransId = '{Id}' and t1.CompanyCode = '{CompanyCode}'";
                string queryPromo = $"select t1.* , t2.ItemName , t3.UOMName from T_InvoicePromo t1 with(nolock)  left join M_Item t2 with(nolock)  on t1.ItemCode = t2.ItemCode AND T1.CompanyCode=t2.CompanyCode left join M_UOM t3 with(nolock)  on t1.UOMCode = t3.UOMCode where t1.TransId = '{Id}' and t1.CompanyCode = '{CompanyCode}'";
                string queryPayment = $"select * from T_InvoicePayment with (nolock) where TransId='{Id}' and CompanyCode= '{CompanyCode}'";

                //List<TInvoiceLine> lines = await _invoiceLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                List<TInvoicePayment> payments = await _invoicepaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId ='{header.CusId}'", null, commandType: CommandType.Text);
      
                var head = _mapper.Map<InvoiceViewModel>(header);
                using (IDbConnection db = _invoiceHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TInvoiceLineViewModel>(queryLine, null, commandType: CommandType.Text);
                        var serialLines = db.Query<TInvoiceLineSerialViewModel>(queryLineSerial, null, commandType: CommandType.Text);
                        foreach (var line in lines)
                        {
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                        }

                        var promoLines = db.Query<TInvoicePromoViewModel>(queryPromo, null, commandType: CommandType.Text);
                        order = _mapper.Map<InvoiceViewModel>(header);
                        order.Lines = lines.ToList();
                        order.SerialLines = serialLines.ToList();
                        order.PromoLines = promoLines.ToList();
                        order.Payments = payments;
                        order.Customer = customer;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

               
                result.Success = true;
                result.Data = order;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            

        }

        public async Task<GenericResult> CheckARExistedBySoId(string SOId, string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                 
                string query= $"select COUNT(*) from T_InvoiceHeader with (nolock) where RefTransId = N'{SOId}' and CompanyCode = '{CompanyCode}' and StoreId = '{StoreId}' and Status = 'C' and IsCanceled = 'N'";

                var header =  _invoiceHeaderRepository.GetScalar(query, null, commandType: CommandType.Text);
                if (header == null || header == "0")
                {
                    result.Success = false; 
                    return result;
                } 
                result.Success = true; 
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;


        }

        public async Task<GenericResult> GetCheckedPayment(string TransId, string EventId, string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                InvoiceViewModel order = new InvoiceViewModel();

                TInvoiceHeader header = await _invoiceHeaderRepository.GetAsync($"select * from T_SalesHeader with (nolock) where TransId='{TransId}' and CompanyCode= '{CompanyCode}' and StoreId= '{StoreId}'", null, commandType: CommandType.Text);

                string queryLine = $"select t1.* from T_InvoiceLine t1 with (nolock)  left join T_InvoiceHeader t2 with (nolock) on t1.TransId = t2.TransId and t2.CompanyCode= '{CompanyCode}' and t2.StoreId= '{StoreId}' where t2.ContractNo = '{EventId}'";
                //queryLineSerial
                string queryPromo = $"select t1.* , t3.ItemName , t4.UOMName from T_InvoicePromo t1 with(nolock) " +
                    $" left join T_InvoiceHeader t2 with(nolock) on t1.TransId = t2.TransId and t2.CompanyCode = t1.CompanyCode " +
                    $" left join M_Item t3 with(nolock)  on t1.ItemCode = t3.ItemCode AND T1.CompanyCode = t3.CompanyCode" +
                    $" left join M_UOM t4 with(nolock)  on t1.UOMCode = t4.UOMCode" +
                    $" where t2.RefTransId = '{TransId}' and t2.CompanyCode = '{CompanyCode}' and t2.StoreId = '{StoreId}'";
                string queryLineSerial = $"select t1.* from T_InvoiceLineSerial t1 with (nolock)   left join T_InvoiceHeader t2 with (nolock) on t1.TransId = t2.TransId and t2.CompanyCode= t1.CompanyCode  " +
                    $" where t2.ContractNo = '{EventId}' and  t2.CompanyCode= '{CompanyCode}' and t2.StoreId= '{StoreId}'";

                string queryPayment = $" select t1.* from  T_InvoicePayment t1 with(nolock) left join T_InvoiceHeader t2 with(nolock) on t1.TransId = t2.TransId and t2.CompanyCode = '{CompanyCode}' and t2.StoreId = '{StoreId}' where t2.ContractNo = '{EventId}'";

                //List<TInvoiceLine> lines = await _invoiceLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                List<TInvoicePayment> payments = await _invoicepaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId ='{header.CusId}'", null, commandType: CommandType.Text);

                var head = _mapper.Map<InvoiceViewModel>(header);
                using (IDbConnection db = _invoiceHeaderRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        var lines = db.Query<TInvoiceLineViewModel>(queryLine, null, commandType: CommandType.Text);
                        var serialLines = db.Query<TInvoiceLineSerialViewModel>(queryLineSerial, null, commandType: CommandType.Text);
                        foreach (var line in lines)
                        {
                            line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                        }

                        var promoLines = db.Query<TInvoicePromoViewModel>(queryPromo, null, commandType: CommandType.Text);
                        order = _mapper.Map<InvoiceViewModel>(header);
                        order.Lines = lines.ToList();
                        order.SerialLines = serialLines.ToList();
                        order.PromoLines = promoLines.ToList();
                        order.Payments = payments;
                        order.Customer = customer;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                  
                result.Success = true;
                result.Data = order;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

            

        }


        public Task<TInvoiceHeader> GetByUser(string User)
        {
            throw new NotImplementedException();
        }
        public async Task<GenericResult> GetCheckOutList(string EventId, string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var header = await _invoiceHeaderRepository.GetAllAsync($"USP_S_CheckOutByDate '{CompanyCode}', '{EventId}'", null, commandType: CommandType.Text);
                
                result.Success = true;
                result.Data = header;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            
        }
        public async Task<List<TInvoiceLine>> GetLinesById(string Id)
        {
            var data = await _invoiceLineRepository.GetAllAsync($"select * from T_InvoiceLine with (nolock) where TransId = N'%{Id}%'", null, commandType: CommandType.Text);
            return data;
        }

        public async Task<string> GetNewOrderCode(string companyCode, string storeId)
        {
            string key = _invoiceHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixAR}','{companyCode}','{storeId}')", null, commandType: CommandType.Text);
            return key;
        }

        public async Task<PagedList<TInvoiceHeader>> GetPagedList(UserParams userParams)
        {
            try
            {
                string query = $"select * from T_InvoiceHeader with (nolock) " +
                    $"where ( Remarks like N'%{userParams.keyword}%' or TransId like N'%{userParams.keyword}%' or StoreId like N'%{userParams.keyword}%' or CusId like N'%{userParams.keyword}%'  or InvoicePerson like N'%{userParams.keyword}%' )";
                if (!string.IsNullOrEmpty(userParams.status))
                {
                    query += $" and Status='{userParams.status}'";
                }
                var data = await _invoiceHeaderRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.CusId);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.TransId);
                }
                return await PagedList<TInvoiceHeader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public Task<GenericResult> Update(TInvoiceHeader model)
        {
            throw new NotImplementedException();
        }
    }

}
