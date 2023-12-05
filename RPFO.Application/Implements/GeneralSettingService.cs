
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
    public class GeneralSettingService : IGeneralSettingService
    {
        private readonly IGenericRepository<SGeneralSetting> _settingRepository;
        IStoreService _storeService;
        private readonly IMapper _mapper;
        public GeneralSettingService(IGenericRepository<SGeneralSetting> settingRepository, IStoreService storeService , IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _settingRepository = settingRepository;
            _mapper = mapper;
            _storeService = storeService;
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

        //public async Task<bool> checkExist(string CompanyCode, string UomCode)
        //{
        //    var parameters = new DynamicParameters();

        //    //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
        //    //model.ShiftId = key;
        //    parameters.Add("CompanyCode", CompanyCode);
        //    parameters.Add("UomCode", UomCode);
        //    parameters.Add("Status", "");
        //    var affectedRows = await _uomRepository.GetAsync("USP_S_M_UOM", parameters, commandType: CommandType.StoredProcedure);
        //    if (affectedRows != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public async Task<GenericResult> Create(SGeneralSetting model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
 

                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("SettingId", model.SettingId);
                parameters.Add("SettingName", model.SettingName); 
                parameters.Add("SettingValue", model.SettingValue); 
                parameters.Add("SettingDescription", model.SettingDescription); 
                parameters.Add("ValueType", model.ValueType); 
                parameters.Add("Status", model.Status); 
                parameters.Add("StoreId", model.StoreId); 
                parameters.Add("TokenExpired", model.TokenExpired); 
                parameters.Add("CreatedBy", model.CreatedBy); 
                parameters.Add("DefaultValue", model.DefaultValue);
                parameters.Add("CustomField1", model.CustomField1);
                parameters.Add("CustomField2", model.CustomField2);
                parameters.Add("CustomField3", model.CustomField3);
                parameters.Add("CustomField4", model.CustomField4);
                parameters.Add("CustomField5", model.CustomField5);
              
                //var exist = await checkExist(model.CompanyCode, model.UomCode);
                //if (exist == true)
                //{
                //    result.Success = false;
                //    result.Message = model.UomCode + " existed.";
                //    return result;
                //}
                var affectedRows = _settingRepository.Insert("USP_I_S_GeneralSetting", parameters, commandType: CommandType.StoredProcedure);
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
            GenericResult rs = new GenericResult();
            try
            {
                var data = await _settingRepository.GetAllAsync($"select * from S_GeneralSetting with (nolock) where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
        public async Task<GenericResult> GetByStore(string CompanyCode, string StoreId)
        {
            GenericResult rs = new GenericResult();
            try
            {

                var data = await _settingRepository.GetAllAsync($"select * from S_GeneralSetting with (nolock) where CompanyCode='{CompanyCode}' and StoreId ='{StoreId}' ", null, commandType: CommandType.Text);
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
        
        public async Task<GenericResult> GetGeneralSettingByStore(string CompanyCode, string StoreId)
        {
            GenericResult rs = new GenericResult();
            try
            {
                List<GeneralSettingStore>  list = new List<GeneralSettingStore>();
                if (!string.IsNullOrEmpty(StoreId))
                {
                    GeneralSettingStore model = new GeneralSettingStore();
                    var storeData = await _storeService.GetByCode(CompanyCode, StoreId);
                    var store = storeData.Data as MStore;
                    //var data = await _settingRepository.GetAllAsync($"select * from S_GeneralSetting with (nolock) where CompanyCode='{CompanyCode}' and StoreId ='{StoreId}' ", null, commandType: CommandType.Text);
                    var data = await _settingRepository.GetAllAsync($"[USP_GetGeneralSettingStore] N'{CompanyCode}', N'{StoreId}' ", null, commandType: CommandType.Text);
                    model = _mapper.Map<GeneralSettingStore>(store);
                    model.GeneralSettings = data;
                    list.Add(model);
                }   
                else
                {
                    var storeListData = await _storeService.GetAll(CompanyCode);
                    var storeList = storeListData.Data as List<MStore>;
                    foreach (var store in storeList )
                    {
                        GeneralSettingStore model = new GeneralSettingStore();
                        //var store = await _storeService.GetByCode(StoreId);
                        var data = await _settingRepository.GetAllAsync($"[USP_GetGeneralSettingStore] N'{CompanyCode}', N'{store}' ", null, commandType: CommandType.Text);
                        //var data = await _settingRepository.GetAllAsync($"select * from S_GeneralSetting with (nolock) where CompanyCode='{store.CompanyCode}' and StoreId ='{store.StoreId}' ", null, commandType: CommandType.Text);
                        model = _mapper.Map<GeneralSettingStore>(store);
                        model.GeneralSettings = data;
                        list.Add(model);
                    }    
                }
                rs.Success = true;
                rs.Data = list;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
        public async Task<GenericResult> GetByCode(string CompanyCode, string StoreId, string Code, Boolean? ischeck = null)
        {
            GenericResult rs = new GenericResult();
            //Boolean? ischeck = null;
            try
            {
                SGeneralSetting data = null;
                if (ischeck == null)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("SettingId", Code); 

                    data = await _settingRepository.GetAsync($"[USP_GetGeneralSettingStore]", parameters, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", CompanyCode);
                    parameters.Add("StoreId", StoreId);
                    parameters.Add("SettingId", Code);
                    parameters.Add("IsCheck", ischeck);

                    data = await _settingRepository.GetAsync($"[USP_GetGeneralSettingStore]", parameters, commandType: CommandType.StoredProcedure);
                }
                //model = _mapper.Map<GeneralSettingStore>(store);
                //var data = await _settingRepository.GetAsync($"select * from S_GeneralSetting with (nolock) where CompanyCode='{CompanyCode}' and StoreId ='{StoreId}' and SettingId='{Code}'", null, commandType: CommandType.Text);
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }

        public async Task<GenericResult> UpdateList(List<SGeneralSetting> models)
        {
            GenericResult result = new GenericResult();
            try
            {
                string mess = "";
                foreach (var item in models)
                {
                  var resultItem =  await Update(item);
                   if(!resultItem.Success)
                    {
                        mess += resultItem.Message + ", ";
                    }    
                }
                if(mess.Length > 0)
                {
                    result.Success = false;
                    result.Message = mess;
                }    
                else
                {
                    result.Success = true;
                }    
               
                 
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }


        public async Task<GenericResult> Update(SGeneralSetting model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var checkModel = await GetByCode(model.CompanyCode, model.StoreId, model.SettingId, true);
                if (checkModel != null)
                {

                    //var checkData = await GetByCode(model.CompanyCode, model.StoreId, model.SettingId, true);
                    var data = checkModel.Data as SGeneralSetting;
                    if(data!=null)
                    {
                        var parameters = new DynamicParameters();
                        if(model.ValueType=="Time")
                        {
                            model.SettingValue = DateTime.Parse(model.SettingValue).ToString("HH:mm:ss");
                        }    
                        parameters.Add("CompanyCode", model.CompanyCode);
                        parameters.Add("SettingId", model.SettingId);
                        parameters.Add("SettingName", model.SettingName);
                        parameters.Add("SettingValue", model.SettingValue);
                        parameters.Add("SettingDescription", model.SettingDescription);
                        parameters.Add("ValueType", model.ValueType);
                        parameters.Add("Status", model.Status);
                        parameters.Add("StoreId", model.StoreId);
                        parameters.Add("TokenExpired", model.TokenExpired);
                        parameters.Add("ModifiedBy", model.ModifiedBy);
                        parameters.Add("DefaultValue", model.DefaultValue);
                        parameters.Add("CustomField1", model.CustomField1);
                        parameters.Add("CustomField2", model.CustomField2);
                        parameters.Add("CustomField3", model.CustomField3);
                        parameters.Add("CustomField4", model.CustomField4);
                        parameters.Add("CustomField5", model.CustomField5);
                        //var exist = await checkExist(model.CompanyCode, model.UomCode);
                        //if (exist == true)
                        //{
                        //    result.Success = false;
                        //    result.Message = model.UomCode + " existed.";
                        //    return result;
                        //}
                        var affectedRows = _settingRepository.Insert("USP_U_S_GeneralSetting", parameters, commandType: CommandType.StoredProcedure);
                        result.Success = true;
                    }    
                    else
                    {
                        model.CreatedBy = model.ModifiedBy;
                        result = await Create(model);
                    }    
                }   
                else
                {
                    model.CreatedBy = model.ModifiedBy;
                    result = await Create(model);
                }    
              
               
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
