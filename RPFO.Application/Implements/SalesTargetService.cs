
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
    public class SalesTargetService : ISalesTargetService
    {
        private readonly IGenericRepository<MEmployeeSalary> _targetRepository;

        private readonly IMapper _mapper;
        public SalesTargetService(IGenericRepository<MEmployeeSalary> targetRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _targetRepository = targetRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        //public async Task<GenericResult> Create(MSalesTarget model)
        //{
        //    GenericResult result = new GenericResult();
        //    try
        //    {
        //        var parameters = new DynamicParameters();

        //        //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //        //model.ShiftId = key;
        //        parameters.Add("CompanyCode", model.CompanyCode);
        //        //parameters.Add("Id", model.Id, DbType.String);
        //        parameters.Add("StoreId", model.StoreId);
        //        parameters.Add("Name", model.Name);
        //        parameters.Add("Description", model.Description);
        //        parameters.Add("FromDate", model.FromDate);
        //        parameters.Add("ToDate", model.ToDate);
        //        parameters.Add("SetupType", model.SetupType);
        //        parameters.Add("SetupType", model.SetupType);
        //        parameters.Add("SetupValue", model.SetupValue);
        //        parameters.Add("Target", model.Target);
        //        parameters.Add("CommissionType", model.CommissionType);
        //        parameters.Add("CommissionValue", model.CommissionValue);
        //        parameters.Add("FilterBy", model.FilterBy);
        //        parameters.Add("Remark", model.Remark);
        //        parameters.Add("Status", model.Status);
        //        parameters.Add("CustomF1", model.CustomF1);
        //        parameters.Add("CustomF2", model.CustomF2);
        //        parameters.Add("CustomF3", model.CustomF3);
        //        parameters.Add("CustomF4", model.CustomF4);
        //        parameters.Add("CustomF5", model.CustomF5);
        //        parameters.Add("CreatedBy", model.CreatedBy);
        //        //var exist = await checkExist(model.CompanyCode, model.Igid);
        //        //if (exist == true)
        //        //{
        //        //    result.Success = false;
        //        //    result.Message = model.Igid + " existed.";
        //        //    return result;
        //        //}
        //        var affectedRows = _targetRepository.Insert("USP_I_M_SalesTarget", parameters, commandType: CommandType.StoredProcedure);
        //        result.Success = true;
        //        //result.Message = key;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //    }
        //    return result;
        //}

        //public async Task<GenericResult> Delete(MSalesTarget model)
        //{
        //    GenericResult result = new GenericResult();
        //    try
        //    {
        //        var parameters = new DynamicParameters();

        //        //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //        //model.ShiftId = key;
        //        parameters.Add("CompanyCode", model.CompanyCode);
        //        parameters.Add("Id", model.Id, DbType.String);
        //        //parameters.Add("EmployeeId", model.EmployeeId);
              
        //        var affectedRows = _targetRepository.Insert("USP_D_M_SalesTarget", parameters, commandType: CommandType.StoredProcedure);
        //        result.Success = true;
        //        //result.Message = key;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //    }
        //    return result;
        //}
        ////Task<GenericResult> GetAll(string CompanyCode, string SetupType, string Id, DateTime? FromDate, DateTime? ToDate);
        //public async Task<GenericResult> GetAll(string CompanyCode, string SetupType, string Id, DateTime? FromDate, DateTime? ToDate)
        //{
        //    GenericResult result = new GenericResult();
        //    try
        //    {
        //        var parameters = new DynamicParameters();

        //        //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //        //model.ShiftId = key;
        //        parameters.Add("CompanyCode", CompanyCode);
        //        parameters.Add("SetupType", string.IsNullOrEmpty(SetupType) ? "" : SetupType);
        //        parameters.Add("Id", string.IsNullOrEmpty(Id) ? "" : Id);

        //        parameters.Add("FromDate", FromDate.HasValue ? FromDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : null);
        //        parameters.Add("ToDate", ToDate.HasValue ? ToDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : null);

        //        var affectedRows = _targetRepository.GetAll("USP_S_M_SalesTarget", parameters, commandType: CommandType.StoredProcedure);
        //        result.Success = true;
        //        result.Data = affectedRows;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //    }
        //    return result;
        //}

        //public async Task<GenericResult> Update(MSalesTarget model)
        //{
        //    GenericResult result = new GenericResult();
        //    try
        //    {
        //        var parameters = new DynamicParameters();

        //        //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //        //model.ShiftId = key;
        //        parameters.Add("CompanyCode", model.CompanyCode);
        //        parameters.Add("Id", model.Id, DbType.String);
        //        parameters.Add("StoreId", model.StoreId);
        //        parameters.Add("Name", model.Name);
        //        parameters.Add("Description", model.Description);
        //        parameters.Add("FromDate", model.FromDate);
        //        parameters.Add("ToDate", model.ToDate);
        //        parameters.Add("SetupType", model.SetupType);
        //        parameters.Add("SetupType", model.SetupType);
        //        parameters.Add("SetupValue", model.SetupValue);
        //        parameters.Add("Target", model.Target);
        //        parameters.Add("CommissionType", model.CommissionType);
        //        parameters.Add("CommissionValue", model.CommissionValue);
        //        parameters.Add("FilterBy", model.FilterBy);
        //        parameters.Add("Remark", model.Remark);
        //        parameters.Add("Status", model.Status);
        //        parameters.Add("CustomF1", model.CustomF1);
        //        parameters.Add("CustomF2", model.CustomF2);
        //        parameters.Add("CustomF3", model.CustomF3);
        //        parameters.Add("CustomF4", model.CustomF4);
        //        parameters.Add("CustomF5", model.CustomF5);
        //        parameters.Add("ModifiedBy", model.ModifiedBy);
        //        //var exist = await checkExist(model.CompanyCode, model.Igid);
        //        //if (exist == true)
        //        //{
        //        //    result.Success = false;
        //        //    result.Message = model.Igid + " existed.";
        //        //    return result;
        //        //}
        //        var affectedRows = _targetRepository.Insert("USP_U_M_SalesTarget", parameters, commandType: CommandType.StoredProcedure);
        //        result.Success = true;
        //        //result.Message = key;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = ex.Message;
        //    }
        //    return result;
        //}

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
