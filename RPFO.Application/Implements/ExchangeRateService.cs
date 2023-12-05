
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
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IGenericRepository<MExchangeRate> _rateRepository;

        private readonly IMapper _mapper;
        public ExchangeRateService(IGenericRepository<MExchangeRate> rateRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _rateRepository = rateRepository;
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

         
        public async Task<GenericResult> Create(MExchangeRate model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("Id", Guid.NewGuid());
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("Date", model.Date);
                parameters.Add("Currency", model.Currency);
                parameters.Add("Rate", model.Rate);
                parameters.Add("Status", model.Status);

                bool existed = false;
                var data = await GetExchangeRateByStore(model.CompanyCode, model.StoreId, model.Currency);
                if(data.Success)
                {
                    foreach(var item in data.Data as List<MExchangeRate>)
                    { 
                        if(item.Date == model.Date)
                        {
                            existed = true;
                        }    
                    }    
                    
                }    
                if (existed == true)
                {
                    result.Success = false;
                    result.Message = model.Currency + " " + model.Date.Value.ToString("yyyy/MM/dd") + " existed.";
                    return result;
                }
                var affectedRows = _rateRepository.Insert("USP_I_M_ExchangeRate", parameters, commandType: CommandType.StoredProcedure);
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

     

        public async Task<GenericResult> Delete(MExchangeRate model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
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
                var affectedRows = _rateRepository.Insert("USP_D_M_ExchangeRate", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string Currency, DateTime? From , DateTime? To)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _rateRepository.GetAllAsync($"[USP_S_M_ExchangeRate] '{CompanyCode}','{StoreId}', '{From.Value}', '{To.Value}', '{Currency}'", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetExchangeRateIsNullByDate(string CompanyCode, string StoreId, DateTime? Date)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _rateRepository.GetAllAsync($"[USP_S_M_ExchangeRateIsNullByDate] '{CompanyCode}','{StoreId}', '{Date.Value}'", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetExchangeRateByStore(string CompanyCode, string StoreId, string Currency)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _rateRepository.GetAllAsync($"[USP_S_M_ExchangeRate_ByStore] '{CompanyCode}','{StoreId}', '{Currency}'", null, commandType: CommandType.Text);

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
        public async Task<GenericResult> GetByDate(string CompanyCode, string StoreId, DateTime? Date)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _rateRepository.GetAsync($"[USP_S_M_ExchangeRate] '{CompanyCode}','{StoreId}', '{Date.Value }', '{Date.Value }', ''", null, commandType: CommandType.Text);
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
        public async Task<GenericResult> GetByCurrency(string CompanyCode, string StoreId, string CurrencyCode)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _rateRepository.GetAllAsync($"[USP_S_M_ExchangeRate] '{CompanyCode}','{StoreId}', '', '{CurrencyCode}'", null, commandType: CommandType.Text);
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

     
        public async Task<GenericResult> Update(MExchangeRate model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("Date", model.Date);
                parameters.Add("Currency", model.Currency);
                parameters.Add("Rate", model.Rate); 
                parameters.Add("Status", model.Status);
                var affectedRows = _rateRepository.Update("USP_U_M_ExchangeRate", parameters, commandType: CommandType.StoredProcedure);
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
