
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
    public class ThirdPartyLogService : IThirdPartyLogService
    {
        private readonly IGenericRepository<SThirdPartyLog> _logRepository;

        private readonly IMapper _mapper;
        public ThirdPartyLogService(IGenericRepository<SThirdPartyLog> logRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _logRepository = logRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(SThirdPartyLog model)
        {
            GenericResult result = new GenericResult();

            try
            {
                using (IDbConnection db = _logRepository.GetConnection())
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
                                string id = Guid.NewGuid().ToString();
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("Id", id);
                                parameters.Add("TransId", model.TransId);
                                parameters.Add("StoreId", model.StoreId);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                parameters.Add("Status", model.Status);
                                parameters.Add("Type", model.Type);
                                parameters.Add("Remark", model.Remark);
                                var affectedRows = db.Execute("USP_I_S_ThirdPartyLog", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                int numOfline = 1;
                                foreach (var line in model.Lines)
                                {
                                    parameters = new DynamicParameters();
                                    
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("HeaderId", id);
                                    parameters.Add("TransId", model.TransId);
                                    parameters.Add("StoreId", model.StoreId);
                                    parameters.Add("LineId", numOfline);
                                    parameters.Add("JsonBody", line.JsonBody);
                                    parameters.Add("StartTime", line.StartTime);
                                    parameters.Add("EndTime", line.EndTime);
                                    parameters.Add("Key1", line.Key1);
                                    parameters.Add("Key2", line.Key2);
                                    parameters.Add("Key3", line.Key3);
                                    parameters.Add("Key4", line.Key4);
                                    parameters.Add("Key5", line.Key5);
                                    parameters.Add("CustomF1", line.CustomF1);
                                    parameters.Add("CustomF2", line.CustomF2);
                                    parameters.Add("CustomF3", line.CustomF3);
                                    parameters.Add("CustomF4", line.CustomF4);
                                    parameters.Add("CustomF5", line.CustomF5);
                                    parameters.Add("Remark", line.Remark);
                                    parameters.Add("Status", line.Status);
                                    db.Execute("USP_I_S_ThirdPartyLogLine", parameters, commandType: CommandType.StoredProcedure, transaction: tran);
                                    numOfline++;
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

        //public async Task<GenericResult> Delete(string CompanyCode, string Code)
        //{
        //    GenericResult result = new GenericResult();
        //    try
        //    {
        //        var parameters = new DynamicParameters();
                 
        //        parameters.Add("CompanyCode",  CompanyCode);
        //        parameters.Add("Id", Code);
             
        //        var affectedRows = _reasonRepository.Insert("USP_D_M_Reason", parameters, commandType: CommandType.StoredProcedure);
        //        result.Success = true;
        //        //result.Message = key;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //    }
        //    return result;
        //}

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Id", "");
                var data = await _logRepository.GetAllAsync($"USP_S_M_Reason", parameters, commandType: CommandType.StoredProcedure);
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
       
        public async Task<GenericResult> GetById(string CompanyCode, string StoreId, string Id )
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("Id", StoreId);
                var data = await _logRepository.GetAsync($"USP_S_M_Reason", parameters, commandType: CommandType.StoredProcedure);
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

      
        public async Task<GenericResult> Update(SThirdPartyLog model)
        {
            GenericResult result = new GenericResult();
            try
            {
                //var parameters = new DynamicParameters();


                //parameters.Add("CompanyCode", model.CompanyCode);
                //parameters.Add("Id", model.Id);
                //parameters.Add("Value", model.Value);
                //parameters.Add("Remark", model.Remark);
                //parameters.Add("Type", model.Type);
                //parameters.Add("Language", model.Language);
                //parameters.Add("ModifiedBy", model.ModifiedBy);
                //parameters.Add("Status", model.Status);

                //var affectedRows = _reasonRepository.Update("USP_U_M_Reason", parameters, commandType: CommandType.StoredProcedure);
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
