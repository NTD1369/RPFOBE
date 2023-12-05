
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class PickupAmountService : IPickupAmountService
    {
        private readonly IGenericRepository<TPickupAmount> _pickupRepository;
        private ICommonService _commonService;
        private readonly IMapper _mapper;
        public PickupAmountService(IGenericRepository<TPickupAmount> pickupRepository, ICommonService commonService, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _pickupRepository = pickupRepository;
            _mapper = mapper;
            _commonService = commonService;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(TPickupAmount model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                
                if (!string.IsNullOrEmpty(model.ShiftId))
                {
                    string query = $" select DailyId from T_ShiftHeader  with (nolock) where CompanyCode = N'{model.CompanyCode}' and StoreId = N'{model.StoreId}' and ShiftId = N'{model.ShiftId}'";
                    string DailyIdData = _pickupRepository.GetScalar(query, null, commandType: CommandType.Text);
                    if (string.IsNullOrEmpty(DailyIdData))
                    {
                        result.Success = false;
                        result.Message = "Daily Id is null. Please contact IT ";
                        return result;
                    }
                    else
                    {
                        model.DailyId = DailyIdData;
                    }    
                }
                if(string.IsNullOrEmpty(model.DailyId))
                {
                    var DailyData = await _commonService.GetDailyId(model.CompanyCode, model.StoreId, DateTime.Now);
                    if(DailyData.Success)
                    {
                        model.DailyId = DailyData.Data.ToString();
                    }   
                    
                }
                if (string.IsNullOrEmpty(model.DailyId))
                {
                    result.Success = false;
                    result.Message = "Daily Id is null. Please contact IT ";
                    return result;
                }
                string StatusDailyId = _pickupRepository.GetScalar($"select isnull(Status,'O') from T_EndDate where CompanyCode = N'{model.CompanyCode}' and StoreId = N' {model.StoreId}' and  Description = N' { model.DailyId }' ", null, commandType: CommandType.Text);
                if (string.IsNullOrEmpty(StatusDailyId))
                {
                    StatusDailyId = "O";
                }
                if (StatusDailyId == "C")
                {
                    result.Success = false;
                    result.Message = "Can't add new pickup. Daily Id is Closed";
                    return result;
                }
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("CounterId", model.CounterId);
                parameters.Add("ShiftId", model.ShiftId);
                parameters.Add("DailyId", model.DailyId);
                parameters.Add("PickupBy", model.PickupBy);
                parameters.Add("Amount", model.Amount);
                parameters.Add("Remarks", model.Remarks);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                model.CreatedOn = DateTime.Now;
                var affectedRows = _pickupRepository.Insert("USP_I_T_PickupAmount", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
               
                result.Data = model;
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> Delete(TPickupAmount model)
        {
            GenericResult result = new GenericResult();
            try
            {


                var parameters = new DynamicParameters();

                
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", "");
                parameters.Add("CounterId", "");
                parameters.Add("ShiftId", "");
                parameters.Add("PickupBy", "");
                parameters.Add("CreatedBy", "");
                parameters.Add("FDate", "");
                parameters.Add("TDate", "");
                var data = await _pickupRepository.GetAsync("USP_S_T_PickupAmount", parameters, commandType: CommandType.StoredProcedure);
                if(data!= null)
                {
                    if(data.CreatedBy == model.CreatedBy)
                    {
                        //var item =  GetItems()
                        parameters = new DynamicParameters();

                        //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                        //model.ShiftId = key; 
                        parameters.Add("CompanyCode", model.CompanyCode);
                        parameters.Add("Id", model.Id);
                        var affectedRows = _pickupRepository.Execute("USP_D_T_PickupAmount", parameters, commandType: CommandType.StoredProcedure);
                        result.Success = true;
                    }    
                    else
                    {
                        result.Success = false;
                        result.Message = "Can't Delete. " +model.CreatedBy+ " doesn't owner";
                    }    
                   
                }    
                else
                {
                    result.Success = false;
                    result.Message = "Not existed";

                }    


                //result.Message = key;
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
                var parameters = new DynamicParameters();

                
                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key; 
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", "");
                parameters.Add("CounterId", "");
                parameters.Add("ShiftId", "");
                parameters.Add("PickupBy", "");
                parameters.Add("CreatedBy", "");
                var data = await _pickupRepository.GetAsync("USP_S_T_PickupAmount", parameters, commandType: CommandType.Text);  
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
      
        public async Task<GenericResult> GetItems(string CompanyCode, string StoreId,string DailyId, string CounterId, string ShiftId, string PickupBy, string CreatedBy
            ,DateTime? FDate, DateTime? TDate, string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key; 
                if(string.IsNullOrEmpty(Id))
                {
                    Id = "";
                }    
                parameters.Add("Id", Id);
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("DailyId", DailyId);
                parameters.Add("CounterId", CounterId);
                parameters.Add("ShiftId", string.IsNullOrEmpty(ShiftId) ? "" : ShiftId );
                parameters.Add("PickupBy", string.IsNullOrEmpty(PickupBy) ? "" : PickupBy); 
                parameters.Add("CreatedBy", string.IsNullOrEmpty(CreatedBy) ? "" : CreatedBy); 
                parameters.Add("FDate", !FDate.HasValue ? null : FDate); 
                parameters.Add("TDate", !TDate.HasValue ? null : TDate); 
                var data = await _pickupRepository.GetAllAsync("USP_S_T_PickupAmount", parameters, commandType: CommandType.StoredProcedure);

                result.Success = true;
                result.Data =data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetItem(string CompanyCode, string StoreId, string DailyId, string CounterId, string ShiftId, string Id, string NumOfList)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key; 
                if (string.IsNullOrEmpty(Id))
                {
                    if (!string.IsNullOrEmpty(NumOfList))
                    {
                         
                        parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                        parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                        parameters.Add("DailyId", string.IsNullOrEmpty(DailyId) ? "" : DailyId);
                        parameters.Add("CounterId", string.IsNullOrEmpty(CounterId) ? "" : CounterId);
                        parameters.Add("ShiftId", string.IsNullOrEmpty(ShiftId) ? "" : ShiftId); 
                        parameters.Add("NumOfList", string.IsNullOrEmpty(NumOfList) ? null : NumOfList); 
                        var dataList = await _pickupRepository.GetAsync("USP_GetPickupAmountByNumOfList", parameters, commandType: CommandType.StoredProcedure);

                        result.Success = true;
                        result.Data = dataList;
                        
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Please input num of picked amount list to view";
                    }
                    return result;
                }

                parameters.Add("Id", Id);
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("DailyId", DailyId);
                parameters.Add("CounterId", CounterId);
                parameters.Add("ShiftId", string.IsNullOrEmpty(ShiftId) ? "" : ShiftId);
                parameters.Add("PickupBy", "");
                parameters.Add("CreatedBy", "");
                parameters.Add("FDate", null);
                parameters.Add("TDate", null);
                var data = await _pickupRepository.GetAllAsync("USP_S_T_PickupAmount", parameters, commandType: CommandType.StoredProcedure);

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
        public async Task<GenericResult> GetPickupAmountLst(string CompanyCode, string StoreId, string DailyId, string ShiftId, string IsSales, DateTime? FDate, DateTime? TDate)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
 
                parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                parameters.Add("DailyId", string.IsNullOrEmpty(DailyId) ? "" : DailyId);
                parameters.Add("ShiftId", string.IsNullOrEmpty(ShiftId) ? "" : ShiftId);
                parameters.Add("ColumnName", "CustomF");
                parameters.Add("ColumnToPivot", "rn");
                parameters.Add("ListToPivot", "CustomF1,CustomF2,CustomF3,CustomF4,CustomF5");
                parameters.Add("FDate", !FDate.HasValue ? "" : FDate.Value.ToString("yyyy-MM-dd"));
                parameters.Add("TDate", !TDate.HasValue ? "" : TDate.Value.ToString("yyyy-MM-dd"));
                if (!string.IsNullOrEmpty(IsSales)) parameters.Add("IsSales", string.IsNullOrEmpty(IsSales) ? "" : IsSales);
                    
               
                var data = await _pickupRepository.GetAllAsync("USP_GetPickupAmountLst", parameters, commandType: CommandType.StoredProcedure);

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
        public async Task<GenericResult> Update(TPickupAmount model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key; 
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("DailyId", model.DailyId);
                parameters.Add("CounterId", model.CounterId);
                parameters.Add("ShiftId", model.ShiftId);
                parameters.Add("PickupBy", model.PickupBy);
                parameters.Add("Amount", model.Amount);
                parameters.Add("Remarks", model.Remarks);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _pickupRepository.Update("USP_U_T_PickupAmount", parameters, commandType: CommandType.StoredProcedure);
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
