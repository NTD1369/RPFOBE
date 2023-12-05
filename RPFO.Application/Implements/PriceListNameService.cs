
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
    public class PriceListNameService : IPriceListNameService
    {
        private readonly IGenericRepository<MPriceListName> _priceListRepository;

        private readonly IMapper _mapper;
        public PriceListNameService(IGenericRepository<MPriceListName> priceListRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _priceListRepository = priceListRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
       
        //public async Task<bool> checkExist(string CompanyCode, string StoreId,   string ItemCode, string UomCode)
        //{
        //    var parameters = new DynamicParameters();

        //    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //    //model.ShiftId = key;
        //    parameters.Add("CompanyCode", CompanyCode);
        //    parameters.Add("StoreId", StoreId); 
        //    parameters.Add("ItemCode", ItemCode);
        //    parameters.Add("UomCode", UomCode);
        //    parameters.Add("Status", "");
        //    var affectedRows = await _priceListRepository.GetAsync("USP_S_M_PriceList", parameters, commandType: CommandType.StoredProcedure);
        //    if (affectedRows != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public async Task<GenericResult> Create(MPriceListName model)
        {
            GenericResult result = new GenericResult();
            try
            {
                //var exist = await checkExist(model.CompanyCode, model.StoreId, model.ItemCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.StoreId + " - " + model.ItemCode + " - " + model.UomCode + " existed.";
                //    return result;
                //}
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", Guid.NewGuid());
                parameters.Add("PriceListId", model.PriceListId);
                parameters.Add("PriceListName", model.PriceListName); 
                var affectedRows = _priceListRepository.Insert("USP_I_M_PriceListName", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(MPriceListName model)
        {
            GenericResult result = new GenericResult();
            try
            {
                //var exist = await checkExist(model.CompanyCode, model.StoreId, model.ItemCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.StoreId + " - " + model.ItemCode + " - " + model.UomCode + " existed.";
                //    return result;
                //}
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", Guid.NewGuid());
                //parameters.Add("PriceListId", model.PriceListId);
                //parameters.Add("PriceListName", model.PriceListName);
                var affectedRows = _priceListRepository.Execute("USP_D_M_PriceListName", parameters, commandType: CommandType.StoredProcedure);
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
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                //parameters.Add("Id", Guid.NewGuid());
                var data = await _priceListRepository.GetAllAsync($"USP_S_M_PriceListName", parameters, commandType: CommandType.StoredProcedure);
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
       
        public async Task<GenericResult> GetById(string CompanyCode, string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Id", Guid.NewGuid());
                var data = await _priceListRepository.GetAsync($"USP_S_M_PriceListName", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<PagedList<MPriceListName>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _priceListRepository.GetAllAsync($"select * from M_PriceList with (nolock) where  CompanyCode like N'%{userParams.keyword}%'  or ItemCode like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.StoreGroupName);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.StoreGroupId);
                //}
                return await PagedList<MPriceListName>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(MPriceListName model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", model.Id);
                parameters.Add("PriceListId", model.PriceListId);
                parameters.Add("PriceListName", model.PriceListName);
                var affectedRows = _priceListRepository.Insert("USP_U_M_PriceListName", parameters, commandType: CommandType.StoredProcedure);
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
