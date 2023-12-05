
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
    public class TaxService : ITaxService
    {
        private readonly IGenericRepository<MTax> _taxRepository;

        private readonly IMapper _mapper;
        public TaxService(IGenericRepository<MTax> taxRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _taxRepository = taxRepository;
            _mapper = mapper; 

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<TaxResultViewModel> resultlist = new List<TaxResultViewModel>();
            try
            {
                foreach (var item in model.Tax)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    //if (itemResult.Success == false)
                    //{
                        TaxResultViewModel itemRs = new TaxResultViewModel();
                        itemRs = _mapper.Map<TaxResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                    resultlist.Add(itemRs);
                    //}
                }
                result.Success = true;
                result.Data = resultlist;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                //result.Data = failedlist;
            } 
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string TaxCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("TaxCode", TaxCode);
            parameters.Add("Status", "");
            var affectedRows = await _taxRepository.GetAsync("USP_S_M_Tax", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MTax model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("TaxCode", model.TaxCode);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("TaxName", model.TaxName);
                parameters.Add("TaxPercent", model.TaxPercent);
                parameters.Add("TaxType", model.TaxType); 
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);

                var exist = await checkExist(model.CompanyCode, model.TaxCode);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.TaxCode + " existed.";
                    return result;
                }

                var affectedRows = _taxRepository.Insert("USP_I_M_Tax", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _taxRepository.GetAllAsync($"select * from M_Tax with (nolock) where  CompanyCode = '{CompanyCode}'", null, commandType: CommandType.Text);
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
                var data = await _taxRepository.GetAsync($"select * from M_Tax with (nolock) where  CompanyCode = '{CompanyCode}' and TaxCode='{Code}'", null, commandType: CommandType.Text);
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

        public Task<GenericResult> GetByUser(string User)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<MTax>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _taxRepository.GetAllAsync($"select * from M_Tax with (nolock)", null, commandType: CommandType.Text);
             
                return await PagedList<MTax>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<GenericResult> Update(MTax model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("TaxCode", model.TaxCode);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("TaxName", model.TaxName);
                parameters.Add("TaxPercent", model.TaxPercent);
                parameters.Add("TaxType", model.TaxType);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _taxRepository.Update("USP_U_M_Tax", parameters, commandType: CommandType.StoredProcedure);
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
