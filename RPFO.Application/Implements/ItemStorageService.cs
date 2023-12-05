
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
    public class ItemStorageService : IItemStorageService
    {
        private readonly IGenericRepository<TItemStorage> _itemStorageRepository;

        private readonly IMapper _mapper;
        public ItemStorageService(IGenericRepository<TItemStorage> itemStorageRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _itemStorageRepository = itemStorageRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<TItemStorageResultViewModel> failedlist = new List<TItemStorageResultViewModel>();
            try
            {
                foreach (var item in model.ItemStorage)
                {
                    //item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);

                    TItemStorageResultViewModel itemRs = new TItemStorageResultViewModel();
                    itemRs = _mapper.Map<TItemStorageResultViewModel>(item);
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
 
        public async Task<GenericResult> Create(TItemStorage model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId, DbType.String);
                parameters.Add("SLocId", model.SlocId);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("UOMCode", model.UomCode);
                parameters.Add("Quantity", model.Quantity);
                var affectedRows = _itemStorageRepository.Insert("USP_I_T_ItemStorage", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string companyCode, string StoreId, string SlocId, string ItemCode, string Uomcode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("StoreId", StoreId, DbType.String);
                parameters.Add("SLocId", SlocId);
                parameters.Add("ItemCode", ItemCode);
                parameters.Add("UOMCode", Uomcode);
                var data = await _itemStorageRepository.GetAllAsync($"USP_S_T_ItemStorage", parameters, commandType: CommandType.StoredProcedure);
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
        public bool checkInList(TItemStorage item , List<TItemStorage> list)
        {
            if(list!=null && list.Count >0)
            {
                foreach (var itemCheck in list)
                {
                    if (itemCheck.ItemCode == item.ItemCode && itemCheck.UomCode == item.UomCode && itemCheck.SlocId == item.SlocId)
                    {
                        return true;
                    }
                }
            }    
           
            return false;
        }
        public async Task<GenericResult> GetAllX(string companyCode, string StoreId, string SlocId, string ItemCode, string Uomcode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("StoreId", StoreId, DbType.String);
                parameters.Add("SLocId", SlocId);
                parameters.Add("ItemCode", ItemCode);
                parameters.Add("UOMCode", Uomcode);
                var data = await _itemStorageRepository.GetAllAsync($"USP_S_T_ItemStorage", parameters, commandType: CommandType.StoredProcedure);
                List<TItemStorage> headerList = new List<TItemStorage>();
                //foreach(TItemStorage item in data)
                //{
                //    if(item)
                //    headerList.Add(item);
                //}    
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
        public async Task<GenericResult> GetByCode(string companyCode, string StoreId, string SlocId, string ItemCode, string Uomcode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", companyCode);
                parameters.Add("StoreId", StoreId, DbType.String);
                parameters.Add("SLocId", SlocId);
                parameters.Add("ItemCode", ItemCode);
                parameters.Add("UOMCode", Uomcode);
                var data = await _itemStorageRepository.GetAsync($"USP_S_T_ItemStorage", parameters, commandType: CommandType.StoredProcedure);
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

       

        public async Task<GenericResult> Update(TItemStorage model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId, DbType.String);
                parameters.Add("SLocId", model.SlocId);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("UOMCode", model.UomCode);
                parameters.Add("Quantity", model.Quantity);
                var affectedRows = _itemStorageRepository.Insert("USP_U_T_ItemStorage", parameters, commandType: CommandType.StoredProcedure);
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
