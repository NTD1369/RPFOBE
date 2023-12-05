
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
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
    public class ReleaseNoteService : IReleaseNoteService
    {
        private readonly IGenericRepository<SReleaseNote> _releaseRepository;
        private readonly IGeneralSettingService _settingService;
        private readonly IMapper _mapper;
      
        public ReleaseNoteService(IGenericRepository<SReleaseNote> releaseRepository, IGeneralSettingService settingService, IMapper mapper, IConfiguration config/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _releaseRepository = releaseRepository;
            
            _mapper = mapper;
            _settingService = settingService;
           
        }
     
        //public async Task<bool> checkExist(string CompanyCode, string CustomerId, string Phone)
        //{
        //    var parameters = new DynamicParameters();

        //    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //    //model.ShiftId = key;
        //    parameters.Add("CompanyCode", CompanyCode);
        //    parameters.Add("CustomerGrpId", "");
        //    parameters.Add("CustomerId", CustomerId);
        //    parameters.Add("Status", "");
        //    parameters.Add("Key", "");
        //    parameters.Add("Type", "");
        //    parameters.Add("CustomerName", "");
        //    parameters.Add("Address", "");
        //    parameters.Add("Phone", Phone);
        //    parameters.Add("DOB", null);
        //    var affectedRows = await _releaseRepository.GetAsync("USP_S_M_Customer", parameters, commandType: CommandType.StoredProcedure);
        //    if (affectedRows != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public async Task<GenericResult> Create(SReleaseNote model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                model.Id = Guid.NewGuid();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", model.Id);
                parameters.Add("Version", model.Version, DbType.String);
                parameters.Add("Description", string.IsNullOrEmpty(model.Description) ? "" : model.Description);
                parameters.Add("ReleaseTime", model.ReleaseTime == null ? DateTime.Now : model.ReleaseTime, DbType.DateTime);
                parameters.Add("ReleaseContent", string.IsNullOrEmpty(model.ReleaseContent) ? "" : model.ReleaseContent);
                parameters.Add("ReleaseContentForeign", string.IsNullOrEmpty(model.ReleaseContentForeign) ? "" : model.ReleaseContentForeign); 
                parameters.Add("CustomF1", string.IsNullOrEmpty(model.CustomF1) ? "" : model.CustomF1);
                parameters.Add("CustomF2", string.IsNullOrEmpty(model.CustomF2) ? "" : model.CustomF2);
                parameters.Add("CustomF3", string.IsNullOrEmpty(model.CustomF3) ? "" : model.CustomF3);
                parameters.Add("CustomF4", string.IsNullOrEmpty(model.CustomF4) ? "" : model.CustomF4);
                parameters.Add("CustomF5", string.IsNullOrEmpty(model.CustomF5) ? "" : model.CustomF5);
                parameters.Add("CreatedBy", string.IsNullOrEmpty(model.CreatedBy) ? "" :  model.CreatedBy); 
                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "A" : model.Status);

                var affectedRows = _releaseRepository.Insert("USP_I_S_ReleaseNote", parameters, commandType: CommandType.StoredProcedure);
               
                result.Success = true;
                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> Delete(SReleaseNote model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                model.Id = Guid.NewGuid();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", model.Id);
                parameters.Add("Version", model.Version, DbType.String);
            
                var affectedRows = _releaseRepository.Insert("USP_D_S_ReleaseNote", parameters, commandType: CommandType.StoredProcedure);

                result.Success = true;
                result.Data = model;
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

                parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                parameters.Add("Id","");
                parameters.Add("Version", "");
                parameters.Add("Keyword", "");
                parameters.Add("Status", "");
                var data = await _releaseRepository.GetAllAsync("USP_S_S_ReleaseNote", parameters, commandType: CommandType.StoredProcedure);

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

        public async Task<GenericResult> GetByCode(string CompanyCode, string Id, string Version)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode);
                parameters.Add("Id", string.IsNullOrEmpty(Id) ? "" : Id);
                parameters.Add("Version", string.IsNullOrEmpty(Version) ? "" : Version, DbType.String);
                parameters.Add("Keyword", "");
                parameters.Add("Status", ""); 
                var data = await _releaseRepository.GetAsync("USP_S_S_ReleaseNote", parameters, commandType: CommandType.StoredProcedure);
                //var data = await _customerRepository.GetAsync($"select * from M_Customer where  CompanyCode='{CompanyCode}' and CustomerId = '{Code} or '" , null, commandType: CommandType.Text);
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



        public async Task<PagedList<SReleaseNote>> GetPagedList(UserParams userParams)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", string.IsNullOrEmpty(userParams.CompanyCode) ? "" : userParams.CompanyCode);
                parameters.Add("Id", string.IsNullOrEmpty(userParams.Id) ? "" : userParams.Id);
                parameters.Add("Version", string.IsNullOrEmpty(userParams.Version) ? "" : userParams.Version, DbType.String);
                parameters.Add("Keyword", string.IsNullOrEmpty(userParams.Keyword) ? "" : userParams.Keyword);
                parameters.Add("Status", string.IsNullOrEmpty(userParams.Status) ? "A" : userParams.Status);
                //parameters.Add("ReleaseTime", model.ReleaseTime == null ? DateTime.Now : model.ReleaseTime, DbType.DateTime);
                //parameters.Add("ReleaseContent", string.IsNullOrEmpty(model.ReleaseContent) ? "" : model.ReleaseContent);
                //parameters.Add("ReleaseContentForeign", string.IsNullOrEmpty(model.ReleaseContentForeign) ? "" : model.ReleaseContentForeign);
                //parameters.Add("CustomF1", string.IsNullOrEmpty(model.CustomF1) ? "" : model.CustomF1);
                //parameters.Add("CustomF2", string.IsNullOrEmpty(model.CustomF2) ? "" : model.CustomF2);
                //parameters.Add("CustomF3", string.IsNullOrEmpty(model.CustomF3) ? "" : model.CustomF3);
                //parameters.Add("CustomF4", string.IsNullOrEmpty(model.CustomF4) ? "" : model.CustomF4);
                //parameters.Add("CustomF5", string.IsNullOrEmpty(model.CustomF5) ? "" : model.CustomF5);
                //parameters.Add("ModifiedBy", string.IsNullOrEmpty(model.ModifiedBy) ? "" : model.ModifiedBy);

                var data = await _releaseRepository.GetAllAsync($"USP_S_S_ReleaseNote", parameters, commandType: CommandType.StoredProcedure);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byDate")
                {
                    data.OrderByDescending(x => x.ReleaseTime);
                }
                if (userParams.OrderBy == "byVersion")
                {
                    data.OrderByDescending(x => x.Version);
                }
                return await PagedList<SReleaseNote>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GenericResult> Update(SReleaseNote model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", model.Id);
                parameters.Add("Version", model.Version, DbType.String);
                parameters.Add("Description", string.IsNullOrEmpty(model.Description) ? "" : model.Description);
                parameters.Add("ReleaseTime", model.ReleaseTime == null ? DateTime.Now : model.ReleaseTime, DbType.DateTime);
                parameters.Add("ReleaseContent", string.IsNullOrEmpty(model.ReleaseContent) ? "" : model.ReleaseContent);
                parameters.Add("ReleaseContentForeign", string.IsNullOrEmpty(model.ReleaseContentForeign) ? "" : model.ReleaseContentForeign);
                parameters.Add("CustomF1", string.IsNullOrEmpty(model.CustomF1) ? "" : model.CustomF1);
                parameters.Add("CustomF2", string.IsNullOrEmpty(model.CustomF2) ? "" : model.CustomF2);
                parameters.Add("CustomF3", string.IsNullOrEmpty(model.CustomF3) ? "" : model.CustomF3);
                parameters.Add("CustomF4", string.IsNullOrEmpty(model.CustomF4) ? "" : model.CustomF4);
                parameters.Add("CustomF5", string.IsNullOrEmpty(model.CustomF5) ? "" : model.CustomF5);
                parameters.Add("ModifiedBy", string.IsNullOrEmpty(model.ModifiedBy) ? "" : model.ModifiedBy);
                parameters.Add("Status", string.IsNullOrEmpty(model.Status) ? "A" : model.Status);
 
                var affectedRows = _releaseRepository.Update("USP_U_S_ReleaseNote", parameters, commandType: CommandType.StoredProcedure);
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
