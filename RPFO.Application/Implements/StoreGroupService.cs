
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
    public class StoreGroupService : IStoreGroupService
    {
        private readonly IGenericRepository<MStoreGroup> _storeGroupRepository;

        private readonly IMapper _mapper;
        public StoreGroupService(IGenericRepository<MStoreGroup> storeGroupRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _storeGroupRepository = storeGroupRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<StoreGroupResultViewModel> resultlist = new List<StoreGroupResultViewModel>();
            try
            {
                foreach (var item in model.StoreGroup)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    //if (itemResult.Success == false)
                    //{
                        StoreGroupResultViewModel itemRs = new StoreGroupResultViewModel();
                        itemRs = _mapper.Map<StoreGroupResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                        resultlist.Add(itemRs);
                    //}
                }
                result.Success = true; 
                result.Data = resultlist;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                //result.Data = failedlist;
            } 
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string StoreGroupId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("StoreGroupId", StoreGroupId);
            parameters.Add("Status", "");
            var affectedRows = await _storeGroupRepository.GetAsync("USP_S_M_StoreGroup", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MStoreGroup model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("StoreGroupId", model.StoreGroupId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreGroupName", model.StoreGroupName);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                var exist = await checkExist(model.CompanyCode, model.StoreGroupId);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.StoreGroupId + " existed.";
                    return result;
                }
                var affectedRows = _storeGroupRepository.Insert("USP_I_M_StoreGroup", parameters, commandType: CommandType.StoredProcedure);
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

        public Task<GenericResult> Delete(string CompanyCode, string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _storeGroupRepository.GetAllAsync($"select * from M_StoreGroup with (nolock) where CompanyCode= '{CompanyCode}'", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _storeGroupRepository.GetAsync($"select * from M_StoreGroup with (nolock)  where CompanyCode= '{CompanyCode}' and StoreGroupId ='{Code}'", null, commandType: CommandType.Text);
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

        public async Task<PagedList<MStoreGroup>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _storeGroupRepository.GetAllAsync($"select * from M_StoreGroup with (nolock) where StoreGroupId like N'%{userParams.keyword}%' or CompanyCode like N'%{userParams.keyword}%'  or StoreGroupName like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.StoreGroupName);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.StoreGroupId);
                }
                return await PagedList<MStoreGroup>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(MStoreGroup model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("StoreGroupId", model.StoreGroupId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreGroupName", model.StoreGroupName);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _storeGroupRepository.Insert("USP_U_M_StoreGroup", parameters, commandType: CommandType.StoredProcedure);
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
