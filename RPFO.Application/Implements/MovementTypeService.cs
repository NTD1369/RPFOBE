
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
    public class MovementTypeService : IMovementTypeService
    {
        private readonly IGenericRepository<MMovementType> _movementTypeRepository;
        private readonly IMapper _mapper;
        public MovementTypeService(IGenericRepository<MMovementType> movementTypeRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _movementTypeRepository = movementTypeRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(MMovementType model)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Code", model.Code, DbType.String);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                parameters.Add("Status", model.Status);
                var data = _movementTypeRepository.Execute("USP_I_M_Function", parameters, commandType: CommandType.StoredProcedure); 
                rs.Success = true;
               
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
            throw new NotImplementedException();
        }

        public async Task<GenericResult> Delete(string Code)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Code", Code, DbType.String); 
                var data = _movementTypeRepository.Execute("USP_D_M_Function", parameters, commandType: CommandType.StoredProcedure);
                rs.Success = true;

            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
       

        public async Task<GenericResult> GetAll()
        {
            GenericResult rs = new GenericResult();
            try
            { 
                var data = await _movementTypeRepository.GetAllAsync("select * from M_MovementType with (nolock) where status='A'", null, commandType: CommandType.Text);
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;

        }
       
        public async Task<GenericResult> GetByCode(string Code)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _movementTypeRepository.GetAsync($"select * from M_MovementType with (nolock) where Code = '{Code}'", null, commandType: CommandType.Text);
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
          
        public async Task<GenericResult> Update(MMovementType model)
        {
            GenericResult rs = new GenericResult();
            try
            { 
                string query = "";
                var parameters = new DynamicParameters();
                parameters.Add("Code", model.Code, DbType.String);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description); 
                parameters.Add("Status", model.Status);
              
                var data = _movementTypeRepository.Execute("USP_U_M_MovementType", parameters, commandType: CommandType.StoredProcedure);
                //model.UserId = Id;
                rs.Success = true;
                rs.Data = model;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
            throw new NotImplementedException();
        }
    } 

}
