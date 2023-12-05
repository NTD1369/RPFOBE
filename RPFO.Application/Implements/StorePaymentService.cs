
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
    public class StorePaymentService : IStorePaymentService
    {
        private readonly IGenericRepository<MStorePayment> _storePaymentRepository;
        private readonly IGenericRepository<MStore> _storeRepository;
        private readonly IMapper _mapper;
        public StorePaymentService(IGenericRepository<MStorePayment> storePaymentRepository, IGenericRepository<MStore> storeRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _storePaymentRepository = storePaymentRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<StorePaymentResultViewModel> resultlist = new List<StorePaymentResultViewModel>();
            try
            {
                foreach (var item in model.StorePayment)
                {
                    
                    var itemResult = await Create(item);
                    //if (itemResult.Success == false)
                    //{
                        StorePaymentResultViewModel itemRs = new StorePaymentResultViewModel();
                        itemRs = _mapper.Map<StorePaymentResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                        resultlist.Add(itemRs);
                    //}
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

        public async Task<bool> checkStoreExist(string CompanyCode, string StoreId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("StoreId", StoreId);
            parameters.Add("Status", "");
            var affectedRows = await _storeRepository.GetAsync("USP_S_M_Store", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> checkExist(string StoreId, string PaymentCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("StoreId", StoreId);
            parameters.Add("PaymentCode", PaymentCode);
            parameters.Add("Status", "");
            var affectedRows = await _storePaymentRepository.GetAsync("USP_S_M_StorePayment", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MStorePayment model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var storeExist = await checkStoreExist("", model.StoreId);
                if (storeExist == false)
                {
                    result.Success = false;
                    result.Message = "Store: " + model.StoreId + " not existed.";
                    return result;
                }
                var exist = await checkExist(model.StoreId, model.PaymentCode);
                if (exist == true)
                {
                    string DelQuery = $"Update M_StorePayment set Status = 'A' where StoreId = N'{model.StoreId}' and PaymentCode= N'{model.PaymentCode}'";
                    _storeRepository.GetScalar(DelQuery, null, commandType: CommandType.Text);
                    result.Success = true;
                    result.Message = "";// model.PaymentCode + " existed.";
                    return result;
                }

              

                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("StoreId", model.StoreId, DbType.String);
                parameters.Add("PaymentCode", model.PaymentCode);
                parameters.Add("isShow", model.IsShow);
                parameters.Add("AllowMix", model.AllowMix);
                parameters.Add("OrderNum", model.OrderNum);
                parameters.Add("Status", model.Status);
                

                var affectedRows = _storePaymentRepository.Insert("USP_I_M_StorePayment", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(string StoreId, string PaymentCode)
        {
            GenericResult result = new GenericResult();
            try
            { 

                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("StoreId", StoreId, DbType.String);
                parameters.Add("PaymentCode", PaymentCode); 
                var affectedRows = _storePaymentRepository.Execute("USP_D_M_StorePayment", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreId, string CounterId, bool? IsSetup)
        {
            //List<StorePaymentViewModel>
            GenericResult rs = new GenericResult();
            using (IDbConnection db = _storePaymentRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = $"[USP_S_M_StorePaymentByStore] '','{CompanyCode}','{StoreId}' ,'{CounterId}'";
                    if (IsSetup.HasValue && IsSetup.Value)
                    {
                        query = $"[USP_S_M_StorePaymentByStore] '','{CompanyCode}','{StoreId}' ,'{CounterId}', '{IsSetup}'";
                    }   
                    
                   
                    var data = await db.QueryAsync<StorePaymentViewModel>(query, null);
                    rs.Success = true;
                    rs.Data = data;
 
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

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _storePaymentRepository.GetAllAsync($"select * from M_StorePayment with (nolock) ", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _storePaymentRepository.GetAsync($"select * from M_StorePayment with (nolock)  where StoreGroupId ='{Code}'", null, commandType: CommandType.Text);
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

        public async Task<PagedList<MStorePayment>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _storePaymentRepository.GetAllAsync($"select * from M_StorePayment with (nolock) where StoreGroupId like N'%{userParams.keyword}%' or CompanyCode like N'%{userParams.keyword}%'  or StoreGroupName like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.PaymentCode);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.PaymentCode);
                }
                return await PagedList<MStorePayment>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(MStorePayment model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("StoreId", model.StoreId, DbType.String);
                parameters.Add("PaymentCode", model.PaymentCode);
                parameters.Add("isShow", model.IsShow);
                parameters.Add("AllowMix", model.AllowMix);
                parameters.Add("OrderNum", model.OrderNum);
                parameters.Add("Status", model.Status);
                var affectedRows = _storePaymentRepository.Insert("USP_U_M_StorePayment", parameters, commandType: CommandType.StoredProcedure);
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
