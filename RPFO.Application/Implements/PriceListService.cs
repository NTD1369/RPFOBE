
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
    public class PriceListService : IPriceListService
    {
        private readonly IGenericRepository<MPriceList> _priceListRepository;

        private readonly IMapper _mapper;
        public PriceListService(IGenericRepository<MPriceList> priceListRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _priceListRepository = priceListRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<PriceListResultViewModel> resultlist = new List<PriceListResultViewModel>();
            try
            {
                foreach (var item in model.PriceList)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    
                        PriceListResultViewModel itemRs = new PriceListResultViewModel();
                        itemRs = _mapper.Map<PriceListResultViewModel>(item);
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
            } 
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string StoreId,   string ItemCode, string UomCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("StoreId", StoreId); 
            parameters.Add("ItemCode", ItemCode);
            parameters.Add("UomCode", UomCode);
            parameters.Add("Status", "");
            var affectedRows = await _priceListRepository.GetAsync("USP_S_M_PriceList", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MPriceList model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var exist = await checkExist(model.CompanyCode, model.StoreId, model.ItemCode, model.UomCode);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.StoreId + " - " + model.ItemCode + " - " + model.UomCode + " existed.";
                    return result;
                }
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("UOMCode", model.UomCode);
                parameters.Add("BarCode", model.BarCode);
                parameters.Add("PriceListId", model.PriceListId);
                parameters.Add("PriceBeforeTax", model.PriceBeforeTax);
                parameters.Add("PriceAfterTax", model.PriceAfterTax);
                parameters.Add("ValidFrom", model.ValidFrom);
                parameters.Add("ValidTo", model.ValidTo);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
              
                var affectedRows = _priceListRepository.Insert("USP_I_M_PriceList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string ItemCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", string.IsNullOrEmpty(StoreId) ? "" : StoreId);
                parameters.Add("ItemCode", string.IsNullOrEmpty(ItemCode) ? "" : ItemCode);
                parameters.Add("UomCode","");
                parameters.Add("Status","");
                var data = await _priceListRepository.GetAllAsync($"USP_S_M_PriceList", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<GenericResult> GetAllId(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _priceListRepository.GetAllAsync($"select distinct PriceListId  from M_PriceList with (nolock) where CompanyCode= N'{CompanyCode}' ", null, commandType: CommandType.Text);
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
                var data = await _priceListRepository.GetAsync($"select * from M_PriceList with (nolock)  where  CompanyCode= N'{CompanyCode}' and PriceListId ='{Code}'", null, commandType: CommandType.Text);
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

        public async Task<PagedList<MPriceList>> GetPagedList(UserParams userParams)
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
                return await PagedList<MPriceList>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(MPriceList model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                 
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("ItemCode", model.ItemCode);
                parameters.Add("UOMCode", model.UomCode);
                parameters.Add("BarCode", model.BarCode);
                parameters.Add("PriceListId", model.PriceListId);
                parameters.Add("PriceBeforeTax", model.PriceBeforeTax);
                parameters.Add("PriceAfterTax", model.PriceAfterTax);
                parameters.Add("ValidFrom", model.ValidFrom);
                parameters.Add("ValidTo", model.ValidTo);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _priceListRepository.Insert("USP_U_M_PriceList", parameters, commandType: CommandType.StoredProcedure);
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
