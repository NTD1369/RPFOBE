
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
    public class UomService : IUomService
    {
        private readonly IGenericRepository<MUom> _uomRepository;

        private readonly IMapper _mapper;
        public UomService(IGenericRepository<MUom> uomRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _uomRepository = uomRepository;
            _mapper = mapper; 

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<UOMResultViewModel> resultlist = new List<UOMResultViewModel>();
            try
            {
                foreach (var item in model.Uom)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    //if (itemResult.Success == false)
                    //{
                        UOMResultViewModel itemRs = new UOMResultViewModel();
                        itemRs = _mapper.Map<UOMResultViewModel>(item);
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

        public async Task<bool> checkExist(string CompanyCode, string UomCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("UomCode", UomCode);
            parameters.Add("Status", "");
            var affectedRows = await _uomRepository.GetAsync("USP_S_M_UOM", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MUom model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
 

                parameters.Add("UOMCode", model.UomCode);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("UOMName", model.UomName); 
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("AllowDecimal", model.AllowDecimal);
                parameters.Add("DecimalFormat", model.DecimalFormat);
                parameters.Add("ThousandFormat", model.ThousandFormat);
                parameters.Add("DecimalPlacesFormat", model.DecimalPlacesFormat);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                var exist = await checkExist(model.CompanyCode, model.UomCode);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.UomCode + " existed.";
                    return result;
                }
                var affectedRows = _uomRepository.Insert("USP_I_M_UOM", parameters, commandType: CommandType.StoredProcedure);
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
                var data = await _uomRepository.GetAllAsync($"select *,isnull([AllowDecimal], 1) AllowDecimal from M_Uom with (nolock)", null, commandType: CommandType.Text);
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
                var data = await _uomRepository.GetAsync($"select * from M_Uom with (nolock) where UOMCode='{Code}'", null, commandType: CommandType.Text);
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

        public Task<GenericResult> GetByItem(string Item)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<MUom>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _uomRepository.GetAllAsync($"select * from M_Uom with (nolock)", null, commandType: CommandType.Text);
             
                return await PagedList<MUom>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<GenericResult> Update(MUom model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
               
                parameters.Add("UOMCode", model.UomCode); 
                parameters.Add("CompanyCode", model.CompanyCode); 
                parameters.Add("UOMName", model.UomName);  
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("AllowDecimal", model.AllowDecimal);
                parameters.Add("DecimalFormat", model.DecimalFormat);
                parameters.Add("ThousandFormat", model.ThousandFormat);
                parameters.Add("DecimalPlacesFormat", model.DecimalPlacesFormat);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                var affectedRows = _uomRepository.Update("USP_U_M_UOM", parameters, commandType: CommandType.StoredProcedure);
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
