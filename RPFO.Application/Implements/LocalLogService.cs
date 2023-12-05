
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
    public class LocalLogService : ILocalLogService
    {
        private readonly IGenericRepository<SLog> _logRepository;

        private readonly IMapper _mapper;
        public LocalLogService(IGenericRepository<SLog> logRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _logRepository = logRepository;
            _mapper = mapper; 

        }
       
        public async Task<GenericResult> Create(SLog model, string DbType)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("Type", model.Type);
                parameters.Add("TransId", model.TransId );
                parameters.Add("LineNum", model.LineNum.ToString() );
                parameters.Add("Action", model.Action );
                parameters.Add("Time", model.Time);
                parameters.Add("Value", model.Value);
                parameters.Add("Result", model.Result);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("CreatedBy", model.CreatedBy);

                //var exist = await checkExist(model.CompanyCode, model.TaxCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.TaxCode + " existed.";
                //    return result;
                //}
                if (DbType == "LocalDB")
                {
                    _logRepository.Insert("USP_I_S_LocalLog", parameters, commandType: CommandType.StoredProcedure);
                    
                }
                else
                {
                    _logRepository.Insert("USP_I_S_Log", parameters, commandType: CommandType.StoredProcedure);
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

        //public async Task<GenericResult> Delete(string Code)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<GenericResult> GetAll(string DbType, string User, string CompanyCode, string StoreId, string Type, DateTime? FromDate, DateTime? ToDate)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Type", Type);
                parameters.Add("User", User);
                parameters.Add("StoreId", StoreId);
                parameters.Add("FromDate", FromDate);
                parameters.Add("ToDate", ToDate);
 
                 
                if (DbType == "LocalDB")
                {
                    var data = _logRepository.GetAllAsync("USP_S_S_LocalLog", parameters, commandType: CommandType.StoredProcedure);

                }
                else
                {
                    var data = _logRepository.GetAllAsync("USP_S_S_Log", parameters, commandType: CommandType.StoredProcedure);
                    result.Success = true;
                    result.Data = data;
                }
                //var data = await _logRepository.GetAllAsync($"select * from M_Tax with (nolock) where  CompanyCode = '{CompanyCode}'", null, commandType: CommandType.Text);
                //result.Success = true;
                //result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

      
        public async Task<GenericResult> GetPagedList(string DbType, UserParams userParams)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _logRepository.GetAllAsync($"select * from M_Tax with (nolock)", null, commandType: CommandType.Text);
             
                var dataPage = await PagedList<SLog>.Create(data, userParams.PageNumber, userParams.PageSize);
                result.Success = true;
                result.Data = dataPage;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        
        public async Task<GenericResult> Update(SLog model, string DbType)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("Type", model.Type);
                parameters.Add("TransId", model.TransId);
                parameters.Add("LineNum", model.LineNum.ToString());
                parameters.Add("Action", model.Action);
                parameters.Add("Time", model.Time);
                parameters.Add("Value", model.Value);
                parameters.Add("Result", model.Result);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5); 

                //var exist = await checkExist(model.CompanyCode, model.TaxCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.TaxCode + " existed.";
                //    return result;
                //}
                if (DbType == "LocalDB")
                {
                    _logRepository.Update("USP_U_S_LocalLog", parameters, commandType: CommandType.StoredProcedure);

                }
                else
                {
                    _logRepository.Update("USP_U_S_Log", parameters, commandType: CommandType.StoredProcedure);
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
    }

}
