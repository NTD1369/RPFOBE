
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
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
    public class CapacityService : ICapacityService
    {
        private readonly IGenericRepository<MStoreCapacity> _capacityRepository;

        private readonly IMapper _mapper;
        public CapacityService(IGenericRepository<MStoreCapacity> capacityRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _capacityRepository = capacityRepository;

            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<StoreCapacityResultViewModel> resultlist = new List<StoreCapacityResultViewModel>();
            try
            {
                foreach (var item in model.StoreCapacity)
                {

                    var check = checkExist(model.CompanyCode, model.StoreId, item.StoreAreaId, item.TimeFrameId).Result;
                 
                   
                    item.CompanyCode = model.CompanyCode;
                    if (check)
                    {
                        item.ModifiedBy = model.CreatedBy;
                        var itemResult = await Update(item); 
                        StoreCapacityResultViewModel itemRs = new StoreCapacityResultViewModel();
                        itemRs = _mapper.Map<StoreCapacityResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                        resultlist.Add(itemRs);
                    }    
                    else
                    {
                        item.CreatedBy = model.CreatedBy;
                        var itemResult = await Create(item); 
                        StoreCapacityResultViewModel itemRs = new StoreCapacityResultViewModel();
                        itemRs = _mapper.Map<StoreCapacityResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                        resultlist.Add(itemRs);
                    }    
                   


                }
                result.Success = true;
                result.Data = resultlist;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;

            }
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string StoreId, string StoreAreaId, string TimeFrameId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;

            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("StoreId", StoreId);
            parameters.Add("StoreAreaId", StoreAreaId);
            parameters.Add("TimeFrameId", TimeFrameId);
            parameters.Add("Status", "");
            var affectedRows = await _capacityRepository.GetAsync("USP_S_M_StoreCapacity", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MStoreCapacity model)
        {
            GenericResult result = new GenericResult();
            try
            {

                var exist = await checkExist(model.CompanyCode, model.StoreId, model.StoreAreaId, model.TimeFrameId);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.StoreId + " - " + model.StoreAreaId + " - " + model.TimeFrameId + " existed.";
                    return result;
                }

                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("StoreAreaId", model.StoreAreaId);
                parameters.Add("TimeFrameId", model.TimeFrameId);
                parameters.Add("MaxCapacity", model.MaxCapacity);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);

                var affectedRows = _capacityRepository.Insert("USP_I_M_StoreCapacity", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;

            }
            catch (Exception ex)
            {
                //throw ex;
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> Delete(MStoreCapacity model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("StoreAreaId", model.StoreAreaId);
                parameters.Add("TimeFrameId", model.TimeFrameId);


                var affectedRows = _capacityRepository.Execute("USP_D_M_StoreCapacity", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
            }
            catch (Exception ex)
            {
                //throw ex;
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }
        public async Task<GenericResult> GetCapacityFromTo(string CompanyCode, DateTime FromDate, DateTime ToDate, int? Quantity, string StoreId, string StoreAreaId, string TimeFrameId)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _capacityRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    DateTime startDate = FromDate;
                    DateTime stopDate = ToDate;
                    int interval = 1;
                    List<CapacityViewModel> list = new List<CapacityViewModel>();
                    for (DateTime dateTime = startDate;
                         dateTime <= stopDate;
                         dateTime += TimeSpan.FromDays(interval))
                    {
                        List<CapacityViewModel> listIndate = new List<CapacityViewModel>();
                        var listIndateData = await GetCapacity(CompanyCode, dateTime, Quantity, StoreId, StoreAreaId, TimeFrameId);
                        listIndate = listIndateData.Data as List<CapacityViewModel>;
                        list.AddRange(listIndate);
                    }
                    db.Close();
                    //return list.ToList();
                    result.Success = true;
                    result.Data = list.ToList();
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public async Task<GenericResult> GetCapacity(string CompanyCode, DateTime TransDate, int? Quantity, string StoreId, string StoreAreaId, string TimeFrameId)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _capacityRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string querycheck = $"select * from M_StoreCapacity where StoreId = '{StoreId}' and CompanyCode= '{CompanyCode}'";
                    var dataCheck = await db.QueryAsync<MStoreCapacity>(querycheck, null, commandType: CommandType.Text);
                    if (dataCheck.Count() <= 0)
                    {
                        throw new InvalidOperationException("Store Capacity of Store is null. ");
                    }

                    var parameters = new DynamicParameters();

                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("TransDate", TransDate.ToString("yyyy-MM-dd"));
                    parameters.Add("Quantity", Quantity);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("StoreAreaId", StoreAreaId);
                    parameters.Add("TimeFrameId", TimeFrameId);
                    string query = $"[USP_GetCapacity] '{CompanyCode}', '{TransDate.ToString("yyyy-MM-dd")}', '{Quantity}','{StoreId}','{StoreAreaId}','{TimeFrameId}'";
                    //var dataCheck = await db.QueryAsync<CapacityViewModel>(querycheck, parameters, commandType: CommandType.Text);
                    var data = await db.QueryAsync<CapacityViewModel>(query, null, commandType: CommandType.Text);
                    foreach (var capa in data)
                    {

                        capa.StartDate = TransDate + capa.StartTime;
                        capa.EndDate = TransDate + capa.EndTime;
                    }
                    result.Success = true;
                    result.Data = data.ToList();
                   
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }
        public async Task<GenericResult> GetCapacityByStore(string CompanyCode, DateTime TransDate, int? Quantity, string StoreId)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _capacityRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();

                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("TransDate", TransDate.ToString("yyyy-MM-dd"));
                    parameters.Add("Quantity", Quantity);
                    parameters.Add("StoreId", StoreId);

                    string querycheck = $"[USP_GetCapacityByStore] '{CompanyCode}', '{TransDate.ToString("yyyy-MM-dd")}', '{Quantity}','{StoreId}'";
                    //var dataCheck = await db.QueryAsync<CapacityViewModel>(querycheck, parameters, commandType: CommandType.Text);
                    var data = await db.QueryAsync(querycheck, parameters, commandType: CommandType.Text);
                    result.Success = true;
                    result.Data = data;

                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }
        public async Task<GenericResult> GetCapacityByAreaStore(string CompanyCode, string StoreId, string StoreAreaId)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _capacityRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();

                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("StoreAreaId", StoreAreaId);

                    string querycheck = $"[USP_GetCapacityByAreaStore] '{CompanyCode}','{StoreId}','{StoreAreaId}'";
                    //var dataCheck = await db.QueryAsync<CapacityViewModel>(querycheck, parameters, commandType: CommandType.Text);
                    var data = await db.QueryAsync<MStoreCapacity>(querycheck, parameters, commandType: CommandType.Text);
                    result.Success = true;
                    result.Data = data;

                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }
        public async Task<GenericResult> GetCapacityAreaStore(string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _capacityRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var parameters = new DynamicParameters();

                    parameters.Add("CompanyCode", CompanyCode);

                    parameters.Add("StoreId", StoreId);

                    string querycheck = $"[USP_GetCapacityAreaStore] '{CompanyCode}','{StoreId}'";
                    //var dataCheck = await db.QueryAsync<CapacityViewModel>(querycheck, parameters, commandType: CommandType.Text);
                    var data = await db.QueryAsync(querycheck, parameters, commandType: CommandType.Text);
                    result.Success = true;
                    result.Data = data;

                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }
        public async Task<GenericResult> Update(MStoreCapacity model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("StoreAreaId", model.StoreAreaId);
                parameters.Add("TimeFrameId", model.TimeFrameId);
                parameters.Add("MaxCapacity", model.MaxCapacity);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);

                var affectedRows = _capacityRepository.Update("USP_U_M_StoreCapacity", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
            }
            catch (Exception ex)
            {
                //throw ex;
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        //public async Task<GenericResult> UpdateAllByStore(MStoreCapacity model)
        //{
        //    GenericResult result = new GenericResult();
        //    try
        //    {
        //        var parameters = new DynamicParameters();

        //        parameters.Add("CompanyCode", model.CompanyCode);
        //        parameters.Add("StoreId", model.StoreId);
        //        parameters.Add("StoreAreaId", model.StoreAreaId);
        //        parameters.Add("TimeFrameId", model.TimeFrameId);
        //        parameters.Add("MaxCapacity", model.MaxCapacity);
        //        parameters.Add("ModifiedBy", model.ModifiedBy);
        //        //parameters.Add("Status", model.Status); 
        //        var affectedRows = _capacityRepository.Update("USP_U_StoreCapacityAllByStore", parameters, commandType: CommandType.StoredProcedure);
        //        result.Success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        //throw ex;
        //        result.Success = false;
        //        result.Message = ex.Message;
        //    }
        //    return result;
        //}
    }

}
