
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
    public class ReasonService : IReasonService
    {
        private readonly IGenericRepository<MReason> _reasonRepository;

        private readonly IMapper _mapper;
        public ReasonService(IGenericRepository<MReason> reasonRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _reasonRepository = reasonRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(MReason model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Value", model.Value);
                parameters.Add("Remark", model.Remark);
                parameters.Add("Type", model.Type);
                parameters.Add("Language", model.Language);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
              
                var affectedRows = _reasonRepository.Insert("USP_I_M_Reason", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                 
                parameters.Add("CompanyCode",  CompanyCode);
                parameters.Add("Id", Code);
             
                var affectedRows = _reasonRepository.Insert("USP_D_M_Reason", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Id", "");
                var data = await _reasonRepository.GetAllAsync($"USP_S_M_Reason", parameters, commandType: CommandType.StoredProcedure);
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
       

        public async Task<GenericResult> GetByCode(string CompanyCode, string ControlId )
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Id", ControlId);
                var data = await _reasonRepository.GetAsync($"USP_S_M_Reason", parameters, commandType: CommandType.StoredProcedure);
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

      
        public async Task<GenericResult> Update(MReason model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", model.Id);
                parameters.Add("Value", model.Value);
                parameters.Add("Remark", model.Remark);
                parameters.Add("Type", model.Type);
                parameters.Add("Language", model.Language);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);

                var affectedRows = _reasonRepository.Update("USP_U_M_Reason", parameters, commandType: CommandType.StoredProcedure);
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
