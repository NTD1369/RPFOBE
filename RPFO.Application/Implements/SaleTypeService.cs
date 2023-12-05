
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
    public class SaleTypeService : ISaleTypeService
    {
        private readonly IGenericRepository<SSalesType> _salesTypeRepository;

        private readonly IMapper _mapper;
        public SaleTypeService(IGenericRepository<SSalesType> salesTypeRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _salesTypeRepository = salesTypeRepository;
            _mapper = mapper; 

        }
       
        public async Task<bool> checkExist(string CompanyCode, string UomCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("UomCode", UomCode);
            parameters.Add("Status", "");
            var affectedRows = await _salesTypeRepository.GetAsync("USP_S_M_UOM", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(SSalesType model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                

                parameters.Add("Code", model.Code);
                parameters.Add("Descriptipon", model.Descriptipon);  
                parameters.Add("Status", model.Status);
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _salesTypeRepository.Insert("USP_I_S_SalesType", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<SSalesType>> GetAll()
        {
            try
            {
                var data = await _salesTypeRepository.GetAllAsync($"select * from S_SalesType with (nolock)", null, commandType: CommandType.Text);
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<SSalesType> GetByCode(string Code)
        {
            try
            {
                var data = await _salesTypeRepository.GetAsync($"select * from S_SalesType with (nolock) where UOMCode='{Code}'", null, commandType: CommandType.Text);
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

       
        
        public async Task<GenericResult> Update(SSalesType model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                parameters.Add("Code", model.Code); 
                parameters.Add("Descriptipon", model.Descriptipon);  
                parameters.Add("Status", model.Status);
                var affectedRows = _salesTypeRepository.Update("USP_U_S_SalesType", parameters, commandType: CommandType.StoredProcedure);
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
