
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
    public class PriorityPriceListService : IPriorityPriceListService
    {
        private readonly IGenericRepository<MPriorityPriceList> _priotyRepository;

        private readonly IMapper _mapper;
        public PriorityPriceListService(IGenericRepository<MPriorityPriceList> priotyRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _priotyRepository = priotyRepository;
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

        //public async Task<bool> checkExist(string CompanyCode, string StoreId,   string ItemCode, string UomCode)
        //{
        //    var parameters = new DynamicParameters();

        //    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //    //model.ShiftId = key;
        //    parameters.Add("CompanyCode", CompanyCode);
        //    parameters.Add("StoreId", StoreId); 
        //    parameters.Add("ItemCode", ItemCode);
        //    parameters.Add("UomCode", UomCode);
        //    parameters.Add("Status", "");
        //    var affectedRows = await _priotyRepository.GetAsync("USP_S_M_PriceList", parameters, commandType: CommandType.StoredProcedure);
        //    if (affectedRows != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public async Task<GenericResult> Create(MPriorityPriceList model)
        {
            GenericResult result = new GenericResult();
            try
            {
                //var exist = await checkExist(model.CompanyCode, model.StoreId, model.ItemCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.StoreId + " - " + model.ItemCode + " - " + model.UomCode + " existed.";
                //    return result;
                //}
                var parameters = new DynamicParameters();

                parameters.Add("Id", Guid.NewGuid().ToString());
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("CusGrpId", model.CusGrpId);
                parameters.Add("CusGrpDesc", model.CusGrpDesc);
                parameters.Add("PriceListId", model.PriceListId);
                parameters.Add("Priority", model.Priority);
                parameters.Add("PriceListId", model.PriceListId);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status);

                var affectedRows = _priotyRepository.Insert("USP_I_M_PriorityPriceList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(MPriorityPriceList model)
        {
            GenericResult result = new GenericResult();
            try
            {
                //var exist = await checkExist(model.CompanyCode, model.StoreId, model.ItemCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.StoreId + " - " + model.ItemCode + " - " + model.UomCode + " existed.";
                //    return result;
                //}
                var parameters = new DynamicParameters();

                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
               

                var affectedRows = _priotyRepository.Execute("USP_D_M_PriorityPriceList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> GetAll(string CompanyCode, string CusGrpId, string Id)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyCode", CompanyCode);
                parameters.Add("@CusGrpId", string.IsNullOrEmpty(CusGrpId)? "" : CusGrpId); 
                parameters.Add("@Id", string.IsNullOrEmpty(Id) ? "" : Id);
                var data = await _priotyRepository.GetAllAsync($"USP_S_M_PriorityPriceList", parameters, commandType: CommandType.StoredProcedure);
                rs.Data = data;
                rs.Success = true;
                
            }
            catch (Exception ex)
            {
                rs.Message = ex.Message;
                rs.Success = false;
            }
            return rs;
        }

        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("CusGrpId", "");
                parameters.Add("Id", Code);
                var data = await _priotyRepository.GetAsync($"USP_S_M_PriorityPriceList", null, commandType: CommandType.StoredProcedure);
                rs.Data = data;
                rs.Success = true;

            }
            catch (Exception ex)
            {
                rs.Message = ex.Message;
                rs.Success = false;
            }
            return rs;
        }

       
        public async Task<GenericResult> Update(MPriorityPriceList model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("CusGrpId", model.CusGrpId);
                parameters.Add("CusGrpDesc", model.CusGrpDesc);
                parameters.Add("PriceListId", model.PriceListId);
                parameters.Add("Priority", model.Priority);
                parameters.Add("PriceListId", model.PriceListId); 
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
                var affectedRows = _priotyRepository.Update("USP_U_M_PriorityPriceList", parameters, commandType: CommandType.StoredProcedure);
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
