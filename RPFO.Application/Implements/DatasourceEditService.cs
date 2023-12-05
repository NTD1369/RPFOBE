
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
    public class DatasourceEditService : IDatasourceEditService
    {
        private readonly IGenericRepository<SDatasourceEdit> _repository;

        private readonly IMapper _mapper;
        public DatasourceEditService(IGenericRepository<SDatasourceEdit> repository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _repository = repository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(SDatasourceEdit model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                //parameters.Add("ControlId", model.Id, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("DataSource", model.DataSource);
                parameters.Add("Field", model.Field);
                parameters.Add("CanEdit", model.CanEdit); 
                parameters.Add("CreatedBy", model.CreatedBy); 
                var affectedRows = _repository.Insert("USP_I_S_DatasourceEdit", parameters, commandType: CommandType.StoredProcedure);
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

        public async  Task<GenericResult> Delete(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("Id", Code);
                parameters.Add("CompanyCode", CompanyCode);
             
                var data = await _repository.GetAllAsync($"USP_D_S_DatasourceEdit", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string DataSource)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                 
                parameters.Add("Id", "");
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("DataSource", DataSource);
                var data = await _repository.GetAllAsync($"USP_S_S_DatasourceEdit", parameters, commandType: CommandType.StoredProcedure);
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
        
        public async Task<GenericResult> GetByCode(string CompanyCode, string DataSource, string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("Id", Id);
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("DataSource", DataSource);
                var data = await _repository.GetAsync($"USP_S_S_DatasourceEdit", parameters, commandType: CommandType.StoredProcedure);

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

     

        public async Task<GenericResult> Update(SDatasourceEdit model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("DataSource", model.DataSource);
                parameters.Add("Field", model.Field);
                parameters.Add("CanEdit", model.CanEdit);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                var affectedRows = _repository.Insert("USP_U_S_DatasourceEdit", parameters, commandType: CommandType.StoredProcedure);
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


        //public async Task<GenericResult> UpdateOrderNum(List<MControl> list)
        //{
        //    GenericResult result = new GenericResult();
        //    try
        //    {
        //        int num = 1;
        //        foreach(MControl control in list)
        //        {
        //            var parameters = new DynamicParameters();

        //            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //            //model.ShiftId = key;
        //            parameters.Add("CompanyCode", control.CompanyCode); 
        //            parameters.Add("FunctionId", control.FunctionId); 
        //            parameters.Add("ControlId", control.ControlId );
        //            parameters.Add("OrderNum", num);
        //            string query = $"USP_U_M_ControlOrder '{control.CompanyCode}','{control.FunctionId}','{control.ControlId}','{num}'";
        //            var affectedRows = _controlRepository.Update("USP_U_M_ControlOrder", parameters, commandType: CommandType.StoredProcedure);
        //            num++;
        //        }     
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
    }

}
