
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
    public class StoreCurrencyService : IStoreCurrencyService
    {
        private readonly IGenericRepository<MStoreCurrency> _currencyRepository;

        private readonly IMapper _mapper;
        public StoreCurrencyService(IGenericRepository<MStoreCurrency> currencyRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _currencyRepository = currencyRepository;
            _mapper = mapper; 

        }
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<UOMResultViewModel> resultlist = new List<UOMResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.Uom)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);
        //            //if (itemResult.Success == false)
        //            //{
        //                UOMResultViewModel itemRs = new UOMResultViewModel();
        //                itemRs = _mapper.Map<UOMResultViewModel>(item);
        //                itemRs.Success = itemResult.Success;
        //                itemRs.Message = itemResult.Message;
        //            resultlist.Add(itemRs);
        //            //}
        //        }
        //        result.Success = true;
        //        result.Data = resultlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //        //result.Data = failedlist;
        //    } 
        //    return result;
        //}

         
        public async Task<GenericResult> Create(MStoreCurrency model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("Id", Guid.NewGuid());
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                //parameters.Add("Date", model.Date);
                parameters.Add("Currency", model.Currency);
                //parameters.Add("Rate", model.Rate);
                parameters.Add("Status", model.Status);
                var exist = await GetByCode(model.CompanyCode,model.StoreId, model.Currency);
                if (exist.Success == true)
                {
                    var data = exist.Data as List<MStoreCurrency>;
                    if(data!=null && data.Count > 0)
                    {
                        result.Success = false;
                        result.Message = model.Currency + " in store "+ model.StoreId + " has existed.";
                        return result;
                    }    
                   
                }
                var affectedRows = _currencyRepository.Insert("USP_I_M_StoreCurrency", parameters, commandType: CommandType.StoredProcedure);
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

     

        public async Task<GenericResult> Delete(string CompanyCode, string StoreId, string Currency)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                //parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("Currency", Currency);
                //parameters.Add("Date", model.Date);
                //parameters.Add("Currency", model.Currency);
                //parameters.Add("Rate", model.Rate);
                //parameters.Add("Status", model.Status);
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _currencyRepository.Insert("USP_D_M_StoreCurrency", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId )
        {
            GenericResult rs = new GenericResult();
            try
            {
              


                var data = await _currencyRepository.GetAllAsync($"[USP_S_M_StoreCurrency] '{CompanyCode}','{StoreId}',  ''", null, commandType: CommandType.Text);

                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }

        public async Task<GenericResult> GetByStoreWExchangeRate(string CompanyCode, string StoreId)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("StoreId", StoreId);
                parameters.Add("Currency", "");
                var data = await _currencyRepository.GetAllAsync($"[USP_GetByStoreWExchangeRate]", parameters, commandType: CommandType.StoredProcedure);

                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Currency)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _currencyRepository.GetAsync($"[USP_S_M_StoreCurrency] '{CompanyCode}','{StoreId}',  '{Currency}'", null, commandType: CommandType.Text);
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }

     
        public async Task<GenericResult> Update(MStoreCurrency model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                //parameters.Add("Date", model.Date);
                parameters.Add("Currency", model.Currency);
                //parameters.Add("Rate", model.Rate); 
                parameters.Add("Status", model.Status);
                var affectedRows = _currencyRepository.Update("USP_U_M_StoreCurrency", parameters, commandType: CommandType.StoredProcedure);
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
