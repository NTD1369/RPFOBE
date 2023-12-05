
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
    public class PrepaidCardService : IPrepaidCardService
    {
        private readonly IGenericRepository<MPrepaidCard> _prepaidCardRepository;
        private readonly IGenericRepository<TPrepaidCardTrans> _transRepository;
       
        private readonly IMapper _mapper;
        public PrepaidCardService(IGenericRepository<MPrepaidCard> prepaidCardRepository, IGenericRepository<TPrepaidCardTrans> transRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _prepaidCardRepository = prepaidCardRepository;
            _transRepository = transRepository;
             _mapper = mapper;
            
        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<MPrepaidCardResultViewModel> resultlist = new List<MPrepaidCardResultViewModel>();
            try
            {
                foreach (var item in model.PrepaidCard)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);

                    MPrepaidCardResultViewModel itemRs = new MPrepaidCardResultViewModel();
                    itemRs = _mapper.Map<MPrepaidCardResultViewModel>(item);
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

       
        
        public async Task<GenericResult> Create(MPrepaidCard model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("PrepaidCardNo", model.PrepaidCardNo);
                parameters.Add("MainBalance", model.MainBalance);
                parameters.Add("SubBalance", model.SubBalance);
                parameters.Add("StartDate", model.StartDate);
                parameters.Add("Duration", model.Duration);
                parameters.Add("Status", model.Status);
                parameters.Add("CreatedBy", model.CreatedBy);

                var exist = await GetByCode(model.CompanyCode, model.PrepaidCardNo);
                if (exist.Success==true && exist.Data != null)
                {
                    result.Success = false;
                    result.Message = model.PrepaidCardNo + " existed.";
                    return result;
                }
                var affectedRows = _prepaidCardRepository.Insert("USP_I_M_PrepaidCard", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string Status)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _prepaidCardRepository.GetAllAsync($"[USP_S_S_PrepaidCard] '{CompanyCode}','','{Status}'", null, commandType: CommandType.Text);
                rs.Data = data;
                rs.Success = true;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }

        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _prepaidCardRepository.GetAsync($"USP_S_S_PrepaidCard '{CompanyCode}' ,'{Code}',''", null, commandType: CommandType.Text);
                rs.Data = data;
                rs.Success = true;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;

        }
        public async Task<GenericResult> GetHistoryByCode(string CompanyCode, string Code)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _transRepository.GetAllAsync($"USP_S_S_PrepaidCardHistory '{CompanyCode}' ,'{Code}'", null, commandType: CommandType.Text);
                rs.Data = data;
                rs.Success = true;
               
            }
            catch (Exception ex)
            { 
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }

        public async Task<GenericResult> Update(MPrepaidCard model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("PrepaidCardNo", model.PrepaidCardNo);
                parameters.Add("MainBalance", model.MainBalance);
                parameters.Add("SubBalance", model.SubBalance);
                parameters.Add("StartDate", model.StartDate);
                parameters.Add("Duration", model.Duration);
                parameters.Add("Status", model.Status);
                parameters.Add("ModifiedBy", model.ModifiedBy); 
                var affectedRows = _prepaidCardRepository.Insert("USP_U_M_PrepaidCard", parameters, commandType: CommandType.StoredProcedure);
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
