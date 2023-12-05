
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
    public class EmployeeSalesTargetSummaryService : IEmployeeSalesTargetSummaryService
    {
        private readonly IGenericRepository<TEmployeeSalesTargetSummary> _targetRepository;

        private readonly IMapper _mapper;
        public EmployeeSalesTargetSummaryService(IGenericRepository<TEmployeeSalesTargetSummary> targetRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _targetRepository = targetRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        public async Task<GenericResult> CreateByList(List<TEmployeeSalesTargetSummary> models)
        {
            GenericResult result = new GenericResult();
            try
            {
                foreach(var model in models)
                {
                    var parameters = new DynamicParameters();

                    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                    //model.ShiftId = key;
                    parameters.Add("CompanyCode", model.CompanyCode);
                    //parameters.Add("Id", model.Id, DbType.String);
                    parameters.Add("EmployeeId", model.EmployeeId);
                    parameters.Add("Position", model.Position);
                    parameters.Add("FromDate", model.FromDate.HasValue ? model.FromDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : null);
                    parameters.Add("ToDate", model.ToDate.HasValue ? model.ToDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : null);
                    parameters.Add("Salary", model.Salary);
                    parameters.Add("CommisionValue", model.CommisionValue);
                    parameters.Add("SalesTarget", model.SalesTarget);
                    parameters.Add("LineTotal", model.LineTotal);
                    parameters.Add("LineTotal1", model.LineTotal1);
                    parameters.Add("LineTotal2", model.LineTotal2);
                    //parameters.Add("Status", model.Status);
                    parameters.Add("CustomF1", model.CustomF1);
                    parameters.Add("CustomF2", model.CustomF2);
                    parameters.Add("CustomF3", model.CustomF3);
                    parameters.Add("CustomF4", model.CustomF4);
                    parameters.Add("CustomF5", model.CustomF5);
                    parameters.Add("CreatedBy", model.CreatedBy);
                    //var exist = await checkExist(model.CompanyCode, model.Igid);
                    //if (exist == true)
                    //{
                    //    result.Success = false;
                    //    result.Message = model.Igid + " existed.";
                    //    return result;
                    //}
                    var affectedRows = _targetRepository.Insert("USP_I_T_EmployeeTargetSummary", parameters, commandType: CommandType.StoredProcedure);
                   
                }
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

        public async Task<GenericResult> Create(TEmployeeSalesTargetSummary model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                //parameters.Add("Id", model.Id, DbType.String);
                parameters.Add("EmployeeId", model.EmployeeId);
                parameters.Add("Position", model.Position);
                parameters.Add("FromDate", model.FromDate.HasValue ? model.FromDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : null);
                parameters.Add("ToDate", model.ToDate.HasValue ? model.ToDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : null);
                parameters.Add("Salary", model.Salary);
                parameters.Add("CommisionValue", model.CommisionValue);
                parameters.Add("SalesTarget", model.SalesTarget);
                parameters.Add("LineTotal", model.LineTotal);
                parameters.Add("LineTotal1", model.LineTotal1);
                parameters.Add("LineTotal2", model.LineTotal2); 
                //parameters.Add("Status", model.Status);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("CreatedBy", model.CreatedBy);
                //var exist = await checkExist(model.CompanyCode, model.Igid);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.Igid + " existed.";
                //    return result;
                //}
                var affectedRows = _targetRepository.Insert("USP_I_T_EmployeeTargetSummary", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<GenericResult> Delete(TEmployeeSalesTargetSummary model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", model.Id, DbType.String);
                //parameters.Add("EmployeeId", model.EmployeeId);
              
                var affectedRows = _targetRepository.Insert("USP_D_T_EmployeeSalesTargetSummary", parameters, commandType: CommandType.StoredProcedure);
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
        //Task<GenericResult> GetAll(string CompanyCode, string SetupType, string Id, DateTime? FromDate, DateTime? ToDate);
 
        public async Task<GenericResult> GetAll(string CompanyCode, string Employee, string Position, DateTime? FromDate, DateTime? ToDate, string ViewType)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("EmployeeId", string.IsNullOrEmpty(Employee) ? "" : Employee);
                parameters.Add("Position", string.IsNullOrEmpty(Position) ? "" : Position); 
                parameters.Add("ViewType", string.IsNullOrEmpty(ViewType) ? "" : ViewType); 
                parameters.Add("FromDate", FromDate.HasValue ? FromDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : null);
                parameters.Add("ToDate", ToDate.HasValue ? ToDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : null);

                var affectedRows = _targetRepository.GetAll("USP_S_T_EmployeeTargetSummary", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;

                var checkStatus = affectedRows.Where(x => x.Status == "N");
                if(checkStatus!=null && checkStatus.Count() > 0)
                {
                    result.Message = "N";
                }    
                else
                {
                    result.Message = "Y";
                }    
                result.Data = affectedRows;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> Update(TEmployeeSalesTargetSummary model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Id", model.Id, DbType.String);
                parameters.Add("EmployeeId", model.EmployeeId);
                parameters.Add("Position", model.Position);
                parameters.Add("FromDate", model.FromDate.HasValue ? model.FromDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : null);
                parameters.Add("ToDate", model.ToDate.HasValue ? model.ToDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : null);
                parameters.Add("Salary", model.Salary);
                parameters.Add("CommisionValue", model.CommisionValue);
                parameters.Add("SalesTarget", model.SalesTarget);
                parameters.Add("LineTotal", model.LineTotal);
                parameters.Add("LineTotal1", model.LineTotal1);
                parameters.Add("LineTotal2", model.LineTotal2);
                parameters.Add("Status", model.Status);
                parameters.Add("CustomF1", model.CustomF1);
                parameters.Add("CustomF2", model.CustomF2);
                parameters.Add("CustomF3", model.CustomF3);
                parameters.Add("CustomF4", model.CustomF4);
                parameters.Add("CustomF5", model.CustomF5);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                //var exist = await checkExist(model.CompanyCode, model.Igid);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.Igid + " existed.";
                //    return result;
                //}
                var affectedRows = _targetRepository.Insert("USP_U_T_EmployeeSalesTargetSummary", parameters, commandType: CommandType.StoredProcedure);
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

        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<ItemGroupResultViewModel> failedlist = new List<ItemGroupResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.ItemGroup)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);

        //            ItemGroupResultViewModel itemRs = new ItemGroupResultViewModel();
        //            itemRs = _mapper.Map<ItemGroupResultViewModel>(item);
        //            itemRs.Success = itemResult.Success;
        //            itemRs.Message = itemResult.Message;
        //            failedlist.Add(itemRs);

        //        }
        //        result.Success = true;
        //        result.Data = failedlist;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //        result.Data = failedlist;
        //    } 
        //    return result;
        //}

        //public async Task<bool> checkExist(string CompanyCode, string IGId)
        //{
        //    var parameters = new DynamicParameters();

        //    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //    //model.ShiftId = key;
        //    parameters.Add("CompanyCode", CompanyCode);
        //    parameters.Add("IGId", IGId);
        //    parameters.Add("Status", "");
        //    var affectedRows = await _itemGroupRepository.GetAsync("USP_S_M_ItemGroup", parameters, commandType: CommandType.StoredProcedure);
        //    if (affectedRows != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

    }

}
