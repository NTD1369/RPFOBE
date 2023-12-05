
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
    public class ItemListingService : IItemListingService
    {
        private readonly IGenericRepository<MItemStoreListing> _listingRepository;

        private readonly IMapper _mapper;
        public ItemListingService(IGenericRepository<MItemStoreListing> listingRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _listingRepository = listingRepository;
            _mapper = mapper; 

        }
       
        public async Task<GenericResult> Create(MItemStoreListing model)
        {
            GenericResult result = new GenericResult();
            try
            {

                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);

                var affectedRows = _listingRepository.Insert("USP_I_M_ItemStoreListing", parameters, commandType: CommandType.StoredProcedure);
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
         
        public async Task<GenericResult> Delete(MItemStoreListing model)
        {
            GenericResult result = new GenericResult();
            try
            { 
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId); 
                parameters.Add("ItemCode", model.ItemCode); 

                var affectedRows = _listingRepository.Insert("USP_D_M_ItemStoreListing", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
       
        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ItemCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("ItemCode", ItemCode);
                var data = await _listingRepository.GetAllAsync($"USP_S_M_ItemStoreListing", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetItemListingStore(string CompanyCode,  string ItemCode, string UserCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode); 
                parameters.Add("ItemCode", ItemCode);
                parameters.Add("UserCode", UserCode);
                var data = await _listingRepository.GetAllAsync($"USP_GetItemListingStore", parameters, commandType: CommandType.StoredProcedure);
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

     

        public async Task<GenericResult> Update(MItemStoreListing model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var rsCheckData = await GetAll(model.CompanyCode, model.StoreId, model.ItemCode);
                if(rsCheckData.Success)
                {
                     var data = rsCheckData.Data as List<MItemStoreListing>;
                    
                    if (data != null && data.Count > 0)
                    {
                        //result = await Delete(model);

                        var parameters = new DynamicParameters();

                        parameters.Add("CompanyCode", model.CompanyCode);
                        parameters.Add("StoreId", model.StoreId);
                        parameters.Add("ItemCode", model.ItemCode);
                        parameters.Add("ModifiedBy", model.ModifiedBy);
                        parameters.Add("Status", model.Status);

                        _listingRepository.Update("USP_U_M_ItemStoreListing", parameters, commandType: CommandType.StoredProcedure);
                        result.Success = true;
                        
                    }
                    else
                    {
                        result = await Create(model);
                    }
                }   
                else
                {
                    result = rsCheckData;
                }
               
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
