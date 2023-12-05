
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
    public class StoreAreaService : IStoreAreaService
    {
        private readonly IGenericRepository<MStoreArea> _storeAreaRepository;

        private readonly IMapper _mapper;
        public StoreAreaService(IGenericRepository<MStoreArea> storeAreaRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _storeAreaRepository = storeAreaRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<StoreAreaResultViewModel> resultlist = new List<StoreAreaResultViewModel>();
            try
            {
                foreach (var item in model.StoreArea)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    
                        StoreAreaResultViewModel itemRs = new StoreAreaResultViewModel();
                        itemRs = _mapper.Map<StoreAreaResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                    resultlist.Add(itemRs);
                    
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

        public async Task<bool> checkExist(string CompanyCode, string StoreAreaId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("StoreAreaType", "");
            parameters.Add("StoreAreaId", StoreAreaId);
            parameters.Add("Status", ""); 
            var affectedRows = await _storeAreaRepository.GetAsync("USP_S_M_StoreArea", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MStoreArea model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                parameters.Add("StoreAreaId", model.StoreAreaId);
                parameters.Add("StoreAreaName", model.StoreAreaName);
                parameters.Add("Description", model.Description);
                parameters.Add("StoreAreaType", model.StoreAreaType);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
              
                var exist = await checkExist(model.CompanyCode.ToString(), model.StoreAreaId);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.StoreAreaId + " existed.";
                    return result;
                }
                var affectedRows = _storeAreaRepository.Insert("USP_I_M_StoreArea", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string companyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _storeAreaRepository.GetAllAsync($"select * from M_StoreArea with (nolock) where companyCode ='{companyCode}' ", null, commandType: CommandType.Text);
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
       
        public async Task<GenericResult> GetStoreAreaCapacity(string companyCode, string StoreId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _storeAreaRepository.GetAllAsync($"USP_GetStoreAreaCapacity '{companyCode}','{StoreId}' ", null, commandType: CommandType.Text);
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

       
        public async Task<GenericResult> GetByCode(string companyCode , string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _storeAreaRepository.GetAsync($"select * from M_StoreArea with (nolock)  where companyCode ='{companyCode}' and StoreAreaId ='{Code}'", null, commandType: CommandType.Text);
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

        public async Task<PagedList<MStoreArea>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _storeAreaRepository.GetAllAsync($"select * from M_StoreArea with (nolock) where StoreGroupId like N'%{userParams.keyword}%' or CompanyCode like N'%{userParams.keyword}%'  or StoreGroupName like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.StoreGroupName);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.StoreGroupId);
                //}
                return await PagedList<MStoreArea>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(MStoreArea model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode, DbType.String);
                parameters.Add("StoreAreaId", model.StoreAreaId);
                parameters.Add("StoreAreaName", model.StoreAreaName);
                parameters.Add("Description", model.Description);
                parameters.Add("StoreAreaType", model.StoreAreaType);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _storeAreaRepository.Insert("USP_U_M_StoreArea", parameters, commandType: CommandType.StoredProcedure);
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
