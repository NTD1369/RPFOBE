
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
    public class StorageService : IStorageService
    {
        private readonly IGenericRepository<MStorage> _storageRepository;

        private readonly IMapper _mapper;
        public StorageService(IGenericRepository<MStorage> storageRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/)//: base(hubContext)
        {
            _storageRepository = storageRepository;
            _mapper = mapper;   
        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<StorageResultViewModel> resultlist = new List<StorageResultViewModel>();
            try
            {
                foreach (var item in model.Storage)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    //if (itemResult.Success == false)
                    //{
                    StorageResultViewModel itemRs = new StorageResultViewModel();
                    itemRs = _mapper.Map<StorageResultViewModel>(item);
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
                result.Data = resultlist;
            } 
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string SlocId, string WhsCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("SlocId", SlocId);
            parameters.Add("WhsCode", WhsCode);
            parameters.Add("Status", "");
            var affectedRows = await _storageRepository.GetAsync("USP_S_M_Storage", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MStorage model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storageRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("SLocId", model.SlocId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("WhsCode", model.WhsCode); 
                parameters.Add("SlocName", model.SlocName); 
                parameters.Add("IsNegative", model.IsNegative);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                var exist = await checkExist(model.CompanyCode, model.SlocId, model.WhsCode);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.SlocId + " existed.";
                    return result;
                }
                var affectedRows = _storageRepository.Insert("USP_I_M_Storage", parameters, commandType: CommandType.StoredProcedure);
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
                var data = await _storageRepository.GetAllAsync($"select * from M_Storage with (nolock) where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
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

   
        public async Task<GenericResult> GetByStore(string StoreId, string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                string query = $"[USP_GetStorageByStore] '{CompanyCode}','{StoreId}' ";
                var data = await _storageRepository.GetAllAsync(query, null, commandType: CommandType.Text);
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

        public async Task<PagedList<MStorage>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _storageRepository.GetAllAsync($"select * from M_Storage with (nolock)  ", null, commandType: CommandType.Text);
               
                return await PagedList<MStorage>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GenericResult> Update(MStorage model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SLocId", model.SlocId);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("WhsCode", model.WhsCode);
                parameters.Add("SlocName", model.SlocName);
                parameters.Add("IsNegative", model.IsNegative);
                parameters.Add("ModifiedBy", model.ModifiedBy); 
                parameters.Add("Status", model.Status);
                var affectedRows = _storageRepository.Insert("USP_U_M_Storage", parameters, commandType: CommandType.StoredProcedure); 
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
