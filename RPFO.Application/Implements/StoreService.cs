
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
    public class StoreService : IStoreService
    {
        private readonly IGenericRepository<MStore> _storeRepository;

        private readonly IMapper _mapper;
        public StoreService(IGenericRepository<MStore> storeRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<StoreResultViewModel> resultlist = new List<StoreResultViewModel>();
            try
            {
                foreach (var item in model.Store)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    //if (itemResult.Success == false)
                    //{
                        StoreResultViewModel itemRs = new StoreResultViewModel();
                        itemRs = _mapper.Map<StoreResultViewModel>(item);
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

        public async Task<bool> checkExist(string CompanyCode, string StoreId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("StoreId", StoreId);
            parameters.Add("Status", "");
            var affectedRows = await _storeRepository.GetAsync("USP_S_M_Store", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MStore model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("StoreId", model.StoreId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreName", model.StoreName);
                parameters.Add("StoreDescription",  model.StoreDescription);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("ForeignName", model.ForeignName);
                parameters.Add("Address", model.Address);
                parameters.Add("Phone", model.Phone);
                parameters.Add("DefaultCusId", model.DefaultCusId);
                parameters.Add("StoreGroupId", model.StoreGroupId);
                parameters.Add("ProvinceId", model.ProvinceId);
                parameters.Add("DistrictId", model.DistrictId);
                parameters.Add("WardId", model.WardId);
                parameters.Add("CountryCode", model.CountryCode);
                parameters.Add("CustomField1", model.CustomField1);
                parameters.Add("CustomField2", model.CustomField2);
                parameters.Add("CustomField3", model.CustomField3);
                parameters.Add("CustomField4", model.CustomField4);
                parameters.Add("CustomField5", model.CustomField5);
                parameters.Add("AreaCode", model.AreaCode);
                parameters.Add("CurrencyCode", model.CurrencyCode);
                parameters.Add("StoreType", model.StoreType);
                parameters.Add("ListType", model.ListType);
                parameters.Add("FormatConfigId", model.FormatConfigId);
                parameters.Add("WhsCode", model.WhsCode);
                parameters.Add("RegionCode", model.RegionCode);
                parameters.Add("PrintRemarks", model.PrintRemarks);

                var exist = await checkExist(model.CompanyCode, model.StoreId);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.StoreId + " existed.";
                    return result;
                }

                var affectedRows = _storeRepository.Insert("USP_I_M_Store", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }
     
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", "");
                parameters.Add("Status", "");
                var data = await _storeRepository.GetAllAsync("USP_S_M_Store", parameters, commandType: CommandType.StoredProcedure);
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
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", "");
                parameters.Add("Status", "");
                var data = await _storeRepository.GetAsync("USP_S_M_Store", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetStoreListByUser(string User)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _storeRepository.GetConnection())
            {
                try
                {
                    var item = new ItemViewModel();
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    
                    var dblist = await db.QueryAsync<StoreViewModel>($"USP_GetStoreListByUser '{User}'", null, commandType: CommandType.Text);
                    result.Success = true;
                    result.Data = dblist.ToList();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                }
               
            }
            return result;


        }
        //, string ComapnyCode, bool? AllStoreStatus
        public async Task<GenericResult> GetByUser(string User)
        {

            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                //parameters.Add("ComapnyCode", ComapnyCode);
                parameters.Add("UserCode", User);
                //parameters.Add("AllStoreStatus", AllStoreStatus);
                var data = await _storeRepository.GetAllAsync("USP_GetStoreByUser", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //try
            //{
            //     var data = await _storeRepository.GetAllAsync($"[USP_GetStoreByUser] '{User}'", null, commandType: CommandType.Text);
            //    return data;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }

        public async Task<PagedList<MStore>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _storeRepository.GetAllAsync($"select * from M_Store with (nolock) where StoreId like N'%{userParams.keyword}%' or StoreName like N'%{userParams.keyword}%'  or StoreDescription like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if(userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.StoreName);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.StoreId);
                }
                return await PagedList<MStore>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GenericResult> Update(MStore model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("StoreId", model.StoreId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreName", model.StoreName);
                parameters.Add("StoreDescription", model.StoreDescription);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("ForeignName", model.ForeignName);
                parameters.Add("Address", model.Address);
                parameters.Add("Phone", model.Phone);
                parameters.Add("DefaultCusId", model.DefaultCusId);
                parameters.Add("StoreGroupId", model.StoreGroupId);
                parameters.Add("ProvinceId", model.ProvinceId);
                parameters.Add("DistrictId", model.DistrictId);
                parameters.Add("WardId", model.WardId);
                parameters.Add("CountryCode", model.CountryCode);
                parameters.Add("CustomField1", model.CustomField1);
                parameters.Add("CustomField2", model.CustomField2);
                parameters.Add("CustomField3", model.CustomField3);
                parameters.Add("CustomField4", model.CustomField4);
                parameters.Add("CustomField5", model.CustomField5);
                parameters.Add("AreaCode", model.AreaCode);
                parameters.Add("CurrencyCode", model.CurrencyCode);
                parameters.Add("StoreType", model.StoreType);
                parameters.Add("ListType", model.ListType);
                parameters.Add("FormatConfigId", model.FormatConfigId);
                parameters.Add("WhsCode", model.WhsCode);
                parameters.Add("RegionCode", model.RegionCode);
                parameters.Add("PrintRemarks", model.PrintRemarks);
                var affectedRows = _storeRepository.Update("USP_U_M_Store", parameters, commandType: CommandType.StoredProcedure);
               
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

        public async Task<GenericResult> GetStoreByUserWhsType(string User)
        {

            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserCode", User);
                var data = await _storeRepository.GetAllAsync("USP_GetStoreByUserWhsType", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAllByWhstype(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", "");
                parameters.Add("Status", "");
                var data = await _storeRepository.GetAllAsync("USP_S_M_StoreByWhstype", parameters, commandType: CommandType.StoredProcedure);
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
    } 

}
