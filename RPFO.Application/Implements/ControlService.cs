
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
    public class ControlService : IControlService
    {
        private readonly IGenericRepository<MControl> _controlRepository;

        private readonly IMapper _mapper;
        public ControlService(IGenericRepository<MControl> controlRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _controlRepository = controlRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;
            InitService();
        }
        public GenericResult InitService()
        {
            GenericResult result = new GenericResult();

            using (IDbConnection db = _controlRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();

                    string queryCheckAndCreate = "IF (OBJECT_ID('USP_S_GetControlByFunction') IS NULL)  begin declare @string nvarchar(MAX) = '';" +
                       " set @string = 'create PROCEDURE [dbo].[USP_S_GetControlByFunction] @CompanyCode nvarchar(50), @Function	nvarchar(150) AS " +
                       "begin  select * from M_Control with(nolock) where CompanyCode = @CompanyCode  and Status = ''A''  and FunctionId = @Function order By OrderNum end '; " +
                       "EXECUTE sp_executesql @string;  end";
                    db.Execute(queryCheckAndCreate);



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
        public async Task<GenericResult> Create(MControl model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("ControlId", model.ControlId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ControlName", model.ControlName);
                parameters.Add("ControlType", model.ControlType);
                parameters.Add("Action", model.Action);
                parameters.Add("FunctionId", model.FunctionId);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("OrderNum", model.OrderNum);
                parameters.Add("Require", model.Require);
                parameters.Add("OptionName", model.OptionName);
                parameters.Add("OptionKey", model.OptionKey);
                parameters.Add("OptionValue", model.OptionValue);
                parameters.Add("Custom1", model.Custom1);
                parameters.Add("Custom2", model.Custom2);
                var affectedRows = _controlRepository.Insert("USP_I_M_Control", parameters, commandType: CommandType.StoredProcedure);
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
                var data = await _controlRepository.GetAllAsync($"select * from M_Control with (nolock) where CompanyCode = '{CompanyCode}' order By OrderNum", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetControlByFunction(string CompanyCode, string Function)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _controlRepository.GetAllAsync($"USP_S_GetControlByFunction N'{CompanyCode}', N'{Function}'", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetByCode(string CompanyCode, string ControlId, string Function)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _controlRepository.GetAsync($"select * from M_Control with (nolock)  where CompanyCode = '{CompanyCode}' and ControlId ='{ControlId}' and FunctionId='{Function}'  order By OrderNum", null, commandType: CommandType.Text);

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

        public async Task<PagedList<MControl>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _controlRepository.GetAllAsync($"select * from M_Control with (nolock) where ControlId like N'%{userParams.keyword}%' or ControlName like N'%{userParams.keyword}%'  ", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.ControlId);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.ControlName);
                }
                return await PagedList<MControl>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(MControl model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("ControlId", model.ControlId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("ControlName", model.ControlName);
                parameters.Add("Action", model.Action);
                parameters.Add("ControlType", model.ControlType);
                parameters.Add("FunctionId", model.FunctionId);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);

                parameters.Add("OrderNum", model.OrderNum);
                parameters.Add("Require", model.Require);
                parameters.Add("OptionName", model.OptionName);
                parameters.Add("OptionKey", model.OptionKey);
                parameters.Add("OptionValue", model.OptionValue);
                parameters.Add("Custom1", model.Custom1);
                parameters.Add("Custom2", model.Custom2);
                var affectedRows = _controlRepository.Update("USP_U_M_Control", parameters, commandType: CommandType.StoredProcedure);
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


        public async Task<GenericResult> UpdateOrderNum(List<MControl> list)
        {
            GenericResult result = new GenericResult();
            try
            {
                int num = 1;
                foreach(MControl control in list)
                {
                    var parameters = new DynamicParameters();

                    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                    //model.ShiftId = key;
                    parameters.Add("CompanyCode", control.CompanyCode); 
                    parameters.Add("FunctionId", control.FunctionId); 
                    parameters.Add("ControlId", control.ControlId );
                    parameters.Add("OrderNum", num);
                    string query = $"USP_U_M_ControlOrder '{control.CompanyCode}','{control.FunctionId}','{control.ControlId}','{num}'";
                    var affectedRows = _controlRepository.Update("USP_U_M_ControlOrder", parameters, commandType: CommandType.StoredProcedure);
                    num++;
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
    }

}
