using AutoMapper;
using Dapper;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class SalesChannelService : ISalesChannelService
    {
        private readonly IGenericRepository<MSalesChannel> _channelRepository;
        private readonly IMapper _mapper;
        public SalesChannelService(IGenericRepository<MSalesChannel> channelRepository, IMapper mapper)
        {
            _channelRepository = channelRepository;
            _mapper = mapper;
        }


        public async Task<GenericResult> Create(MSalesChannel model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Key", model.Key);
                parameters.Add("Value", model.Value);
                parameters.Add("CustomField1", model.CustomField1);
                parameters.Add("CustomField2", model.CustomField2);
                parameters.Add("CustomField3", model.CustomField3);
                parameters.Add("CustomField4", model.CustomField4);
                parameters.Add("CustomField5", model.CustomField5);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);

                var affectedRows =  _channelRepository.Insert("USP_I_M_SalesChannel", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(MSalesChannel model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Key", model.Key);
                var affectedRows = _channelRepository.Execute("USP_D_M_SalesChannel", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string Keyword)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("key", "");
                parameters.Add("Keyword", Keyword);
                var data = await _channelRepository.GetAllAsync($"USP_S_M_SalesChannel", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetByCode(string CompanyCode, string Key)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Key", Key);
                parameters.Add("Keyword", "");
                var data = await _channelRepository.GetAsync($"USP_S_M_SalesChannel", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Update(MSalesChannel model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Key", model.Key);
                parameters.Add("Value", model.Value);
                parameters.Add("CustomField1", model.CustomField1);
                parameters.Add("CustomField2", model.CustomField2);
                parameters.Add("CustomField3", model.CustomField3);
                parameters.Add("CustomField4", model.CustomField4);
                parameters.Add("CustomField5", model.CustomField5);
                parameters.Add("CustomField4", model.CustomField4);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);

                var affectedRows = _channelRepository.Insert("USP_U_M_SalesChannel", parameters, commandType: CommandType.StoredProcedure);
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
