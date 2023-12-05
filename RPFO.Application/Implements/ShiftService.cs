
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Infrastructure;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Constants;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class ShiftService : IShiftService
    {
        private readonly IGenericRepository<TShiftHeader> _shiftHeaderRepository;
        private readonly IGenericRepository<TShiftLine> _shiftLineRepository;
        private readonly IEndDateService _endDateService;
        private readonly IPickupAmountService _pickupService;
        private readonly IStoreService _storeService;
        private readonly IGeneralSettingService _settingService;
        private readonly IMapper _mapper;
         
        string PrefixShift= "";
        public ShiftService(IGenericRepository<TShiftHeader> shiftHeaderRepository, IConfiguration config, IGeneralSettingService settingService, IPickupAmountService pickupService, IStoreService storeService, IEndDateService endDateService, IGenericRepository<TShiftLine> shiftLineRepository, IMapper mapper /*IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _shiftHeaderRepository = shiftHeaderRepository;
            _shiftLineRepository = shiftLineRepository;
            _endDateService = endDateService;
            _pickupService = pickupService;
            _settingService = settingService;
            _mapper = mapper;
            _storeService = storeService;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            PrefixShift = Utilities.Helpers.Encryptor.DecryptString(config.GetConnectionString("PrefixShift"), RPFO.Utilities.Constants.AppConstants.TEXT_PHRASE);
            InitFirstService();


        }
        public GenericResult InitFirstService()
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _shiftHeaderRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
 
                    string USP_ReCalcuAmtNotBankIn = "IF (OBJECT_ID('USP_ReCalcuAmtNotBankIn') IS NULL)  begin declare @string nvarchar(MAX) = ''; " +
                   " set @string = 'create PROCEDURE [dbo].[USP_ReCalcuAmtNotBankIn] @CompanyCode	nvarchar(50), @StoreId	nvarchar(50), @DailyId	nvarchar(50), @DocDate	nvarchar(50), @BankInAmt Decimal(19,6) = null AS  " +
                   " begin   declare @StatusEOD nvarchar(50); declare @AmtnotBankInEOD decimal(19, 6);declare @OldAmountNotBankIn decimal(19, 6);declare @SumBankIn decimal(19, 6);" +
                   " declare @CashAmount decimal(19, 6);declare @AmountNotBankIn decimal(19, 6);declare @MaxDate datetime; Declare @Currency nvarchar(15); DECLARE @EndDateID Nvarchar(50) " +
                   " if(ISNULL(CONVERT(nvarchar(50), @BankInAmt), '''') <> '''') begin " +
                   "    select @StatusEOD = Status from T_EndDate  where Description = @DailyId and CompanyCode= @CompanyCode and StoreId = @StoreId " +
                   "    if (@StatusEOD = ''C'') begin  update T_EndDate set AmtNotInBank = AmtNotInBank - @BankInAmt  where  CONVERT(date, Date) >= CONVERT(date, @DocDate) and CompanyCode= @CompanyCode and StoreId = @StoreId  end" +
                   " end  else begin " +
                   " set @Currency = ( select top 1 CurrencyCode from M_Store with (nolock)  where StoreId = @StoreId and CompanyCode= @CompanyCode)" +
                   " DECLARE EndDateCalcu_LINES CURSOR FOR select Id, Description from T_EndDate where Date >= @DocDate and CompanyCode = @CompanyCode and StoreId= @StoreId" +
                   " OPEN EndDateCalcu_LINES " +
                   " FETCH NEXT FROM EndDateCalcu_LINES " +
                   " INTO @EndDateID, @DailyId WHILE @@FETCH_STATUS = 0 " +
                   " BEGIN " +
                   "    set @OldAmountNotBankIn = (select Top 1 ISNULL( AmtNotInBank,0) from T_EndDate where Description < @DailyId and  StoreId = @StoreId and CompanyCode = @CompanyCode order by Description desc)" +
                   "    select @SumBankIn =  ISNULL( SUM(isnull(BankInAmt,0)),0) from T_BankIn with (nolock)" +
                   "    where DailyId = @DailyId and StoreId = @StoreId and CompanyCode = @CompanyCode and isnull(Status,''I'') = ''A''" +
                   "    SELECT @CashAmount =  SUM(isnull(E.CollectedAmount,0))" +
                   "    FROM T_EndDatePayment  E with (nolock) INNER JOIN M_PaymentMethod P with (nolock) ON E.CompanyCode = P.CompanyCode  AND  E.PaymentCode = P.PaymentCode " +
                   "    WHERE E.CompanyCode = @CompanyCode AND E.EndDateId = @EndDateID	AND E.StoreId = @StoreId AND P.PaymentType = ''C''" +
                   "    GROUP BY  P.PaymentCode, P.PaymentDesc" +
                   "    select @AmountNotBankIn =  ISNULL(( (isnull(@OldAmountNotBankIn,0) + isnull(@CashAmount,0)) - isnull(@SumBankIn,0) ), 0)" +
                   "    update T_EndDate set AmtNotInBank = @AmountNotBankIn where Description = @DailyId and CompanyCode = @CompanyCode" +
                   "    FETCH NEXT FROM EndDateCalcu_LINES" +
                   "    INTO @EndDateID, @DailyId" +
                   "    End CLOSE EndDateCalcu_LINES  DEALLOCATE EndDateCalcu_LINES" +
                   " end end '; EXECUTE sp_executesql @string;  end";

                    db.Execute(USP_ReCalcuAmtNotBankIn);



                    result.Success = true;

                    //tran.Commit();

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
        public async Task<GenericResult> Create(TShiftHeader model)
        {
            GenericResult result = new GenericResult();
            string flag = "";
            try
            {
                var parameters = new DynamicParameters();

                flag = "Get Daily Id";
                string getdailyQuery = $"select dbo.[fnc_GetDailyID]( '{model.StoreId}','{model.CompanyCode}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                flag += " " + getdailyQuery;
                string getDailyId = _shiftHeaderRepository.GetScalar(getdailyQuery, null, commandType: CommandType.Text, GConnection.ShiftConnection);

                if (string.IsNullOrEmpty(getDailyId))
                {
                    result.Success = false;
                    result.Message = $"Can't get DailyId. Please contact to your admin.";
                    return result;
                }
                var endateData = await _endDateService.GetEndDateByDailyId(model.CompanyCode, model.StoreId, getDailyId);
                var endate = endateData.Data as TEndDate;
                if (endate != null)
                {
                    if (endate.Status == "C")
                    {
                        result.Success = false;
                        result.Message = $"Can't create new shift. Because {DateTime.Now.ToString("yyyy-MM-dd")} has been closed";
                        return result;
                    }
                }
                flag = "Get Key";
                //string key = "";
                //if (string.IsNullOrEmpty(PrefixShift))
                //{
                //    key = _shiftHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenShift] (N'{model.StoreId}')", null, commandType: CommandType.Text, GConnection.ShiftConnection);
                //}
                //else
                //{
                //    key = _shiftHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenShiftWithPrefix] (N'{model.CompanyCode}', N'{model.StoreId}', N'{PrefixShift}')", null, commandType: CommandType.Text, GConnection.ShiftConnection);

                //}

                //model.ShiftId = key;

                //if(!string.IsNullOrEmpty(model.ShiftId))
                //{
                //    var data = await _shiftHeaderRepository.GetAsync($"select * from T_ShiftHeader with (nolock) where CompanyCode=N'{model.CompanyCode}' and StoreId=N'{model.StoreId}' and ShiftId = N'{model.ShiftId}'  order by CreatedOn desc", null, commandType: CommandType.Text);
                //    if(data!=null)
                //    {
                //        result.Success = false;
                //        result.Message = "Shift " + model.ShiftId + " has existed. Please try again";
                //        return result;
                //    }    
                //}    

                model.ShiftId = "";
                parameters.Add("ShiftId", model.ShiftId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("DailyId", getDailyId);
                //string.IsNullOrEmpty(model.DailyId) ? DateTime.Now.ToString("yyMMdd") : model.DailyId
                parameters.Add("DeviceId", string.IsNullOrEmpty(model.DeviceId) ? "Unsigned" : model.DeviceId);
                parameters.Add("OpenAmt", model.OpenAmt);
                parameters.Add("EndAmt", model.EndAmt);

                parameters.Add("ShiftTotal", model.ShiftTotal);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                string GuidId = Guid.NewGuid().ToString();
                parameters.Add("Id", GuidId);
                if(!string.IsNullOrEmpty(PrefixShift))
                {
                    parameters.Add("PrefixShift", PrefixShift);
                }    
                
                flag = "Insert";
                var affectedRows = _shiftHeaderRepository.GetScalar("USP_I_T_ShiftHeader", parameters, commandType: CommandType.StoredProcedure, GConnection.ShiftConnection);
                //result.Success = true;
                //result.Message = affectedRows;
                var returnData = await GetByGuidId(model.CompanyCode, model.StoreId, GuidId);
                if (returnData.Success && returnData.Data != null)
                {
                    var data = returnData.Data as TShiftHeader;
                    result.Success = true;
                    result.Message = data.ShiftId;
                }
                else
                {
                    result.Success = true;
                    result.Message = affectedRows;
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = flag + " " + ex.Message;


            }
            return result;


        }

    
        public async Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> EndShift(TShiftHeader model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ShiftId", model.ShiftId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("EndAmt", model.EndAmt);
                parameters.Add("CreateBy", model.CreatedBy);
                var data = _shiftHeaderRepository.Execute("USP_EndShift", parameters, commandType: CommandType.StoredProcedure, GConnection.ShiftConnection);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _shiftHeaderRepository.GetAllAsync($"select * from T_ShiftHeader with (nolock) where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text, GConnection.ShiftConnection);
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
        public async Task<GenericResult> GetByGuidId(string CompanyCode, string StoreId, string GuidId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _shiftHeaderRepository.GetAsync($"select * from T_ShiftHeader with (nolock) where CompanyCode=N'{CompanyCode}' and StoreId=N'{StoreId}' and Id = N'{GuidId}'  order by CreatedOn desc", null, commandType: CommandType.Text, GConnection.ShiftConnection);
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
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreId, string top)
        {
            GenericResult result = new GenericResult();
            try
            {
                string query = $"select * from T_ShiftHeader with (nolock) where CompanyCode='{CompanyCode}' and StoreId = '{StoreId}'  ";
                if(!string.IsNullOrEmpty(top))
                {
                    query = $"select top {top} * from T_ShiftHeader with (nolock) where CompanyCode='{CompanyCode}' and StoreId = '{StoreId}'  ";
                }    
               
                query += " order by CreatedOn desc";

                var data = await _shiftHeaderRepository.GetAllAsync(query, null, commandType: CommandType.Text, GConnection.ShiftConnection);
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

        public async Task<GenericResult> LoadOpenShift(string companyCode, string storeId, string transdate, string UserId, string CounterId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("StoreId", storeId);
                parameters.Add("TransDate", transdate);
                parameters.Add("UserId", UserId);
                parameters.Add("CounterId", CounterId);
                var data = await _shiftHeaderRepository.GetAllAsync("[USP_S_T_OpenShift]", parameters, commandType: CommandType.StoredProcedure, GConnection.ShiftConnection);
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
        //public async Task<GenericResult> EndDateSummary(string companyCode, string storeId, string transdate)
        //{
        //    GenericResult result = new GenericResult();
        //    using (IDbConnection db = _shiftHeaderRepository.GetConnection())
        //    {
        //        try
        //        {
        //            if (db.State == ConnectionState.Closed)
        //                db.Open();


        //            string query = $" USP_GetSumaryHeaderDate '{companyCode}','{storeId}' ,'{transdate}' ";
        //            var dataX = await db.QueryFirstAsync<EndShiftPrintViewModel>(query, null);
        //            string queryLine = $"USP_S_SummaryShiftDate '{companyCode}','{storeId}','{transdate}'";
        //            string queryPayment = $"USP_S_EndDateSummary '{companyCode}','{storeId}','{transdate}'";
        //            string queryInventory = $"USP_S_InventorySummaryShiftDate '{companyCode}','{storeId}','{transdate}'";

        //            var dataLine = await db.QueryAsync<EndShiftItemSumary>(queryLine, null);

        //            var dataPayment = await db.QueryAsync<EndShiftPayment>(queryPayment, null);
        //            var dataInventory = await db.QueryAsync<EndShiftItemSumary>(queryInventory, null);
        //            dataX.Payments = dataPayment.ToList();
        //            dataX.ItemSumary = dataLine.ToList();
        //            dataX.ItemInventorySumary = dataInventory.ToList();
        //            db.Close();
        //            result.Success = true;
        //            result.Data = dataX;
        //        }
        //        catch (Exception ex)
        //        {
        //            if (db.State == ConnectionState.Open)
        //                db.Close();
        //            result.Success = false;
        //            result.Message = ex.Message;
        //            //throw ex;
        //        }
        //        finally
        //        {
        //            if (db.State == ConnectionState.Open)
        //                db.Close();
        //        }
        //        return result;
        //    }

        //}
        public async Task<GenericResult> ShiftSummaryByDepartment(string companyCode, string storeId, string Userlogin, string FDate, string TDate, string dailyId, string shiftId)
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _shiftHeaderRepository.GetConnection())
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
                    parameters.Add("ShiftId", shiftId);
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

        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                ShiftViewModel Model = new ShiftViewModel();

                TShiftHeader header = await _shiftHeaderRepository.GetAsync($"select * from T_ShiftHeader with (nolock) where CompanyCode='{CompanyCode}' and ShiftId='{Code}'", null, commandType: CommandType.Text, GConnection.ShiftConnection);

                List<TShiftLine> lines = await _shiftLineRepository.GetAllAsync($"select * from T_ShiftLine with (nolock) where  CompanyCode='{CompanyCode}' and ShiftId='{Code}'", null, commandType: CommandType.Text, GConnection.ShiftConnection);

                //var settingData = await _settingService.GetGeneralSettingByStore(CompanyCode, header.StoreId);
                //List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                //if (settingData.Success)
                //{
                //    SettingList = settingData.Data as List<GeneralSettingStore>;
                //}
                //var summaryByDepartment = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "SumByDepInShift").FirstOrDefault();
                //if (summaryByDepartment != null && (summaryByDepartment.SettingValue == "true" || summaryByDepartment.SettingValue == "1"))
                //{
                //    await ShiftSummaryByDepartment(CompanyCode, header.StoreId, "", header.CreatedOn.Value.ToString("yyyy-MM-dd HH:mm:ss"), header.CreatedOn.Value.ToString("yyyy-MM-dd HH:mm:ss"), "", header.ShiftId);
                //}
              
                Model = _mapper.Map<ShiftViewModel>(header);
                Model.Lines = lines;
                result.Success = true;
                result.Data = Model;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }


        public async Task<GenericResult> GetOpenShiftSummary(string companyCode, string storeId, DateTime? Date)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _shiftLineRepository.GetConnection())
            {
                //string StoreId,
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var openListData = await LoadOpenShift(companyCode, storeId, Date.Value.ToString("yyyy/MM/dd"), "", "");
                    var openList = openListData.Data as List<TShiftHeader>;
                    List<EndShiftPrintViewModel> SumList = new List<EndShiftPrintViewModel>();
                    if (openList != null && openList.Count > 0)
                    {
                        foreach (var shift in openList)
                        {
                            var sumShift = await GetEndShiftSummary(companyCode, storeId, shift.ShiftId);
                            SumList.Add(sumShift.Data as EndShiftPrintViewModel);
                        }

                        db.Close();
                        result.Success = true;
                        result.Data = SumList;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Open list is null";
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


        public async Task<GenericResult> GetEndShiftSummary(string companyCode, string storeId, string shiftId)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _shiftLineRepository.GetConnection(GConnection.ShiftConnection))
            {
                //string StoreId,
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    var settingData = await _settingService.GetGeneralSettingByStore(companyCode, storeId);
                    List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                    if (settingData.Success)
                    { 
                        SettingList = settingData.Data as List<GeneralSettingStore>;  
                    }


                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", companyCode);
                    parameters.Add("StoreId", storeId);
                    parameters.Add("ShiftId", shiftId);

                    //string query = $"USP_GetSumaryHeaderShift '{companyCode}', '{storeId}' ,'{shiftId}' ";
                    string query = $"USP_GetSumaryHeaderShift";
                    var dataX = db.QueryFirstOrDefault<EndShiftPrintViewModel>(query, parameters, commandType: CommandType.StoredProcedure);
                    if (dataX != null)
                    {

                        string queryPayment = "";
                        //if (dataX.Status == "C")
                        //{
                        //    queryPayment = $"USP_S_T_ShiftPayment";
                        //}

                        var groupByCashier = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "EOSGroupByCashier").FirstOrDefault();
                        if (groupByCashier != null && (groupByCashier.SettingValue == "true" || groupByCashier.SettingValue == "1"))
                        {
                           queryPayment = $"USP_S_SummaryShiftPaymentGroupCashier"; 
                        }
                        else
                        {
                            queryPayment = $"USP_S_SummaryShiftPayment";
                            //if (dataX.Status == "C")
                            //{
                            //    queryPayment = $"USP_S_T_ShiftPayment";
                            //}
                        }
                        if (dataX.Status == "C")
                        {
                            queryPayment = $"USP_S_T_ShiftPayment";
                        }
                        string queryLine = $"USP_S_SummaryShiftDetail";

                        string queryInventory = $"USP_S_InventorySummaryShiftDetail";

                        //string paymentGroupCashier = $"USP_S_SummaryShiftPaymentGroupCashier";
                     
                         
                        //    $"left join T_SalesHeader t2  with (nolock) on t1.TransId = t2.TransId where t2.companyCode='{companyCode}'  and t2.ShiftId = '{shiftId}'" +
                        //    $"  group by ItemCode, Description, UOMCode, Price, LineTotal";
                        var dataLine = await db.QueryAsync<EndShiftItemSumary>(queryLine, parameters, commandType: CommandType.StoredProcedure );
                        //string queryPayment = $"select PaymentCode,  Sum(CollectedAmount ) TotalAmount from T_SalesPayment t1 left join T_SalesHeader t2  with (nolock) on t1.TransId = t2.TransId " +
                        //    $" where t2.companyCode='{companyCode}' and t2.ShiftId = '{shiftId}' group by PaymentCode ";
                        var dataPayment = await db.QueryAsync<EndShiftPayment>(queryPayment, parameters, commandType: CommandType.StoredProcedure );
                        //var dataCashierPayment = await db.QueryAsync<EndShiftPayment>(paymentGroupCashier, parameters, commandType: CommandType.StoredProcedure);
                        var dataInventory = await db.QueryAsync<EndShiftItemSumary>(queryInventory, parameters, commandType: CommandType.StoredProcedure );
                        dataX.Payments = dataPayment.ToList();
                        dataX.ItemSumary = dataLine.ToList();
                        //dataX.CashierPayments = dataCashierPayment.ToList();
                        dataX.ItemInventorySumary = dataInventory.ToList();
                        db.Close();
                        result.Success = true;
                        result.Data = dataX;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Shift " + shiftId + " data not found ";
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
        public async Task<string> GetNewShiftCode(string CompanyCode, string StoreId, string TerminalId)
        {
            string key = "";
            if (string.IsNullOrEmpty(PrefixShift))
            {
                //, N'{TerminalId}'
                key = _shiftHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenShift] (N'{StoreId}')", null, commandType: CommandType.Text, GConnection.ShiftConnection);
            }
            else
            {
                 //, N'{TerminalId}'
                key = _shiftHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenShiftWithPrefix] (N'{CompanyCode}', N'{StoreId}', N'{PrefixShift}' )", null, commandType: CommandType.Text, GConnection.ShiftConnection);

            }
            //_shiftHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{Id}')", null, commandType: CommandType.Text);
            return key;
        }

        public async Task<PagedList<TShiftHeader>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _shiftHeaderRepository.GetAllAsync($"select * from T_ShiftHeader with (nolock) where CreateBy like N'%{userParams.keyword}%' or ShiftId like N'%{userParams.keyword}%'", null, commandType: CommandType.Text, GConnection.ShiftConnection);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.CompanyCode);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.ShiftId);
                }
                return await PagedList<TShiftHeader>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task<GenericResult> Update(EndShiftPrintViewModel model)
        {
            GenericResult result = new GenericResult();
            string flag = "";
            using (IDbConnection db = _shiftHeaderRepository.GetConnection(GConnection.ShiftConnection))
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
                            //var endate = await _endDateService.GetEndDateByDate(model.CompanyCode, model.StoreId, DateTime.Now.ToString("yyyy-MM-dd"));
                            //if (endate != null)
                            //{
                            //    if (endate.Status == "C")
                            //    {
                            //        result.Success = false;
                            //        result.Message = $"Can't create new shift. Because {DateTime.Now.ToString("yyyy-MM-dd")} has been closed";
                            //        return result;
                            //    }
                            //}

                            //string key = _shiftHeaderRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                            //model.ShiftId = key;
                            if(string.IsNullOrEmpty(model.DailyId))
                            {
                                result.Success = false;
                                result.Message = $"Can't found DailyId. Please check your data";
                                return result;
                            }
                            parameters.Add("ShiftId", model.ShiftId, DbType.String);
                            parameters.Add("CompanyCode", model.CompanyCode);
                            parameters.Add("StoreId", model.StoreId);
                            parameters.Add("DailyId", string.IsNullOrEmpty(model.DailyId) ? DateTime.Now.ToString("yyMMdd") : model.DailyId);
                            parameters.Add("DeviceId", string.IsNullOrEmpty(model.DeviceId) ? "Unsigned" : model.DeviceId);
                            parameters.Add("OpenAmt", model.OpenAmt);
                            parameters.Add("EndAmt", model.EndAmt);
                            parameters.Add("ShiftTotal", model.ShiftTotal);
                            parameters.Add("ModifiedBy", model.ModifiedBy);
                            parameters.Add("Status", model.Status);

                            flag = "Shift Header";

                            db.Execute("USP_U_T_ShiftHeader", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                            //var affectedRows = db.Execute("USP_U_T_ShiftHeader", parameters, commandType: CommandType.StoredProcedure, tran);

                            foreach (var line in model.Payments)
                            {
                                if (string.IsNullOrEmpty(line.Currency))
                                {
                                    var StoreData = await _storeService.GetByCode(model.CompanyCode, model.StoreId);
                                    var Store = StoreData.Data as MStore;
                                    line.Currency = Store.CurrencyCode;
                                }
                                parameters = new DynamicParameters();
                                parameters.Add("ShiftId", model.ShiftId, DbType.String);
                                parameters.Add("Currency", line.Currency, DbType.String);
                                parameters.Add("PaymentCode", line.PaymentCode, DbType.String);
                                parameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                                parameters.Add("FCValue", line.FCAmount);
                                parameters.Add("Value", line.TotalAmt);
                                parameters.Add("CounterId", line.CounterId);

                                parameters.Add("CollectAmount", line.CollectedAmount);
                                parameters.Add("FCCollectedAmount", null);

                                if (line.Rate > 1)
                                {

                                    parameters.Add("FCCollectedAmount", line.CollectedAmount);
                                    parameters.Add("CollectAmount", line.CollectedAmount * line.Rate);
                                }
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("BankInAmt", line.BankInAmt);
                                if (!line.BankInAmt.HasValue)
                                {
                                    line.BankInAmt = 0;
                                }

                                if (!line.CountedBalance.HasValue)
                                {
                                    line.CountedBalance = 0;
                                }

                                parameters.Add("BankInBalance", line.BankInAmt - line.TotalAmt);
                                parameters.Add("CountedBalance", line.CollectedAmount - line.TotalAmt);
                                parameters.Add("Status", "C");
                                parameters.Add("Cashier", line.Cashier);
                                parameters.Add("CustomF1", line.CustomF1);
                                parameters.Add("CustomF2", line.CustomF1);
                                parameters.Add("CustomF3", line.CustomF1);
                                parameters.Add("CustomF4", line.CustomF1);
                                parameters.Add("CustomF5", line.CustomF1);

                                flag = "Shift Line";

                                db.Execute("USP_I_T_ShiftLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                //var affectedLine = _shiftLineRepository.Insert("USP_I_T_ShiftLine", parameters, commandType: CommandType.StoredProcedure);
                            }
                            parameters = new DynamicParameters();
                            parameters.Add("ShiftId", model.ShiftId, DbType.String);
                            parameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                            parameters.Add("StoreId", model.StoreId, DbType.String);

                            flag = "Shift Summary";

                            db.Execute("USP_I_T_ShiftSummary", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

                            //get Counter Id  to  Pickup

                            //var Amounts=  _pickupService.GetItems(model.CompanyCode, model.StoreId, model.CounterId,"" ,"","", DateTime.Now , DateTime.Now);

                            var settingData = await _settingService.GetGeneralSettingByStore(model.CompanyCode, model.StoreId);
                            List<GeneralSettingStore> SettingList = new List<GeneralSettingStore>();
                            if (settingData.Success)
                            {
                                SettingList = settingData.Data as List<GeneralSettingStore>;
                            }
                            var setting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "PickupAmount").FirstOrDefault();
                            if (setting != null && (setting.SettingValue == "true" || setting.SettingValue == "1"))
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                                parameters.Add("StoreId", model.StoreId, DbType.String);
                                parameters.Add("CounterId", model.CounterId, DbType.String);
                                parameters.Add("ShiftId", model.ShiftId, DbType.String);
                                parameters.Add("DailyId", model.DailyId, DbType.String);

                                //string queryUpdate = $"USP_U_UpShift2PickupByCounter '{model.CompanyCode}','{model.StoreId}','{model.CounterId}','{model.ShiftId}','{DateTime.Now.ToString("yyyy-MM-dd")}'";
                                flag = "Pickup Update";

                                db.Execute("USP_U_UpShift2PickupByCounter", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                 
                            }

                            var clearHoldBillSetting = SettingList.FirstOrDefault().GeneralSettings.Where(x => x.SettingId == "ClearHoldBillAfterEOS").FirstOrDefault();
                            if (clearHoldBillSetting != null && (clearHoldBillSetting.SettingValue == "true" || clearHoldBillSetting.SettingValue == "1"))
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                                parameters.Add("StoreId", model.StoreId, DbType.String); 
                                parameters.Add("ShiftId", model.ShiftId, DbType.String);
                                parameters.Add("ModifiedBy", "System", DbType.String);

                                //string queryUpdate = $"USP_U_UpShift2PickupByCounter '{model.CompanyCode}','{model.StoreId}','{model.CounterId}','{model.ShiftId}','{DateTime.Now.ToString("yyyy-MM-dd")}'";
                                flag = "Clear Hold Bill";

                                db.Execute("USP_ClearHoldBillByShift", parameters, commandType: CommandType.StoredProcedure, transaction: tran);

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

                    result.Success = false;
                    result.Message = flag + " - " + ex.Message;
                }

            }
            return result;
        }
    }

}
