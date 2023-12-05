
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
    public class ShortcutService : IShortcutService
    {
        private readonly IGenericRepository<MShortcutKeyboard> _shortcutRepository;

        private readonly IMapper _mapper;
        public ShortcutService(IGenericRepository<MShortcutKeyboard> shortcutRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _shortcutRepository = shortcutRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }
        //public async Task<GenericResult> Import(DataImport model)
        //{
        //    GenericResult result = new GenericResult();
        //    List<PaymentMethodResultViewModel> resultlist = new List<PaymentMethodResultViewModel>();
        //    try
        //    {
        //        foreach (var item in model.PaymentMethod)
        //        {
        //            item.CreatedBy = model.CreatedBy;
        //            item.CompanyCode = model.CompanyCode;
        //            var itemResult = await Create(item);
                  
        //                PaymentMethodResultViewModel itemRs = new PaymentMethodResultViewModel();
        //                itemRs = _mapper.Map<PaymentMethodResultViewModel>(item);
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
        //        //result.Data = failedlist;
        //    } 
        //    return result;
        //}

        public async Task<bool> checkExist(string CompanyCode, string PaymentCode)
        {
            var parameters = new DynamicParameters();

            //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
            //model.ShiftId = key;
            parameters.Add("CompanyCode", CompanyCode);
            parameters.Add("PaymentCode", PaymentCode);
            parameters.Add("Status", "");
            var affectedRows = await _shortcutRepository.GetAsync("USP_S_M_Payment", parameters, commandType: CommandType.StoredProcedure);
            if (affectedRows != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<GenericResult> Create(MShortcutKeyboard model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("Id", Guid.NewGuid());
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Description", model.Description);
                parameters.Add("Name", model.Name);
                parameters.Add("Key1", model.Key1);
                parameters.Add("Key2", model.Key2);
                parameters.Add("Key3", model.Key3);
                parameters.Add("Key4", model.Key4);
                parameters.Add("Key5", model.Key5);
                parameters.Add("Custom1", model.Custom1);
                parameters.Add("Custom2", model.Custom2);
                parameters.Add("Custom3", model.Custom3);
                parameters.Add("FunctionCode", model.FunctionCode);
                parameters.Add("Window", model.Window);
                parameters.Add("Status", model.Status);
                //var exist = await checkExist(model.CompanyCode, model.PaymentCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.PaymentCode + " existed.";
                //    return result;
                //}
                var affectedRows = _shortcutRepository.Insert("USP_I_M_ShortcutKeyboard", parameters, commandType: CommandType.StoredProcedure);
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

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Id", Code); 
                var data = _shortcutRepository.Execute($"USP_D_M_ShortcutKeyboard", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetByFunction(string CompanyCode, string FunctionCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode); 
                parameters.Add("FunctionCode", FunctionCode);
                var data = await _shortcutRepository.GetAllAsync($"[USP_S_M_ShortcutKeyboardByFunction]", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("Id", "");
                parameters.Add("Name", "");
                parameters.Add("Filter", "");
                var data = await _shortcutRepository.GetAllAsync($"USP_S_M_ShortcutKeyboard", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetByCode(string CompanyCode,   string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode",  CompanyCode);
                parameters.Add("Id", Code); 
                parameters.Add("Name", "");
                parameters.Add("Filter", ""); 
                var data = await _shortcutRepository.GetAsync($"USP_S_M_ShortcutKeyboard", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                result.Data = data;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
       
        public async Task<GenericResult> Update(MShortcutKeyboard model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("Id", model.Id);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Description", model.Description);
                parameters.Add("Name", model.Name);
                parameters.Add("Key1", model.Key1);
                parameters.Add("Key2", model.Key2);
                parameters.Add("Key3", model.Key3);
                parameters.Add("Key4", model.Key4);
                parameters.Add("Key5", model.Key5);
                parameters.Add("Custom1", model.Custom1);
                parameters.Add("Custom2", model.Custom2);
                parameters.Add("Custom3", model.Custom3);
                parameters.Add("FunctionCode", model.FunctionCode);
                parameters.Add("Window", model.Window); 
                parameters.Add("Status", model.Status);
                var affectedRows = _shortcutRepository.Insert("USP_U_M_ShortcutKeyboard", parameters, commandType: CommandType.StoredProcedure);
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
