
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
    public class DenominationService : IDenominationService
    {
        private readonly IGenericRepository<MDenomination> _denoRepository;

        private readonly IMapper _mapper;
        public DenominationService(IGenericRepository<MDenomination> denoRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _denoRepository = denoRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<PriceListResultViewModel> resultlist = new List<PriceListResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.PriceList)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);
                    
        //                PriceListResultViewModel itemRs = new PriceListResultViewModel();
        //                itemRs = _mapper.Map<PriceListResultViewModel>(item);
        //                itemRs.Success = itemResult.Success;
        //                itemRs.Message = itemResult.Message;
        //            resultlist.Add(itemRs);
                    
        //        }
        //        result.Success = true;
        //        result.Data = resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message; 
        //    } 
        //    return result;
        //}

        public async Task<bool> checkExist(string Currency, string Value )
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("Currency", Currency);
            parameters.Add("Value", Value);  
            var affectedRows = await _denoRepository.GetAsync("USP_S_M_Denomination", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MDenomination model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var exist = await checkExist(model.Currency, model.Value);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.Currency + " - " + model.Value + " existed.";
                    return result;
                }
                var parameters = new DynamicParameters();
                 
                parameters.Add("Currency", model.Currency);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                parameters.Add("Value", model.Value);
                parameters.Add("Status", model.Status);
                parameters.Add("Remarks", model.Remarks);
                parameters.Add("ShowOnDiscount", model.ShowOnDiscount);
                parameters.Add("ShowOnPayment", model.ShowOnPayment);

                var affectedRows = _denoRepository.Insert("USP_I_M_Denomination", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CurrencyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("Currency", CurrencyCode);
                parameters.Add("Value", null);
                var data = await _denoRepository.GetAllAsync("USP_S_M_Denomination", parameters, commandType: CommandType.StoredProcedure);
                //var data = await _denoRepository.GetAllAsync($"select * from M_PriceList with (nolock) ", null, commandType: CommandType.Text);
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
                var data = await _denoRepository.GetAsync($"select * from M_PriceList with (nolock)  where PriceListId ='{Code}'", null, commandType: CommandType.Text);
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

        public async Task<PagedList<MDenomination>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _denoRepository.GetAllAsync($"select * from M_PriceList with (nolock) where  CompanyCode like N'%{userParams.keyword}%'  or ItemCode like N'%{userParams.keyword}%'", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                //if (userParams.OrderBy == "byName")
                //{
                //    data.OrderByDescending(x => x.StoreGroupName);
                //}
                //if (userParams.OrderBy == "byId")
                //{
                //    data.OrderByDescending(x => x.StoreGroupId);
                //}
                return await PagedList<MDenomination>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GenericResult> Update(MDenomination model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                 
                parameters.Add("Id", model.Id);
                parameters.Add("Currency", model.Currency);
                parameters.Add("Name", model.Name);
                parameters.Add("Description", model.Description);
                parameters.Add("Value", model.Value);
                parameters.Add("Status", model.Status);
                parameters.Add("Remarks", model.Remarks);
                parameters.Add("ShowOnDiscount", model.ShowOnDiscount);
                parameters.Add("ShowOnPayment", model.ShowOnPayment);
                var affectedRows = _denoRepository.Insert("USP_U_M_Denomination", parameters, commandType: CommandType.StoredProcedure);
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
