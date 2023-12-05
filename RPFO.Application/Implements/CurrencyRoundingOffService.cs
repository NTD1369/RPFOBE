
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
    public class CurrencyRoundingOffService : ICurrencyRoundingOffService
    {
        private readonly IGenericRepository<SCurrencyRoundingOff> _roundingRepository;

        private readonly IMapper _mapper;
        public CurrencyRoundingOffService(IGenericRepository<SCurrencyRoundingOff> roundingRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _roundingRepository = roundingRepository;
            _mapper = mapper; 

        }
        
        public async Task<GenericResult> Create(SCurrencyRoundingOff model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("CurrencyCode", model.CurrencyCode);
                parameters.Add("RoundingOff", model.RoundingOff);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("Status", model.Status);
                //var exist = await GetById(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _roundingRepository.Insert("USP_I_S_CurrencyRoundingOff", parameters, commandType: CommandType.StoredProcedure);
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
         
        public async Task<GenericResult> Delete(SCurrencyRoundingOff model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Currency", model.CurrencyCode);
              
                var affectedRows = _roundingRepository.Execute("USP_D_S_CurrencyRoundingOff", parameters, commandType: CommandType.StoredProcedure);
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
       
        public async Task<GenericResult> GetAll(string CompanyCode,string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", "");
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                var data = await _roundingRepository.GetAllAsync($"USP_S_S_CurrencyRoundingOff", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetById(string CompanyCode, string StoreId, string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                parameters.Add("Id", Id);
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                var data = await _roundingRepository.GetAsync($"USP_S_S_CurrencyRoundingOff", parameters, commandType: CommandType.StoredProcedure);
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

     

        public async Task<GenericResult> Update(SCurrencyRoundingOff model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("CurrencyCode", model.CurrencyCode);
                parameters.Add("RoundingOff", model.RoundingOff);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("Status", model.Status);
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _roundingRepository.Update("USP_U_S_CurrencyRoundingOff", parameters, commandType: CommandType.StoredProcedure);
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
