
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
    public class UserStoreService : IUserStoreService
    {
        private readonly IGenericRepository<MUserStore> _userStoreRepository;

        private readonly IMapper _mapper;
        public UserStoreService(IGenericRepository<MUserStore> userStoreRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _userStoreRepository = userStoreRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<UserStoreResultViewModel> resultlist = new List<UserStoreResultViewModel>();
            try
            {
                foreach (var item in model.UserStore)
                {
                    //item.CreatedBy = model.CreatedBy;
                    //item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    //if (itemResult.Success == false)
                    //{
                        UserStoreResultViewModel itemRs = new UserStoreResultViewModel();
                        itemRs = _mapper.Map<UserStoreResultViewModel>(item);
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

        public async Task<bool> checkExist(string UserId, string StoreId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("UserId", UserId);
            parameters.Add("StoreId", StoreId); 
            var affectedRows = await _userStoreRepository.GetAsync("USP_S_M_UserStore", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MUserStore model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("UserId", model.UserId);
                parameters.Add("StoreId", model.StoreId);
              
                var exist = await checkExist(model.UserId.ToString(), model.StoreId);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.StoreId + " existed.";
                    return result;
                }
                var affectedRows = _userStoreRepository.Insert("USP_I_M_UserStore", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll()
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _userStoreRepository.GetAllAsync($"select * from M_UserStore with (nolock) ", null, commandType: CommandType.Text);
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

        public async Task<GenericResult> GetByCode(string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _userStoreRepository.GetAsync($"select * from M_UserStore with (nolock)  where StoreGroupId ='{Code}'", null, commandType: CommandType.Text);
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

        public async Task<PagedList<MUserStore>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _userStoreRepository.GetAllAsync($"select * from M_UserStore with (nolock) where StoreGroupId like N'%{userParams.keyword}%' or CompanyCode like N'%{userParams.keyword}%'  or StoreGroupName like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.StoreGroupName);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.StoreGroupId);
                //}
                return await PagedList<MUserStore>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public async Task<GenericResult> Update(MUserStore model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                var exist = await checkExist(model.UserId.ToString(), model.StoreId);
                if (exist == true)
                {
                    var affectedRows = _userStoreRepository.Execute($"USP_D_M_UserStore '{model.StoreId}','{model.UserId}' ", null, commandType: CommandType.Text);
                }
                else
                {
                   
                    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                    //model.ShiftId = key;
                    parameters.Add("UserId", model.UserId);
                    parameters.Add("StoreId", model.StoreId);
                     
                    var affectedRows = _userStoreRepository.Insert("USP_I_M_UserStore", parameters, commandType: CommandType.StoredProcedure);
                  
                }
                result.Success = true;
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
