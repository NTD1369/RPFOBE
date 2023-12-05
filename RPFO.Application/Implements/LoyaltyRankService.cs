
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
    public class LoyaltyRankService : ILoyaltyRankService
    {
        private readonly IGenericRepository<SLoyaltyRank> _rankRepository;

        private readonly IMapper _mapper;
        public LoyaltyRankService(IGenericRepository<SLoyaltyRank> rankRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _rankRepository = rankRepository;
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

        public async Task<bool> checkExist(string CompanyCode, string RankId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("RankId", RankId);
            parameters.Add("Keyword", "");
           
            var affectedRows = await _rankRepository.GetAsync("USP_S_S_LoyaltyRank", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(SLoyaltyRank model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
 

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("RankId", model.RankId);
                parameters.Add("RankName", model.RankName); 
                parameters.Add("TargetAmount", model.TargetAmount);
                parameters.Add("Factor", model.Factor);
                parameters.Add("Period", model.Period);
                parameters.Add("Status", model.Status);
                var exist = await checkExist(model.CompanyCode, model.RankId);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.RankId + " existed.";
                    return result;
                }
                var affectedRows = _rankRepository.Insert("USP_I_S_LoyaltyRank", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("RankId", Code);
                
                var affectedRows =  _rankRepository.Execute("USP_D_S_LoyaltyRank", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = affectedRows;

            }
            catch (Exception ex)
            {
                result.Success = true;
                result.Data = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters(); 
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("RankId", "");
                parameters.Add("Keyword", "");

                var affectedRows = await _rankRepository.GetAllAsync("USP_S_S_LoyaltyRank", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = affectedRows;
               
            }
            catch (Exception ex)
            {
                result.Success = true;
                result.Data = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("RankId", "");
                parameters.Add("Keyword", "");

                var data = await _rankRepository.GetAsync("USP_S_S_LoyaltyRank", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = true;
                result.Data = ex.Message;
            }
            return result;
        }
 
       
        
        public async Task<GenericResult> Update(SLoyaltyRank model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("RankId", model.RankId);
                parameters.Add("RankName", model.RankName);
                parameters.Add("TargetAmount", model.TargetAmount);
                parameters.Add("Factor", model.Factor);
                parameters.Add("Period", model.Period);
                parameters.Add("Status", model.Status);
                var affectedRows = _rankRepository.Update("USP_U_S_LoyaltyRank", parameters, commandType: CommandType.StoredProcedure);
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
