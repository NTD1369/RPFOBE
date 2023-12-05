
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
    public class MerchandiseCategoryService : IMerchandiseCategoryService
    {
        private readonly IGenericRepository<MMerchandiseCategory> _merchandiseRepository;
        private readonly IMapper _mapper;
        public MerchandiseCategoryService(IGenericRepository<MMerchandiseCategory> merchandiseRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _merchandiseRepository = merchandiseRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> Import(DataImport model)
        {
            GenericResult result = new GenericResult();
            List<MerchandiseResultViewModel> resultList = new List<MerchandiseResultViewModel>();
            try
            {
                foreach (var item in model.Merchandise)
                {
                    item.CreatedBy = model.CreatedBy;
                    item.CompanyCode = model.CompanyCode;
                    var itemResult = await Create(item);
                    //if (itemResult.Success == false)
                    //{
                        MerchandiseResultViewModel itemRs = new MerchandiseResultViewModel();
                        itemRs = _mapper.Map<MerchandiseResultViewModel>(item);
                        itemRs.Success = itemResult.Success;
                        itemRs.Message = itemResult.Message;
                    resultList.Add(itemRs);
                    //}
                }
                result.Success = true;
                result.Data = resultList;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                //result.Data = failedlist;
            } 
            return result;
        }

        public async Task<bool> checkExist(string CompanyCode, string MCId)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("MCId", MCId);
            parameters.Add("Status", "");
            var affectedRows = await _merchandiseRepository.GetAsync("USP_S_M_Merchandise", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MMerchandiseCategory model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("MCId", model.Mcid);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("MCHier", model.Mchier);
                parameters.Add("MCName", model.Mcname); 
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("isShow", model.isShow);
                parameters.Add("OrderNum", model.OrderNum);
                var exist = await checkExist(model.CompanyCode, model.Mcid);
                if (exist == true)
                {
                    result.Success = false;
                    result.Message = model.Mcid + " existed.";
                    return result;
                }
                var affectedRows = _merchandiseRepository.Insert("USP_I_M_MerchandiseCategory", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<GenericResult> GetMerchandiseCategoryShow(string companyCode, string mcid, string status, string keyword)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", string.IsNullOrEmpty(companyCode) ? "" : companyCode);
                parameters.Add("MCID", string.IsNullOrEmpty(mcid) ? "" : mcid);
                parameters.Add("Status", string.IsNullOrEmpty(status) ? "" : status);
                parameters.Add("Keyword", string.IsNullOrEmpty(keyword) ? "" : keyword);

                var data = await _merchandiseRepository.GetAllAsync("USP_S_M_MerchandiseCategoryShow", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetByCompany(string companyCode, string mcid, string status, string keyword)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", string.IsNullOrEmpty(companyCode)?"": companyCode);
                parameters.Add("MCID", string.IsNullOrEmpty(mcid) ? "" : mcid);
                parameters.Add("Status", string.IsNullOrEmpty(status) ? "" : status);
                parameters.Add("Keyword", string.IsNullOrEmpty(keyword) ? "" : keyword);

                var data = await _merchandiseRepository.GetAllAsync("USP_S_M_MerchandiseCategory", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<GenericResult> GetMerchandiseIsShow(string CompanyCode,string storeId)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", "");
                parameters.Add("MCID", "");
                parameters.Add("Status", "");
                parameters.Add("Keyword", "");

                var data = await _merchandiseRepository.GetAllAsync("USP_S_M_MerchandiseCategory", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
            //var data = await _merchandiseRepository.GetAllAsync("select distinct t1.*  from M_MerchandiseCategory t1 with(nolock) right join M_Item t2 with(nolock) on t1.MCId = t2.MCId", null, commandType: CommandType.Text);
            //return data;
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("MCID", "");
                parameters.Add("Status", "");
                parameters.Add("Keyword", "");

                var data = await _merchandiseRepository.GetAllAsync("USP_S_M_MerchandiseCategory", parameters, commandType: CommandType.StoredProcedure);
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

     

        public Task<PagedList<MMerchandiseCategory>> GetPagedList(UserParams userParams)
        {
            throw new NotImplementedException();
        }
        public async Task<GenericResult> UpdateSetting(List<MMerchandiseCategory> model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                  _merchandiseRepository.Update("Update M_MerchandiseCategory set isShow=null, OrderNum=null", null, commandType: CommandType.Text);
                foreach (var item in model)
                {
                    parameters.Add("MCId", item.Mcid);
                    parameters.Add("CompanyCode", item.CompanyCode);
                    parameters.Add("MCHier", item.Mchier);
                    parameters.Add("MCName", item.Mcname);
                    parameters.Add("ModifiedBy", item.ModifiedBy);
                    parameters.Add("Status", item.Status); 
                    parameters.Add("isShow", item.isShow); 
                    parameters.Add("OrderNum", item.OrderNum); 
                    var affectedRows = _merchandiseRepository.Update("USP_U_M_MerchandiseCategory", parameters, commandType: CommandType.StoredProcedure);
                    result.Success = true;
                }    
                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
               
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> Update(MMerchandiseCategory model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("MCId", model.Mcid);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("MCHier", model.Mchier);
                parameters.Add("MCName", model.Mcname);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                parameters.Add("isShow", model.isShow);
                parameters.Add("OrderNum", model.OrderNum);
                var affectedRows = _merchandiseRepository.Update("USP_U_M_MerchandiseCategory", parameters, commandType: CommandType.StoredProcedure);
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

        public Task<GenericResult> GetByCode(string companyCode, string Code)
        {
            throw new NotImplementedException();
        }
    } 

}
