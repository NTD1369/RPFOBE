
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
    public class VoidOrderSettingService : IVoidOrderSettingService
    {
        private readonly IGenericRepository<SVoidOrderSetting> _voidOrderSettingRepository;

        private readonly IMapper _mapper;
        public VoidOrderSettingService(IGenericRepository<SVoidOrderSetting> voidOrderSettingRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _voidOrderSettingRepository = voidOrderSettingRepository;
            _mapper = mapper; 

        }
       
        //public async Task<bool> checkExist(string CompanyCode, string UomCode)
        //{
        //    var parameters = new DynamicParameters();
 
        //    parameters.Add("CompanyCode", CompanyCode);
        //    parameters.Add("UomCode", UomCode);
        //    parameters.Add("Status", "");
        //    var affectedRows = await _uomRepository.GetAsync("USP_S_M_UOM", parameters, commandType: CommandType.StoredProcedure);
        //    if (affectedRows != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public async Task<GenericResult> Create(SVoidOrderSetting model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                 
                parameters.Add("Id", model.Id);
                parameters.Add("Type", model.Type);
                parameters.Add("Code", model.Code);
                parameters.Add("Description", model.Description);
                parameters.Add("Value", model.Value);
                parameters.Add("Status", model.Status);
                parameters.Add("Custom1", model.Custom1);
                parameters.Add("Custom2", model.Custom2);
                parameters.Add("Custom3", model.Custom3);
                var affectedRows = _voidOrderSettingRepository.Update("USP_I_S_VoidOrderSetting", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll()
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _voidOrderSettingRepository.GetAllAsync($"select * from S_VoidOrderSetting with (nolock)", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByCode(string Type, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _voidOrderSettingRepository.GetAsync($"select * from S_VoidOrderSetting with (nolock) where  Type= '{Type}' and Code='{Code}'", null, commandType: CommandType.Text);
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

        public Task<List<MUom>> GetByItem(string Item)
        {
            throw new NotImplementedException();
        }

       
        
        public async Task<GenericResult> Update(SVoidOrderSetting model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
               
                parameters.Add("Id", model.Id); 
                parameters.Add("Type", model.Type); 
                parameters.Add("Code", model.Code);  
                parameters.Add("Description", model.Description);
                parameters.Add("Value", model.Value);
                parameters.Add("Status", model.Status);
                parameters.Add("Custom1", model.Custom1);
                parameters.Add("Custom2", model.Custom2);
                parameters.Add("Custom3", model.Custom3);
                var affectedRows = _voidOrderSettingRepository.Update("USP_U_S_VoidOrderSetting", parameters, commandType: CommandType.StoredProcedure);
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
