
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
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
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IGenericRepository<MPaymentMethod> _paymentRepository;
        private readonly IGenericRepository<MPaymentMethodMapping> _mappingRepository;

        private readonly IMapper _mapper;
        public PaymentMethodService(IGenericRepository<MPaymentMethod> paymentRepository, IGenericRepository<MPaymentMethodMapping> mappingRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _paymentRepository = paymentRepository;
            _mappingRepository = mappingRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            initService();
        }
        public GenericResult initService()
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _paymentRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string queryCheckAndCreateGetQuery = "IF (OBJECT_ID('USP_S_M_PaymentMethodMapping') IS NULL)  begin declare @string nvarchar(MAX) = '';" +
                      " set @string = 'create PROCEDURE [dbo].[USP_S_M_PaymentMethodMapping] @CompanyCode nvarchar(50), @PaymentCode	nvarchar(150) AS " +
                      " begin " +
                      " SELECT * FROM M_PaymentMethodMapping P WITH (NOLOCK)  WHERE (ISNULL(@CompanyCode, '''') = '''' OR P.CompanyCode = @CompanyCode ) AND (P.PaymentCode = @PaymentCode OR ISNULL(@PaymentCode, '''') = '''') " +
                      " end '; " +
                      "EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreateGetQuery);


                    string queryCheckAndCreateTableQuery = "IF ( not EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'M_PaymentMethodMapping'))   begin declare @string nvarchar(MAX) = '';" +
                   " set @string = 'create TABLE [dbo].[M_PaymentMethodMapping] [CompanyCode] [nvarchar](50) NOT NULL, [PaymentCode] [nvarchar](150) NOT NULL, [FatherId] [nvarchar](150) NOT NULL, [CustomF1] [nvarchar](500) NULL, [CustomF2] [nvarchar](500) NULL, [CustomF3] [nvarchar](500) NULL, [CustomF4] [nvarchar](500) NULL, [CustomF5] [nvarchar](500) NULL, [CustomF6] [nvarchar](500) NULL, [Status] [nvarchar](50) NULL,  CONSTRAINT [PK_M_PaymentMethodMapping] PRIMARY KEY CLUSTERED  ( [CompanyCode] ASC, [PaymentCode] ASC, [FatherId] ASC ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] ) ON [PRIMARY] " +
                    
                   "EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreateTableQuery);



                    string queryCheckAndCreateAddQuery = "IF (OBJECT_ID('USP_I_M_PaymentMethodMapping') IS NULL)  begin declare @string nvarchar(MAX) = '';" +
                  " set @string = 'create PROCEDURE [dbo].[USP_I_M_PaymentMethodMapping] @CompanyCode nvarchar(50), @PaymentCode	nvarchar(150), @FatherId nvarchar(150), @CustomF1 nvarchar(500), @CustomF2	nvarchar(500), @CustomF3	nvarchar(500), @CustomF4	nvarchar(500), @CustomF5	nvarchar(500), @CustomF6	nvarchar(500), @Status	nvarchar(50) AS " +
                  " begin " +
                  " INSERT INTO [dbo].M_PaymentMethodMapping ([CompanyCode], [PaymentCode], FatherId, CustomF1, CustomF2, CustomF3, CustomF4, CustomF5, CustomF6 ,[Status] )" +
                  " VALUES  (@CompanyCode, @PaymentCode, @FatherId, @CustomF1, @CustomF2, @CustomF3, @CustomF4, @CustomF5, @CustomF6, @Status )" +
                  " end '; " +
                  "EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreateAddQuery);


                    string queryCheckAndCreateDelQuery = "IF (OBJECT_ID('USP_D_M_PaymentMethodMapping') IS NULL)  begin declare @string nvarchar(MAX) = '';" +
                        " set @string = 'create PROCEDURE [dbo].[USP_D_M_PaymentMethodMapping] @CompanyCode nvarchar(50), @PaymentCode	nvarchar(150) AS " +
                        "begin " +
                        "DELETE  FROM [dbo].M_PaymentMethodMapping  WHERE CompanyCode = @CompanyCode AND PaymentCode = @PaymentCode end '; " +
                        "EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreateDelQuery);


                    //queryCheckAndCreate = $" IF NOT EXISTS (SELECT *  FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[fnc_GetRoundingPaymentDifByDefCurStore]') AND type IN(N'FN', N'IF', N'TF', N'FS', N'FT')) " +
                    //    $" begin declare @string nvarchar(MAX) = ''; " +
                    //    $" set @string = 'create FUNCTION[dbo].[fnc_GetRoundingPaymentDifByDefCurStore]( @CompanyCode nvarchar(50), @StoreId nvarchar(50) ) " +
                    //    $" RETURNS decimal(19, 6) AS " +
                    //    $" BEGIN " +
                    //    $" Declare @RoundingOff decimal(19, 6)  " +
                    //    $" set @RoundingOff = ( select RoundingPaymentDif from M_Currency t1 with (nolock) " +
                    //    $" left join M_Store t2 with (nolock) on t1.CurrencyCode = t2.CurrencyCode  " +
                    //    $" where t2.CompanyCode = @CompanyCode and t2.StoreId = @StoreId ) " +
                    //    $" set @RoundingOff = (select ISNULL(@RoundingOff, 0)) " +
                    //    $" RETURN @RoundingOff " +
                    //    $" END'; " +

                    //    $" EXECUTE sp_executesql @string;  end";

                    //db.Execute(queryCheckAndCreate);



                    db.Close();
                    result.Success = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    return result;

                }
            }


        }

        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<PaymentMethodResultViewModel> resultlist = new List<PaymentMethodResultViewModel>();
            try
            {
                foreach (var item in model.PaymentMethod)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                  
                        PaymentMethodResultViewModel itemRs = new PaymentMethodResultViewModel();
                        itemRs = _mapper.Map<PaymentMethodResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                    resultlist.Add(itemRs);
                   
                }
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

        public async Task<bool> checkExist(string CompanyCode, string PaymentCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("PaymentCode", PaymentCode);
            parameters.Add("Status", "");
            var affectedRows = await _paymentRepository.GetAsync("USP_S_M_Payment", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MPaymentMethod model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("PaymentCode", model.PaymentCode);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ForfeitCode", model.ForfeitCode);
                parameters.Add("PaymentDesc", model.PaymentDesc);
                parameters.Add("ShortName", model.ShortName);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("AccountCode", model.AccountCode);
                parameters.Add("AllowChange", model.AllowChange);
                parameters.Add("IsRequireRefnum", model.IsRequireRefnum); 
                parameters.Add("RejectReturn", model.RejectReturn); 
                parameters.Add("RejectVoid", model.RejectVoid);
                parameters.Add("RejectExchange", model.RejectExchange);
                parameters.Add("PaymentType", model.PaymentType);
                parameters.Add("EODApply", model.EODApply);
                parameters.Add("EODCode", model.EODCode);
                parameters.Add("AllowRefund", model.AllowRefund);
                parameters.Add("RequireTerminal", model.RequireTerminal);
                parameters.Add("VoucherCategory", model.VoucherCategory);
                parameters.Add("Currency", model.Currency);
                if (!string.IsNullOrEmpty(model.CustomF1))
                {
                    parameters.Add("CustomF1", model.CustomF1);
                }
                if (!string.IsNullOrEmpty(model.CustomF2))
                {
                    parameters.Add("CustomF2", model.CustomF2);
                }
                if (!string.IsNullOrEmpty(model.CustomF3))
                {
                    parameters.Add("CustomF3", model.CustomF3);
                }
                if (!string.IsNullOrEmpty(model.CustomF4))
                {
                    parameters.Add("CustomF4", model.CustomF4);
                }
                if (!string.IsNullOrEmpty(model.CustomF5))
                {
                    parameters.Add("CustomF5", model.CustomF5);
                }

                var exist = await checkExist(model.CompanyCode, model.PaymentCode);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.PaymentCode + " existed.";
                    return result;
                }
                var affectedRows = _paymentRepository.Insert("USP_I_M_PaymentMethod", parameters, commandType: CommandType.StoredProcedure);

                if(model.Mappings!=null && model.Mappings.Count() >0)
                {
                    foreach(var mappingLine  in model.Mappings)
                    {
                        var Xparameters = new DynamicParameters();
 
                        Xparameters.Add("CompanyCode", model.CompanyCode);
                        Xparameters.Add("PaymentCode", model.PaymentCode);
                        Xparameters.Add("FatherId", mappingLine.FatherId);
                        Xparameters.Add("CustomF1", mappingLine.CustomF1);
                        Xparameters.Add("CustomF2", mappingLine.CustomF2);
                        Xparameters.Add("CustomF3", mappingLine.CustomF3);
                        Xparameters.Add("CustomF4", mappingLine.CustomF4);
                        Xparameters.Add("CustomF5", mappingLine.CustomF5);
                        Xparameters.Add("CustomF6", mappingLine.CustomF6);
                        Xparameters.Add("Status", mappingLine.Status);

                        _paymentRepository.Insert("USP_I_M_PaymentMethodMapping", Xparameters, commandType: CommandType.StoredProcedure);
                    }    
                   
                }    
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

        public Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                //select* from M_PaymentMethod with(nolock) where CompanyCode = '{CompanyCode}'
                var data = await _paymentRepository.GetAllAsync($"USP_S_M_PaymentMethod '',N'{CompanyCode}',''", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data =  _paymentRepository.Get($"[USP_S_M_PaymentMethod] '{Code}','{CompanyCode}','{StoreId}'", null, commandType: CommandType.Text);
                if (data != null)
                {
                    var dataMapping = _mappingRepository.GetAll($"[USP_S_M_PaymentMethodMapping] '{CompanyCode}','{Code}'", null, commandType: CommandType.Text);
                    data.Mappings = dataMapping;

                }

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
        public async Task<GenericResult> GetPaymentType()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _paymentRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = " select * from M_PaymentType ";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
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
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreId)
        {
            //List<StorePaymentViewModel>
            GenericResult rs = new GenericResult();
            using (IDbConnection db = _paymentRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = $"[USP_S_M_PaymentMethod] '','{CompanyCode}','{StoreId}'";
                    var data = await db.QueryAsync<StorePaymentViewModel>(query, null);
                    rs.Success = true;
                    rs.Data = data;
                    db.Close();
                    //return await PagedList<StorePaymentViewModel>.Create(data.ToList(), userParams.PageNumber, userParams.PageSize);

                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    rs.Success = false;
                    rs.Data = ex.Message;
                }
                 
            }
            return rs;

        }

        public async Task<PagedList<MPaymentMethod>> GetPagedList(UserParams userParams)
        {
            try
            {
                string query = $"select * from M_PaymentMethod with (nolock) where PaymentDesc like N'%{userParams.keyword}%' or PaymentCode like N'%{userParams.keyword}%'";
                if(!string.IsNullOrWhiteSpace(userParams.Store))
                {
                    query += "";
                }

                var data = await _paymentRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.PaymentCode);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.PaymentDesc);
                }
                return await PagedList<MPaymentMethod>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PagedList<StorePaymentViewModel>> GetByStorePagedList(UserParams userParams)
        {
         
            using (IDbConnection db = _paymentRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            string query = $"[USP_S_M_PaymentMethod] '','{userParams.Company}','{userParams.Store}'";


                            //if (!string.IsNullOrWhiteSpace(userParams.Store))
                            //{
                            //    query += $" and t2.StoreId = '{userParams.Store}'";
                            //}

                            var data = await db.QueryAsync<StorePaymentViewModel>(query, null, tran);

                            db.Close();
                            if (userParams.OrderBy == "byName")
                            {
                                data.OrderByDescending(x => x.PaymentCode);
                            }
                            if (userParams.OrderBy == "byId")
                            {
                                data.OrderByDescending(x => x.PaymentDesc);
                            }
                            
                            return await PagedList<StorePaymentViewModel>.Create(data.ToList(), userParams.PageNumber, userParams.PageSize);
                           
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
                    if (db.State == ConnectionState.Open)
                        db.Close();

                    throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                 
            }
          
        }
        public async Task<GenericResult> Update(MPaymentMethod model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("PaymentCode", model.PaymentCode);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ForfeitCode", model.ForfeitCode);
                parameters.Add("PaymentDesc", model.PaymentDesc);
                parameters.Add("ShortName", model.ShortName);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("AccountCode", model.AccountCode);
                parameters.Add("IsRequireRefnum", model.IsRequireRefnum);
                parameters.Add("AllowChange", model.AllowChange);
                parameters.Add("RejectReturn", model.RejectReturn);
                parameters.Add("RejectVoid", model.RejectVoid);
                parameters.Add("RejectExchange", model.RejectExchange);
                parameters.Add("PaymentType", model.PaymentType);
                parameters.Add("EODApply", model.EODApply);
                parameters.Add("EODCode", model.EODCode);
                parameters.Add("AllowRefund", model.AllowRefund);
                parameters.Add("RequireTerminal", model.RequireTerminal);
                parameters.Add("VoucherCategory", model.VoucherCategory);
                parameters.Add("Currency", model.Currency);
                if(!string.IsNullOrEmpty(model.FatherId) )
                {
                    parameters.Add("FatherId", model.FatherId);
                }
                if (!string.IsNullOrEmpty(model.BankPaymentType))
                {
                    parameters.Add("BankPaymentType", model.BankPaymentType);
                }
                if (!string.IsNullOrEmpty(model.CustomF1))
                {
                    parameters.Add("CustomF1", model.CustomF1);
                }
                if (!string.IsNullOrEmpty(model.CustomF2))
                {
                    parameters.Add("CustomF2", model.CustomF2);
                }
                if (!string.IsNullOrEmpty(model.CustomF3))
                {
                    parameters.Add("CustomF3", model.CustomF3);
                }
                if (!string.IsNullOrEmpty(model.CustomF4))
                {
                    parameters.Add("CustomF4", model.CustomF4);
                }
                if (!string.IsNullOrEmpty(model.CustomF5))
                {
                    parameters.Add("CustomF5", model.CustomF5);
                }
                var affectedRows = _paymentRepository.Update("USP_U_M_PaymentMethod", parameters, commandType: CommandType.StoredProcedure);
                if (model.Mappings != null && model.Mappings.Count() > 0)
                {
                    var Dparameters = new DynamicParameters(); 
                    Dparameters.Add("CompanyCode", model.CompanyCode);
                    Dparameters.Add("PaymentCode", model.PaymentCode); 
                    _paymentRepository.Execute("USP_D_M_PaymentMethodMapping", Dparameters, commandType: CommandType.StoredProcedure);

                    foreach (var mappingLine in model.Mappings)
                    {
                        var Xparameters = new DynamicParameters();

                        Xparameters.Add("CompanyCode", model.CompanyCode);
                        Xparameters.Add("PaymentCode", model.PaymentCode);
                        Xparameters.Add("FatherId", mappingLine.FatherId);
                        Xparameters.Add("CustomF1", mappingLine.CustomF1);
                        Xparameters.Add("CustomF2", mappingLine.CustomF2);
                        Xparameters.Add("CustomF3", mappingLine.CustomF3);
                        Xparameters.Add("CustomF4", mappingLine.CustomF4);
                        Xparameters.Add("CustomF5", mappingLine.CustomF5);
                        Xparameters.Add("CustomF6", mappingLine.CustomF6);
                        Xparameters.Add("Status", mappingLine.Status);

                        _paymentRepository.Insert("USP_I_M_PaymentMethodMapping", Xparameters, commandType: CommandType.StoredProcedure);
                    }

                }
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

        public async Task<List<MPaymentMethod>> GetMPayments(string companyCode, string paymentCode, string storeId, string status)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("PaymentCode", paymentCode);
                parameters.Add("StoreId", storeId);
                parameters.Add("Status", status);
                return await _paymentRepository.GetAllAsync("USP_S_M_PaymentStoreMapping", parameters, CommandType.StoredProcedure);
            }
            catch
            {
                return null;
            }
        }
    }
}
