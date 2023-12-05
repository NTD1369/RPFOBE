
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
    public class BankInService : IBankInService
    {
        private readonly IGenericRepository<TBankIn> _bankInRepository;

        private readonly IMapper _mapper;
        public BankInService(IGenericRepository<TBankIn> bankInRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _bankInRepository = bankInRepository;
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

         
        public async Task<GenericResult> Create(TBankIn model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                model.Rate = model.Rate ?? 1;
                if (model.BankInAmt == null)
                {
                    if(model.FCAmt == null)
                    {
                        result.Success = false;
                        result.Message = "Bank In Amount can't null. Please input FC Amount / Bank In Amount value ";
                        return result;
                    }
                    else
                    {
                        if (model.BankInAmt == null)
                        {
                            model.BankInAmt = model.FCAmt * model.Rate;
                        }
                       
                    }    
                   
                }
                if (string.IsNullOrEmpty(model.CompanyCode))
                {
                    result.Success = false;
                    result.Message = "CompanyCode can't null. Please input Company Code value ";
                    return result;
                }
                if (string.IsNullOrEmpty(model.StoreId))
                {
                    result.Success = false;
                    result.Message = "Store Id can't null. Please input Store Id value ";
                    return result;
                }
                if (string.IsNullOrEmpty(model.CompanyCode))
                {
                    result.Success = false;
                    result.Message = "Daily Id can't null. Please input Daily Id value ";
                    return result;
                }
                if (string.IsNullOrEmpty(model.CompanyCode))
                {
                    result.Success = false;
                    result.Message = "Currency can't null. Please input Currency value ";
                    return result;
                }
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("DailyId", model.DailyId);
                parameters.Add("LineNum", model.LineNum ?? 1);
                parameters.Add("Currency", model.Currency);
                parameters.Add("FCAmt", model.FCAmt);
                parameters.Add("Rate", model.Rate ?? 1);
                parameters.Add("BankInAmt", model.BankInAmt);
                parameters.Add("RefNum", model.RefNum);
                parameters.Add("RefNum2", model.RefNum2);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("DocDate", model.DocDate);
                parameters.Add("Status", model.Status ?? "A");
                //var exist = await GetByCode(model.CompanyCode, model.StoreId, model.DailyId, model.Id.ToString());
                //if (exist.Data!=null)
                //{
                //    result.Success = false;
                //    result.Message = model.Id + " existed.";
                //    return result;
                //}
                var affectedRows = _bankInRepository.Insert("USP_I_T_BankIn", parameters, commandType: CommandType.StoredProcedure);
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
         
        public async Task<GenericResult> Delete(TBankIn model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();


                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("DailyId", model.DailyId);
                parameters.Add("Id", model.Id);
                var exist = await GetByCode(model.CompanyCode, model.StoreId, model.DailyId, model.Id.ToString());
                if (exist != null && exist.Data == null)
                {
                    result.Success = false;
                    result.Message = model.Id + " not existed.";
                    return result;
                }
                var affectedRows = _bankInRepository.Insert("USP_D_T_BankIn", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string StoreId, string DailyId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                parameters.Add("CompanyCode", CompanyCode ?? "");
                parameters.Add("StoreId", StoreId ?? "");
                parameters.Add("DailyId", DailyId ?? "");
                parameters.Add("Id", "");
                var data = await _bankInRepository.GetAllAsync($"USP_S_T_BankIn", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string DailyId, string Id)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode ?? "");
                parameters.Add("StoreId", StoreId ?? "");
                parameters.Add("DailyId", DailyId ?? "");
                parameters.Add("Id", Id ?? "");
                var data = await _bankInRepository.GetAsync($"USP_S_T_BankIn", parameters, commandType: CommandType.StoredProcedure);
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

     

        public async Task<GenericResult> Update(TBankIn model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("StoreId", model.StoreId);
                parameters.Add("DailyId", model.DailyId);
                parameters.Add("LineNum", model.LineNum);
                parameters.Add("Currency", model.Currency);
                parameters.Add("FCAmt", model.FCAmt);
                parameters.Add("Rate", model.Rate);
                parameters.Add("BankInAmt", model.BankInAmt);
                parameters.Add("RefNum", model.RefNum);
                parameters.Add("RefNum2", model.RefNum2);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("DocDate", model.DocDate);
                parameters.Add("Status", model.Status);
                var affectedRows = _bankInRepository.Insert("USP_U_T_BankIn", parameters, commandType: CommandType.StoredProcedure);
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
