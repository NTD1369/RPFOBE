
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class EndDateService : IEndDateService
    {
        private readonly IGenericRepository<TEndDate> _endateRepository;
        private readonly IGenericRepository<TStoreDaily> _storeDailyRepository;
        private readonly IGenericRepository<TEndDateDetail> _endateDetailRepository;
        private readonly IGenericRepository<TEndDatePayment> _endatePaymentRepository;
        private readonly IGeneralSettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IMapper _mapper;
        public EndDateService(IGenericRepository<TEndDate> endateRepository, IGeneralSettingService settingService, IStoreService storeService, IGenericRepository<TStoreDaily> storeDailyRepository, IGenericRepository<TEndDateDetail> endateDetailRepository, IGenericRepository<TEndDatePayment> endatePaymentRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _endateRepository = endateRepository;
            _endateDetailRepository = endateDetailRepository;
            _endatePaymentRepository = endatePaymentRepository;
            _storeDailyRepository = storeDailyRepository;
            _settingService = settingService;
            _mapper = mapper;
            _storeService = storeService;
        }
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<UOMResultViewModel> resultlist = new List<UOMResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.Uom)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);
        //            //if (itemResult.Success == false)
        //            //{
        //                UOMResultViewModel itemRs = new UOMResultViewModel();
        //                itemRs = _mapper.Map<UOMResultViewModel>(item);
        //                itemRs.Success = itemResult.Success;
        //                itemRs.Message = itemResult.Message;
        //            resultlist.Add(itemRs);
        //            //}
        //        }
        //        result.Success = true;
        //        result.Data = resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //        //result.Data = failedlist;
        //    } 
        //    return result;
        //}

        public async Task<bool> checkExist(string CompanyCode, string HldCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("HldCode", HldCode);
            parameters.Add("StrDate", "");
            parameters.Add("EndDate", "");
            parameters.Add("Rmrks", "");
            parameters.Add("Status", "");
            parameters.Add("Keyword", "");
            var affectedRows = await _endateRepository.GetAsync("USP_S_M_Holiday", parameters, commandType: CommandType.StoredProcedure, GConnection.EndDateConnection);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> CheckOpenShift(string CompanyCode, string StoreId, DateTime Date)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _endateRepository.GetConnection(GConnection.EndDateConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Date", Date);
 
                    var details = await db.QueryAsync<TShiftHeader>("USP_S_T_ShiftOpenInDate", parameters, commandType: CommandType.StoredProcedure );
                   
                    db.Close();
                    if(details!= null && details.ToList().Count > 0)
                    {
                        string shiftLst = "";
                        foreach(var shift in details.ToList())
                        {
                            shiftLst += shift.ShiftId + ",";
                        }    
                        result.Success = false;
                        result.Message = "The shift is not end: " + shiftLst.Substring(0, shiftLst.Length - 1);
                       
                    }   
                    else 
                    {
                        result.Success = true;
                    }    
                   
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
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


        public async Task<GenericResult> CheckCOUNTER_CONNECT(string CompanyCode, string StoreId, string Date)
        {
            GenericResult result = new GenericResult();
            try

            {
                using (IDbConnection db = _endateRepository.GetConnection(GConnection.EndDateConnection))
                {
                    try
                    {
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //var parameters = new DynamicParameters();
                        //parameters.Add("CompanyCode", CompanyCode);
                        //parameters.Add("StoreId", StoreId);
                        //parameters.Add("Date", Date);

                        //var details = await db.QueryAsync<TShiftHeader>("USP_S_T_ShiftOpenInDate", parameters, commandType: CommandType.StoredProcedure);
                        var parametersCheck = new DynamicParameters();

                        var checkList = db.Query("Z_DEMO_COUNTER_CONNECT_LST", parametersCheck, commandType: CommandType.StoredProcedure);
                        if (checkList != null && checkList.Count() > 0)
                        {
                            result.Success = false;
                            result.Data = checkList;
                            result.Message = "Counter lost connection";
                            result.Code = 3000;
                            return result;
                        }
                        db.Close();
                        result.Success = true;

                    }
                    catch (Exception ex)
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                        result.Success = false;
                        result.Message = ex.Message;
                        return result;
                    }
                    finally
                    {
                        if (db.State == ConnectionState.Open)
                            db.Close();
                    }
                    //return result;
                }
            }
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
           
        }



        public async Task<GenericResult> Create(TEndDate model)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _endateRepository.GetConnection(GConnection.EndDateConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {


                            var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                            List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                            if (settingData.Success)
                            {
                                SettingList = settingData.Data as List<GeneralSettingStore>;
                            }

                         
                                var dataCheck = await CheckOpenShift(model.CompanyCode, model.StoreId, model.Date.Value);
                                if (dataCheck.Success)
                                {
                                    var dataInsert = await GetDetailBfEnd(model.CompanyCode, model.StoreId, model.Date.Value);
                                    if (dataInsert.Success)
                                    {
                                        TEndDate data = dataInsert.Data as TEndDate;
                                        var parameters = new DynamicParameters();
                                        //Guid guid = Guid.NewGuid();
                                        string DateId = model.CompanyCode + model.StoreId + model.Date.Value.ToString("yyMMdd");
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("StoreId", model.StoreId);
                                        parameters.Add("Id", DateId);
                                        parameters.Add("Date", model.Date);
                                        //parameters.Add("DateId", );
                                        parameters.Add("Description", model.Date.Value.ToString("yyMMdd"));
                                        parameters.Add("Remark", model.Remark);
                                        parameters.Add("Status", model.Status == null ? model.Status = "C" : model.Status);
                                        parameters.Add("CreateBy", model.CreateBy);
                                        parameters.Add("TotalSales", model.TotalSales);
                                        parameters.Add("TotalCount", model.TotalCount);
                                        parameters.Add("TotalCollected", model.TotalCollected);
                                        parameters.Add("TotalBalance", model.TotalBalance);

                                        parameters.Add("TaxTotal", model.TaxTotal);
                                        parameters.Add("DiscountTotal", model.DiscountTotal);
                                        parameters.Add("PaymentTotal", model.PaymentTotal);
                                        parameters.Add("LineItemCount", model.LineItemCount);
                                        parameters.Add("TaxCount", model.TaxCount);
                                        parameters.Add("DiscountCount", model.DiscountCount);
                                        parameters.Add("PaymentCount", model.PaymentCount);  

                                        //var exist = await checkExist(model.CompanyCode, model.HldCode);
                                        //if (exist == true)
                                        //{
                                        //    result.Success = false;
                                        //    result.Message = model.HldCode + " existed.";
                                        //    return result;
                                        //}
                                        //var affectedRows = _endateRepository.Insert("USP_I_T_EndDate", parameters, commandType: CommandType.StoredProcedure);

                                        var affectedRows = db.Execute("USP_I_T_EndDate", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                        parameters = new DynamicParameters();

                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("StoreId", model.StoreId);
                                        parameters.Add("DailyId", model.Date.Value.ToString("yyMMdd"));
                                        parameters.Add("DeviceId", string.IsNullOrEmpty(model.TerminalId) ? "Undefined" : model.TerminalId);
                                        parameters.Add("CreateDate", model.Date.Value.ToString("yyyy/MM/dd HH:mm:ss"));
                                        parameters.Add("TotalSales", model.TotalSales);
                                        parameters.Add("TotalCount", model.TotalCount);
                                        parameters.Add("CreatedBy", model.CreateBy);
                                        parameters.Add("Status", model.Status == null ? model.Status = "C" : model.Status);
                                        parameters.Add("TotalCollected", model.TotalCollected);
                                        parameters.Add("TotalBalance", model.TotalBalance);


                                        //_storeDailyRepository.Insert("USP_I_T_StoreDaily", parameters, commandType: CommandType.StoredProcedure);
                                        db.Execute("USP_I_T_StoreDaily", parameters, commandType: CommandType.StoredProcedure, transaction: tran);


                                        int numline = 0;
                                        if (model.Lines != null && model.Lines.Count > 0)
                                        {
                                            foreach (var line in model.Lines)
                                            {
                                                numline++;
                                                line.LineId = numline;
                                                line.EndDateId = DateId;
                                                parameters = new DynamicParameters();

                                                parameters.Add("CompanyCode", model.CompanyCode);
                                                parameters.Add("StoreId", model.StoreId);
                                                parameters.Add("Id", Guid.NewGuid());
                                                parameters.Add("EndDateId", DateId);
                                                parameters.Add("LineId", numline);
                                                parameters.Add("ItemCode", line.ItemCode);
                                                parameters.Add("UoMCode", line.UoMCode);
                                                parameters.Add("Description", line.Description);
                                                parameters.Add("Barcode", line.Barcode);
                                                parameters.Add("Price", line.Price);
                                                parameters.Add("Quantity", line.Quantity);
                                                parameters.Add("LineTotal", line.LineTotal);
                                                db.Execute("USP_I_T_EndDateDetail", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                                //var affectedRowLine = _endateRepository.Insert("USP_I_T_EndDateDetail", parameters, commandType: CommandType.StoredProcedure);
                                            }
                                        }
                                        int numpayline = 0;
                                        if (model.Payments != null && model.Payments.Count > 0)
                                        {
                                            foreach (var line in model.Payments)
                                            {
                                                line.LineId = numpayline;
                                                numpayline++;
                                                line.EndDateId = DateId;
                                                parameters = new DynamicParameters();
                                                if (string.IsNullOrEmpty(line.Currency))
                                                {
                                                    var StoreData = await _storeService.GetByCode(model.CompanyCode, model.StoreId);
                                                    var Store = StoreData.Data as MStore;
                                                    line.Currency = Store.CurrencyCode;
                                                }
                                                parameters.Add("CompanyCode", model.CompanyCode);
                                                parameters.Add("Currency", line.Currency, DbType.String);
                                                parameters.Add("StoreId", model.StoreId);
                                                parameters.Add("Id", Guid.NewGuid());
                                                parameters.Add("EndDateId", DateId);
                                                parameters.Add("ShiftId", line.ShiftId);
                                                parameters.Add("LineId", numpayline);
                                                parameters.Add("PaymentCode", line.PaymentCode);
                                                parameters.Add("Amount", line.TotalAmt);
                                                parameters.Add("FCAmount", line.FCAmount);
                                                parameters.Add("CollectedAmount", line.CollectedAmount);
                                                parameters.Add("FCCollectedAmount", line.FCCollectedAmount);
                                                parameters.Add("Balance", line.CollectedAmount - line.TotalAmt);
                                                parameters.Add("EOD_Code", line.EOD_Code);
                                                parameters.Add("CounterId", line.CounterId);
                                                //if(!line.BankInAmt.HasValue)
                                                //{
                                                //    line.BankInAmt = 0;
                                                //}    
                                                parameters.Add("BankInAmt", line.BankInAmt);
                                                parameters.Add("BankInBalance", line.BankInAmt - line.TotalAmt);

                                                db.Execute("USP_I_T_EndDatePayment", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                                //var affectedRowLine = _endateRepository.Insert("USP_I_T_EndDatePayment", parameters, commandType: CommandType.StoredProcedure);

                                            }
                                        }
                                        parameters = new DynamicParameters();
                                        string DailyId = model.Date.Value.ToString("yyMMdd");
                                        //Chưa dám đổi DateId vào
                                        parameters.Add("DailyId", DailyId);
                                        parameters.Add("CompanyCode", model.CompanyCode);
                                        parameters.Add("StoreId", model.StoreId);
                                        db.Execute("USP_I_T_StoreSummary", parameters, commandType: CommandType.StoredProcedure, transaction: tran);


                                        if (SettingList != null && SettingList.Count > 0)
                                        {
                                            var clearHoldBillSetting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ClearHoldBillAfterEOD").FirstOrDefault();
                                            if (clearHoldBillSetting != null && (clearHoldBillSetting.SettingValue == "true" || clearHoldBillSetting.SettingValue == "1"))
                                            {
                                                parameters = new DynamicParameters();
                                                parameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                                                parameters.Add("StoreId", model.StoreId, DbType.String);
                                                parameters.Add("DailyId", DailyId, DbType.String);
                                                parameters.Add("ModifiedBy", "System", DbType.String);

                                                db.Execute("USP_ClearHoldBillByDate", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                                            }
                                        }



                                        data.Id = DateId;
                                        result.Success = true;
                                        result.Data = data;
                                        tran.Commit();
                                    }
                                }
                                else
                                {
                                    result = dataCheck;
                                }
                            
                             
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            result.Success = false;
                            result.Message = ex.Message;
                        }
                        return result;
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

        public async Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Top)
        {
            GenericResult result = new GenericResult();
            try
            {
                string query = $"USP_S_EndOfDateList N'{CompanyCode}', N'{StoreId}'";
                //if(!string.IsNullOrEmpty(Top))
                //{
                //    query = $"USP_S_EndOfDateList N'{CompanyCode}', N'{StoreId}', N'{Top}'";
                //}    
                var data = await _endateRepository.GetAllAsync(query, null, commandType: CommandType.Text, GConnection.EndDateConnection);
                result.Success = true;
                //Store Đang lỗi dùng tạm hardcode
                if (!string.IsNullOrEmpty(Top))
                {
                    int topX = int.Parse(Top);
                    data = data.Take(topX).ToList();
                }
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetEndDateList(string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {

                var data = await _endateRepository.GetAllAsync($"select * from T_EndDate with (nolock) where CompanyCode='{CompanyCode}'" +
                    $" and StoreId = '{StoreId}'", null, commandType: CommandType.Text, GConnection.EndDateConnection);
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
        public async Task<GenericResult> GetEndDateByDailyId(string CompanyCode, string StoreId, string DailyId)
        {
            GenericResult result = new GenericResult();
            try
            {
                string checkDate = $"select * from T_EndDate with(nolock) where CompanyCode = N'{CompanyCode}' " +
                    $"and StoreId = N'{StoreId}' and Description = N'{DailyId}'";

                var data = await _endateRepository.GetAsync(checkDate, null, commandType: CommandType.Text, GConnection.EndDateConnection);
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

        public async Task<GenericResult> GetEndDateByDate(string CompanyCode, string StoreId, string Date)
        {
            GenericResult result = new GenericResult();
            try
            {
                string checkDate = $"select * from T_EndDate with(nolock) where CompanyCode = '{CompanyCode}' and StoreId = '{CompanyCode}' and CONVERT(Date, Date) = '{Date}'";

                var data = await _endateRepository.GetAsync(checkDate, null, commandType: CommandType.Text, GConnection.EndDateConnection);
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
       
        public async Task<GenericResult> EndDateSummary(string companyCode, string storeId, string transdate)
        {
            GenericResult result = new GenericResult();
            var listdata = await GetAll( companyCode,  storeId, "");
            var list = listdata.Data as List<TEndDate>;
            var item =  list.Where(x => x.Date.Value.ToString("yyyy/MM/dd") == DateTime.Parse(transdate).ToString("yyyy/MM/dd")).FirstOrDefault();
            using (IDbConnection db = _endateRepository.GetConnection(GConnection.EndDateConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    //if (db.State == ConnectionState.Closed)
                    //    db.Open();
                    //var parameters = new DynamicParameters();
                    //parameters.Add("CompanyCode", companyCode);
                    //parameters.Add("StoreId", storeId);
                    //parameters.Add("Date", transdate);
                    ////ar dataX = await db.QueryAsync(query, null);
                    //var data = await db.QueryAsync("[USP_S_EndDateSummary]", parameters, commandType: CommandType.StoredProcedure);
                    //result.Success = true;
                    //result.Data = data;
                    //db.Close();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("Date", transdate);
                    //'{companyCode}','{storeId}' ,'{transdate}'
                    string query = $"USP_GetSumaryHeaderDate";
                    var dataX =  db.QueryFirstOrDefault<TEndDate>(query,  parameters, commandType: CommandType.StoredProcedure);
                    if (dataX != null)
                    {
                        string queryPayment = $"USP_S_EndDateSummary";
                        if (dataX.Status == "C")
                        {
                            queryPayment = $"USP_S_EndDatePayment";
                        }
                        string queryLine = $"USP_S_SummaryLinesShiftDate";

                        string queryInventory = $"USP_S_InventorySummaryShiftDate";
                         

                        var dataLine = await db.QueryAsync<EndDateItemSumary>(queryLine, parameters, commandType: CommandType.StoredProcedure);
                        //string queryPayment = $"select PaymentCode,  Sum(CollectedAmount ) TotalAmount from T_SalesPayment t1 left join T_SalesHeader t2  with (nolock) on t1.TransId = t2.TransId " +
                        //    $" where t2.companyCode='{companyCode}' and t2.ShiftId = '{shiftId}' group by PaymentCode ";
                        var dataPayment = await db.QueryAsync<TEndDatePayment>(queryPayment, parameters, commandType: CommandType.StoredProcedure);
                        var dataInventory = await db.QueryAsync<EndDateItemSumary>(queryInventory, parameters, commandType: CommandType.StoredProcedure);
                        dataX.Payments = dataPayment.ToList();
                        dataX.ItemSumary = dataLine.ToList();
                        dataX.ItemInventorySumary = dataInventory.ToList();
                        db.Close();
                        result.Success = true;
                        result.Data = dataX;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Date " + transdate + " data not found ";
                    }


                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }

        }

        public class EODPrintViewModel
        {
            public string CompanyCode { get; set; }
            public string StoreId { get; set; }
            public string DailyId { get; set; }
            public string StoreName { get; set; }
            public string CreatedBy { get; set; }
            public string CreatedOn { get; set; }
            public string Reference { get; set; }
            public string Status { get; set; }

            public decimal? TotalAmount { get; set; }



            public string CustomF1 { get; set; }
            public string CustomF2 { get; set; }
            public string CustomF3 { get; set; }
            public string CustomF4 { get; set; }
            public string CustomF5 { get; set; }
            public string CustomF6 { get; set; }
            public string CustomF7 { get; set; }
            public string CustomF8 { get; set; }
            public string CustomF9 { get; set; }
            public string CustomF10 { get; set; }

            public decimal? CustomNumF1 { get; set; }
            public decimal? CustomNumF2 { get; set; }
            public decimal? CustomNumF3 { get; set; }
            public decimal? CustomNumF4 { get; set; }
            public decimal? CustomNumF5 { get; set; }
            public decimal? CustomNumF6 { get; set; }
            public decimal? CustomNumF7 { get; set; }
            public decimal? CustomNumF8 { get; set; }
            public decimal? CustomNumF9 { get; set; }
            public decimal? CustomNumF10 { get; set; }

            public List<EODPrintCollectionByDepartment> Departments { get; set; } = new List<EODPrintCollectionByDepartment>();
            public List<EODPrintCashCollection> CashCollections { get; set; } = new List<EODPrintCashCollection>();
            public List<EODPrintNoneCashCollection> NoneCashCollections { get; set; } = new List<EODPrintNoneCashCollection>();

        }



        public class EODPrintCollectionByDepartment
        {
            public string CompanyCode { get; set; }
            public string StoreId { get; set; }
            public string Code { get; set; }
            public string Department { get; set; }

            public decimal? POSSALES { get; set; }
            public decimal? ONLINESALES { get; set; }
            public decimal? TOTALSALES { get; set; } 

            public string CustomF1 { get; set; }
            public string CustomF2 { get; set; }
            public string CustomF3 { get; set; }
            public string CustomF4 { get; set; }
            public string CustomF5 { get; set; }

            public decimal? CustomNumF1 { get; set; }
            public decimal? CustomNumF2 { get; set; }
            public decimal? CustomNumF3 { get; set; }
            public decimal? CustomNumF4 { get; set; }
            public decimal? CustomNumF5 { get; set; }
            public bool? IsBold { get; set; }
            public bool? IsTitle { get; set; }
        }

        public class EODPrintCashCollection
        {
            public string CompanyCode { get; set; }
            public string StoreId { get; set; }
            public string Code { get; set; }
            public string Department { get; set; }
            public string Ref { get; set; }
            public DateTime Date { get; set; }
           
            public decimal? Amount { get; set; }
            public decimal? TotalAmount { get; set; }
          
            public string CustomF1 { get; set; }
            public string CustomF2 { get; set; }
            public string CustomF3 { get; set; }
            public string CustomF4 { get; set; }
            public string CustomF5 { get; set; }

            public decimal? CustomNumF1 { get; set; }
            public decimal? CustomNumF2 { get; set; }
            public decimal? CustomNumF3 { get; set; }
            public decimal? CustomNumF4 { get; set; }
            public decimal? CustomNumF5 { get; set; }
            public bool? IsBold { get; set; }
            public bool? IsTitle { get; set; }
        }

        public class EODPrintNoneCashCollection
        {
            public string CompanyCode { get; set; }
            public string StoreId { get; set; }
            public string Code { get; set; }
            public string Department { get; set; }
            public string Ref { get; set; }
            public DateTime Date { get; set; }

            public decimal? Amount { get; set; }
            public decimal? TotalAmount { get; set; }

            public string CustomF1 { get; set; }
            public string CustomF2 { get; set; }
            public string CustomF3 { get; set; }
            public string CustomF4 { get; set; }
            public string CustomF5 { get; set; }

            public decimal? CustomNumF1 { get; set; }
            public decimal? CustomNumF2 { get; set; }
            public decimal? CustomNumF3 { get; set; }
            public decimal? CustomNumF4 { get; set; }
            public decimal? CustomNumF5 { get; set; }
            public bool? IsBold { get; set; }
            public bool? IsTitle { get; set; }
        }
        public async Task<GenericResult> EndDateSummaryPaymentPrint(string companyCode, string storeId, string dailyId)
        {
            GenericResult result = new GenericResult();
            //var listdata = await GetAll(companyCode, storeId);
            //var list = listdata.Data as List<TEndDate>;
            //var item = list.Where(x => x.Date.Value.ToString("yyyy/MM/dd") == DateTime.Parse(transdate).ToString("yyyy/MM/dd")).FirstOrDefault();
            using (IDbConnection db = _endateRepository.GetConnection(GConnection.EndDateConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    //if (db.State == ConnectionState.Closed)
                    //    db.Open();
                    //var parameters = new DynamicParameters();
                    //parameters.Add("CompanyCode", companyCode);
                    //parameters.Add("StoreId", storeId);
                    //parameters.Add("Date", transdate);
                    ////ar dataX = await db.QueryAsync(query, null);
                    //var data = await db.QueryAsync("[USP_S_EndDateSummary]", parameters, commandType: CommandType.StoredProcedure);
                    //result.Success = true;
                    //result.Data = data;
                    //db.Close();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("DailyId", dailyId);

                    var data = await db.QueryAsync<EODSummaryModel>("[USP_RPT_EODSummary]", parameters, commandType: CommandType.StoredProcedure);
                    result.Success = true;
                    result.Data = data;
                    db.Close();
                    return result;


                    //'{companyCode}','{storeId}' ,'{transdate}'
                    //string query = $"USP_RPT_EODSummary_New";
                    //var reader = await db.QueryMultipleAsync(query, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    //var Header = reader.Read<EODPrintViewModel>().FirstOrDefault();
                    //if (Header == null || string.IsNullOrEmpty(Header.DailyId))
                    //{
                    //    Header = new EODPrintViewModel();
                    //}
                    //var SalesByDepartment = reader.Read<EODPrintCollectionByDepartment>().ToList();
                    //var CashCollected = reader.Read<EODPrintCashCollection>().ToList();
                    //var NoneCashCollected = reader.Read<EODPrintNoneCashCollection>().ToList();

                    //Header.Departments = SalesByDepartment;
                    //Header.CashCollections = CashCollected;
                    //Header.NoneCashCollections = NoneCashCollected;
                    
                    //db.Close();
                    //result.Success = true;
                    //result.Data = Header;

                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }

        }

        public async Task<GenericResult> EndDateSummaryByDepartment(string companyCode, string storeId, string Userlogin, string FDate, string TDate, string dailyId)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _endateRepository.GetConnection(GConnection.EndDateConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("Userlogin", Userlogin);
                    parameters.Add("FDate", FDate);
                    parameters.Add("TDate", TDate);
                    parameters.Add("DailyId", dailyId);
                    var items = await db.QueryAsync("USP_RPT_EOD_SummaryByDeparment", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
                    result.Success = true;
                    result.Data = items;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    //result.Data = failedlist;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
            }
            //result.Data = failedlist;
            return result;

        }

        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Code)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _endateRepository.GetConnection(GConnection.EndDateConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Id", Code);

                    string query = $"select * from T_EndDate with (nolock) where CompanyCode='{CompanyCode}' and StoreId = '{StoreId}' and Id='{Code}'";
                    string queryDetail = $"select * from T_EndDateDetail with (nolock) where CompanyCode='{CompanyCode}' and StoreId = '{StoreId}' and EndDateId='{Code}'";
                    string queryPayment = $"select * from T_EndDatePayment with (nolock) where CompanyCode='{CompanyCode}' and StoreId = '{StoreId}' and EndDateId='{Code}'";


                    TEndDate Data = new TEndDate();
                    var res = await _endateRepository.GetAsync(query, null, commandType: CommandType.Text, GConnection.EndDateConnection);
                    Data = res;
                    var details = await db.QueryAsync<TEndDateDetail>("USP_S_T_EndDateDetail", parameters, commandType: CommandType.StoredProcedure);
                    var payments = await db.QueryAsync<TEndDatePayment>(queryPayment, null, commandType: CommandType.Text);
                    Data.Lines = details.ToList();
                    Data.Payments = payments.ToList();

                    db.Close();
                    result.Success = true;
                    result.Data = Data;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
            //try
            //{

            //    var data = await _endateRepository.GetAsync(query, null, commandType: CommandType.Text);
            //    return data;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }

        public async Task<GenericResult> GetDetailBfEnd(string CompanyCode, string StoreId, DateTime Date)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _endateRepository.GetConnection(GConnection.EndDateConnection))
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters(); 
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("Date", Date);
                    //var dataX = await db.QueryAsync(query, null);
                    var details = await db.QueryAsync<TEndDateDetail>("USP_S_EndOfDateDetail", parameters, commandType: CommandType.StoredProcedure);

                    var payments = await db.QueryAsync<TEndDatePayment>("USP_S_EndOfDatePayment", parameters, commandType: CommandType.StoredProcedure);

                    TEndDate Data = new TEndDate();
                    Data.CompanyCode = CompanyCode;
                    Data.StoreId = StoreId;
                    Data.Date = Date;

                    Data.Lines = details.ToList();
                    Data.Payments = payments.ToList();

                    db.Close();
                    result.Success = true;
                    result.Data = Data;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
            //try
            //{
            //    var parameters = new DynamicParameters();
                 

            //    parameters.Add("CompanyCode", CompanyCode);
            //    parameters.Add("StoreId", StoreId);
            //    parameters.Add("Date", Date);
              
            //    var details = await _endateRepository.GetAllAsync("USP_S_EndOfDateDetail", parameters, commandType: CommandType.StoredProcedure);
                 
            //    var payments = await _endateRepository.GetAllAsync("USP_S_EndOfDatePayment", parameters, commandType: CommandType.StoredProcedure);

            //    TEndDate result = new TEndDate();
            //    result.CompanyCode = CompanyCode;
            //    result.StoreId = StoreId;
            //    result.Date = Date;

            //    result.Lines = details;
            //    result.Payments = payments;



            //    //var data = await _endateRepository.GetAsync($"select * from T_EndDate with (nolock) where CompanyCode='{CompanyCode}' and StoreId = '{StoreId}' and Id='{Code}'", null, commandType: CommandType.Text);
            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }


        public async Task<GenericResult> Update(TEndDate model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("Id", model.Id);
                parameters.Add("Description", model.Description);
                parameters.Add("Remark", model.Remark);
                parameters.Add("Status", model.Status);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                var affectedRows = _endateRepository.Update("USP_U_T_EndDate", parameters, commandType: CommandType.StoredProcedure, GConnection.EndDateConnection);
                result.Success = true;
                //result.Message = key;
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
