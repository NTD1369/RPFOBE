
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
    public class ItemGroupService : IItemGroupService
    {
        private readonly IGenericRepository<MItemGroup> _itemGroupRepository;

        private readonly IMapper _mapper;
        public ItemGroupService(IGenericRepository<MItemGroup> itemGroupRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _itemGroupRepository = itemGroupRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<ItemGroupResultViewModel> failedlist = new List<ItemGroupResultViewModel>();
            try
            {
                foreach (var item in model.ItemGroup)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    
                    ItemGroupResultViewModel itemRs = new ItemGroupResultViewModel();
                    itemRs = _mapper.Map<ItemGroupResultViewModel>(item);
                    itemRs.Success = itemResult.Success;
                    itemRs.Message = itemResult.Message;
                    failedlist.Add(itemRs);
                    
                }
                result.Success = true;
                result.Data = failedlist;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.Data = failedlist;
            } 
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string IGId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("IGId", IGId);
            parameters.Add("Status", "");
            var affectedRows = await _itemGroupRepository.GetAsync("USP_S_M_ItemGroup", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MItemGroup model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("IGId", model.Igid, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("IGName", model.Igname);
                parameters.Add("IGDescription", model.Igdescription);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                var exist = await checkExist(model.CompanyCode, model.Igid);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.Igid + " existed.";
                    return result;
                }
                var affectedRows = _itemGroupRepository.Insert("USP_I_M_ItemGroup", parameters, commandType: CommandType.StoredProcedure);
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
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("IGId", "");
                parameters.Add("Status","");
                
                var data = await _itemGroupRepository.GetAllAsync($"USP_S_M_ItemGroup", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetByCode(string CompanyCode , string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("IGId", Code);
                parameters.Add("Status", "");

                var data = await _itemGroupRepository.GetAsync($"USP_S_M_ItemGroup", parameters, commandType: CommandType.StoredProcedure);
                //var data = await _itemGroupRepository.GetAsync($"select * from M_ItemGroup with (nolock)  where CompanyCode =N'{CompanyCode}' and IGId ='{Code}'", null, commandType: CommandType.Text);
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

        public async Task<PagedList<MItemGroup>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _itemGroupRepository.GetAllAsync($"select * from M_ItemGroup with (nolock) where IGId like N'%{userParams.keyword}%' or CompanyCode like N'%{userParams.keyword}%'  or StoreGroupName like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.StoreGroupName);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.StoreGroupId);
                //}
                return await PagedList<MItemGroup>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(MItemGroup model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("IGId", model.Igid, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("IGName", model.Igname);
                parameters.Add("IGDescription", model.Igdescription);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _itemGroupRepository.Insert("USP_U_M_ItemGroup", parameters, commandType: CommandType.StoredProcedure);
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
