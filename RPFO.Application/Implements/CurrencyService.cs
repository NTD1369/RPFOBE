
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
    public class CurrencyService : ICurrencyService
    {
        private readonly IGenericRepository<MCurrency> _currencyRepository;

        private readonly IMapper _mapper;
        public CurrencyService(IGenericRepository<MCurrency> currencyRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
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

         
        public async Task<GenericResult> Create(MCurrency model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
 
                 
                parameters.Add("CurrencyCode", model.CurrencyCode);
                parameters.Add("CurrencyName", model.CurrencyName); 
                parameters.Add("Rounding", model.Rounding); 
                parameters.Add("MaxValue", model.MaxValue);   
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _currencyRepository.Insert("USP_I_M_Currency", parameters, commandType: CommandType.StoredProcedure);
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
         
        public Task<GenericResult> Delete(MCurrency model)
        {
            throw new NotImplementedException();
        }
        public async Task<GenericResult> GetRoundingMethod()
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _currencyRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    string query = $"select * from fn_GetRoundingMethod()";

                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    result.Success = true;
                    result.Data = dataX;
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
        public async Task<GenericResult> GetAll()
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _currencyRepository.GetAllAsync($"USP_S_M_Currency ''", null, commandType: CommandType.Text);
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
                var data = await _currencyRepository.GetAsync($"USP_S_M_Currency '{Code}'", null, commandType: CommandType.Text);
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

     

        public async Task<GenericResult> Update(MCurrency model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CurrencyCode", model.CurrencyCode);
                parameters.Add("CurrencyName", model.CurrencyName);
                parameters.Add("Rounding", model.Rounding);
                parameters.Add("MaxValue", model.MaxValue);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _currencyRepository.Insert("USP_U_M_Currency", parameters, commandType: CommandType.StoredProcedure);
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
