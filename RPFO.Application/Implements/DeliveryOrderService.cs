
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
using static RPFO.Application.Implements.SaleService;

namespace RPFO.Application.Implements
{
    public class DeliveryOrderService : IDeliveryOrderService
    {
        private readonly IGenericRepository<TDeliveryHeader> _headerRepository;
        private readonly IGenericRepository<TDeliveryLine> _lineRepository;
        private readonly IGenericRepository<TDeliveryLineSerial> _lineSerialRepository;
        private readonly IGenericRepository<MBomline> _bomLineRepository;
        private readonly IGeneralSettingService _settingService;
        private readonly IBOMService _bomeService;
        private readonly IItemService _itemService;
        //private readonly IGenericRepository<TPurchaseOrderHeader> _PurchaseHeaderRepository;
        //private readonly IGenericRepository<MCustomer> _customerRepository;
        IMapper _mapper;
        private IResponseCacheService cacheService;
        private string PrefixCacheActionGR = "QAGR-{0}-{1}";
        private string PrefixDO = "";
        private TimeSpan timeQuickAction = TimeSpan.FromSeconds(15);

        private readonly ICommonService _commonService;
        string ServiceName = "T_Delivery";
        List<string> TableNameList = new List<string>();

        public DeliveryOrderService(IGenericRepository<TDeliveryHeader> headerRepository, IConfiguration config, ICommonService commonService, IGenericRepository<MBomline> bomLineRepository,
            IGenericRepository<TDeliveryLineSerial> lineSerialRepository, IGeneralSettingService settingService, IBOMService bomeService,
            IGenericRepository<TDeliveryLine> lineRepository, IGenericRepository<MCustomer> customerRepository, IItemService itemService,
            IGenericRepository<TPurchaseOrderHeader> purchaseHeaderRepository, IMapper mapper, IResponseCacheService responseCacheService /*, IHubContext<RequestHub> hubContext IGenericRepository<TPurchaseOrderPayment> invoicepaymentLineRepository,*/
)//: base(hubContext)
        {
            _headerRepository = headerRepository;
            _lineRepository = lineRepository;
            _lineSerialRepository = lineSerialRepository;
            _settingService = settingService;
            _itemService = itemService;
            _bomeService = bomeService;
            _bomLineRepository = bomLineRepository;
            _commonService = commonService;
            //_GRPOpaymentLineRepository = invoicepaymentLineRepository;
            //_customerRepository = customerRepository;
            _mapper = mapper;
            //_PurchaseHeaderRepository = purchaseHeaderRepository;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            this.cacheService = responseCacheService;
            TableNameList.Add(ServiceName + "OrderLine");
            _commonService.InitService(ServiceName, TableNameList);
            string timeCache = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("TimeCacheAction"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            if (!string.IsNullOrEmpty(timeCache) && double.TryParse(timeCache, out double timeAction))
            {
                timeQuickAction = TimeSpan.FromSeconds(timeAction);
            }
            PrefixDO = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixDO"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            if (string.IsNullOrEmpty(PrefixDO))
            {
                PrefixDO = "DO";
            }
        }


        public class ResultModel
        {
            public int ID { get; set; }
            public string Message { get; set; }
        }
        //public async Task<GenericResult> UpdateStatus(GRPOViewModel model)
        //{
        //    GenericResult result = new GenericResult();
        //    if (model.DocDate == null)
        //    {
        //        result.Success = false;
        //        result.Message = "Doc date not null.";
        //        return result;
        //    }
        //    if (model.Lines == null || model.Lines.Count() == 0)
        //    {
        //        result.Success = false;
        //        result.Message = "Doc line not null.";
        //        return result;
        //    }

        //    if (model.StoreId == null)
        //    {
        //        result.Success = false;
        //        result.Message = "From Store / To Store not null.";
        //        return result;
        //    }

        //    try
        //    {

        //        using (IDbConnection db = _GRPOHeaderRepository.GetConnection())
        //        {
        //            try
        //            {
        //                if (db.State == ConnectionState.Closed)
        //                    db.Open();
        //                using (var tran = db.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var parameters = new DynamicParameters();

        //                        string query = $"update T_GoodsReceiptPOHeader set Status = 'C',  DocStatus = 'C' where CompanyCode='{model.CompanyCode}' and PurchaseId = '{model.PurchaseId}'  and StoreId = '{model.StoreId}'";
        //                        var delAffectedRows = db.Execute(query, parameters, commandType: CommandType.Text, transaction: tran); 
        //                        result.Success = true; 
        //                        tran.Commit();

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        tran.Rollback();
        //                        throw ex;
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                if (db.State == ConnectionState.Open)
        //                    db.Close();
        //            }
        //            return result;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //    }
        //    return result;

        //}
        public async Task<List<MBomline>> getBOMLine(string itemCode)
        {


            try
            {
                List<MBomline> lines = await _bomLineRepository.GetAllAsync($"select * from M_BOMLine with (nolock) where BOMId=N'{itemCode}'", null, commandType: CommandType.Text);
                return lines;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<GenericResult> CreateByDate(string CompanyCode, string Date, string CreatedBy)
        {
            //throw new NotImplementedException();
            GenericResult result = new GenericResult();

            try
            {
                //var data = await _DeliveryHeaderlRepository.GetAllAsync($"USP_GetDelivery", parameters, commandType: CommandType.StoredProcedure);
                using (IDbConnection db = _headerRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();


                        string POLineTbl = ServiceName + "Line";
                        var POLines = _commonService.CreaDataTable(POLineTbl);

                        string POHeaderTbl = ServiceName + "Header";
                        var POTbl = _commonService.CreaDataTable(POHeaderTbl);


                        string getItemType = $"select t1.* from T_ShippingDivisionLine t1 with (nolock)   " +
                            $"left join  T_ShippingDivisionheader t2 with (nolock)  on t1.Id= t2.Id where CONVERT(date ,t2.DocDate) = CONVERT(date , '{Date}') ";
                        //_ShippingDivisionLineRepository.GetAll();
                        var customerList = await db.QueryAsync<TShippingDivisionLine>(getItemType, null, commandType: CommandType.Text, commandTimeout: 3600);
                        var ListA = new List<TDeliveryLine>();
                        var ListHeader = new List<TDeliveryHeader>();
                        foreach (var customer in customerList)
                        {
                            var newDeli = new TDeliveryHeader();
                            newDeli.CompanyCode = CompanyCode;
                            newDeli.TransId = "";
                            newDeli.StoreId = customer.CusId;
                            newDeli.ContractNo = "";
                            newDeli.CusId = customer.CusId;
                            newDeli.TotalAmount = 0;
                            newDeli.TotalPayable = 0;
                            newDeli.TotalTax = 0;
                            newDeli.CreatedBy = CreatedBy;
                            newDeli.IsCanceled = "N";
                            newDeli.Remarks = customer.Remark;
                            newDeli.RefTransId = "";
                            newDeli.LuckyNo = "";
                            newDeli.DeliveryBy = customer.ShippingCode;
                            //newDeli.RefTransId = "";

                            decimal TotalAmount = 0;
                            decimal TotalPayable = 0;
                            decimal TotalTax = 0;

                            string query = $"[USP_GetDOLineFromDivisionBy] N'{CompanyCode}', N'{customer.CusId }', N'{Date}'";
                            var itemsX = await db.QueryAsync<TDeliveryLine>(query, null, commandType: CommandType.Text, commandTimeout: 3600);
                            if (itemsX != null && itemsX.ToList().Count > 0)
                            {
                                var queryDivision = itemsX.Where(x => x.Result == false);

                                var newList = new List<TDeliveryLine>();
                                newList = itemsX.Where(x => x.Result == true).ToList();
                                foreach (var itemU in newList)
                                {
                                    itemU.DeliveryQty = itemU.OrgQty;
                                    itemU.Quantity = itemU.OrgQty;

                                }
                                foreach (var itemX in queryDivision)
                                {
                                    if (itemX.DivisionQty > 0 && itemX.DivisionQty > itemX.OrgQty)
                                    {
                                        //itemX.Quantity = itemX.OrgQty;
                                        itemX.DeliveryQty = itemX.OrgQty;
                                    }
                                    else
                                    {
                                        itemX.DeliveryQty = itemX.DivisionQty;
                                        //itemX.Quantity = itemX.DivisionQty ;
                                    }
                                    if (itemX.DeliveryQty != 0)
                                    {
                                        itemX.Quantity = itemX.DeliveryQty;
                                        var newNumDivisionQty = itemX.DivisionQty - itemX.Quantity;

                                        var listItem = queryDivision.Where(x => x.ItemCode == itemX.ItemCode && x.UomCode == itemX.UomCode);
                                        foreach (var itemU in listItem)
                                        {
                                            itemU.DivisionQty = newNumDivisionQty;
                                        }
                                        newList.Add(itemX);
                                    }
                                }
                                ListA.AddRange(newList);
                            }
                            TotalPayable = ListA.Sum(x => x.LineTotal.HasValue ? x.LineTotal.Value : 0);
                            TotalAmount = TotalPayable;
                            TotalTax = ListA.Sum(x => x.TaxAmt.HasValue ? x.TaxAmt.Value : 0);
                            
                            newDeli.TotalPayable = TotalPayable;
                            newDeli.TotalAmount = TotalAmount;
                            newDeli.TotalTax = TotalTax;

                            ListHeader.Add(newDeli);
                            //string itemType = db.GetList(getItemType, null, commandType: CommandType.Text);
                        }

                        POLines = ExtensionsNew.ConvertListToDataTable(ListA, POLines);
                        POTbl = ExtensionsNew.ConvertListToDataTable(ListHeader, POTbl);

                        string tblLineType = POLineTbl + "TableType";

                        var parameters = new DynamicParameters();

                        //parameters.Add("Id", model.Id, DbType.String);
                        parameters.Add("CompanyCode", CompanyCode);
                        //parameters.Add("StoreId", model.StoreId);
                        //parameters.Add("StoreName", model.StoreName);
                        //parameters.Add("ContractNo", model.ContractNo);
                        //parameters.Add("ShiftId", model.ShiftId);
                        parameters.Add("CreatedBy", CreatedBy);
                        parameters.Add("@Headers", POTbl.AsTableValuedParameter(POHeaderTbl + "TableType"));
                        parameters.Add("@Lines", POLines.AsTableValuedParameter("T_DeliveryOrderLineTableType"));
                        //parameters.Add("@LineSerials", POLineSerial.AsTableValuedParameter(POSerialTbl + "TableType"));
                         //db.Insert("USP_I_T_DeliveryOrderByDate", parameters, commandType: CommandType.StoredProcedure);
                         db.Execute("USP_I_T_DeliveryOrderByDate", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                        result.Success = true;

                        //using (var tran = db.BeginTransaction())
                        //{

                        //}
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
                }
            }

            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> Create(TDeliveryHeader model)
        {
            GenericResult result = new GenericResult();
            string flag = "";
          
            try
            {
                #region check data
                if (string.IsNullOrEmpty(model.DataSource))
                {
                    result.Success = false;
                    result.Message = "Data Source can't null.";
                    return result;
                }
                

                var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                if (settingData.Success)
                {

                    SettingList = settingData.Data as List<GeneralSettingStore>;


                }

               
                if (string.IsNullOrEmpty(model.POSType))
                {
                    model.POSType = "";
                }
                 
                if (model.CompanyCode == null || string.IsNullOrEmpty(model.CompanyCode))
                {
                    result.Success = false;
                    result.Message = "Company Code cannot null.";
                    return result;
                }
                if (model.Lines == null || model.Lines.Count() == 0)
                {
                    result.Success = false;
                    result.Message = "Doc line cannot null.";
                    return result;
                }
                if (model.StoreId == null || string.IsNullOrEmpty(model.StoreId))
                {
                    result.Success = false;
                    result.Message = "Store cannot null.";
                    return result;
                }
                if (string.IsNullOrEmpty(model.CusId))
                {
                    result.Success = false;
                    result.Message = "Customer cannot null.";
                    return result;
                }
                if (string.IsNullOrEmpty(model.SalesMode))
                {
                    result.Success = false;
                    result.Message = "SalesMode cannot null.";
                    return result;
                }
                 

                //if (!string.IsNullOrEmpty(model.OrderId.ToString()))
                //{
                //    var OrderIdparameters = new DynamicParameters();
                //    OrderIdparameters.Add("CompanyCode", model.CompanyCode);
                //    OrderIdparameters.Add("StoreId", model.StoreId);
                //    OrderIdparameters.Add("OrderId", model.OrderId);

                //    var socheck = await _headerRepository.GetAllAsync("USP_S_T_SalesHeaderByOrderId", OrderIdparameters, commandType: CommandType.StoredProcedure);
                //    if (socheck != null && socheck.Count > 0)
                //    {
                //        var firstTran = socheck.FirstOrDefault();
                //        if (firstTran != null)
                //        {
                //            result.Success = false;
                //            result.Message = "Order has been created or processing. Trans Id: " + firstTran.TransId + " Order Id: " + model.OrderId;
                //            string checkTransId = await CheckOrderData(model.CompanyCode, model.StoreId, model.TransId, model.TotalAmount, model.Lines.Count(), model.Lines.Sum(x => x.Quantity));
                //            if (!string.IsNullOrEmpty(checkTransId))
                //            {
                //                result.Data = checkTransId;
                //            }
                //            string Folder = Path.Combine(
                //             Directory.GetCurrentDirectory(),
                //             "wwwroot", "Logs");
                //            if (!Directory.Exists(Folder))
                //                Directory.CreateDirectory(Folder);
                //            string filename = firstTran.TransId + "Double";
                //            var path = Path.Combine(
                //                       Directory.GetCurrentDirectory(),
                //                       "wwwroot/Logs", "");
                //            LogUtils.WriteLogData(path, "", filename, model.ToJson());
                //            return result;
                //        }

                //    }
                //}
                //if (model.SalesMode.ToLower() == "return" || ((model.SalesMode.ToLower() == "ex" || model.SalesMode.ToLower() == "exchange") && model.TotalPayable < 0))
                //{
                //    decimal? numOfPayment = 0;
                //    foreach (var line in model.Payments)
                //    {
                //        numOfPayment += line.CollectedAmount;
                //    }
                //    if (Math.Abs((decimal)numOfPayment) != Math.Abs((decimal)model.TotalPayable))
                //    {
                //        result.Success = false;
                //        result.Message = "Please check return amount. Return amount can't different collected amount.";
                //        return result;
                //    }

                //}
                //if (model.SalesMode.ToLower() == "return")
                //{
                //    decimal? numOfPayment = 0;
                //    foreach (var line in model.Payments)
                //    {
                //        numOfPayment += line.CollectedAmount;
                //    }
                //    if (Math.Abs((decimal)model.TotalPayable) > Math.Abs((decimal)numOfPayment) )
                //    {
                //        result.Success = false;
                //        result.Message = "Please check return amount. Total amount can't less than return amount.";
                //        return result;
                //    }
                //    //var ItemCannotCancel = model.Lines.Where(line => string.IsNullOrEmpty( line.ItemType.ToLower() == "pn" || line.ItemType.ToLower() == "tp" || line.ItemType.ToLower() == "pin" || line.ItemType.ToLower() == "bp" || line.ItemType.ToLower() == "topup");
                //    //if (ItemCannotCancel != null && ItemCannotCancel.Count() > 0)
                //    //{
                //    //    result.Success = false;
                //    //    result.Message = "Order (PIN/TopUp) can't return.";
                //    //    return result;
                //    //}
                //}
                if (string.IsNullOrEmpty(model.IsCanceled))
                {
                    model.IsCanceled = "N";
                }
                //if (model.IsCanceled == "C" && model.Status != "H" && model.Status != "Hold")
                //{
                //    string userCheck = model.ApprovalId ?? model.CreatedBy;
                //    var checkCancel = await _permissionService.CheckFunctionByUserName(model.CompanyCode, userCheck, "Spc_CancelOrder", "", "I");
                //    if (checkCancel != null && checkCancel.Success)
                //    {

                //    }
                //    else
                //    {
                //        return checkCancel;
                //    }
                //}
                if (model.DataSource.ToLower() != "pos")
                {
                    if (string.IsNullOrEmpty(model.TotalAmount.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Amount. Total Amount can't null";
                        return result;
                    }
                    if (string.IsNullOrEmpty(model.TotalPayable.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Payable. Total Payable can't null";
                        return result;
                    }
                    if (string.IsNullOrEmpty(model.TotalDiscountAmt.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Discount Amount. Total Discount Amount can't null";
                        return result;
                    }
                    if (string.IsNullOrEmpty(model.TotalReceipt.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Receipt. Total Receipt can't null";
                        return result;
                    }
                    if (string.IsNullOrEmpty(model.PaymentDiscount.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Payment Discount. Payment Discount can't null";
                        return result;
                    }
                    if (string.IsNullOrEmpty(model.TotalTax.ToString()))
                    {
                        result.Success = false;
                        result.Message = "Please Input Total Tax. Total Tax can't null";
                        return result;
                    }
                    //var listLine = model.Lines.Where(x => x.IsSerial == true || x.IsVoucher == true).ToList();
                    //if(listLine!=null && listLine.Count > 0)
                    //{
                    //    var checkValidSerialNum = listLine.Where(x=>x.lin)
                    //}    

                    List<ItemCheckModel> itemCheckList = new List<ItemCheckModel>();
                    if (model.Lines != null)
                    {
                        var qtyCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.Quantity.ToString()) || x.Quantity == 0).ToList();
                        string mes = "";
                        if (qtyCheck != null && qtyCheck.Count > 0)
                        {
                            foreach (var line in qtyCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Quantity of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }
                        var priceCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.Price.ToString())).ToList();

                        if (priceCheck != null && priceCheck.Count > 0)
                        {
                            foreach (var line in priceCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Price of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }
                        var lineTotalCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.LineTotal.ToString())).ToList();

                        if (lineTotalCheck != null && lineTotalCheck.Count > 0)
                        {
                            foreach (var line in lineTotalCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Line Total of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }



                        var itemCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.ItemCode.ToString())).ToList();

                        if (itemCodeCheck != null && itemCodeCheck.Count > 0)
                        {
                            result.Success = false;
                            result.Message = "Please Input Item Code";
                            return result;
                        }
                        var DiscountRateCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.DiscountRate.ToString())).ToList();

                        if (DiscountRateCheck != null && DiscountRateCheck.Count > 0)
                        {
                            foreach (var line in DiscountRateCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Discount Rate of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }

                        var TaxRateCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.TaxRate.ToString())).ToList();

                        if (TaxRateCheck != null && TaxRateCheck.Count > 0)
                        {
                            foreach (var line in TaxRateCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Tax Rate of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }
                        var TaxAmtCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.TaxAmt.ToString())).ToList();

                        if (TaxAmtCheck != null && TaxAmtCheck.Count > 0)
                        {
                            foreach (var line in TaxAmtCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input Tax Amt of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }
                        //var TaxCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.TaxCode)).ToList();

                        //if (TaxCodeCheck != null && TaxCodeCheck.Count > 0)
                        //{
                        //    foreach (var line in TaxCodeCheck)
                        //    {
                        //        mes += line.ItemCode + ", ";
                        //    }
                        //    if (!string.IsNullOrEmpty(mes))
                        //    {
                        //        result.Success = false;
                        //        result.Message = "Please Input Tax Code of items: " + mes.Substring(0, mes.Length - 2);
                        //        return result;
                        //    }
                        //}



                        var UOMCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.UomCode)).ToList();

                        if (UOMCodeCheck != null && UOMCodeCheck.Count > 0)
                        {
                            foreach (var line in UOMCodeCheck)
                            {
                                mes += line.ItemCode + ", ";
                            }
                            if (!string.IsNullOrEmpty(mes))
                            {
                                result.Success = false;
                                result.Message = "Please Input UOM Code of items: " + mes.Substring(0, mes.Length - 2);
                                return result;
                            }
                        }
                        foreach (var line in model.Lines)
                        {
                            ItemCheckModel itemCheck = new ItemCheckModel();
                            itemCheck.ItemCode = String.IsNullOrEmpty(line.ItemCode) ? "" : line.ItemCode;
                            itemCheck.UomCode = String.IsNullOrEmpty(line.UomCode) ? "" : line.UomCode;
                            itemCheck.Barcode = String.IsNullOrEmpty(line.BarCode) ? "" : line.BarCode;
                            itemCheckList.Add(itemCheck);
                        }
                        string ItemFailed = "";
                        var itemListFilter = _itemService.GetItemFilter(model.CompanyCode, model.StoreId, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", null, null, null, null, "", "", "", null, null, null, itemCheckList).Result;
                        if (itemListFilter != null && itemListFilter.Success && itemListFilter.Data != null)
                        {


                            var itemList = itemListFilter.Data as List<ItemViewModel>;

                            if (itemList.Count < itemCheckList.Count)
                            {

                                foreach (var line in itemCheckList)
                                {
                                    var itemFind = itemList.Where(x => x.ItemCode == line.ItemCode);
                                    if (itemFind != null)
                                    {

                                    }
                                    else
                                    {
                                        ItemFailed += line.ItemCode + ";";
                                    }

                                }
                                if (string.IsNullOrEmpty(ItemFailed))
                                {
                                    ItemFailed = ItemFailed.Substring(0, mes.Length - 1);
                                }
                                result.Success = false;
                                result.Message = "Please Items: " + ItemFailed + " in store " + model.StoreId;
                                return result;
                            }
                        }
                        else
                        {
                            foreach (var line in itemCheckList)
                            {
                                ItemFailed += line.ItemCode + ";";
                            }
                            if (string.IsNullOrEmpty(ItemFailed))
                            {
                                ItemFailed = ItemFailed.Substring(0, mes.Length - 1);
                            }
                            result.Success = false;
                            result.Message = "Please Items: " + ItemFailed + " in store " + model.StoreId;
                            return result;
                        }
                    }

                }
                if (model.Lines != null)
                {
                    var qtyCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.Quantity.ToString()) || x.Quantity == 0).ToList();
                    string mes = "";
                    if (qtyCheck != null && qtyCheck.Count > 0)
                    {
                        foreach (var line in qtyCheck)
                        {
                            mes += line.ItemCode + ", ";
                        }
                        if (!string.IsNullOrEmpty(mes))
                        {
                            result.Success = false;
                            result.Message = "Please Input Quantity of items: " + mes.Substring(0, mes.Length - 2);
                            return result;
                        }
                    }
                }
                if (model.IsCanceled != "C")
                {
                    var TaxCodeCheck = model.Lines.Where(x => string.IsNullOrEmpty(x.BomId) && string.IsNullOrEmpty(x.TaxCode)).ToList();

                    if (TaxCodeCheck != null && TaxCodeCheck.Count > 0)
                    {
                        string mes = "";
                        foreach (var line in TaxCodeCheck)
                        {
                            mes += line.ItemCode + ", ";
                        }
                        if (!string.IsNullOrEmpty(mes))
                        {
                            result.Success = false;
                            result.Code = 701;
                            result.Message = "Tax Code of items: " + mes.Substring(0, mes.Length - 2) + " null.";
                            return result;
                        }
                    }
                }


              
                 
                #endregion

                flag = "After Check Data";
                List<string> holdList = new List<string>();
                using (IDbConnection db = _headerRepository.GetConnection())
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        using (var tran = db.BeginTransaction())
                        {

                            string key = "";

                            var SOLines = new DataTable("T_DeliveryOrderLineTableType");
                            SOLines.Columns.Add("TransId", typeof(string));
                            SOLines.Columns.Add("LineId", typeof(string));
                            SOLines.Columns.Add("CompanyCode", typeof(string));
                            SOLines.Columns.Add("ItemCode", typeof(string));
                            SOLines.Columns.Add("SLocId", typeof(string));
                            SOLines.Columns.Add("BarCode", typeof(string));
                            SOLines.Columns.Add("UOMCode", typeof(string));
                            SOLines.Columns.Add("Quantity", typeof(decimal));
                            SOLines.Columns.Add("Price", typeof(decimal));
                            SOLines.Columns.Add("LineTotal", typeof(decimal));
                            SOLines.Columns.Add("DiscountType", typeof(string));
                            SOLines.Columns.Add("DiscountAmt", typeof(decimal));
                            SOLines.Columns.Add("DiscountRate", typeof(decimal));
                            SOLines.Columns.Add("CreatedBy", typeof(string));
                            SOLines.Columns.Add("CreatedOn", typeof(DateTime));
                            SOLines.Columns.Add("ModifiedBy", typeof(string));
                            SOLines.Columns.Add("ModifiedOn", typeof(DateTime));
                            SOLines.Columns.Add("Status", typeof(string));
                            SOLines.Columns.Add("Remark", typeof(string));
                            SOLines.Columns.Add("PromoId", typeof(string));
                            SOLines.Columns.Add("PromoType", typeof(string));
                            SOLines.Columns.Add("PromoPercent", typeof(decimal));
                            SOLines.Columns.Add("PromoBaseItem", typeof(string));
                            SOLines.Columns.Add("SalesMode", typeof(string));
                            SOLines.Columns.Add("TaxRate", typeof(decimal));
                            SOLines.Columns.Add("TaxAmt", typeof(decimal));
                            SOLines.Columns.Add("TaxCode", typeof(string));
                            SOLines.Columns.Add("MinDepositAmt", typeof(decimal));
                            SOLines.Columns.Add("MinDepositPercent", typeof(decimal));
                            SOLines.Columns.Add("DeliveryType", typeof(string));
                            SOLines.Columns.Add("POSService", typeof(string));
                            SOLines.Columns.Add("StoreAreaId", typeof(string));
                            SOLines.Columns.Add("TimeFrameId", typeof(string));
                            SOLines.Columns.Add("Duration", typeof(int));
                            SOLines.Columns.Add("AppointmentDate", typeof(DateTime));
                            SOLines.Columns.Add("BomID", typeof(string));
                            SOLines.Columns.Add("PromoPrice", typeof(decimal));
                            SOLines.Columns.Add("PromoLineTotal", typeof(decimal));
                            SOLines.Columns.Add("BaseLine", typeof(string));
                            SOLines.Columns.Add("BaseTransId", typeof(string));
                            SOLines.Columns.Add("OpenQty", typeof(decimal));
                            SOLines.Columns.Add("PromoDisAmt", typeof(decimal));
                            SOLines.Columns.Add("IsPromo", typeof(string));
                            SOLines.Columns.Add("IsSerial", typeof(bool));
                            SOLines.Columns.Add("IsVoucher", typeof(bool));
                            SOLines.Columns.Add("PrepaidCardNo", typeof(string));
                            SOLines.Columns.Add("MemberDate", typeof(DateTime));
                            SOLines.Columns.Add("MemberValue", typeof(int));
                            SOLines.Columns.Add("StartDate", typeof(DateTime));
                            SOLines.Columns.Add("EndDate", typeof(DateTime));
                            SOLines.Columns.Add("ItemType", typeof(string));
                            SOLines.Columns.Add("Description", typeof(string));
                            SOLines.Columns.Add("LineTotalBefDis", typeof(decimal));
                            SOLines.Columns.Add("LineTotalDisIncludeHeader", typeof(decimal));
                            SOLines.Columns.Add("SerialNum", typeof(string));
                            SOLines.Columns.Add("Name", typeof(string));
                            SOLines.Columns.Add("Phone", typeof(string));
                            SOLines.Columns.Add("ItemTypeS4", typeof(string));
                            SOLines.Columns.Add("Custom1", typeof(string));
                            SOLines.Columns.Add("Custom2", typeof(string));
                            SOLines.Columns.Add("Custom3", typeof(string));
                            SOLines.Columns.Add("Custom4", typeof(string));
                            SOLines.Columns.Add("Custom5", typeof(string));
                            SOLines.Columns.Add("PriceListId", typeof(string));
                            SOLines.Columns.Add("ProductId", typeof(string));
                            SOLines.Columns.Add("WeightScaleBarcode", typeof(string));
                            SOLines.Columns.Add("StoreId", typeof(string));
                            SOLines.Columns.Add("BookletNo", typeof(string));
                            SOLines.Columns.Add("OrgQty", typeof(decimal));
                            SOLines.Columns.Add("DeliveryQty", typeof(decimal));
                            SOLines.Columns.Add("ReceiptQty", typeof(decimal));



                            var SOLineSerials = new DataTable("T_DeliveryOrderLineSerialTableType");
                            SOLineSerials.Columns.Add("TransId", typeof(string));
                            SOLineSerials.Columns.Add("LineId", typeof(string));
                            SOLineSerials.Columns.Add("CompanyCode", typeof(string));
                            SOLineSerials.Columns.Add("ItemCode", typeof(string));
                            SOLineSerials.Columns.Add("SerialNum", typeof(string));
                            SOLineSerials.Columns.Add("SLocId", typeof(string));
                            SOLineSerials.Columns.Add("Quantity", typeof(string));
                            SOLineSerials.Columns.Add("UOMCode", typeof(string));
                            SOLineSerials.Columns.Add("CreatedBy", typeof(string));
                            SOLineSerials.Columns.Add("CreatedOn", typeof(DateTime));
                            SOLineSerials.Columns.Add("ModifiedBy", typeof(string));
                            SOLineSerials.Columns.Add("ModifiedOn", typeof(DateTime));
                            SOLineSerials.Columns.Add("Status", typeof(string));
                            SOLineSerials.Columns.Add("OpenQty", typeof(decimal));
                            SOLineSerials.Columns.Add("BaseLine", typeof(string));
                            SOLineSerials.Columns.Add("BaseTransId", typeof(string));
                            SOLineSerials.Columns.Add("LineNum", typeof(int));
                            SOLineSerials.Columns.Add("Description", typeof(string));
                            SOLineSerials.Columns.Add("Phone", typeof(string));
                            SOLineSerials.Columns.Add("Name", typeof(string));
                            SOLineSerials.Columns.Add("CustomF1", typeof(string));
                            SOLineSerials.Columns.Add("CustomF2", typeof(string));
                            SOLineSerials.Columns.Add("Prefix", typeof(string));
                            SOLineSerials.Columns.Add("ExpDate", typeof(DateTime));
                            SOLineSerials.Columns.Add("StoreId", typeof(string));
                            SOLineSerials.Columns.Add("CustomF3", typeof(string));
                            SOLineSerials.Columns.Add("CustomF4", typeof(string));
                            SOLineSerials.Columns.Add("CustomF5", typeof(string));

                       
                            flag = "After Create table Type";


                            //var cmdText = customers.Aggregate(
                            //    new StringBuilder(),
                            //    (sb, customer) => sb.AppendLine(@$"
                            //        insert into dbo.Customers (Id, FirstName, LastName, Street, City, State, PhoneNumber, EmailAddress)
                            //        values('{customer.Id}', '{customer.FirstName}', '{customer.LastName}', '{customer.Street}', '{customer.City}', '{customer.State}', '{customer.PhoneNumber}', '{customer.EmailAddress}')")
                            //);
                            try
                            {
                                var parameters = new DynamicParameters();
                                if ((model.DataSource.ToLower() != "pos" && model.POSType.ToLower() == "e"))
                                {
                                    foreach (var line in model.Lines)
                                    {
                                        string getItemType = $"select CustomField1 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                        string itemType = _headerRepository.GetScalar(getItemType, null, commandType: CommandType.Text);
                                        if (string.IsNullOrEmpty(itemType))
                                        {
                                            result.Success = false;
                                            result.Message = "Please check master data (Item Type ) with your admin. Item " + line.ItemCode;
                                            return result;
                                        }
                                        else
                                        {
                                            if (model.POSType.ToLower() == "e" && (itemType.ToLower() == "class" || itemType.ToLower() == "member" || itemType.ToLower() == "voucher" || itemType.ToLower() == "card"))
                                            {
                                                result.Success = false;
                                                result.Message = "Event can't order item " + line.ItemCode + " b/c item in " + itemType.ToLower() + " group";
                                                return result;
                                            }
                                        }
                                        //if()
                                    }
                                    //var ItemBan = model.Lines.Where(x=>x.ItemType)
                                }
                                if (string.IsNullOrEmpty(model.IsCanceled))
                                {
                                    model.IsCanceled = "N";
                                }
                                if (model.SalesMode.ToLower() == "return" || model.IsCanceled.ToLower() == "c")
                                {
                                    var ItemCannotCancel = model.Lines.Where(line => !string.IsNullOrEmpty(line.ItemType) && (line.ItemType.ToLower() == "pn" || line.ItemType.ToLower() == "tp" || line.ItemType.ToLower() == "pin" || line.ItemType.ToLower() == "bp" || line.ItemType.ToLower() == "topup"));
                                    if (ItemCannotCancel != null && ItemCannotCancel.Count() > 0)
                                    {
                                        result.Success = false;
                                        result.Message = "Order (PIN/TopUp) can't cancel / return.";
                                        return result;
                                    }

                                }

                                if (model.IsCanceled == "C" && ((model.Status.ToLower() == "h" || model.Status.ToLower() == "hold") || model.SalesType.ToLower() == "table"))
                                {
                                    //xxx
                                    //Status = 'C',

                                    string checkOrderStatus = $"select Status from T_SalesHeader with (nolock) where CompanyCode = '{model.CompanyCode}' and TransId = N'{model.RefTransId}' ";
                                    string statusField = _headerRepository.GetScalar(checkOrderStatus, null, commandType: CommandType.Text);
                                    if (string.IsNullOrEmpty(statusField))
                                    {
                                        statusField = "";
                                    }
                                    if (model.SalesType.ToLower() == "table")
                                    {
                                        string queryUpdate = $"Update T_DeliveryHeader set  IsCanceled = N'Y', CollectedStatus = N'Canceled' , ModifiedBy= N'{model.CreatedBy}' , ModifiedOn= N'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' where TransId=N'{model.RefTransId}' and CompanyCode=N'{model.CompanyCode}' and StoreId=N'{model.StoreId}'";
                                        db.Execute(queryUpdate, null, commandType: CommandType.Text, transaction: tran);
                                        result.Success = true;
                                        result.Message = model.RefTransId;
                                        result.Data = model;
                                        tran.Commit();
                                        return result;
                                    }
                                    else
                                    {
                                        if (statusField.ToLower() != "h")
                                        {
                                            result.Success = false;
                                            result.Message = $"Status of bill: {model.RefTransId} has been changed. Or doesn't existed";
                                            tran.Rollback();
                                            return result;
                                        }
                                        else
                                        {
                                            string queryUpdate = $"Update T_DeliveryHeader set  IsCanceled = N'Y', CollectedStatus = N'Canceled' , ModifiedBy= N'{model.CreatedBy}' , ModifiedOn= N'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' where TransId=N'{model.RefTransId}' and CompanyCode=N'{model.CompanyCode}' and StoreId=N'{model.StoreId}'";
                                            db.Execute(queryUpdate, null, commandType: CommandType.Text, transaction: tran);
                                            result.Success = true;
                                            result.Message = model.RefTransId;
                                            result.Data = model;
                                            tran.Commit();
                                            return result;
                                        }
                                    }


                                }
                                //check BOM Value
                                flag = "Check BOM";

                                var excludeBOMLines = model.Lines.Where(x => string.IsNullOrEmpty(x.BomId));
                                foreach (var line in excludeBOMLines)
                                {
                                    var bom = _bomeService.GetByItemCode(model.CompanyCode, line.ItemCode).Result;
                                    if (bom.Success)
                                    {
                                        var bomModel = bom.Data as BOMViewModel;
                                        if (bomModel != null && bomModel.Lines != null && bomModel.Lines.Count > 0)
                                        {
                                            var chekItemBOMLine = model.Lines.Where(x => x.BomId == bomModel.ItemCode);
                                            if (chekItemBOMLine == null || chekItemBOMLine.Count() == 0)
                                            {
                                                foreach (var bomline in bomModel.Lines)
                                                {
                                                    var coppyLine = new TDeliveryLine();
                                                    var sttLineX = model.Lines.Count() + 1;
                                                    coppyLine = model.Lines[0];
                                                    coppyLine.LineId = sttLineX.ToString();
                                                    coppyLine.BomId = bomModel.ItemCode;
                                                    //coppyLine.Bom = bomline.Quantity;
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
                                            //else
                                            //{

                                            //}    
                                        }
                                    }
                                }


                                var BOMLINE = model.Lines.Where(x => !string.IsNullOrEmpty(x.BomId)).ToList();
                                List<string> BOMLIst = new List<string>();
                                if (BOMLINE != null && BOMLINE.Count() > 0)
                                {
                                    foreach (var line in BOMLINE)
                                    {
                                        if (BOMLIst != null)
                                        {
                                            var itemCheck = BOMLIst.Where(x => x == line.BomId).FirstOrDefault();
                                            if (itemCheck == null)
                                            {
                                                BOMLIst.Add(line.BomId);
                                            }
                                        }
                                        else
                                        {
                                            BOMLIst.Add(line.BomId);
                                        }

                                    }
                                    foreach (string BomId in BOMLIst)
                                    {
                                        var bomLines = getBOMLine(BomId).Result;
                                        if (bomLines != null && bomLines.Count() > 0)
                                        {
                                            var bomItem = model.Lines.Where(x => x.ItemCode == BomId).FirstOrDefault();
                                            decimal SoLuongMua = bomItem.Quantity.Value;
                                            var BOMOfBOMID = BOMLINE.Where(x => x.BomId == BomId).ToList();
                                            foreach (var line in BOMOfBOMID)
                                            {
                                                if (line != null)
                                                {
                                                    var bomLine = bomLines.Where(x => x.ItemCode == line.ItemCode && x.Bomid == line.BomId).FirstOrDefault();
                                                    var quantityInline = bomLine.Quantity;
                                                    var lineTotalQty = line.LineTotal;
                                                    var quantyAfterBuy = quantityInline * SoLuongMua;
                                                    if (lineTotalQty != quantyAfterBuy)
                                                    {
                                                        line.Quantity = quantyAfterBuy;
                                                        line.LineTotal = quantyAfterBuy;
                                                        //result.Success = false;
                                                        //result.Message = "Please check your BOM Setup And Quantity Of Item.";
                                                        //return result;
                                                    }
                                                }

                                            }

                                        }


                                    }
                                }


                                string CheckCompStr = $"select CONVERT(nvarchar(50), count (*)) from M_Company with (nolock)  where CompanyCode = '{model.CompanyCode}' and REPLACE( CompanyName ,' ','') like '%JumpArena%'";
                                var checkCompValue = _headerRepository.GetScalar(CheckCompStr, null, commandType: CommandType.Text);



                                foreach (var line in model.Lines)
                                {



                                    string getItemType = $"select CustomField1 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                    string itemType = _headerRepository.GetScalar(getItemType, null, commandType: CommandType.Text);


                                    if (string.IsNullOrEmpty(itemType))
                                    {
                                        result.Success = false;
                                        result.Message = "Please check master data (Item Type ) with your admin. Item " + line.ItemCode;
                                        return result;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(line.ItemType))
                                        {
                                            line.ItemType = itemType;
                                        }
                                        if (itemType.ToLower() == "class" || itemType.ToLower() == "member")
                                        {
                                            if (string.IsNullOrEmpty(line.StartDate.ToString()) || string.IsNullOrEmpty(line.EndDate.ToString()))
                                            {
                                                result.Success = false;
                                                result.Message = "Member Class Start Date / End Date can't null";
                                                return result;
                                            }

                                        }
                                        else if (itemType.ToLower() == "card")
                                        {
                                            if (string.IsNullOrEmpty(line.StartDate.ToString()) || string.IsNullOrEmpty(line.EndDate.ToString()))
                                            {
                                                result.Success = false;
                                                result.Message = "Card Member Start Date / End Date can't null";
                                                return result;
                                            }

                                        }
                                        if (checkCompValue == "1")
                                        {
                                            string getCapacity = $"select CustomField8 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                            var capaValue = _headerRepository.GetScalar(getCapacity, null, commandType: CommandType.Text);
                                            if (!string.IsNullOrEmpty(capaValue))
                                            {
                                                if (string.IsNullOrEmpty(line.TimeFrameId))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Time Frame Id can't null";
                                                    return result;
                                                }
                                                if (string.IsNullOrEmpty(line.AppointmentDate.ToString()))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Appointment Date can't null";
                                                    return result;
                                                }

                                                if (!string.IsNullOrEmpty(line.StoreAreaId))
                                                {
                                                    string queryCheckStoreArea = $" [USP_CheckStoreAreaInStoreCapacity] N'{model.CompanyCode}', N'{model.StoreId}',N'{line.StoreAreaId}'";
                                                    var AreaCount = _headerRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
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
                                                    var AreaId = _headerRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
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
                                        }

                                    }


                                }

                                //if (model.SalesType == "Table")
                                //{
                                //    PrefixSO = Utilities.Helpers.Encryptor.DecryptString(_config.GetConnectionString("PrefixSO"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
                                //    PrefixSO = PrefixSO + "T";
                                //}

                                if (!string.IsNullOrEmpty(model.TransId))
                                {

                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("TransId", model.TransId);
                                    parameters.Add("StoreId", model.StoreId);
                                    var delAffectedRows = db.Execute("USP_D_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    key = model.TransId;
                                }
                                else
                                {
                                    key = _headerRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixDO}',N'{model.CompanyCode}', N'{model.StoreId}')", null, commandType: CommandType.Text);
                                    model.TransId = key;
                                }
                                flag = "After Create TransId";


                                // Check thêm điều kiện khi cancled thì k đc ref
                                if (!string.IsNullOrEmpty(model.RefTransId))
                                {
                                    string checkOrderStatusX = $"select IsCanceled  from T_SalesHeader with (nolock) where CompanyCode = '{model.CompanyCode}' and TransId = N'{model.RefTransId}' ";
                                    string statusFieldX = _headerRepository.GetScalar(checkOrderStatusX, null, commandType: CommandType.Text);
                                    if (string.IsNullOrEmpty(statusFieldX))
                                    {
                                        statusFieldX = "";
                                    }
                                    if (statusFieldX.ToLower() != "n")
                                    {
                                        result.Success = false;
                                        result.Message = $"Status of bill: {model.RefTransId} has been changed. Or doesn't existed";
                                        tran.Rollback();
                                        return result;
                                    }
                                }

                                if (model.DataSource.ToLower() != "pos")
                                {
                                    var Ecomparameters = new DynamicParameters();
                                    Ecomparameters.Add("CompanyCode", model.CompanyCode);
                                    Ecomparameters.Add("EcomId", model.OMSId);
                                    if (string.IsNullOrEmpty(model.IsCanceled))
                                    {
                                        model.IsCanceled = "N";
                                    }
                                    if (model.IsCanceled != "C")
                                    {
                                        var socheck = await _headerRepository.GetAllAsync("USP_S_T_SalesEcom", Ecomparameters, commandType: CommandType.StoredProcedure);
                                        if (socheck != null && socheck.Count > 0)
                                        {
                                            result.Success = false;
                                            result.Data = socheck.FirstOrDefault().TransId;
                                            result.Message = $"Can't add order. {model.RefTransId} has existed. POS ID: " + socheck.FirstOrDefault().TransId;
                                            return result;
                                        }
                                    }

                                    List<TDeliveryLine> ListA = new List<TDeliveryLine>();
                                    foreach (var line in model.Lines)
                                    {
                                        string getItemType = $"select CustomField1 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                        string itemType = _headerRepository.GetScalar(getItemType, null, commandType: CommandType.Text);
                                        if (string.IsNullOrEmpty(itemType))
                                        {
                                            result.Success = false;
                                            result.Message = "Please check master data (Item Type ) with your admin. Item " + line.ItemCode;
                                            return result;
                                        }
                                        else
                                        {
                                            if (itemType.ToLower() == "class" || itemType.ToLower() == "member")
                                            {
                                                if (string.IsNullOrEmpty(line.StartDate.ToString()) || string.IsNullOrEmpty(line.EndDate.ToString()))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Member Class StartDate/EndDate can't null";
                                                    return result;
                                                }
                                            }
                                            else if (itemType.ToLower() == "card")
                                            {
                                                if (string.IsNullOrEmpty(line.StartDate.ToString()) || string.IsNullOrEmpty(line.EndDate.ToString()))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Card Member StartDate/EndDate can't null";
                                                    return result;
                                                }

                                            }
                                            if (checkCompValue == "1")
                                            {
                                                string getCapacity = $"select CustomField8 from M_Item where  ItemCode =N'{line.ItemCode}' ";
                                                var capaValue = _headerRepository.GetScalar(getCapacity, null, commandType: CommandType.Text);
                                                if (!string.IsNullOrEmpty(capaValue))
                                                {
                                                    if (string.IsNullOrEmpty(line.TimeFrameId))
                                                    {
                                                        result.Success = false;
                                                        result.Message = "Time Frame Id can't null";
                                                        return result;
                                                    }
                                                    if (string.IsNullOrEmpty(line.AppointmentDate.ToString()))
                                                    {
                                                        result.Success = false;
                                                        result.Message = "Appointment Date can't null";
                                                        return result;
                                                    }

                                                    if (!string.IsNullOrEmpty(line.StoreAreaId))
                                                    {
                                                        string queryCheckStoreArea = $" [USP_CheckStoreAreaInStoreCapacity] N'{model.CompanyCode}', N'{model.StoreId}',N'{line.StoreAreaId}'";
                                                        var AreaCount = _headerRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
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
                                                        var AreaId = _headerRepository.GetScalar(queryCheckStoreArea, null, commandType: CommandType.Text);
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
                                            }
                                        }

                                        var bomLines = await getBOMLine(line.ItemCode);

                                        if (bomLines != null && bomLines.Count > 0)
                                        {
                                            foreach (var bomLineX in bomLines)
                                            {
                                                TDeliveryLine salesLine = new TDeliveryLine();
                                                salesLine.ItemCode = bomLineX.ItemCode;
                                                salesLine.Description = bomLineX.ItemName;
                                                salesLine.Price = 0;
                                                salesLine.UomCode = bomLineX.UomCode;
                                                salesLine.Quantity = line.Quantity * bomLineX.Quantity;
                                                salesLine.LineTotal = 0;
                                                salesLine.Status = "C";
                                                salesLine.BomId = line.ItemCode;
                                                ListA.Add(salesLine);
                                            }

                                        }
                                    }

                                    model.Lines.AddRange(ListA);
                                }


                                if (model.IsCanceled == "C")
                                {
                                    string querycheck = $"select isnull(count(*),0) from T_SalesHeader with (nolock) where RefTransId = N'{model.RefTransId}' and CompanyCode = N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'";
                                    string num = _headerRepository.GetScalar(querycheck, null, commandType: CommandType.Text);
                                    if (int.Parse(num) > 0)
                                    {
                                        result.Success = false;
                                        result.Message = "Can't cancel order. Because the order have return/exchange.";
                                        return result;
                                    }
                                    var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "VoidOrder").FirstOrDefault();
                                    if (setting != null && setting.SettingValue == "BeforeSyncData")
                                    {
                                        var orderGetData = await GetById( model.CompanyCode, model.StoreId, model.RefTransId);
                                        var orderGet = orderGetData.Data as TDeliveryHeader;
                                        if (orderGet.SyncMWIStatus == "Y")
                                        {
                                            result.Success = false;
                                            result.Message = "The order cannot be canceled because the order has been synced with MWI.";
                                            return result;
                                        }
                                    }
                                }

                                if (model.SalesMode != null && model.SalesMode == "Return")
                                {
                                    string checkResult = _headerRepository.GetScalar($"USP_Check_ReturnOrder N'{model.CompanyCode}', N'{model.StoreId}', N'{model.TransId}',N'{model.SalesType}',N'{model.SalesMode}'", null, commandType: CommandType.Text);
                                    if (checkResult == "0")
                                    {
                                        result.Success = false;
                                        result.Message = "Can't return order. Because the order date is not valid.";
                                        return result;
                                    }
                                }

                                flag = "After Create Check Sale Mode";
                                string itemList = "";
                                string defaultWhs = _headerRepository.GetScalar($"select WhsCode from M_Store with (nolock) where companyCode =N'{model.CompanyCode}' and StoreId = N'{model.StoreId}'", null, commandType: CommandType.Text);

                                List<ItemCheckModel> listItemCheck = new List<ItemCheckModel>();
                                foreach (var line in model.Lines)
                                {
                                    if (string.IsNullOrEmpty(line.SlocId))
                                    {
                                        line.SlocId = defaultWhs;
                                    }

                                    if (string.IsNullOrEmpty(line.TimeFrameId) && line.Quantity > 0)
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
                                    //}     
                                }
                                if (listItemCheck != null && listItemCheck.Count > 0)
                                {
                                    foreach (var line in listItemCheck)
                                    {
                                        itemList += line.ItemCode + "-" + line.SlocId + "-" + line.UomCode + "-" + line.Quantity + ";";
                                    }

                                }

                                if (model.SalesMode.ToLower() != "return")
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
                                flag = "After Check Negative";
                                //if (model.SalesType != "Table")
                                //{
                                //    string keyCache = string.Format(PrefixCacheActionSO, model.StoreId, model.TerminalId);
                                //    string storeCache = cacheService.GetCachedData<string>(keyCache);
                                //    if (string.IsNullOrEmpty(storeCache))
                                //    {
                                //        cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
                                //    }
                                //    else
                                //    {
                                //        result.Success = false;
                                //        result.Message = "Your actions are too fast and too dangerous. Please wait for your order to be completed.";
                                //        return result;
                                //    }

                                //}
                                //else
                                //{
                                //    model.Status = "C";
                                //    model.Payments = null;

                                //}
                                //Create and fill-up master table data

                                parameters.Add("TransId", model.TransId, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("StoreName", model.StoreName);
                                parameters.Add("ShiftId", model.ShiftId);
                                parameters.Add("ContractNo", model.ContractNo);
                                parameters.Add("CusId", model.CusId);
                                parameters.Add("CusIdentifier", model.CusIdentifier);
                                parameters.Add("RoundingOff", model.RoundingOff);
                                parameters.Add("TotalAmount", model.TotalAmount);
                                parameters.Add("TotalPayable", model.TotalPayable);
                                parameters.Add("TotalDiscountAmt", model.TotalDiscountAmt);
                                parameters.Add("TotalReceipt", model.TotalReceipt);
                                parameters.Add("AmountChange", model.AmountChange);
                                parameters.Add("PaymentDiscount", model.PaymentDiscount);
                                parameters.Add("CusGrpId", model.CusGrpId);
                                if (!string.IsNullOrEmpty(model.OrderId.ToString()))
                                {
                                    parameters.Add("OrderId", model.OrderId);
                                }
                                if (model.IsCanceled == "C" && string.IsNullOrEmpty(model.OrderId.ToString()))
                                {
                                    model.OrderId = Guid.NewGuid();
                                    parameters.Add("OrderId", model.OrderId);
                                }

                                parameters.Add("TotalTax", model.TotalTax);
                                parameters.Add("DiscountType", model.DiscountType);
                                parameters.Add("DiscountAmount", model.DiscountAmount);
                                parameters.Add("DiscountRate", model.DiscountRate);
                                parameters.Add("CreatedBy", model.CreatedBy);

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
                                parameters.Add("Phone", model.Phone);
                                parameters.Add("CusName", model.CusName);
                                parameters.Add("CusAddress", model.CusAddress);
                                parameters.Add("Reason", model.Reason);
                                parameters.Add("OMSId", model.OMSId);
                                parameters.Add("Chanel", model.Chanel);
                                parameters.Add("TerminalId", model.TerminalId);
                                parameters.Add("ShortOrderID", model.ShortOrderID);
                                parameters.Add("MerchantId", model.MerchantId);
                                parameters.Add("PromoId", model.PromoId);
                                parameters.Add("ApprovalId", model.ApprovalId);
                                parameters.Add("StartTime", model.StartTime == null ? DateTime.Now.AddMinutes(-1) : model.StartTime);
                                parameters.Add("RewardPoints", model.RewardPoints);
                                parameters.Add("ExpiryDate", model.ExpiryDate == null ? DateTime.Now : model.ExpiryDate);
                                parameters.Add("DocDate", model.DocDate == null ? DateTime.Now : model.DocDate);

                                parameters.Add("PrefixDO", PrefixDO);
                                parameters.Add("DeliveryBy", model.DeliveryBy);
                                parameters.Add("ReceiptBy", model.ReceiptBy);
                                parameters.Add("From", model.From);
                                parameters.Add("To", model.To);
                                parameters.Add("ToCustom1", model.ToCustom1);
                                parameters.Add("ToCustom2", model.ToCustom1);
                                parameters.Add("ToCustom3", model.ToCustom1);

                         
                                //parameters.Add("PrefixAR", PrefixAR);
                                if (!string.IsNullOrEmpty(model.SalesChanel))
                                {
                                    parameters.Add("SalesChanel", model.SalesChanel);
                                }
                                if (model.SalesMode.ToLower() == "return" || model.SalesMode.ToLower() == "ex" || model.SalesMode.ToLower() == "exchange")
                                {
                                    parameters.Add("CollectedStatus", "Closed");
                                }
                                else
                                {
                                    if ((model.DataSource == "POS" && model.Status.ToLower() != "h" && model.Status.ToLower() != "hold") || (model.DataSource != "POS" && (model.IsCanceled == "C" || model.Status == "C")))
                                    {
                                        model.Status = "C";
                                        parameters.Add("CollectedStatus", "Completed");
                                    }
                                    else
                                    {
                                        parameters.Add("CollectedStatus", "Hold");
                                    }

                                }
                                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "C" : model.Status);
                                //_headerRepository.Insert("InsertSaleHeader", parameters, commandType: CommandType.StoredProcedure);


                                //Insert record in master table. Pass transaction parameter to Dapper.
                                //var affectedRows = db.Execute("USP_I_T_SalesHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                //Get the Id newly created for master table record.
                                //If this is not an Identity, use different method here
                                //newId = Convert.ToInt64(connection.ExecuteScalar<object>("SELECT @@IDENTITY", null, transaction: transaction));

                                //Create and fill-up detail table data
                                //Use suitable loop as you want to insert multiple records.
                                //for(......)
                                int stt = 0;

                                SThirdPartyLog log = new SThirdPartyLog();
                                log.TransId = model.TransId;
                                log.Type = "S4Voucher";
                                log.StoreId = model.StoreId;
                                log.Remark = "";
                                log.CompanyCode = model.CompanyCode;
                                log.CreatedBy = model.CreatedBy;
                                log.Lines = new List<SThirdPartyLogLine>();

                                List<EpayModel> epayList = new List<EpayModel>();
                                //MWIEpay: any;
                                //MWIGrab: any;
                                //MWISarawak: any;
                                //MWICRM: any;
                                //MWIVoucherCheck: any;



                                var MWIEpay = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "MWIEpay").FirstOrDefault();
                                var MWIGrab = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "MWIGrab").FirstOrDefault();
                                var MWISarawak = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "MWISarawak").FirstOrDefault();
                                var MWICRM = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "MWICRM").FirstOrDefault();
                                var MWIVoucherCheck = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "MWIVoucherCheck").FirstOrDefault();
                                RPFO.Utilities.Helpers.LogUtils.WriteLogData("C:\\RPFO.API.MWIEpayModel\\", "SalesOrders", "MWIEpayModel", MWIEpay.ToJson());

                                //if (model.Staffs != null && model.Staffs.Count() > 0)
                                //{
                                //    int sttStaff = 1;
                                //    foreach (var staff in model.Staffs)
                                //    {
                                //        staff.ItemLine = model.TransId;
                                //        staff.LineId = sttStaff;
                                //        sttStaff++;
                                //    }
                                //}
                                //else
                                //{
                                //    model.Staffs = new List<TSalesStaff>();

                                //}
                                foreach (var line in model.Lines)
                                {
                                    stt++;
                                    if (line.Quantity.HasValue && Math.Abs(line.Quantity.Value) > 0)
                                    {

                                        if (line.ItemType == null)
                                        {
                                            line.ItemType = "";
                                        }

                                        line.LineId = stt.ToString();

                                        if (string.IsNullOrEmpty(line.SlocId))
                                        {
                                            line.SlocId = defaultWhs;
                                        }

                                        line.TaxAmt = (line.LineTotal * (line.TaxRate.HasValue ? line.TaxRate : 1)) / 100;


 

                                        if (model.DataSource == "POS" && model.Status != "H" && (line.ItemType.ToLower() == "pn" || line.ItemType.ToLower() == "tp" || line.ItemType.ToLower() == "pin" || line.ItemType.ToLower() == "bp" || line.ItemType.ToLower() == "topup"))
                                        {

                                            var counterParameters = new DynamicParameters();
                                            counterParameters.Add("CompanyCode", model.CompanyCode);
                                            counterParameters.Add("StoreId", model.StoreId);
                                            counterParameters.Add("TerminalId", model.TerminalId, DbType.String);
                                            var storeClients = db.Query<SStoreClient>("USP_S_CounterInforEpay", counterParameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                            if (storeClients != null && storeClients.Count() > 0)
                                            {
                                                var storeClient = storeClients.FirstOrDefault();
                                                flag = "Replace Epay Value";
                                                if (MWIEpay != null)
                                                {

                                                }
                                                else
                                                {
                                                    MWIEpay = new SGeneralSetting();
                                                }
                                                if (!string.IsNullOrEmpty(storeClient.Custom1))
                                                {
                                                    MWIEpay.CustomField1 = storeClient.Custom1;
                                                }
                                                if (!string.IsNullOrEmpty(storeClient.Custom2))
                                                {
                                                    MWIEpay.CustomField2 = storeClient.Custom2;
                                                }
                                                if (!string.IsNullOrEmpty(storeClient.Custom3))
                                                {
                                                    MWIEpay.CustomField3 = storeClient.Custom3;
                                                }
                                                if (!string.IsNullOrEmpty(storeClient.Custom4))
                                                {
                                                    MWIEpay.CustomField4 = storeClient.Custom4;
                                                }
                                                if (!string.IsNullOrEmpty(storeClient.Custom5))
                                                {
                                                    MWIEpay.CustomField5 = storeClient.Custom5;
                                                }

                                            }
                                            RPFO.Utilities.Helpers.LogUtils.WriteLogData("C:\\RPFO.API.MWIEpayModel\\", "SalesOrders", "MWIEpayModelReplace", MWIEpay.ToJson());
                                            if (MWIEpay != null)
                                            {

                                                if (string.IsNullOrEmpty(MWIEpay.CustomField1))
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = "Can't get URL Epay. Please check Setup";
                                                    return result;
                                                }
                                                if (string.IsNullOrEmpty(MWIEpay.CustomField2))
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = "Can't get Merchant Id. Please check Setup";
                                                    return result; ;
                                                }
                                                if (string.IsNullOrEmpty(MWIEpay.CustomField3))
                                                {
                                                    tran.Rollback();
                                                    result.Success = false;
                                                    result.Message = "Can't get Terminal Id. Please check Setup";
                                                    return result;
                                                }
 
                                            }
                                            else
                                            {
                                                tran.Rollback();
                                                result.Success = false;
                                                result.Message = "Can't get Epay setup. Please check Setup";
                                                return result;
                                            }


                                        }
                                        SOLines.Rows.Add(key, line.LineId, model.CompanyCode, line.ItemCode, line.SlocId, line.BarCode, line.UomCode, line.Quantity, line.Price, line.LineTotal,
                                            string.IsNullOrEmpty(line.DiscountType) && string.IsNullOrEmpty(line.PromoType) ? line.PromoType : line.DiscountType,
                                            !line.DiscountAmt.HasValue && line.PromoDisAmt.HasValue ? line.PromoDisAmt : line.DiscountAmt,
                                            !line.DiscountRate.HasValue && line.PromoPercent.HasValue ? line.PromoPercent : line.DiscountRate
                                            , model.CreatedBy, DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss"), null, null,
                                            line.Status, line.Remark, line.PromoId, line.PromoType, line.PromoPercent, line.PromoBaseItem, line.SalesMode,
                                            line.TaxRate, line.TaxAmt, line.TaxCode, line.MinDepositAmt, line.MinDepositPercent, line.DeliveryType, line.Posservice, line.StoreAreaId,
                                            line.TimeFrameId, line.Duration, line.AppointmentDate, line.BomId, line.PromoPrice, line.PromoLineTotal, line.BaseLine, line.BaseTransId, line.OpenQty,

                                            line.PromoDisAmt, line.IsPromo, line.IsSerial, line.IsVoucher, line.PrepaidCardNo, line.MemberDate, line.MemberValue, line.StartDate, line.EndDate, line.ItemType,
                                            line.Description, line.LineTotalBefDis, line.LineTotalDisIncludeHeader, line.SerialNum, line.Name, line.Phone, line.ItemTypeS4, line.Custom1, line.Custom2, line.Custom3, line.Custom4, line.Custom5,
                                            line.PriceListId, line.ProductId, line.WeightScaleBarcode, model.StoreId, line.BookletNo, line.OrgQty, line.DeliveryQty, line.ReceiptQty);


                                        //string query = $"INSERT INTO[dbo].[T_SalesLine] ([TransId] ,[LineId] ,[CompanyCode] ,[ItemCode] ,[BarCode] ,[UOMCode] ,[Quantity] ,[Price] , LineTotal , DiscountType ,[DiscountAmt] ,[DiscountRate] ,[CreatedBy], CreatedOn ,[ModifiedBy] ,[ModifiedOn] ,[Status]  ,[Remark] ,[PromoId] ,[PromoType] ,[PromoPercent] ,[PromoBaseItem] ,[SalesMode] ,[TaxRate] ,[TaxAmt] ,[TaxCode] ,[SLocId] ,[MinDepositAmt] ,[MinDepositPercent] ,[DeliveryType] ,[POSService]  , StoreAreaId , TimeFrameId , Duration , AppointmentDate , BomId, PromoPrice, PromoLineTotal, BaseLine, BaseTransId, OpenQty, PromoDisAmt, IsPromo, IsSerial, IsVoucher, PrepaidCardNo, MemberDate, MemberValue, StartDate, EndDate, ItemType, Description, LineTotalBefDis, LineTotalDisIncludeHeader, SerialNum, Phone, Name, ItemTypeS4, Custom1, Custom2, Custom3, Custom4,  Custom5, ProductId, PriceListId, WeightScaleBarcode, StoreId, BookletNo)";
                                        //query += $"select '{key}' , '{line.LineId}' , '{model.CompanyCode}', '{line.ItemCode}' , '{line.BarCode}' , '{line.UomCode}' , '{line.Quantity}' " +
                                        //    $", '{line.Price}' , '{line.LineTotal}'  , '{line.DiscountType}' , '{line.DiscountAmt}' , '{line.DiscountRate}' , '{ model.CreatedBy}' ,  '{ line.CreatedOn}',  '{ line.ModifiedBy}' ,  '{ line.ModifiedBy}' " +
                                        //    $",'{line.Status}' ,'{line.Remark}' ,'{line.PromoId}' , '{line.PromoType}', '{line.PromoPercent}' ,'{line.PromoBaseItem}', '{line.SalesMode}' , '{line.TaxRate}' , '{line.TaxAmt}', '{line.TaxCode}', '{line.SlocId}'" +
                                        //    $",'{line.MinDepositAmt}' , '{line.MinDepositPercent}', '{line.DeliveryType}', '{line.Posservice}', '{line.StoreAreaId}' , '{line.TimeFrameId}' , '{line.Duration}' , '{line.AppointmentDate}' , '{line.BomId}', '{line.PromoPrice}', '{line.PromoLineTotal}'" +
                                        //    $", '{line.BaseLine}', '{line.BaseTransId}', '{line.OpenQty}', '{line.PromoDisAmt}', '{line.IsPromo}', '{line.IsSerial}', '{line.IsVoucher}', '{line.PrepaidCardNo}', '{line.MemberDate}', '{line.MemberValue}', '{line.StartDate}', '{line.EndDate}', '{line.ItemType}'" +
                                        //    $", '{line.Description}', '{line.LineTotalBefDis}', '{line.LineTotalDisIncludeHeader}', '{line.SerialNum}', '{line.Phone}', '{line.Name}',  isnull(ItemTypeS4, (select top 1  ItemCategory_1 from M_Item where ItemCode = '{line.ItemCode}'  ) ) ItemTypeS4, '{line.Custom1}'  , '{line.Custom2}' , '{line.Custom3}' , '{line.Custom4}'  ,  '{line.Custom5}' ,'{line.ProductId}', '{line.PriceListId}', '{line.WeightScaleBarcode}', '{line.StoreId}', '{line.BookletNo}' ";
 

                                    }
                                    else
                                    {
                                        tran.Rollback();
                                        result.Success = false;
                                        result.Message = "Quantity of order not null";
                                        return result;
                                    }

                                    //await _headerRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                }
                                int sttLine = 0;
                                //var datatbl = SOLines.AsTableValuedParameter("SalesLineTableType");
                                parameters.Add("@Lines", SOLines.AsTableValuedParameter("[dbo].[T_DeliveryOrderLineTableType]"));


                               

                                //if (model.SerialLines != null && model.SerialLines.Count > 0)
                                //{
                                //    foreach (var line in model.SerialLines)
                                //    {
                                //        sttLine++;

                                //        if (string.IsNullOrEmpty(line.SlocId))
                                //        {
                                //            line.SlocId = defaultWhs;
                                //        }
                                //        line.LineNum = sttLine;
                                //        var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "SerialCheck").FirstOrDefault();
                                //        if (setting != null && line.Prefix != "PIN" && line.Prefix != "PN" && setting.SettingValue == "MWI.S4SV")
                                //        {

                                //            if (MWIVoucherCheck != null)
                                //            {
                                //                if (string.IsNullOrEmpty(MWIVoucherCheck.CustomField1))
                                //                {
                                //                    tran.Rollback();
                                //                    result.Success = false;
                                //                    result.Message = "Can't get URL S4SV. Please check Setup";
                                //                    return result;
                                //                }

                                //            }
                                //            S4VoucherDetail acttiveVoucher = new S4VoucherDetail();
                                //            acttiveVoucher.customername = model.CusName;
                                //            acttiveVoucher.customeraddress = model.CusAddress;
                                //            acttiveVoucher.identificationcard = model.CusId;
                                //            acttiveVoucher.actionsdate = DateTime.Now.ToString("yyyyMMdd");
                                //            acttiveVoucher.validfrom = DateTime.Now.ToString("yyyyMMdd");
                                //            acttiveVoucher.plantcode = model.StoreId;
                                //            acttiveVoucher.transactionid = key;
                                //            acttiveVoucher.phonenumber = model.Phone;
                                //            acttiveVoucher.serialnumber = line.SerialNum;
                                //            acttiveVoucher.materialnumber = line.ItemCode;
                                //            acttiveVoucher.salesvalue = line.Price.ToString();
                                //            acttiveVoucher.statuscode = "ACTI";
                                //            if (!string.IsNullOrEmpty(line.SapBonusBuyId))
                                //            {
                                //                acttiveVoucher.bonusbuyid = line.SapBonusBuyId.Split(',')[0];
                                //            }

                                //            //s4VoucherActiveList.Add(acttiveVoucher);

                                //            var jsonString = acttiveVoucher.ToJson();
                                //            SThirdPartyLogLine lineLog = new SThirdPartyLogLine();
                                //            lineLog.CompanyCode = model.CompanyCode;
                                //            lineLog.TransId = model.TransId;

                                //            lineLog.JsonBody = jsonString;
                                //            lineLog.Key1 = line.SerialNum;
                                //            lineLog.Key2 = acttiveVoucher.bonusbuyid;
                                //            lineLog.StartTime = DateTime.Now;
                                //            var resultHold = await ServayUpdateVoucher(MWIVoucherCheck.CustomField1, acttiveVoucher);

                                //            if (resultHold != null && resultHold.success.Value)
                                //            {
                                //                lineLog.Status = resultHold.success.Value.ToString();
                                //                lineLog.EndTime = DateTime.Now;

                                //                var listData = JsonConvert.DeserializeObject<List<S4VoucherDetail>>(resultHold.Data.ToString());

                                //                var data = listData.FirstOrDefault();
                                //                if (data != null)
                                //                {

                                //                    if (data.statuscode != acttiveVoucher.statuscode)
                                //                    {
                                //                        //tran.Rollback(); 
                                //                        result.Success = false;
                                //                        result.Message = data.statusmessage;
                                //                        lineLog.Remark = "S4 Voucher Message: " + data.statusmessage;
                                //                        //return result;
                                //                    }
                                //                    string date = "";
                                //                    if (!string.IsNullOrEmpty(data.todate) && data.todate != "00000000")
                                //                    {
                                //                        date = data.todate.Substring(0, 4) + "/" + data.todate.Substring(4, 2) + "/" + data.todate.Substring(6, 2);
                                //                        line.ExpDate = DateTime.Parse(date);
                                //                    }

                                //                }
                                //                log.Lines.Add(lineLog);
                                //            }
                                //            else
                                //            {
                                //                lineLog.Status = resultHold.success.Value.ToString();
                                //                lineLog.EndTime = DateTime.Now;
                                //                lineLog.Remark = "S4 Voucher Message (Failed): " + resultHold.Msg;
                                //                log.Lines.Add(lineLog);
                                //                throw new Exception(resultHold.Msg);
                                //            }

                                //        }

                                //        SOLineSerials.Rows.Add(key, Guid.NewGuid(), model.CompanyCode, line.ItemCode, line.SerialNum, line.SlocId, line.Quantity, line.UomCode, line.CreatedBy, line.CreatedOn, line.ModifiedBy, line.ModifiedOn,
                                //            line.Status, line.OpenQty, line.BaseLine, line.BaseTransId, line.LineNum, line.Description, line.Phone, line.Name, line.CustomF1, line.CustomF2, line.Prefix, line.ExpDate, line.StoreId, line.CustomF3, line.CustomF4, line.CustomF5);

                                //    }
                                //}

                                parameters.Add("@LineSerials", SOLineSerials.AsTableValuedParameter("[dbo].[T_DeliveryOrderLineSerialTableType]"));

                                //if (s4VoucherActiveList != null && s4VoucherActiveList.Count > 0 && model.Status != "H")
                                //{
                                //    if (MWIVoucherCheck != null)
                                //    {
                                //        if (string.IsNullOrEmpty(MWIVoucherCheck.CustomField1))
                                //        {
                                //            tran.Rollback();
                                //            result.Success = false;
                                //            result.Message = "Can't get URL S4SV. Please check Setup";
                                //            return result;
                                //        }

                                //    }
                                //    var resultHold = await ServayUpdateVouchers(MWIVoucherCheck.CustomField1, s4VoucherActiveList);
                                 
                                //    if (resultHold != null && resultHold.success.Value)
                                //    {
                                //        //lineLog.Status = resultHold.success.Value.ToString();
                                //        //lineLog.EndTime = DateTime.Now;


                                //        var listData = JsonConvert.DeserializeObject<List<S4VoucherDetail>>(resultHold.Data.ToString());

                                //        var data = listData.FirstOrDefault();
                                //        if (data != null)
                                //        {

                                //            //if (data.statuscode != acttiveVoucher.statuscode)
                                //            //{
                                //            //    //tran.Rollback(); 
                                //            //    result.Success = false;
                                //            //    result.Message = data.statusmessage;
                                //            //    //lineLog.Remark = "S4 Message: " + data.statusmessage;
                                //            //    //return result;
                                //            //}
                                //            string date = "";
                                //            if (!string.IsNullOrEmpty(data.todate) && data.todate != "00000000")
                                //            {
                                //                date = data.todate.Substring(0, 4) + "/" + data.todate.Substring(4, 2) + "/" + data.todate.Substring(6, 2);
                                //                //line.ExpDate = DateTime.Parse(date);
                                //            }

                                //        }
                                //        //log.Lines.Add(lineLog);
                                //    }
                                //    else
                                //    {
                                //        throw new Exception(resultHold.Msg);
                                //    }
                                //}
                                //if (log.Lines != null && log.Lines.Count > 0)
                                //{
                                //    await _log3Service.Create(log);
                                //}

                                //if (model.PromoLines != null && model.PromoLines.Count > 0)
                                //{
                                //    foreach (var line in model.PromoLines)
                                //    {
                                //        if (!string.IsNullOrEmpty(line.PromoId))
                                //        {
                                //            stt++;
                                //            string[] splt = line.PromoId.Split(",");
                                //            var voucher = model.VoucherApply.Where(x => splt.Any(y => y == x.discount_code)).FirstOrDefault();

                                //            line.RefTransId = "";
                                //            if (voucher != null)
                                //            {
                                //                line.RefTransId = voucher.voucher_code;
                                //                line.ApplyType = "Ecom";

                                //            }
                                //            else
                                //            {
                                //                line.RefTransId = line.RefTransId;
                                //                line.ApplyType = line.ApplyType;

                                //            }


                                //            SOPromo.Rows.Add(Guid.NewGuid(), key, model.CompanyCode, line.ItemCode, line.BarCode, line.RefTransId, line.ApplyType, line.ItemGroupId, line.UomCode, line.Value, line.PromoId, line.PromoType,
                                //            line.PromoTypeLine, model.CreatedBy, DateTime.Now.ToString("yyyy-MM-dd"), null, null, line.Status, line.PromoPercent, line.PromoAmt, model.StoreId);


                                //        }
                                //    }
                                //}


                                //parameters.Add("@LinePromos", SOPromo.AsTableValuedParameter("[dbo].[SalesPromoTableType]"));


                                stt = 0;
                                //var document = MapSOtoDocument(model);
                                //if (model.SalesType == null)
                                //{
                                //    model.SalesType = "";
                                //}

                                //document.UDiscountAmount = (double)(model.DiscountAmount ?? 0);
                                //if (model.Payments != null && model.Payments.Count > 0)
                                //{
                                //    foreach (var payment in model.Payments)
                                //    {
                                //        stt++;
                                //        if (string.IsNullOrEmpty(payment.Currency))
                                //        {
                                //            string CurrencyStr = $"select CurrencyCode from M_Store with (nolock) where StoreId =N'{model.StoreId}' and CompanyCode =N'{model.CompanyCode}' ";
                                //            string Currency = _headerRepository.GetScalar(CurrencyStr, null, commandType: CommandType.Text);
                                //            payment.Currency = Currency;
                                //        }
                                //        var getPayment = await _paymentService.GetByCode(model.CompanyCode, model.StoreId, payment.PaymentCode);
                                //        if (getPayment.Success)
                                //        {
                                //            var paymentCheck = getPayment.Data as MPaymentMethod;
                                //            if (paymentCheck != null && paymentCheck.RequireTerminal.HasValue && paymentCheck.RequireTerminal.Value)
                                //            { 
                                //            }
                                //        }
                                //        else
                                //        {
                                //            tran.Rollback();
                                //            return getPayment;
                                //        }

                                //        if (Math.Abs((payment.CollectedAmount ?? 0)) - Math.Abs((payment.ChargableAmount ?? 0)) > 0 && (payment.ChangeAmt == 0 || payment.ChangeAmt == null))
                                //        {
                                //            payment.ChangeAmt = (payment.CollectedAmount ?? 0) - (payment.ChargableAmount ?? 0);
                                //        }

                                //        if (!string.IsNullOrEmpty(payment.CardNo) && model.Status != "H")
                                //        {
                                //            var prepaidCardData = await GetPrepaidCard(model.CompanyCode, payment.CardNo);
                                //            var prepaidCar = prepaidCardData.Data as MPrepaidCard;
                                //            if (prepaidCar != null)
                                //            {
                                //                decimal main = prepaidCar.MainBalance == null ? 0 : prepaidCar.MainBalance.Value;
                                //                decimal sub = prepaidCar.SubBalance == null ? 0 : prepaidCar.SubBalance.Value;
                                //                if (main + sub <= 0 || payment.CollectedAmount > main + sub)
                                //                {
                                //                    tran.Rollback();
                                //                    result.Success = false;
                                //                    result.Message = "Balance of Card No " + payment.CardNo + " not available.";
                                //                    return result;
                                //                }
                                //            }


                                //        }

                                //        if (model.IsCanceled == "C" && model.Status != "H" && payment.CustomF2 == "E")
                                //        {

                                //            if (payment.CustomF3 == "EWallet")
                                //            {

                                //                var counterParameters = new DynamicParameters();
                                //                counterParameters.Add("CompanyCode", model.CompanyCode);
                                //                counterParameters.Add("StoreId", model.StoreId);
                                //                counterParameters.Add("TerminalId", model.TerminalId, DbType.String);
                                //                var storeClients = db.Query<SStoreClient>("USP_S_CounterInforEpay", counterParameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                //                if (storeClients != null && storeClients.Count() > 0)
                                //                {
                                //                    flag = "Replace Epay Value 2";
                                //                    var storeClient = storeClients.FirstOrDefault();
                                //                    if (MWIEpay != null)
                                //                    {

                                //                    }
                                //                    else
                                //                    {
                                //                        MWIEpay = new SGeneralSetting();
                                //                    }
                                //                    if (!string.IsNullOrEmpty(storeClient.Custom1))
                                //                    {
                                //                        MWIEpay.CustomField1 = storeClient.Custom1;
                                //                    }
                                //                    if (!string.IsNullOrEmpty(storeClient.Custom2))
                                //                    {
                                //                        MWIEpay.CustomField2 = storeClient.Custom2;
                                //                    }
                                //                    if (!string.IsNullOrEmpty(storeClient.Custom3))
                                //                    {
                                //                        MWIEpay.CustomField3 = storeClient.Custom3;
                                //                    }
                                //                    if (!string.IsNullOrEmpty(storeClient.Custom4))
                                //                    {
                                //                        MWIEpay.CustomField4 = storeClient.Custom4;
                                //                    }
                                //                    if (!string.IsNullOrEmpty(storeClient.Custom5))
                                //                    {
                                //                        MWIEpay.CustomField5 = storeClient.Custom5;
                                //                    }
                                //                }

                                //                if (MWIEpay != null)
                                //                {

                                //                    if (string.IsNullOrEmpty(MWIEpay.CustomField1))
                                //                    {
                                //                        tran.Rollback();
                                //                        result.Success = false;
                                //                        result.Message = "Can't get URL Epay. Please check Setup";
                                //                        return result;
                                //                    }
                                //                    if (string.IsNullOrEmpty(MWIEpay.CustomField2))
                                //                    {
                                //                        tran.Rollback();
                                //                        result.Success = false;
                                //                        result.Message = "Can't get Merchant Id. Please check Setup";
                                //                        return result; ;
                                //                    }
                                //                    if (string.IsNullOrEmpty(MWIEpay.CustomField3))
                                //                    {
                                //                        tran.Rollback();
                                //                        result.Success = false;
                                //                        result.Message = "Can't get Terminal Id. Please check Setup";
                                //                        return result;
                                //                    }
                                //                }
                                //                model.CustomF1 = MWIEpay.CustomField3;
                                //                decimal? value = payment.CollectedAmount.Value * 100;

                                //                var CancelResult = await EpayVoidPaymentOrder(MWIEpay.CustomField1, MWIEpay.CustomField3, MWIEpay.CustomField2, value, model.StoreId, "", payment.RefNumber, model.OrderId?.ToString());
                                //                if (CancelResult.Success)
                                //                {
                                //                    //var resultData = CancelResult.Data as EpayModel;
                                //                    payment.CustomF1 = CancelResult.Message.ToString();
                                //                }
                                //                else
                                //                {
                                //                    tran.Rollback();
                                //                    result.Success = false;
                                //                    result.Message = "Cancel Sarawak failed. Message " + CancelResult.Message;
                                //                    return result;
                                //                }
                                //            }
                                //            if (payment.CustomF3 == "Sarawak")
                                //            {
                                //                if (MWISarawak != null)
                                //                {
                                //                    if (MWISarawak.CustomField1 == "")
                                //                    {
                                //                        tran.Rollback();
                                //                        result.Success = false;
                                //                        result.Message = "Can't get URL Epay. Please check Setup";
                                //                        return result;
                                //                    }
                                //                    if (MWISarawak.CustomField2 == "")
                                //                    {
                                //                        tran.Rollback();
                                //                        result.Success = false;
                                //                        result.Message = "Can't get Merchant Id. Please check Setup";
                                //                        return result; ;
                                //                    }
                                //                    //if (MWIEpay.CustomField3 == "")
                                //                    //{
                                //                    //    tran.Rollback();
                                //                    //    result.Success = false;
                                //                    //    result.Message = "Can't get Terminal Id. Please check Setup";
                                //                    //    return result;
                                //                    //}
                                //                }
                                //                var CancelResult = await ServaySarawakRefund(MWISarawak.CustomField1, MWISarawak.CustomField2, model.OMSId, payment.CustomF1, "https://abeoinc.com", payment.CollectedAmount);
                                //                //var CancelResult = await ServaySarawakRefund(MWISarawak.CustomField1"M100004203", model.OMSId, payment.CustomF1, "https://abeoinc.com", payment.CollectedAmount);
                                //                if (CancelResult != null)
                                //                {
                                //                    if (CancelResult.success.HasValue && !CancelResult.success.Value)
                                //                    {
                                //                        tran.Rollback();
                                //                        result.Success = false;
                                //                        result.Message = "Cancel Sarawak failed. Message " + CancelResult.message;
                                //                        return result;
                                //                    }
                                //                    else
                                //                    {
                                //                        payment.CustomF1 = CancelResult.Data.ToString();
                                //                    }
                                //                }
                                //                else
                                //                {
                                //                    tran.Rollback();
                                //                    result.Success = false;
                                //                    result.Message = "Cancel Sarawak failed.";
                                //                    return result;
                                //                }
                                //            }

                                //        }
                                //        if (model.IsCanceled == "C" && model.Status != "H" && !string.IsNullOrEmpty(payment.PaymentMode) && payment.PaymentMode == "BankTerminal")
                                //        {
                                //            if (payment.PaymentMode == "BankTerminal")
                                //            {

                                //                string message = "";
                                //                string comName = "COM1";
                                //                if (!string.IsNullOrEmpty(ComBankTerminal))
                                //                {
                                //                    comName = ComBankTerminal;
                                //                }
                                //                string queryGetPort = $"select CustomF1 from M_BankTerminal with(nolock) where PaymentMethod =N'{payment.CustomF5}' and CounterId = N'{model.TerminalId}' ";
                                //                //string queryGetPort = $"select CustomF1 from M_BankTerminal with(nolock) where PaymentMethod = ( select FatherId from M_PaymentMethod with(nolock) where PaymentCode = N'{payment.PaymentCode}' and CompanyCode = N'{model.CompanyCode}')";
                                //                string GetPort = _headerRepository.GetScalar(queryGetPort, null, commandType: CommandType.Text);
                                //                if (!string.IsNullOrEmpty(GetPort))
                                //                {
                                //                    comName = GetPort;
                                //                }
                                //                Data.Models.TerminalDataModel response = _bankTerminalService.SendPaymentToTerminal("5", payment.CustomF5, comName, (double)payment.CollectedAmount.Value, payment.RefNumber, 60, model.OrderId.ToString(), out message);
                                //                if (response != null)
                                //                {
                                //                    if ((response.StatusCode != "0" && response.StatusCode != "00") || !string.IsNullOrEmpty(message))
                                //                    {
                                //                        tran.Rollback();
                                //                        result.Data = response;
                                //                        result.Success = false;
                                //                        result.Message = message;
                                //                        return result;
                                //                    }
                                //                    else
                                //                    {
                                //                        payment.RefNumber = response.InvoiceNumber;
                                //                        //result.Data = response;
                                //                        //result.Success = true;
                                //                    }
                                //                }
                                //                else
                                //                {
                                //                    tran.Rollback();
                                //                    result.Message = message;
                                //                    result.Success = false;
                                //                    return result;
                                //                }
                                //            }
                                //        }




                                //        SOPayments.Rows.Add(payment.PaymentCode, model.CompanyCode, key, stt, payment.TotalAmt == null ? payment.ChargableAmount : payment.TotalAmt,
                                //        payment.ReceivedAmt, payment.PaidAmt, payment.ChangeAmt, payment.PaymentMode, payment.CardType, payment.CardHolderName, payment.CardNo, payment.VoucherBarCode,
                                //        payment.VoucherSerial, model.CreatedBy, null, null, null, payment.Status, payment.ChargableAmount, payment.PaymentDiscount, payment.CollectedAmount, payment.RefNumber,
                                //        model.DataSource, payment.Currency, payment.FCAmount, payment.Rate, model.ShiftId, payment.CardExpiryDate,
                                //        payment.AdjudicationCode, payment.AuthorizationDateTime, model.TerminalId, payment.RoundingOff, payment.FCRoundingOff, payment.ForfeitCode, payment.Forfeit,
                                //        payment.CustomF1, payment.CustomF2, payment.CustomF3, payment.CustomF4, payment.CustomF5, model.StoreId);



                                //        if (model.Status != "H" && payment.PaymentCode == "Point")
                                //        {
                                //            //document.UDiscountAmount += payment.CollectedAmount == null ? 0 : (double)payment.CollectedAmount;
                                //            //db.Execute($"USP_UpdateLoyaltyPoint N'{model.CompanyCode}' ,N'{model.CusId}' , N'{payment.RefNumber}'", parameters, commandType: CommandType.Text, transaction: tran);
                                //            //_loyaltyService.InsertLoyaltyLog(true, document, 0, double.Parse(payment.RefNumber), double.Parse(payment.CollectedAmount.ToString()), out string _);

                                //            double outPoint = double.Parse(payment.RefNumber);
                                //            double outAmt = double.Parse(payment.CollectedAmount.ToString());
                                //            if (outAmt < 0)
                                //            {
                                //                outPoint *= -1;
                                //            }
                                //            document.UDiscountAmount += payment.CollectedAmount == null ? 0 : (double)payment.CollectedAmount;
                                //            //db.Execute($"USP_UpdateLoyaltyPoint N'{model.CompanyCode}' ,N'{model.CusId}' , N'{outPoint}'", parameters, commandType: CommandType.Text, transaction: tran);
                                //            _loyaltyService.InsertLoyaltyLog(true, document, 0, outPoint, outAmt, out string _);
                                //        }

                                //        //await _headerRepository.GetConnection().InsertAsync<string, TSalesLine>(line);
                                //    }
                                //}


                                //parameters.Add("@LinePayments", SOPayments.AsTableValuedParameter("[dbo].[SalesPaymentTableType]"));


                                //if (model.Staffs != null && model.Staffs.Count > 0)
                                //{
                                //    foreach (var staff in model.Staffs)
                                //    {
                                      
                                //        SOStaffTableType.Rows.Add(model.CompanyCode, model.StoreId, key, staff.ItemLine, staff.LineId, staff.Staff, staff.Position, staff.Percent, staff.Amount, staff.Remark, staff.CreatedBy, null, null, null, model.Status, "", "", "", "", "");

                                //    }
                                //}

                                //if (SOStaffTableType != null && SOStaffTableType.Rows.Count > 0)
                                //{
                                //    parameters.Add("@LineStaffs", SOStaffTableType.AsTableValuedParameter("[dbo].[T_SalesStaffTableType]"));
                                //}


                                //if (model.Invoice != null)
                                //{

                                //    SOInvoice.Rows.Add(key, model.CompanyCode, model.StoreId, model.StoreName, model.Invoice.CustomerName, model.Invoice.TaxCode
                                //        , model.Invoice.Email, model.Invoice.Address, model.Invoice.Phone, model.Invoice.Remark, DateTime.Now.ToString("yyyy-MM-dd"),
                                //        model.CreatedBy, null, null, model.Invoice.Name);


                                //}

                                //parameters.Add("@LineInvoices", SOInvoice.AsTableValuedParameter("[dbo].[SalesInvoiceTableType]"));

                                //if (model.Delivery != null)
                                //{
                                //    var parametersDeliver = new DynamicParameters();

                                //    parametersDeliver.Add("CompanyCode", model.CompanyCode);
                                //    parametersDeliver.Add("TransId", model.TransId, DbType.String);
                                //    parametersDeliver.Add("StoreId", model.StoreId);
                                //    parametersDeliver.Add("StoreName", model.StoreName);
                                //    parametersDeliver.Add("DeliveryPartner", model.Delivery.DeliveryPartner);
                                //    parametersDeliver.Add("DeliveryId", model.Delivery.DeliveryId);
                                //    parametersDeliver.Add("Email", model.Delivery.Email);
                                //    parametersDeliver.Add("Address", model.Delivery.Address);
                                //    parametersDeliver.Add("Phone", model.Delivery.Phone);
                                //    parametersDeliver.Add("Remark", model.Delivery.Remark);
                                //    parametersDeliver.Add("CreatedBy", model.CreatedBy);
                                //    db.Execute("USP_I_T_SalesDelivery", parametersDeliver, commandType: CommandType.StoredProcedure, transaction: tran);

                                //}
                                //if (model.DataSource == "POS" && model.Status != "H")
                                //{
                                //    //Tạo Delivery

                                //    //Get default delivery
                                //    var defaultDeliver = await _deliveryInforService.GetDefault(model.CompanyCode);
                                //    if (defaultDeliver.Success == true && defaultDeliver.Data != null)
                                //    {
                                //        var defData = defaultDeliver.Data as MDeliveryInfor;
                                //        TSalesDelivery delivery = new TSalesDelivery();
                                //        delivery.TransId = key;
                                //        delivery.CompanyCode = model.CompanyCode;
                                //        delivery.DeliveryFee = defData.DeliveryFee;
                                //        delivery.DeliveryMethod = defData.DeliveryId;
                                //        delivery.DeliveryType = defData.DeliveryType;
                                    

                                //        //db.Execute("USP_I_T_Sales_Delivery", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                //        SODeliverry.Rows.Add(Guid.NewGuid().ToString(), key, model.CompanyCode, delivery.DeliveryType, delivery.DeliveryMethod, delivery.DeliveryFee);
                                //        parameters.Add("@LineDelivery", SODeliverry.AsTableValuedParameter("[dbo].[Sales_DeliveryTableType]"));
                                //    }

                                   

                                //}
                                //else
                                //{
                                //    if (model.Deliveries != null && model.Deliveries.Count > 0)
                                //    {
                                //        foreach (var delivery in model.Deliveries)
                                //        {
                                           
                                //            SODeliverry.Rows.Add(Guid.NewGuid().ToString(), model.CompanyCode, key, delivery.DeliveryType, delivery.DeliveryMethod, delivery.DeliveryFee);


                                //        }
                                //        parameters.Add("@LineDelivery", SODeliverry.AsTableValuedParameter("[dbo].[Sales_DeliveryTableType]"));
                                //    }
                                //}

                                //if (model.IsCanceled == "C")
                                //{
                                //    db.Execute($"Update T_SalesHeader set IsCanceled = 'Y', CollectedStatus = 'Canceled' where TransId=N'{model.RefTransId}' and CompanyCode=N'{model.CompanyCode}' and StoreId=N'{model.StoreId}'", null, commandType: CommandType.Text, transaction: tran);
                                //}
                                //if (model.VoucherApply != null && model.VoucherApply.Count > 0)
                                //{
                                //    List<string> listRd = new List<string>();
                                //    foreach (var voucher in model.VoucherApply)
                                //    {
                                //        if (voucher.source == "MWI.S4SV")
                                //        {
                                //            if (MWIVoucherCheck != null)
                                //            {
                                //                if (MWIVoucherCheck.CustomField1 == "")
                                //                {
                                //                    tran.Rollback();
                                //                    result.Success = false;
                                //                    result.Message = "Can't get URL S4SV. Please check Setup";
                                //                    return result;
                                //                }

                                //                //if (MWIEpay.CustomField3 == "")
                                //                //{
                                //                //    tran.Rollback();
                                //                //    result.Success = false;
                                //                //    result.Message = "Can't get Terminal Id. Please check Setup";
                                //                //    return result;
                                //                //}
                                //            }
                                //            if (!listRd.Contains(voucher.serialnumber))
                                //            {
                                //                S4VoucherDetail voucherRedeem = new S4VoucherDetail();
                                //                voucherRedeem.customername = model.CusName;
                                //                voucherRedeem.customeraddress = model.CusAddress;
                                //                voucherRedeem.identificationcard = model.CusId;
                                //                voucherRedeem.actionsdate = DateTime.Now.ToString("yyyyMMdd");
                                //                voucherRedeem.plantcode = model.StoreId;
                                //                voucherRedeem.transactionid = key;
                                //                voucherRedeem.phonenumber = model.Phone;
                                //                voucherRedeem.serialnumber = voucher.serialnumber;
                                //                voucherRedeem.materialnumber = voucher.materialnumber;
                                //                //voucherRedeem.salesvalue = voucher.discount_value;
                                //                voucherRedeem.paymentamount = voucher.discount_value;

                                //                //voucherRedeem.materialnumber = voucher.;
                                //                voucherRedeem.statuscode = "REDE";

                                //                var jsonString = voucherRedeem.ToJson();
                                //                SThirdPartyLogLine lineLog = new SThirdPartyLogLine();
                                //                lineLog.CompanyCode = model.CompanyCode;
                                //                lineLog.TransId = key;

                                //                lineLog.JsonBody = jsonString;
                                //                lineLog.Key1 = voucher.serialnumber;
                                //                //lineLog.Key2 = voucherRedeem.transactionid;
                                //                lineLog.StartTime = DateTime.Now;

                                //                if (voucher.vouchercategory != "SPV")
                                //                {
                                //                    var resultHold = await ServayUpdateVoucher(MWIVoucherCheck.CustomField1, voucherRedeem);
                                //                    if (resultHold != null && resultHold.success.Value)
                                //                    {
                                //                        lineLog.Status = resultHold.success.Value.ToString();
                                //                        lineLog.EndTime = DateTime.Now;
                                //                        //var listData = resultHold.Data as List<S4VoucherDetail>;
                                //                        var listData = JsonConvert.DeserializeObject<List<S4VoucherDetail>>(resultHold.Data.ToString());
                                //                        var data = listData.FirstOrDefault();
                                //                        if (data != null)
                                //                        {
                                //                            if (data.statuscode != voucherRedeem.statuscode)
                                //                            {
                                //                                tran.Rollback();
                                //                                result.Success = false;
                                //                                result.Message = "S4 Message: " + data.statusmessage;
                                //                                return result;
                                //                            }
                                //                            else
                                //                            {
                                //                                lineLog.Remark = "S4 Voucher Message: " + data.statusmessage;
                                //                            }

                                //                        }

                                //                    }
                                //                    else
                                //                    {
                                //                        lineLog.Status = resultHold.success.Value.ToString();
                                //                        lineLog.EndTime = DateTime.Now;
                                //                        lineLog.Remark = "S4 Voucher Message ( Redeem Failed):: " + resultHold.Msg;
                                //                        throw new Exception(resultHold.Msg);
                                //                    }

                                //                    log.Lines.Add(lineLog);
                                //                }

                                //                //if (resultHold.Status != 0)
                                //                //{
                                //                //    throw new Exception(resultHold.Msg);
                                //                //}
                                //                listRd.Add(voucher.serialnumber);
                                //            }

                                //            //holdList.Add(voucher.voucher_code);
                                //        }
                                //        else
                                //        {
                                //            var resultHold = await HoldVoucher(voucher.voucher_code, model.CusId, model.StoreId, key);
                                //            if (resultHold.Status != 0)
                                //            {
                                //                throw new Exception(resultHold.Msg);
                                //            }
                                //            SaleViewModel newModel = new SaleViewModel();
                                //            newModel = model;
                                //            newModel.Logs = new List<OrderLogModel>();
                                //            OrderLogModel orderLogModel = new OrderLogModel();
                                //            orderLogModel.CreatedBy = model.CreatedBy;
                                //            orderLogModel.CompanyCode = model.CompanyCode;
                                //            orderLogModel.Type = "RedeemVoucher";
                                //            orderLogModel.Action = "Redeem";
                                //            orderLogModel.Result = resultHold.Status.ToString();
                                //            orderLogModel.Value = voucher.voucher_code;
                                //            orderLogModel.CustomF1 = model.CusId;
                                //            orderLogModel.CustomF2 = key;
                                //            orderLogModel.CustomF3 = "";
                                //            orderLogModel.StoreId = model.StoreId;
                                //            orderLogModel.Time = DateTime.Now;
                                //            WriteLogRemoveBasket(newModel);
                                //            holdList.Add(voucher.voucher_code);
                                //        }

                                //    }
                                //}


                                //if (model.Status.ToLower() != "h" && model.Status.ToLower() != "hold")
                                //{
                                //    var luckyDraw = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "LuckyDraw").FirstOrDefault();
                                //    if (luckyDraw != null && (luckyDraw.SettingValue == "true" || luckyDraw.SettingValue == "1"))
                                //    {
                                //        string LuckyNo = _loyaltyService.GetLuckyNo(document, out string _);

                                //        if (!string.IsNullOrEmpty(LuckyNo))
                                //        {
                                //            model.LuckyNo = LuckyNo;
                                //            //db.Execute($"Update T_SalesHeader set LuckyNo = '{LuckyNo}' where TransId = N'{model.TransId}' and CompanyCode = N'{model.CompanyCode}' and StoreId= N'{model.StoreId}'", null, commandType: CommandType.Text, transaction: tran);
                                //        }
                                //    }
                                //}
                                if (!string.IsNullOrEmpty(model.LuckyNo))
                                {
                                    parameters.Add("LuckyNo", model.LuckyNo);
                                }
                                parameters.Add("CustomF1", model.CustomF1);
                                parameters.Add("CustomF2", model.CustomF2);
                                parameters.Add("CustomF3", model.CustomF3);
                                parameters.Add("CustomF4", model.CustomF4);
                                parameters.Add("CustomF5", model.CustomF5);
                                flag = "Begin Insert Sales Order";
                                key = db.ExecuteScalar("USP_I_T_DeliveryOrder", parameters, commandType: CommandType.StoredProcedure, transaction: tran).ToString();

                                 

                                tran.Commit();

 

                                result.Success = true;
                                result.Message = key;
                                model.TransId = key;
                                result.Data = model;
                                var writeLog = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "WriteLog").FirstOrDefault();
                               
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
                result.Message = flag + " - " + ex.Message;
            }



            return result;
        }

        public Task<GenericResult> Delete(string companycode, string storeId, string Code)
        {
            throw new NotImplementedException();
        }
        public async Task<GenericResult> GetByType(string companyCode, string storeId, string fromdate, string todate, string TransId,
            string DeliveryBy, string key, string status, string ViewBy)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("StoreId", storeId);
                parameters.Add("TransId", TransId);
                parameters.Add("DeliveryBy", DeliveryBy);
                parameters.Add("Fromdate", fromdate);
                parameters.Add("Todate", todate);
                parameters.Add("DataSource", "");
                parameters.Add("Status", status);
                parameters.Add("Key", key);
                parameters.Add("ViewBy", ViewBy);
           
                var lst = await _headerRepository.GetAllAsync("USP_S_T_DeliveryHeaderByType", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = lst;
                //return lst;
            }
            catch (Exception ex)
            {
                //return null;
                result.Success = false;
                result.Data = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _headerRepository.GetAllAsync($"select * from T_GoodsReceiptPOHeader with (nolock) where CompanyCode= '{CompanyCode}' order by CreatedOn desc", null, commandType: CommandType.Text);
                
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

        //public Task<GenericResult> GetById(string companycode, string storeId, string Id)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<GenericResult> GetById(string CompanyCode, string StoreId, string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                TDeliveryHeader order = new TDeliveryHeader();
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("TransId", Id);
                parameters.Add("DeliveryBy", "");
                parameters.Add("Fromdate", "");
                parameters.Add("Todate", "");
                parameters.Add("DataSource", "");
                parameters.Add("Status", "");
                parameters.Add("Key", "");
                parameters.Add("ViewBy", "");
                //string sql= $"USP_S_T_DeliveryHeaderByType '{CompanyCode}', '{StoreId}', '{Id}'";
                //string sql= $"select * from T_GoodsReceiptPOHeader with (nolock) where PurchaseId='{Id}' and CompanyCode= '{CompanyCode}' and StoreId= '{StoreId}'";
                TDeliveryHeader header = await _headerRepository.GetAsync("USP_S_T_DeliveryHeaderByType", parameters, commandType: CommandType.StoredProcedure);
                if(header!=null)
                {
                    //string queryLine = $"select t1.*  from T_GoodsReceiptPOLine t1 with(nolock)  where t1.PurchaseId = '{Id}' and t1.CompanyCode = '{CompanyCode}'";
                    //string queryLineSerial = $"select t1.*   from T_GoodsReceiptPOLineSerial t1 with(nolock)   where t1.PurchaseId = '{Id}' and t1.CompanyCode = '{CompanyCode}'";
                    
                    string queryLine = $"USP_S_T_DeliveryOrderLine '{CompanyCode}', '{StoreId}', '{Id}'";
                    string queryLineSerial = $"USP_S_T_DeliveryOrderLineSerial '{CompanyCode}', '{StoreId}', '{Id}'";

                    //List<TPurchaseOrderLine> lines = await _GRPOLineRepository.GetAllAsync(, null, commandType: CommandType.Text);

                    //List<TPurchaseOrderPayment> payments = await _GRPOpaymentLineRepository.GetAllAsync(queryPayment, null, commandType: CommandType.Text);

                    //var customer = await _customerRepository.GetAsync($"select * from M_Customer with (nolock) where CustomerId ='{header.CusId}'", null, commandType: CommandType.Text);

                    //var head = _mapper.Map<GRPOViewModel>(header);
                    using (IDbConnection db = _headerRepository.GetConnection())
                    {
                        try
                        {
                            if (db.State == ConnectionState.Closed)
                                db.Open();
                            var lines = db.Query<TDeliveryLine>(queryLine, null, commandType: CommandType.Text);
                            var serialLines = db.Query<TDeliveryLineSerial>(queryLineSerial, null, commandType: CommandType.Text);
                            foreach (var line in lines)
                            {
                                line.SerialLines = serialLines.Where(x => x.ItemCode == line.ItemCode && x.UomCode == line.UomCode).ToList();
                            }
                            order = header;// _mapper.Map<GRPOViewModel>(header);
                            order.Lines = lines.ToList();
                            //order.SerialLines = serialLines.ToList();
                            //order.PromoLines = promoLines.ToList();
                            //order.Payments = payments;
                            //order.Customer = customer;

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    result.Success = true;
                    result.Data = order;
                    return result;
                }    
                else
                {
                    result.Success = false;
                    result.Message = "TransId Not found";
                    return result;
                }    
                
                 
            }   
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
            
        }


       

        public async Task<List<TDeliveryLine>> GetLinesById(string companycode, string storeId, string Id)
        {
            var data = await _lineRepository.GetAllAsync($"USP_S_T_GoodsReceiptPOLine '{companycode}', '{storeId}', '{Id}'", null, commandType: CommandType.Text);
            return data;
        }

        public async Task<GenericResult> GetNewOrderCode(string companyCode, string storeId)
        {
            GenericResult rs = new GenericResult();
            try
            {
                string key = _headerRepository.GetScalar($" select dbo.[fnc_AutoGenDocumentCode] ('{PrefixDO}','{companyCode}','{storeId}')", null, commandType: CommandType.Text);
                 
                rs.Success = true;
                rs.Message = key;
            }
            catch(Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
 

        public Task<GenericResult> Update(TDeliveryHeader model)
        {
            throw new NotImplementedException();
        }
    }

}
