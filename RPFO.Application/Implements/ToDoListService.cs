
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
    public class ToDoListService : IToDoListService
    {
        private readonly IGenericRepository<SToDoList> _todolistRepository; 

        private readonly IMapper _mapper;
        public ToDoListService(IGenericRepository<SToDoList> todolistRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _todolistRepository = todolistRepository;
         
            _mapper = mapper;
           
        }
      
        public async Task<GenericResult> Create(SToDoList model)
        {
            GenericResult result = new GenericResult();
            try
            { 
                var parameters = new DynamicParameters();

                parameters.Add("Id", Guid.NewGuid());
                parameters.Add("Code", model.Code);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                parameters.Add("Content", model.Content);
                parameters.Add("Remark", model.Remark);
                parameters.Add("Status", model.Status);
                parameters.Add("FromDate", model.FromDate);
                parameters.Add("ToDate", model.ToDate);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);

                var affectedRows = _todolistRepository.Insert("USP_I_S_ToDoList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(SToDoList model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //parameters.Add("CompanyCode", model.CompanyCode);
                //parameters.Add("StoreId", model.StoreId);
                //parameters.Add("StoreAreaId", model.StoreAreaId);
                //parameters.Add("TimeFrameId", model.TimeFrameId); 
               

                //var affectedRows = _todolistRepository.Execute("USP_D_M_StoreCapacity", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string Id, string Code, string Name, string Description, string Content, string Remark, string Status,
            DateTime? FromDate, DateTime? ToDate, string CreatedBy, DateTime? CreatedOn)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("Id", Id);
                parameters.Add("Code", Code);
                parameters.Add("Name", Name);
                parameters.Add("Description", Description);
                parameters.Add("Content", Content);
                parameters.Add("Remark", Remark);
                parameters.Add("Status", Status);
                parameters.Add("FromDate", FromDate);
                parameters.Add("ToDate", ToDate);
                parameters.Add("CreatedBy", CreatedBy);
                parameters.Add("CreatedOn", CreatedOn);

                var affectedRows = await _todolistRepository.GetAllAsync("USP_S_S_ToDoList", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = affectedRows;
            }
            catch (Exception ex)
            {
                //throw ex;
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetById(string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("Id", Id);
                parameters.Add("Code", "");
                parameters.Add("Name", "");
                parameters.Add("Description", "");
                parameters.Add("Content", "");
                parameters.Add("Remark", "");
                parameters.Add("Status", "");
                parameters.Add("FromDate", "");
                parameters.Add("ToDate", "");
                parameters.Add("CreatedBy", "");
                parameters.Add("CreatedOn", ""); 
                var affectedRows = await _todolistRepository.GetAsync("USP_S_S_ToDoList", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = affectedRows;
            }
            catch (Exception ex)
            {
                //throw ex;
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> Update(SToDoList model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("Id", model.Id);
                parameters.Add("Code", model.Code);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                parameters.Add("Content", model.Content);
                parameters.Add("Remark", model.Remark);
                parameters.Add("Status", model.Status);
                parameters.Add("FromDate", model.FromDate);
                parameters.Add("ToDate", model.ToDate);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);

                var affectedRows = _todolistRepository.Update("USP_U_S_ToDoList", parameters, commandType: CommandType.StoredProcedure);
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
    }

}
